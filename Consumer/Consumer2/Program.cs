using MassTransit;
using MassTransit.RabbitMqTransport;
using Messages;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Consumer2
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

                cfg.ReceiveEndpoint(rmqHost, "consumer2-event-queue", e =>
                {
                    e.Consumer<EventConsumer>();
                });

                cfg.ReceiveEndpoint(rmqHost, "competing-consumer-queue", e =>
                {
                    e.Consumer<CompetingConsumer>();
                });
            });

            bus.Start();

            bool finish = false;

            while (!finish)
            {
                await Task.Delay(1000);
            }

            bus.Stop();
        }
    }

    public class EventConsumer : IConsumer<EventMessage>
    {
        public async Task Consume(ConsumeContext<EventMessage> context)
        {
            await Console.Out.WriteLineAsync(context.Message.Text);
        }
    }

    public class CompetingConsumer : IConsumer<CompetingConsumersMessage>
    {
        public async Task Consume(ConsumeContext<CompetingConsumersMessage> context)
        {
            await Console.Out.WriteLineAsync(context.Message.Text);
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