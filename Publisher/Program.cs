using MassTransit;
using MassTransit.RabbitMqTransport;
using Messages;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Publisher
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            string host = "host";
            string vhost = "vhost";
            string username = "username";
            string password = "password";

            var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                IRabbitMqHost rmqHost = cfg.Host(host, vhost, h =>
                {
                    h.Username(username);
                    h.Password(password);
                });
            });

            bus.Start();

            Console.WriteLine("Нажмите Enter, чтобы опубликовать событийное сообщение.");
            Console.ReadKey();

            await bus.Publish<EventMessage>(new { Text = "Событийное сообщение: приходит ко всем потребителям." });

            Console.WriteLine("Нажмите Enter, чтобы опубликовать соревновательное сообщение.");
            Console.ReadKey();

            await bus.Publish<CompetingConsumersMessage>(new { Text = "Соревновательное сообщение: принимается только одним из подписчиков очереди." });

            Console.ReadKey();

            bus.Stop();
        }
    }
}

namespace Messages
{
    public interface EventMessage
    {
        string Text { get; set; }
    }

    public interface CompetingConsumersMessage
    {
        string Text { get; set; }
    }
}
