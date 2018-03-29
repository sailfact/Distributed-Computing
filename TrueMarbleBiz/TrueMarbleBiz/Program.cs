using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TrueMarbleBiz
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host;
            NetTcpBinding tcpBinding = new NetTcpBinding();
            string url = "net.tcp://localhost:50002/TMBiz";

            // increases message quota to max
            try
            {
                tcpBinding.MaxReceivedMessageSize = System.Int32.MaxValue;
                tcpBinding.ReaderQuotas.MaxArrayLength = System.Int32.MaxValue;
            }
            catch (InvalidOperationException oe)
            {
                throw new FaultException(oe.Message);
            }

            host = new ServiceHost(typeof(TMBizControllerImpl));   // host the implementing class
            host.AddServiceEndpoint(typeof(ITMBizController), tcpBinding, url);    // access via the interface class

            try        
            {
                host.Open();        // enter listening state ready for client requests
                Console.WriteLine("Press Enter to exit");
                Console.ReadLine(); // block waiting for client requests
            }
            catch (ObjectDisposedException e1)
            {
                throw new FaultException(e1.Message);
            }
            catch (TimeoutException e2)
            {
                throw new FaultException(e2.Message);
            }
            catch (CommunicationObjectFaultedException e3)
            {
                throw new FaultException(e3.Message);
            }
            finally
            {
                host.Close();
            }
        }
    }
}
