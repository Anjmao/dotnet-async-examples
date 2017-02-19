using System;
using System.IO;
using System.Threading;
using Microsoft.AspNetCore.Hosting;

namespace webapi_samples
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel(opt => opt.ThreadCount = 4)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}