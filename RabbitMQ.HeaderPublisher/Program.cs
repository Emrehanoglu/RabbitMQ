﻿using RabbitMQ.Client;
using System.Text;

//baglantıyı gercekleştirildi
var factory = new ConnectionFactory();
factory.Uri = new Uri("amqps://teslduyy:ESr7N7TN_h5rGVk75-WpqGkmzTrceyBp@moose.rmq.cloudamqp.com/teslduyy");

//baglantı acıldı
using var connection = factory.CreateConnection();

//kanal oluşturuldu
//bu kanal üzerinden artık rabbitMq ile haberlesme saglanacak
var channel = connection.CreateModel();

//1. parametre exchange --> exchange adı

//2. parametre durable --> rabbitMq 'ya restart atıldıgında içerisindeki datalar fiziksel olarak diske kaydedilir ve kaybolmamış olur.
//true : fiziksel olarak kaydet, datalar silinmesin, false : kaydetme, datalar silinsin

//3. parametre type ---> exchange tipinin Headers oldugunu belirttim

channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers);

//header olusturuldu

Dictionary<string,object> headers = new Dictionary<string, object>();
headers.Add("format", "pdf");
headers.Add("shape", "a4");

var properties = channel.CreateBasicProperties();
properties.Headers = headers;
properties.Persistent = true; //mesajlar kalıcı hale gelsin, yani durable:true gibi davransın

channel.BasicPublish("header-exchange", string.Empty, properties, Encoding.UTF8.GetBytes("header mesajım"));

Console.WriteLine("Mesaj gönderilmiştir");

Console.ReadLine();