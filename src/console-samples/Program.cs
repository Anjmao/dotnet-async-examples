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
        public static void Main(string[] args)
        {
            Console.WriteLine($"Start");
            var timer = new Stopwatch();
            timer.Start();
            MainAsync().GetAwaiter().GetResult();
            timer.Stop();
            TimeSpan ts = timer.Elapsed;
            string elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
            Console.WriteLine($"End in {elapsedTime}");
        }

        private static async Task MainAsync()
        {
            Console.WriteLine($"Thread {ThreadId()}");
            await MixedSamples.RunAsync();
            Console.WriteLine($"Thread {ThreadId()}");
        }

        private static Func<int> ThreadId => () => Thread.CurrentThread.ManagedThreadId;
    }

    public class HttpClientSamples
    {
        public static async Task RunAsync()
        {
            var requests = new List<Request>
            {
                new Request("http://www.google.com"),
                new Request("http://www.bing.com"),
                new Request("http://www.yandex.com")
            };

            var tasks = requests.Select(ProcessApiRequestAsync);
            await Task.WhenAll(tasks);
        }

        public static async Task<HttpResponseMessage> ProcessApiRequestAsync(Request request)
        {
            using (var httpClient = new HttpClient())
            {
                Console.WriteLine($"Starting {request.Url} on thread {ThreadId()}");

                var response = await httpClient.GetAsync(request.Url);

                Console.WriteLine($"End request {request.Url} on thread {ThreadId()}");
                return response;
            }
        }

        private static Func<int> ThreadId => () => Thread.CurrentThread.ManagedThreadId;
    }

    public class CpuJobSamples
    {
        public static async Task RunAsync()
        {
            var jobs = new List<Job>
            {
                new Job("A", 1),
                new Job("B", 2)
            };

            var tasks = jobs.Select(ProcessJobAsync);
            await Task.WhenAll(tasks);
        }

        private static Task<bool> ProcessJobAsync(Job job)
        {
            return Task.Run(() =>
            {
                Console.WriteLine($"Starting {job.Id} on thread {ThreadId()}");
                var end = DateTime.Now + TimeSpan.FromSeconds(job.Seconds);
                while (DateTime.Now < end) ;
                Console.WriteLine($"Ended {job.Id} on thread {ThreadId()}");
                return true;
            });
        }

        private static Func<int> ThreadId => () => Thread.CurrentThread.ManagedThreadId;
    }

    public class MixedSamples
    {
        public static async Task RunAsync()
        {
            var requests = new List<Request>
            {
                new Request("http://www.google.com"),
                new Request("http://www.bing.com"),
                new Request("http://www.yandex.com")
            };

            var tasks = requests.Select(ProcessRequestAsync);
            await Task.WhenAll(tasks);
        }

        public static async Task<HttpResponseMessage> ProcessRequestAsync(Request request)
        {
            using (var httpClient = new HttpClient())
            {
                Console.WriteLine($"Starting {request.Url} on thread {ThreadId()}");

                var response = await httpClient.GetAsync(request.Url);
                var cpuJobResult = await ProcessJobAsync(new Job(request.Url, 2), response);

                Console.WriteLine($"End request {request.Url} on thread {ThreadId()}");
                return response;
            }
        }

        private static Task<bool> ProcessJobAsync(Job job, HttpResponseMessage rsp)
        {
            return Task.Run(() =>
            {
                Console.WriteLine($"Starting cpu job {job.Id} on thread {ThreadId()} {rsp.StatusCode}");
                var end = DateTime.Now + TimeSpan.FromSeconds(job.Seconds);
                while (DateTime.Now < end) ;
                Console.WriteLine($"Ended cpu job {job.Id} on thread {ThreadId()}");
                return true;
            });
        }

        private static Func<int> ThreadId => () => Thread.CurrentThread.ManagedThreadId;
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
