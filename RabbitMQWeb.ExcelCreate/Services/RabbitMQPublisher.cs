using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using Shared;

namespace RabbitMQWeb.ExcelCreate.Services;

public class RabbitMQPublisher
{
    private readonly RabbitMQClientService _rabbitmqClientService;

    public RabbitMQPublisher(RabbitMQClientService rabbitmqClientService)
    {
        _rabbitmqClientService = rabbitmqClientService;
    }

    public void Publish(CreateExcelMessage createExcelMessage)
    {
        var channel = _rabbitmqClientService.Connect();
        
        var bodyString = JsonSerializer.Serialize(createExcelMessage);
        var bodyByte = Encoding.UTF8.GetBytes(bodyString);
        
        var property = channel.CreateBasicProperties();
        property.Persistent = true; //mesajlar kalıcı hale gelsin, yani durable:true gibi davransın
        
        channel.BasicPublish(
            exchange: RabbitMQClientService.ExchangeName,
            routingKey: RabbitMQClientService.RoutingExcel,
            basicProperties: property,
            body: bodyByte);
    }
}
