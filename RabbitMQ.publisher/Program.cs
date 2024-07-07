using RabbitMQ.Client;
using System.Text;

//baglantıyı gercekleştirildi
var factory = new ConnectionFactory();
factory.Uri = new Uri("amqps://teslduyy:ESr7N7TN_h5rGVk75-WpqGkmzTrceyBp@moose.rmq.cloudamqp.com/teslduyy");

//baglantı acıldı
using var connection = factory.CreateConnection();

//kanal oluşturuldu
//bu kanal üzerinden artık rabbitMq ile haberlesme saglanacak
var channel = connection.CreateModel();

//haberlesme saglanması icin mevcutta bir kuyrugun olması gerekiyor
//kuyruk olusturulurken girilen parametreler sırasıyla,

//1. parametre queue --> kuyruk adı

//2. parametre durable --> rabbitMq 'ya restart atıldıgında içerisindeki datalar fiziksel olarak diske kaydedilir ve kaybolmamış olur.
//true : fiziksel olarak kaydet, datalar silinmesin, false : kaydetme, datalar silinsin

//3. parametre exclusive --> olusturulan bu kuyruga farklı kanallar üzerinden de baglanılabilsin mi?
//true : sadece bu kanal baglanalabilsin, false : farklı kanallarda baglanalabilsin

//4. parametre autodelete --> kuyrugu dinleyen subscriber 'ın iletişimi koparsa ve kuyrugu
//dinleyen bir subscribe kalmaz ise kuyrukta silinsin mi?
//true : evet silinsin, false : silinmesin, kuyruk her zaman ayakta kalsın

//channel.QueueDeclare("hello-queue", true, false, false);

//Fanout Exchange tanımlama işlemi

//1. parametre exchange --> exchange adı

//2. parametre durable --> true: uygulamaya restart atılırsa exchange 'im kaybolmasın, fiziksel olarak kaydedilsin

//3. parametre type ---> exchange tipinin fanout oldugunu belirttim

channel.ExchangeDeclare("logs-topic",durable: true, type: ExchangeType.Topic);

//DirectExchange adımları, 4 farklı log seviyesi için 4 farklı queue ve route oluşturuldu

Enum.GetNames(typeof(LogNames)).ToList().ForEach(x =>
{
    ////DirectExchange
    //var queueName = $"direct-queue-{x}";
    //channel.QueueDeclare(queueName, true, false, false);
    //channel.QueueBind(queueName, "logs-direct", routeKey,null);

    ////olusturdugum kuyrugu DirectExchange 'e bind ediyorum ve 
    ////bind sırasında route key 'in girilmesi gerekiyor
    ////DirectExchange için oncelikle routingkey 'in olusturulması gerekiyor
    //var routeKey = $"route-{x}";
});

//50 tane mesaj gönderildi.
Enumerable.Range(1, 50).ToList().ForEach(x =>
{
    //her mesaj üretilip iletilirken random bir log seviyesi üretip ona göre ilgili
    //queue 'ya gönderilecek
    //LogNames log = (LogNames)new Random().Next(1,5);

    

    ////mesajı kuyruga bırabilirim artık.

    ////exchange kullanmadıgım için 1. parametrem string.Empty ve 2. parametre kuyrugun adı.

    ////fanout exchange kullanacagım için 1. parametrem exchange 'in adını alacak ve
    ////2. parametre boş kalacak cunku artık root key 'e ihtiyac yok bunu exchange yapacak.
    ////channel.BasicPublish("logs-fanout", "", null, messageBody);

    ////DirectExchange için oncelikle routingkey 'in olusturulması gerekiyor
    //var routeKey = $"route-{log}";

    //TopicExchange için random kuyruklar ürettim
    Random rnd = new Random();
    LogNames log1 = (LogNames)rnd.Next(1, 5);
    LogNames log2 = (LogNames)rnd.Next(1, 5);
    LogNames log3 = (LogNames)rnd.Next(1, 5);

    var routeKey = $"{log1}.{log2}.{log3}";

    //kuyruga bırakılacak mesaj oluşturuldu.
    string message = $"{x}. log-type: {log1}-{log2}-{log3}";

    //rabbitMq 'ya veriler byte olarak gönderilir
    var messageBody = Encoding.UTF8.GetBytes(message);
    channel.BasicPublish("logs-topic", routeKey, null, messageBody);

    Console.WriteLine($"Log gönderilmiştir: {message}");
});

Console.ReadLine();

public enum LogNames
{
    Critical = 1,
    Error = 2,
    Warning = 3,
    Info = 4
};