using FileCreateWorkerService;
using FileCreateWorkerService.Services;
using RabbitMQ.Client;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {

        IConfiguration Configuration = context.Configuration;
        services.AddSingleton(sp => new ConnectionFactory()
        {
            Uri = new Uri(Configuration.GetConnectionString("RabbitMq")),
            DispatchConsumersAsync = true
        });

        services.AddSingleton<RabbitMQClientService>();

        services.AddHostedService<Worker>();
    }).Build();

await host.RunAsync();
