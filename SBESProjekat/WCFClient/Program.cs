using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WCFClient
{
    class Program
    {
        static void Main(string[] args)
        {

            string srvCertCN = "wcfservice";
            X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, srvCertCN);//podesavanje serverskog sertifikataeateUpnIdentity("wcfservice"));

            NetTcpBinding binding = new NetTcpBinding();
            NetTcpBinding binding2 = new NetTcpBinding();
            string address = "net.tcp://localhost:9999/ICertificateManager";


            EndpointAddress address2 = new EndpointAddress(new Uri("net.tcp://localhost:9998/IWcfService"),
                                      new X509CertificateEndpointIdentity(srvCert));


            binding2.Security.Mode = SecurityMode.Transport;
            binding2.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            

            Console.WriteLine("Korisnik koji je pokrenuo klijenta: " + WindowsIdentity.GetCurrent().Name);
           

            using (ClientProxy proxy = new ClientProxy(binding, address)) //da radim ono za NTLM ovde bih prosledio endpointAddress
            {
                int option = 0;
                string certName = "";

                //proxy.createTrustedRootCA("TestCAClient");

                do
                {
                    Console.WriteLine("Unesi 1 za kreiranje sertifikata sa svim kljucevima, 2 za kreiranje sertifikata bez privatnog kljuca, 3 konekcija sa serverom, 4 povlacenje sertifikata, 0 za izlaz");
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
                            try { 
                            using (ClientProxyService proxy2 = new ClientProxyService(binding2, address2))
                            {
                                Console.WriteLine(proxy2.TestCommunication()); 
                            }
                            }
                            catch
                            {

                            }
                            break;
                        case 4:
                            string myName = WindowsIdentity.GetCurrent().Name.Split('\\')[1];
                            X509Certificate2 certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, myName);
                            Console.WriteLine(proxy.AddToRevocationList(certificate) ); 
                            break;

                    }
                } while (option != 0);
             
            }


            

            Console.ReadLine();


        }
    }
}
