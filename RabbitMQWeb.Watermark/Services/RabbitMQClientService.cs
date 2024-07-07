using RabbitMQ.Client;

namespace RabbitMQWeb.Watermark.Services;

public class RabbitMQClientService:IDisposable
{
    private readonly ConnectionFactory _connectionFactory;
    private IConnection _connection;
    private IModel _channel;
    public static string ExchangeName = "ImageDirectExchange";
    public static string RoutingWatermark = "watermark-route-image";
    public static string QueueName = "queue-watermark-image";
    private readonly ILogger<RabbitMQClientService> _logger;

    public RabbitMQClientService(ConnectionFactory connectionFactory, ILogger<RabbitMQClientService> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
        Connect();
    }

    public IModel Connect()
    {
        _connection = _connectionFactory.CreateConnection();

        //_connecton 'ın Uri değeri Program.cs içerisinde verildi.

        if(_channel is { IsOpen: true }) //channel zaten var ise
        {
            return _channel;
        }

        //channel yoksa oluşturalım
        _channel = _connection.CreateModel();

        //channel üzerinden Exchange Declare edelim
        _channel.ExchangeDeclare(ExchangeName, type:"direct", durable:true, false);

        //channel üzerinden Queue Declare edelim
        _channel.QueueDeclare(QueueName, durable: true, false, false, null);

        _channel.QueueBind(ExchangeName,QueueName,RoutingWatermark);

        _logger.LogInformation("RabbitMQ ile baglantı kuruldu...");

        return _channel; //mesajları göndermek üzere _channel geriye döndürüldü.
    }

    public void Dispose()
    {
        _connection?.Close();
        _connection?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
        _logger.LogInformation("RabbitMQ ile baglantı koptu...");
    }
}
