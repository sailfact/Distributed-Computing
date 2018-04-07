using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
namespace TrueMarbleData
{
    public class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = null;
            NetTcpBinding tcpBinding = new NetTcpBinding();
            //TMDataControllerImpl tMDataControllerImpl = new TMDataControllerImpl();
            string url = "net.tcp://localhost:50001/TMData";
            TMDataControllerImpl tMDataController = new TMDataControllerImpl();

            try        
            {
                // increases message quota to max       
                tcpBinding.MaxReceivedMessageSize = System.Int32.MaxValue;
                tcpBinding.ReaderQuotas.MaxArrayLength = System.Int32.MaxValue;

                host = new ServiceHost(tMDataController);   // host the implementing class
                host.AddServiceEndpoint(typeof(ITMDataController), tcpBinding, url);    // access via the interface class
                
                host.Open();        // enter listening state ready for client requests
                Console.WriteLine("Press Enter to exit");
                Console.ReadLine(); // block waiting for client requests
            }
            catch (FaultException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (ObjectDisposedException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (TimeoutException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (CommunicationObjectFaultedException e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                host.Close();
            }
        }
    }
}
