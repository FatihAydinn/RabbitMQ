using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQProducer.Models;
using System.Diagnostics;
using System.Text;

namespace RabbitMQProducer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        //Kuyruğa yazma işlemi
        public IActionResult Index()
        {
            
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {try
            {
                channel.QueueDeclare(queue: "product",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
            Product p1 = new Product();
            p1.ID = 1;
            p1.Name = "Product1";
            p1.Price = 21;
            var json = JsonConvert.SerializeObject(p1);
            var body = Encoding.UTF8.GetBytes(json);

                channel.BasicPublish(exchange: "",
                                     routingKey: "product",
                                     basicProperties: null,
                                     body: body);
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}