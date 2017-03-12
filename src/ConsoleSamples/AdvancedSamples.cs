using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Linq;
using System.Threading;

using static System.Console;
using System.Runtime.CompilerServices;

namespace ConsoleSamples
{
    public class SomeOldLib
    {
        public delegate void EventHandler(string value);

        public delegate void ErrorHandler(Exception exception);

        public event EventHandler OnComplete;

        public event ErrorHandler OnError;

        public void Run(string value)
        {
            Task.Run(() =>
            {
                var result = new string(value.ToArray().Reverse().ToArray());
                if (OnComplete != null)
                {
                    OnComplete(result);
                }
            });
        }
    }

    public class AdvancedSamples
    {
        public Task<string> TaskCompletionSourceFormEvents()
        {
            var tsc = new TaskCompletionSource<string>();

            var oldLib = new SomeOldLib();
            oldLib.Run("Hello");
            oldLib.OnComplete += (value) => tsc.SetResult(value);
            oldLib.OnError += (exception) => tsc.SetException(exception);
            return tsc.Task;
        }

        public async Task CancellationTokenSource()
        {
            try
            {
                var source = new CancellationTokenSource();
                source.CancelAfter(500);

                await new HttpClient().GetAsync("http://dotnetcrowd.lt/", source.Token);

                source.Dispose();
            }
            catch (TaskCanceledException ex)
            {
                WriteLine($"Task cancellation exception: {ex}");
            }
        }
        
        public async Task AwaitExtensions()
        {
            await TimeSpan.FromSeconds(1);
            WriteLine($"Done");
        }

        public async Task CustomAwaiter()
        {
            var customTask = new Task2<string>(() => "Hello");
            var result = await customTask;
            WriteLine($"Custom waiter result: {result}");
        }
    }

    public static class AwaitExtensions
    {
        public static TaskAwaiter GetAwaiter(this TimeSpan timeSpan)
        {
            return Task.Delay(timeSpan).GetAwaiter();
        }

        public static TaskAwaiter GetAwaiter(this Int32 millisecondsDue)
        {
            return TimeSpan.FromMilliseconds(millisecondsDue).GetAwaiter();
        }
    }

    public class Task2<T> where T : class
    {
        private readonly Func<T> _action;

        public Task2(Func<T> action)
        {
            _action = action;
        }

        public Task2Awaiter<T> GetAwaiter()
        {
            return new Task2Awaiter<T>(this._action);
        }
    }

    public struct Task2Awaiter<T> : INotifyCompletion where T : class
    {
        private readonly Func<T> _action;

        public Task2Awaiter(Func<T> action)
        {
            _action = action;
        }

        public bool IsCompleted
        {
            get
            {
                return false;
            }
        }

        public void OnCompleted(Action continuation)
        {
            var timer = new System.Threading.Timer((state) =>
            {
                continuation();
            },
            null, 1000, Timeout.Infinite);
        }

        public T GetResult()
        {
            return _action();
        }
    }
}