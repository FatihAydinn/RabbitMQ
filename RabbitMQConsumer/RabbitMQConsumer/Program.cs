using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQConsumer;
using System.Text;

//Bağlantı oluşturuldu
var factory = new ConnectionFactory()
{
    HostName = "localhost",
    Port = 5672,
    UserName = "guest",
    Password = "guest"
};

using (var connection = factory.CreateConnection()) // bağlantı açıldı
using (var channel = connection.CreateModel()) // bağlanılacak kuyruk
{
    // bağlanılacak kuyruğun adı belirleniyor
    channel.QueueDeclare(queue: "product",
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

    //kuyruğu tüketecek olan elemandan nesne oluşturulur 
    var consumer = new EventingBasicConsumer(channel);
    consumer.Received += (model, ea) =>
    {
        //kuyruktan okumaya başlayacak
        var body = ea.Body.ToArray();

        //Byte array olarak kuyruktan okuduğu veriyi önce String e dönüştürecek
        var message = Encoding.UTF8.GetString(body);

        Product p1 = new Product();

        //String olarak dönüştürüldüğü veriye Deseralizi ile tekrar product objesine dönüştürülecek
        p1 = JsonConvert.DeserializeObject<Product>(message);
        Console.WriteLine("[x] Alınan Mesaj {0}", p1.Name);
        Console.WriteLine("Alınan {0}", message);
    };
    channel.BasicConsume(queue: "product",
                        autoAck:true,
                        consumer: consumer);

    Console.WriteLine("Çıkmak için bir tuşa basın.");
    Console.ReadLine();
}
