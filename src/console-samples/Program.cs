using System;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Threading;

namespace ConsoleApplication
{
    public class Request 
    {
        public string Url {get;}
        public int Value {get; set;}

        public Request(string url) 
        {
            Url = url;
        }
    }

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
            await WebApiRequests();
            Console.WriteLine($"Thread {ThreadId()}");
        }

        private static async Task WebApiRequests() 
        {
            var requests = new List<Request> 
            {
                new Request("http://www.google.com"),
                new Request("http://www.bing.com"),
                new Request("http://www.yandex.com")
            };

            var tasks = requests.Select(ProcessApiRequestAsync).ToArray();
            await Task.WhenAll(tasks);
        }

        public static async Task<HttpResponseMessage> ProcessApiRequestAsync(Request request) 
        {
            using(var httpClient = new HttpClient()) 
            {
                Console.WriteLine($"Starting {request.Url} on thread {ThreadId()}");

                var response = await httpClient.GetAsync(request.Url);
                
                Console.WriteLine($"End request {request.Url} on thread {ThreadId()}");
                return response;
            }
        }

        private static Func<int> ThreadId => () => Thread.CurrentThread.ManagedThreadId;
    }
}
