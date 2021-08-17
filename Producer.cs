using System;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ChannelStudy
{
    public class Producer
    {
        private readonly ChannelWriter<string> _writer;
        private readonly int _identifier;
        private readonly int _delay;

        public Producer(ChannelWriter<string> writer, int identifier, int delay)
        {
            _writer = writer;
            _identifier = identifier;
            _delay = delay;
        }

        public async Task BeginProducing()
        {
            Console.WriteLine($"Producer ({_identifier}): Starting");

            for (int i = 0; i < 1000; i++)
             {
                await Task.Delay(_delay);
                
                var msg = $"P({_identifier}) - msg {i + 1} - {DateTime.UtcNow})";
                Console.WriteLine($"Producer ({_identifier}): Creating {msg}");

                await _writer.WriteAsync(msg);
            }

            Console.WriteLine($"Producer ({_identifier}): Completed");
        }
    }
}