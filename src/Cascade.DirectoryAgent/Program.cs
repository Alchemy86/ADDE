using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Cascade.DirectoryAgent
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            var startup = new Startup();

            var hostBuilder = new HostBuilder()
                .ConfigureHostConfiguration(startup.ConfigureHostConfiguration)
                .ConfigureAppConfiguration(startup.ConfigureAppConfiguration)
                .ConfigureLogging(startup.ConfigureLogging)
                .ConfigureServices(startup.ConfigureServices)
                .Build();

            await hostBuilder.RunAsync();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            Log.Error("UnhandledException caught : " + ex.Message);
            Log.Error("UnhandledException StackTrace : " + ex.StackTrace);
            Log.Fatal("Runtime terminating: {0}", e.IsTerminating);
        }
    }
}
