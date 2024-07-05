using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;
using System.Text;
using System.Text.Json;

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

//kuyruktaki mesajı okuyacak olan yapı
var consumer = new EventingBasicConsumer(channel);

//haberlesme saglanması icin mevcutta bir kuyrugun olması gerekiyor
//rastgele bir kuyruk oluşturuldu

var queueName = channel.QueueDeclare().QueueName;

//hangi header dinlenecek belirtildi
Dictionary<string, object> headers = new Dictionary<string, object>();
headers.Add("format", "pdf");
headers.Add("shape", "a4");
headers.Add("x-match", "all");

//kuyruk ile exchange bind işlemi gercekleştirildi
channel.QueueBind(queueName, "header-exchange", string.Empty, headers);

//consumer hangi kuyrugu okuyacak
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

    Product product = JsonSerializer.Deserialize<Product>(message);

    Thread.Sleep(1500);

    Console.WriteLine($"Gelen Mesaj: {product.Id}-{product.Name}-{product.Price}-{product.Stock}");

    //gelen mesajı txt dosyasına yazdırma,
    //C:\Users\Emre\Desktop\EmreHanoglu\RabbitMQ\RabbitMQ\RabbitMQ.subscriber\bin\Debug\net6.0
    //File.AppendAllText("log-critical.txt", message + "\n");

    //2. parametre ile sadece ilgili mesajın durumunu yani işlendiği bilgisini rabbitMq'ya
    //haber verir.
    channel.BasicAck(args.DeliveryTag, false);
};

Console.ReadLine();