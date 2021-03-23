using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Message
    {
        public int Id { get; set; }
        public int Priority { get; set; }

        public void Process()
        {
            Console.WriteLine($"Processing message {Id}");
        }
    }
    [SuppressMessage("ReSharper", "UnusedVariable")]
    [SuppressMessage("ReSharper", "FunctionNeverReturns")]
    class Program
    {
        static void Main(string[] args)
        {
            var queue = new BlockingCollection<Message>();

            var consumer = Task.Run(() =>
            {
                while (true)
                {
                    // this will block until a message is available
                    var message = queue.Take();
                    message.Process();
                }
            });

            var count = 10;

            var consumers = Enumerable.Range(1, count).Select(async i => await Task.Run(() =>
            {
                while (true)
                {
                    // this will block until a message is available
                    var message = queue.Take();
                    message.Process();
                }
            }));


            for (int i = 0; i < 100; i++)
            {
                queue.Add(new Message { Id = i });
            }

            Console.WriteLine("Done");

            var high = new BlockingCollection<Message>();
            var normal = new BlockingCollection<Message>();

            var router = Task.Run(() =>
            {
                while (true)
                {
                    var message = queue.Take();
                    if (message.Priority > 5)
                    {
                        high.Add(message);
                    }
                    else
                    {
                        normal.Add(message);
                    }
                }
            });


            Console.ReadLine();


        }
    }
}
