using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqps://teslduyy:ESr7N7TN_h5rGVk75-WpqGkmzTrceyBp@moose.rmq.cloudamqp.com/teslduyy");

var connection = factory.CreateConnection();

var channel = connection.CreateModel();

channel.ExchangeDeclare("tekrar-header-exchange", ExchangeType.Headers,true);

Dictionary<string,object> dic = new Dictionary<string,object>();

dic.Add("format","pdf");
dic.Add("shape","a4");

var properties = channel.CreateBasicProperties();
properties.Headers = dic;
properties.Persistent = true;

channel.BasicPublish("tekrar-header-exchange",string.Empty,properties,Encoding.UTF8.GetBytes("header mesaj"));

Console.WriteLine("Mesaj gönderilmiştir");

Console.ReadLine();