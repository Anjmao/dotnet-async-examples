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
            WriteLine($"Start thread {ThreadId}");
            WriteLine("------------------------------------------");
            var timer = new Stopwatch();
            timer.Start();
            MainAsync().GetAwaiter().GetResult();
            timer.Stop();
            TimeSpan ts = timer.Elapsed;
            string elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds:000}";
            WriteLine("------------------------------------------");
            WriteLine($"End in {elapsedTime}");
            WriteLine($"End thread {ThreadId}");
        }

        private static async Task MainAsync()
        {
            var samples = new BasicSamples();

            //await samples.SimpleTask();

            //await samples.SimpleTaskWithResult();

            //await samples.SimpleAsync();

            //var result = await samples.SimpleAsyncWithResult();
            //WriteLine($"Task result: {result}");

            //await samples.ThreadContextChanges();

            //await samples.HttpRequest();

            //await samples.CpuJob();

            //await samples.StartingTasks();

            //await samples.MoreOnTaskFactoryStartNew();

            //await samples.AwaitInOrder();

            //await samples.AwaitInParallel();

            //samples.BlockCurrentThreadWhileWaiting();

            //await samples.CatchAsyncExeption();

            //samples.CatchAsyncExeptionWithWait();

            //samples.WrongExceptionCatching();

            //samples.WrongExceptionCatching2();

            //var result = await samples.FakeAsync();
            //WriteLine($"{result}");

            //var result = await samples.FakeAsync2();
            //WriteLine($"{result}");

            //await samples.FakeAsync3();

            var advanced = new AdvancedSamples();

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
