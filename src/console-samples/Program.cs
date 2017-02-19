using System;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Threading;

namespace ConsoleApplication
{
    public class Program
    {
        private static int ThreadId => Thread.CurrentThread.ManagedThreadId;

        public static void Main(string[] args)
        {
            Console.WriteLine($"Start on thread {ThreadId}");
            var timer = new Stopwatch();
            timer.Start();
            MainAsync().GetAwaiter().GetResult();
            timer.Stop();
            TimeSpan ts = timer.Elapsed;
            string elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
            Console.WriteLine($"End in {elapsedTime} of thread {ThreadId}");
        }

        private static async Task MainAsync()
        {
            //var operation = new CpuBoundOperation();
            //await operation.RunAsync();

            var mistakes = new CommonMistakesOperation();
            mistakes.UsingThreadSleep();
        }
    }

    public class IOBoundOperation
    {
        public async Task RunAsync()
        {
            var requests = new List<Request>
            {
                new Request("http://www.google.com"),
                new Request("http://www.google.com")
            };

            var tasks = requests.Select(ProcessApiRequestAsync);
            await Task.WhenAll(tasks);
        }

        private async Task<HttpResponseMessage> ProcessApiRequestAsync(Request request)
        {
            using (var httpClient = new HttpClient())
            {
                Console.WriteLine($"Starting {request.Url} on thread {ThreadId}");

                var response = await httpClient.GetAsync(request.Url);

                Console.WriteLine($"End request {request.Url} on thread {ThreadId}");
                return response;
            }
        }

        private int ThreadId => Thread.CurrentThread.ManagedThreadId;
    }

    public class CpuBoundOperation
    {
        public async Task RunAsync()
        {
            var jobs = new List<Job>
            {
                new Job("A", 3),
                new Job("B", 2)
            };

            var tasks = jobs.Select(ProcessJobAsync);
            await Task.WhenAll(tasks);
        }

        private Task<bool> ProcessJobAsync(Job job)
        {
            return Task.Run(() =>
            {
                throw new Exception("ups");
                Console.WriteLine($"Starting {job.Id} on thread {ThreadId}");
                var end = DateTime.Now + TimeSpan.FromSeconds(job.Seconds);
                while (DateTime.Now < end) ;
                Console.WriteLine($"Ended {job.Id} on thread {ThreadId}");
                return true;
            });
        }

        private int ThreadId => Thread.CurrentThread.ManagedThreadId;
    }

    public class CommonMistakesOperation
    {
        public void WrongExceptionCatching() 
        {
            try 
            {
                Task.Run(() => {
                    throw new Exception("ups");
                });
            }
            catch(Exception ex) 
            {
                Console.WriteLine(ex);
            }
        }

        public void BlockingTask()
        {
            Task<string> task = Task.Run(() => {
                return $"Hello from another task with thread {ThreadId}";
            });
            Console.WriteLine(task.Result);
        }

        public void UsingThreadSleep()
        {
            Thread.Sleep(1000);
        }

        private int ThreadId => Thread.CurrentThread.ManagedThreadId;
    }

    public class Job
    {
        public Job(string id, int time)
        {
            Id = id;
            Seconds = time;
        }
        public string Id { get; }
        public int Seconds { get; }
    }

    public class Request
    {
        public string Url { get; }
        public int Value { get; set; }

        public Request(string url)
        {
            Url = url;
        }
    }
}
