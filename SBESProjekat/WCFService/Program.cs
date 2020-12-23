using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WCFService
{
    class Program
    {
        static void Main(string[] args)
        {
            NetTcpBinding binding = new NetTcpBinding();
            NetTcpBinding binding2 = new NetTcpBinding();
            string address = "net.tcp://localhost:9999/ICertificateManager";
            string address2 = "net.tcp://localhost:9998/IWcfService";


            using (ServiceProxy proxy = new ServiceProxy(binding, address)) //da radim ono za NTLM ovde bih prosledio endpointAddress
            {

                int option = 0;
                string certName = "";

                //proxy.createTrustedRootCA("TestCAService");

                do
                {
                    Console.WriteLine("Unesi 1 za kreiranje sertifikata sa svim kljucevima, 2 za kreiranje sertifikata bez privatnog kljuca, 3 izlaz");
                    int.TryParse(Console.ReadLine(), out option);

                    switch (option)
                    {
                        case 1:
                            Console.WriteLine("Unesite ime sertifikata koji hocete da napravite");
                            certName = Console.ReadLine();
                            proxy.createCertificateWithallKeys("TestCA", certName);
                            break;
                        case 2:
                            Console.WriteLine("Unesite ime sertifikata koji hocete da napravite");
                            certName = Console.ReadLine();
                            proxy.createCertificateWithoutPrivateKey("TestCA", certName);
                            break;
                        case 3:
                            ServiceHost host = new ServiceHost(typeof(WcfSevice));
                            host.AddServiceEndpoint(typeof(IWcfService), binding2, address2);
                            host.Open();
                            Console.WriteLine("Server podignut");
                            break;
                    }
                } while (option != 3);

            }

            Console.ReadLine();

        }
    }
}
