using System;
using System.Threading;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.Hosting.Services;
using Microsoft.Owin.Hosting.Starter;

namespace MiniOwinHost
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length >= 2)
            {
                Usage();
                return 0;
            }

            string arg;

            if (args.Length == 1)
            {
                arg = args[0];
                if (arg == "help" || arg == "-help" || arg == "--help")
                {
                    Usage();
                    return 0;
                }
            }
            else
            {
                var portStr = Environment.GetEnvironmentVariable("PORT");
                int port;
                if (portStr != null && int.TryParse(portStr, out port))
                {
                    arg = string.Format("http://localhost:{0}/", port);
                }
                else
                {
                    arg = "http://localhost:5000/";
                }
            }

            try
            {
                var options = new StartOptions(arg);
                options.Settings["boot"] = "Domain";
                using (ServicesFactory.Create().GetService<IHostingStarter>().Start(options))
                {
                    Console.WriteLine(arg);

                    var resetEvent = new ManualResetEvent(false);
                    Console.CancelKeyPress += (sender, e) =>
                    {
                        e.Cancel = true;
                        resetEvent.Set();
                    };

                    resetEvent.WaitOne();
                    Console.WriteLine("Disposing...");
                }
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return 1;
            }
        }

        static void Usage()
        {
            Console.WriteLine("MiniOwinHost.exe [uri to listen on]");
        }
    }
}
