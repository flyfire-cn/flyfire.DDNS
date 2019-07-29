using System;
using System.Threading;
using System.Timers;

namespace flyfire.DDNS.Client
{
    class Program
    {
        private static System.Timers.Timer ddnsClientTimer = new System.Timers.Timer();
        private static readonly long checkCycle = 1000 * 60 * 10;
        private static DDNSClient ddnsClient = null;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello DDNS!\r\n"); 
            Console.WriteLine("https://www.cnblogs.com/flyfire-cn/"); 
            Console.WriteLine("mailto:liugang027@163.com");
            Console.WriteLine(Environment.NewLine);
            if (args.Length < 3)
            {
                Console.WriteLine("Parameter error, please check relevant configuration.");
                Thread.Sleep(3000);
                return;
            }
            string[] showName = { "HostName","UserName","Password","ServiceAddress", "ApiUri" };

            for (int i = 0; i < args.Length; i++)
            {
                Console.WriteLine(string.Format("{0}:{1}",showName[i],i==2?"******":args[i]));
            }
            switch (args.Length)
            {
                case 3:
                    ddnsClient = new DDNSClient(args[0], args[1], args[2]);
                    break;
                case 4:
                    ddnsClient = new DDNSClient(args[0], args[1], args[2], args[3]);
                    break;
                case 5:
                    ddnsClient = new DDNSClient(args[0], args[1], args[2], args[3], args[4]);
                    break;
            }
            Console.WriteLine($"\r\n{DateTime.Now}\tddns client start.\r\n");
            UpdateDDNS();

            StartDDNSClient();
            bool quit = false;
            while (!quit)
            {
                Console.WriteLine("\r\nPress Q or q Key exit.");
#if DEBUG
                var key = Console.ReadKey().KeyChar;
#else
                var key = Console.Read();
#endif
                Console.WriteLine();
                switch (key)
                {
                    case 'q':
                    case 'Q':
                        quit = true;
                        break;
                }
            }

            StopDDNSClient();
            ddnsClient = null;
            Console.WriteLine($"\r\n{DateTime.Now}\tddns client stop.\r\n");
        }

        static void StartDDNSClient()
        {
            ddnsClientTimer.Interval = checkCycle;
            ddnsClientTimer.Elapsed += ddnsClientTimer_Elapsed;
            ddnsClientTimer.Start();
        }

        static void StopDDNSClient()
        {
            if (ddnsClientTimer != null && ddnsClientTimer.Enabled)
            {
                ddnsClientTimer.Stop();
                ddnsClientTimer.Elapsed -= ddnsClientTimer_Elapsed;

            }
        }

        private static void ddnsClientTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            UpdateDDNS();
        }

        private static void UpdateDDNS()
        {
            if (ddnsClient.IsIpAddressChanged())
            {
                var result = ddnsClient.UpdateDDns();
                Console.WriteLine($"{DateTime.Now}\tupdate ddns. info:{result}");
            }
        }
    }
}
