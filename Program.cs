using log4net;
using System;
using System.ServiceProcess;
using System.Threading.Tasks;


namespace IMPowershell
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        #region Service bits
        public const string ServiceName = "PowerShIM";

        public class Service : ServiceBase
        {
            public Service()
            {
                ServiceName = Program.ServiceName;
            }

            protected override void OnStart(string[] args)
            {
                Program.Start(args);
            }

            protected override void OnStop()
            {
                Program.Stop();
            }
        }
        #endregion

        private static LyncServer _server;
        
        static void Main(string[] args)
        {
          if (Environment.UserInteractive)
            {
                Start(args);
                Console.WriteLine("Press any key to stop...");
                Console.ReadKey(true);
                Stop();
            }
          else
            {
                using (var service = new Service())
                    ServiceBase.Run(service);
            }
        }

        private static void Start(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;
            _server = new LyncServer();
            _server.LyncServerReady += server_LyncServerReady;
            _server.IncomingCall += server_IncomingCall;
            Task t = _server.Start();
            log.Info("Started");
        }

        private static void Stop()
        {
            var stopping = _server.Stop();
            stopping.Wait();

            Console.WriteLine("Service Stopped. Exiting...");            
        }

        private static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            log.Error(e.ExceptionObject.ToString());
            if (Environment.UserInteractive)
            {
                Console.WriteLine("Error: {0}", e.ExceptionObject);
                Console.WriteLine("Press Enter to continue");
                Console.ReadLine();
            }
        }

        private static void server_LyncServerReady(object sender, EventArgs e)
        {
            Console.WriteLine("Lync Server Ready");
        }


        static void server_IncomingCall(object sender, Microsoft.Rtc.Collaboration.CallReceivedEventArgs<Microsoft.Rtc.Collaboration.InstantMessagingCall> e)
        {
            var ps = new PSRunner(e.Call, e.ToastMessage.Message);
            ps.ParseAndExecute();
        }
    }
}
