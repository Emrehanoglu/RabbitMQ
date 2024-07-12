
using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqps://teslduyy:ESr7N7TN_h5rGVk75-WpqGkmzTrceyBp@moose.rmq.cloudamqp.com/teslduyy");

var connection = factory.CreateConnection();

var channel = connection.CreateModel();

channel.QueueDeclare("merhaba-queue",durable:true,false,false);

var message = "merhaba dünya";

var messageBody = Encoding.UTF8.GetBytes(message);

channel.BasicPublish(string.Empty, "merhaba-queue", null, messageBody);