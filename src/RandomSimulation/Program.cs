using log4net;
using RandomSimulationEngine;
using System;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;

namespace RandomSimulation
{
    public static class Program
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Program));

        private static readonly AutoResetEvent _closing = new AutoResetEvent(false);
        private static readonly TimeSpan _closingTimeout = TimeSpan.FromSeconds(10);
        private static Task _mainTask;
        private static IEngine _engine;

#warning TODO - zrobic automatycznego, długo działającego testera, któy sprawdzi randomowość w długim czasie i przy dużym obciążeniu

        [MTAThread]
        public static void Main()
        {
            _log.Info("Starting RandomSimulation");

            try
            {
                AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
                AssemblyLoadContext.Default.Unloading += Default_Unloading;

                _log.Info("Initializing injection container");
                _engine = ContainerRegistrator.Register().GetInstance<IEngine>();

                _mainTask = Task.Run(_engine.Start);

                Console.CancelKeyPress += OnExit;
                _closing.WaitOne();
            }
            catch (Exception ex)
            {
                _log.Fatal(ex);
            }
        }

        private static void Default_Unloading(AssemblyLoadContext obj)
        {
            _log.Debug(nameof(Default_Unloading));
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            try
            {
                _log.Info("Process exiting");

                _engine.Stop();

                _log.Debug("Waiting for main task to end");
                _mainTask.Wait(_closingTimeout);
            }
            catch (Exception ex)
            {
                _log.Fatal(ex);
            }
            finally
            {
                _log.Debug("All is finished");
            }
        }

        private static void OnExit(object sender, ConsoleCancelEventArgs args)
        {
            _log.Info("Exit invoked");
            _closing.Set();
        }
    }
}