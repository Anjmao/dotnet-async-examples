using System;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Threading;

using static System.Console;
using Dapper;

namespace ConsoleApplication
{
    public class Program
    {
        private static int ThreadId => Thread.CurrentThread.ManagedThreadId;

        public static void Main(string[] args)
        {
            WriteLine("------------------------------------------");
            var timer = new Stopwatch();
            timer.Start();
            MainAsync().GetAwaiter().GetResult();
            timer.Stop();
            TimeSpan ts = timer.Elapsed;
            string elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
            WriteLine("------------------------------------------");
            WriteLine($"End in {elapsedTime}");
        }

        private static async Task MainAsync()
        {
            var samples = new Samples();

            //await samples.SimpleTask();

            //var result = await samples.TaskWithResult();
            //WriteLine($"Task result: {result}");

            //await samples.ThreadContextChanges();

            //await samples.HttpRequest();

            //await samples.CpuJob();

            //await samples.CatchAsyncExeption();

            //samples.CatchAsyncExeptionWithWait();

            //samples.BlockWithWait();

            //await samples.AwaitInOrder();

            //await samples.AwaitInParallel();
        }
    }

    public class Samples
    {
        private int ThreadId => Thread.CurrentThread.ManagedThreadId;

        public async Task SimpleTask()
        {
            await Task.Delay(100);
            WriteLine($"Hello after 100ms");
        }

        public async Task<string> TaskWithResult()
        {
            Task<string> task = Task.FromResult("Hello");
            string result = await task;
            return result;
        }

        public async Task ThreadContextChanges()
        {
            WriteLine($"Starting at thread {ThreadId}");
            await Task.Delay(100);
            WriteLine($"Ended on thread {ThreadId}");
        }

        public async Task HttpRequest()
        {
            var httpClient = new HttpClient();
            string result = await httpClient.GetStringAsync("https://dogescript.com");
            WriteLine($"Http content result length: {result.Length}");
        }

        public async Task CpuJob()
        {
            Task<int> cpuJob = Task.Run(() =>
            {
                var end = DateTime.Now + TimeSpan.FromSeconds(2);
                while (DateTime.Now < end) ;
                return 1;
            });

            int result = await cpuJob;

            WriteLine($"CPU job result: {result}");
        }

        public async Task AwaitInOrder()
        {
            var dbResult = await GetDbUsersAsync();
            var apiResult1 = await GetApiDataAsync("http://www.google.lt");
            var apiResult2 = await LongRunningApiOperationAsync(2000);
            var apiResult3 = await LongRunningApiOperationAsync(1000);

            WriteLine($"Db result: {dbResult.Count()}");
            WriteLine($"Api result: {apiResult1.Count()}");
            WriteLine($"Api 2 result: {apiResult2.Count()}");
            WriteLine($"Api 3 result: {apiResult3.Count()}");
        }

        public async Task AwaitInParallel()
        {
            Task<IEnumerable<dynamic>> dbTask = GetDbUsersAsync();
            Task<string> apiTask1 = GetApiDataAsync("http://www.google.lt");
            Task<string> apiTask2 = LongRunningApiOperationAsync(2000);
            Task<string> apiTask3 = LongRunningApiOperationAsync(1000);

            await Task.WhenAll(dbTask, apiTask1, apiTask2);

            var dbResult = await dbTask;
            var apiResult1 = await apiTask1;
            var apiResult2 = await apiTask2;
            var apiResult3 = await apiTask2;

            WriteLine($"Db result: {dbResult.Count()}");
            WriteLine($"Api 1 result: {apiResult1.Count()}");
            WriteLine($"Api 2 result: {apiResult2.Count()}");
            WriteLine($"Api 3 result: {apiResult3.Count()}");
        }

        private async Task<IEnumerable<dynamic>> GetDbUsersAsync()
        {
            using (var connection = new MySql.Data.MySqlClient.MySqlConnection("Server=127.0.0.1;Database=demo;Uid=root;Pwd=admin;SslMode=None;"))
            {
                return await connection.QueryAsync("SELECT * FROM user");
            }
        }

        private async Task<string> GetApiDataAsync(string url)
        {
            using (var client = new HttpClient())
            {
                return await client.GetStringAsync(url);
            }
        }

        private async Task<string> LongRunningApiOperationAsync(int milliseconds)
        {
            await Task.Delay(milliseconds);
            return "Long running operation completed";
        }

        public void BlockWithWait()
        {
            WriteLine($"Starting at thread {ThreadId}");
            Task task = Task.Delay(1000);
            task.Wait();
            WriteLine($"Ended on thread {ThreadId}");
        }

        public async Task CatchAsyncExeption()
        {
            try
            {
                await ThrowAsync();
            }
            catch (Exception ex)
            {
                WriteLine($"Exeption catched: {ex}");
            }
        }

        public void CatchAsyncExeptionWithWait()
        {
            try
            {
                ThrowAsync().Wait();
            }
            catch (Exception ex)
            {
                WriteLine($"Exeption catched: {ex}");
            }
        }

        private async Task ThrowAsync()
        {
            await Task.Delay(100);
            throw new Exception("Ups!");
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
                Task.Run(() =>
                {
                    throw new Exception("ups");
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void BlockingTask()
        {
            Task<string> task = Task.Run(() =>
            {
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
