using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using RequestContract;
using RequestService;
namespace RequestHost
{
    class Program
    {
        static void Main(string[] args)
        {
            //Uri[] uri =new Uri[1];
            //uri[0] = new Uri("tcp:localhost");
            ServiceHost host = new ServiceHost(typeof(RequestService.RequestService));
            host.Opened += delegate
            {
                Console.WriteLine("RequestContract已经启动！");
            };
            //host.Opened += (x,y) => { };
            host.Open();
            Console.ReadKey();
        }
    }
}
