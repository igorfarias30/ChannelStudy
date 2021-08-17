using System;
using System.Threading.Channels;
using System.Threading.Tasks;

// https://www.stevejgordon.co.uk/an-introduction-to-system-threading-channels
namespace ChannelStudy
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("1. Single producer, single consumer.");
            Console.WriteLine("2. Multiple producers, single consumer.");
            Console.WriteLine("3. Single producer, multiple consumers.");
            Console.WriteLine("Which sample do you want to run?");

            var key = '3'; // Console.ReadKey();

            switch(key)
            {
                case '1':
                    await SingleProducerSingleConsumer();
                    break;
                case '2': 
                    await MultipleProducersSingleConsumer();
                    break;
                case '3':
                    await SingleProduceMultipleConsumers();
                    break;
                default:
                    Console.WriteLine("That was an invalid choice!");
                    break;
            };
        }

        public static async Task SingleProducerSingleConsumer()
        {
            var channel = Channel.CreateUnbounded<string>();

            var producer = new Producer(channel.Writer, 1, 5000);
            var consumer = new Consumer(channel.Reader, 1, 1500);

            Task consumerTask = consumer.ConsumeData();
            Task producerTask = producer.BeginProducing();

            await producerTask.ContinueWith((_) => channel.Writer.Complete());
            await consumerTask;
        }

        public static async Task MultipleProducersSingleConsumer()
        {
            var channel = Channel.CreateUnbounded<string>();

            var firstProducer = new Producer(channel.Writer, 1, 2000);
            var secondProducer = new Producer(channel.Writer, 2, 2000);
            var consumer = new Consumer(channel.Reader, 1, 250);

            Task consumerTask = consumer.ConsumeData();
            Task firstProducerTask = firstProducer.BeginProducing();
            await Task.Delay(500);
            Task secondProducerTask = secondProducer.BeginProducing();

            await Task.WhenAll(firstProducerTask, secondProducerTask)
                .ContinueWith(_ => channel.Writer.Complete());
            
            await consumerTask;
        }

        public static async Task SingleProduceMultipleConsumers()
        {
            var channel = Channel.CreateUnbounded<string>();

            var producer = new Producer(channel.Writer, 1, 100);
            var firstConsumer = new Consumer(channel.Reader, 1, 200);
            var secondConsumer = new Consumer(channel.Reader, 2, 500);
            var thirdConsumer = new Consumer(channel.Reader, 3, 1500);

            Task firstConsumerTask = firstConsumer.ConsumeData();
            Task secondConsumerTask = secondConsumer.ConsumeData();
            Task thirdConsumerTask = thirdConsumer.ConsumeData();

            Task producerTask = producer.BeginProducing();

            await producerTask.ContinueWith(_ => channel.Writer.Complete());
            await Task.WhenAll(firstConsumerTask, secondConsumerTask, thirdConsumerTask);
        }
    }
}
