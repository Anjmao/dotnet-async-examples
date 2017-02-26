using System.Threading.Tasks;

namespace ConsoleSamples
{
    public class PerformanceSamples
    {
        public async Task AwaitInLoop()
        {
            for (var i = 0; i < 10000000; i++)
            {
                await RunAsync();
                //Run();
            }
        }

        private async Task RunAsync()
        {
        }

        private void Run()
        {
            for (var i = 0; i < 30; i++)
            {
            }
        }
    }
}