using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Threading;

using static System.Console;
using Dapper;

namespace ConsoleSamples
{
    public class BasicSamples
    {
        private int ThreadId => Thread.CurrentThread.ManagedThreadId;

        // 1. Show with exception
        public Task SimpleTask()
        {
            var task = Task.Delay(100);

            return task.ContinueWith((next) => {
                WriteLine($"SimpleTask continue");
            });
        }

        public Task<string> SimpleTaskWithResult()
        {
            Task<string> task = Task.FromResult("Hello");
            return task.ContinueWith((next) =>
            {
                WriteLine($"SimpleTaskWithResult: {next.Result}");
                return next.Result;
            });
        }

        public async Task SimpleAsync()
        {
            await Task.Delay(100);
            WriteLine($"Hello after 100ms");
        }

        public async Task<string> SimpleAsyncWithResult()
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

        public async Task StartingTasks()
        {
            // very bad
            var thread = new Thread(new ThreadStart(ThreadStartMethod));
            thread.Start();

            // bad
            var taskFromCtr = new Task(() => WriteLine($"Starting new task with ctr. Thread id {ThreadId}"));
            taskFromCtr.Start();

            // better
            var taskFromFactory = Task.Factory.StartNew(() => WriteLine($"Starting task from factory {ThreadId}"));

            // best
            var taskFromRun = Task.Run(() => WriteLine($"Starting task from run {ThreadId}"));

            await Task.WhenAll(taskFromCtr, taskFromFactory, taskFromRun);
        }

        public async Task MoreOnTaskFactoryStartNew() 
        {
            // StartNew does not support async delegates and have confusing TaskScheduler
            await Task.Factory.StartNew(() => WriteLine($"Default StartNew Thread {ThreadId}"));

            // reliably, predictably queue work to the thread pool
            await Task.Factory.StartNew(() => WriteLine($"StartNew with correct overloads. Thread {ThreadId}"), 
                    CancellationToken.None, 
                    TaskCreationOptions.DenyChildAttach, 
                    TaskScheduler.Default);
            
            // or just simple
            await Task.Run(() => WriteLine($"Run {ThreadId}"));          
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

        public void BlockCurrentThreadWhileWaiting()
        {
            WriteLine($"Starting on thread {ThreadId}");
            Task task = Task.Run(() =>
            {
                WriteLine($"Running on thread {ThreadId}");
            });
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
                WriteLine(ex);
            }
        }

        public void WrongExceptionCatching2()
        {
            try
            {
                AsyncVoid();
            }
            catch (Exception ex)
            {
                WriteLine(ex);
            }
        }

        private async void AsyncVoid()
        {
            await Task.Delay(100);
            throw new Exception("Void ups");
        }

        public Task<string> FakeAsync()
        {
            return Task.FromResult("So fake async");
        }

        public Task<string> FakeAsync2()
        {
            return Task.Run(() => "One more fake async");
        }

        public async Task FakeAsync3() 
        {
            var response = await new MyHelloAspNetRequestMediaFormatter().FormatResponseAsync("hello");
            WriteLine($"Response {response}");
        }

        private void ThreadStartMethod() 
        {
            WriteLine($"Starting new thread. Thread id {ThreadId}");
        }
    }

    public class AspNetRequestMediaFormatter 
    {
        public virtual Task<string> FormatResponseAsync(string request) 
        { 
            throw new NotImplementedException("base");
        }
    }

    public class MyHelloAspNetRequestMediaFormatter : AspNetRequestMediaFormatter
    {
        public override Task<string> FormatResponseAsync(string request) 
        {
            if (request == "hello") 
            {
                return Task.Run(() => "my hello");
            }

            return base.FormatResponseAsync(request);
        }
    }   
}