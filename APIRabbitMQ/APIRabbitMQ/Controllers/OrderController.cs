using APIRabbitMQ.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace APIRabbitMQ.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private ILogger<OrderController> _logger;

        public OrderController(ILogger<OrderController> logger)
        {
            _logger = logger;
        }

        public IActionResult InsertOrder(Order order)
        {
            try
            {
                #region Inserir na fila

                var factory = new ConnectionFactory { HostName = "localhost" };
                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();

                channel.QueueDeclare(queue: "orderQueue",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
                /*
                for (int i = 0; i < 50000; i++)
                {
                    order = new Order
                    {
                        OrderNumber = i + 1,
                        ItemName = $"Item-{i + 1}",
                        Price = (i + 1) * 10
                    };
                    string message = JsonSerializer.Serialize(order);
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: string.Empty,
                                         routingKey: "orderQueue",
                                         basicProperties: null,
                                         body: body);
                }
                */

                
                string message = JsonSerializer.Serialize(order);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: string.Empty,
                                     routingKey: "orderQueue",
                                     basicProperties: null,
                                     body: body);                
                #endregion

                return Accepted(order);
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro ao tentar criar um novo pedido", ex);

                return new StatusCodeResult(500);
            }
            
        }
    }
}
