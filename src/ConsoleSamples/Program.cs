using System;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

using static System.Console;

namespace ConsoleSamples
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
            string elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds}";
            WriteLine("------------------------------------------");
            WriteLine($"End in {elapsedTime}");
        }

        private static async Task MainAsync()
        {
            var samples = new BasicSamples();

            //await samples.SimpleTask();

            //var result = await samples.TaskWithResult();
            //WriteLine($"Task result: {result}");

            //await samples.ThreadContextChanges();

            //await samples.HttpRequest();

            //await samples.CpuJob();

            //await samples.AwaitInOrder();

            //await samples.AwaitInParallel();

            //samples.BlockCurrentThreadWhileWaiting();

            //await samples.CatchAsyncExeption();

            //samples.CatchAsyncExeptionWithWait();

            //samples.WrongExceptionCatching();

            //samples.WrongExceptionCatching2();

            //var result = await samples.FakeAsync();
            //WriteLine($"{result}");


            //var advanced = new AdvancedSamples();

            //var result = await advanced.TaskCompletionSourceFormEvents();
            //WriteLine($"Task completion source result: {result}");

            //await advanced.CancellationTokenSource();

            //await advanced.AwaitExtensions();

            //await advanced.CustomAwaiter();


            //var performance = new PerformanceSamples();

            //await performance.AwaitInLoop();
        }
    }
}
