﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

//baglantıyı gercekleştirildi
var factory = new ConnectionFactory();
factory.Uri = new Uri("amqps://teslduyy:ESr7N7TN_h5rGVk75-WpqGkmzTrceyBp@moose.rmq.cloudamqp.com/teslduyy");

//baglantı acıldı
using var connection = factory.CreateConnection();

//kanal oluşturuldu
//bu kanal üzerinden artık rabbitMq ile haberlesme saglanacak
var channel = connection.CreateModel();

//subscriber 'lara mesajlar kacar kacar gelecek belirtildi.

//1. parametrenin 0 olması demek herhangi bir boyuttaki mesajın gönderilebileceği anlamına gelir

//2. parametre ile mesajların 1 'er 1 'er gönderileceği anlamına gelir

//3. parametre ile mesajların tek seferde mi gönderileceği yoksa eşit bir şekilde
//dagıtılarak mı gönderileceği anlamına gelir
//true: tek seferde 2. parametredeki sayı kadar mesaj gitsin,
//false : 2. parametredeki sayı kadar eşit dağıtılsın

channel.BasicQos(0, 1, false);

//haberlesme saglanması icin mevcutta bir kuyrugun olması gerekiyor
//kuyruk olusturulurken girilen parametreler sırasıyla,

//1. parametre queue --> kuyruk adı

//2. parametre durable --> rabbitMq 'ya restart atıldıgında içerisindeki datalar hafızaya kaydedilir ve kaybolmamış olur.
//true : fiziksel olarak kaydet, datalar silinmesin, false : kaydetme, datalar silinsin

//3. parametre exclusive --> olusturulan bu kuyruga farklı kanallar üzerinden de baglanılabilsin mi?
//true : sadece bu kanal baglanalabilsin, false : farklı kanallarda baglanalabilsin

//4. parametre autodelete --> kuyrugu dinleyen subscriber 'ın iletişimi koparsa ve kuyrugu
//dinleyen bir subscribe kalmaz ise kuyrukta silinsin mi?
//true : evet silinsin, false : silinmesin, kuyruk her zaman ayakta kalsın

//kuyruktaki mesajı okuyacak olan yapı
var consumer = new EventingBasicConsumer(channel);

//TopicExchange oldugu için kuyrugu burada declare ettim, rastgele bir kuyruk olusturuldu

var queueName = channel.QueueDeclare().QueueName;

//kuyruk oluştu ve ardından kuyruklar route 'lara bind edildi
//basında ve sonunda farklı string ifade bulunan ortasında Error ifadesi bulunan routin-key
//üzerinden gönderilmiş mesajlar dinlenecek
var routeKey = "*.Error.*";
channel.QueueBind(queueName, "logs-topic", routeKey);

//consume hangi kuyrugu okuyacak
//1. parametre kuyrugun adı,
//2. parametre ile subscriber tarafından alınan data doğruda işlense yanlış da işlense
//kuyruk üzerinde kalmaya devam etsin mi ?
//true : mesajı okudugun an sil, false : mesajı silme, dogru bir sekilde islenirse silmen
//için haber vereceğim
//3. parametre ile kuyruktaki mesajı okuyacak olan yapıyı veriyorum
channel.BasicConsume(queueName, false, consumer);


Console.WriteLine("Loglar Dinleniyor...");

//mesajı artık okuyabilirim
consumer.Received += (sender, args) =>
{
    var message = Encoding.UTF8.GetString(args.Body.ToArray());

    Thread.Sleep(1500);

    Console.WriteLine("Gelen Mesaj: " + message);

    //gelen mesajı txt dosyasına yazdırma,
    //C:\Users\Emre\Desktop\EmreHanoglu\RabbitMQ\RabbitMQ\RabbitMQ.subscriber\bin\Debug\net6.0
    //File.AppendAllText("log-critical.txt", message + "\n");

    //2. parametre ile sadece ilgili mesajın durumunu yani işlendiği bilgisini rabbitMq'ya
    //haber verir.
    channel.BasicAck(args.DeliveryTag, false);
};

Console.ReadLine();