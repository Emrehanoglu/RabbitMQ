using FileCreateWorkerService;
using FileCreateWorkerService.Models;
using FileCreateWorkerService.Services;
using Microsoft.EntityFrameworkCore;
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

        services.AddDbContext<AdventureWorks2019Context>(options =>
        {
            options.UseSqlServer(Configuration.GetConnectionString("SqlServer"));
        });

        services.AddSingleton<RabbitMQClientService>();

        services.AddHostedService<Worker>();
    }).Build();

await host.RunAsync();
