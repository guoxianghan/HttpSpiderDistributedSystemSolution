using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RequestContract;
using System.ServiceModel;
namespace RequestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            ChannelFactory<IRequestContract> channelFactory;
            using (channelFactory = new ChannelFactory<RequestContract.IRequestContract>("Request"))
            {
                IRequestContract ito = channelFactory.CreateChannel();

                while (true)
                {
                    string cmd = System.Console.ReadLine();

                    string res = ito.BrowserRequestHtml(1);
                    Console.WriteLine(res);

                }





                // action?.Invoke(channelFactory.CreateChannel());
            }
        }
    }
}
