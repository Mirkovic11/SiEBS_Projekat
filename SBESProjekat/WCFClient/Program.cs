using Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace WCFClient
{
    class Program
    {
        public static int brojac = 0;

        static void Main(string[] args)
        {
            bool serverPovukaoSert3 = false;
            bool serverPovukaoSert5 = false;

             Thread thread = new Thread(new ThreadStart(Obavijesti));//xD
             thread.Start();

            Console.WriteLine("Korisnik koji je pokrenuo klijenta: " + WindowsIdentity.GetCurrent().Name);

            string srvCertCN = "wcfservice";
            X509Certificate2 srvCert = null;
            bool a = false;
            while (true)
            {
                srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, srvCertCN);//podesavanje serverskog sertifikataeateUpnIdentity("wcfservice"));
                if (srvCert == null)
                {
                    if (a == false)
                    {
                        Console.WriteLine("Morate sacekati dok server ne instalira sertifikat.");
                        a = true;
                    }

                }
                else
                {
                    break;
                }
                Thread.Sleep(1000);
            }
            NetTcpBinding binding2 = new NetTcpBinding();

           
            EndpointAddress address2 = new EndpointAddress(new Uri("net.tcp://localhost:9998/IWcfService"),
                                      new X509CertificateEndpointIdentity(srvCert));

            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:9999/ICertificateManager";

            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;

            binding2.Security.Mode = SecurityMode.Transport;
            binding2.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

           
            using (ClientProxy proxy = new ClientProxy(binding, address)) //da radim ono za NTLM ovde bih prosledio endpointAddress
            {
                int option = 0;
                string certName = "";
                ClientProxyService proxy2 = null;

                do
                {
                    Console.WriteLine("----------------------------------------------------------\n" +
                        "Izaberite opciju:\n" +
                        "1 za kreiranje sertifikata sa svim kljucevima\n" +
                        "2 za kreiranje sertifikata bez privatnog kljuca\n" +
                        "3 za konekciju sa serverom\n" +
                        "4 za povlacenje sertifikata\n" +
                        "5 za javljanje serveru\n" +
                        "0 izlaz\n" +
                        "----------------------------------------------------------");

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
                             
                            try
                            {
                                if (serverPovukaoSert3)
                                {
                                    srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, srvCertCN);//podesavanje serverskog sertifikataeateUpnIdentity("wcfservice"));
                                    EndpointAddress address2nova = new EndpointAddress(new Uri("net.tcp://localhost:9998/IWcfService"),
                                  new X509CertificateEndpointIdentity(srvCert));

                                    proxy2 = new ClientProxyService(binding2, address2nova);
                                    Console.WriteLine(proxy2.TestCommunication());
                                    CloseProxy(proxy2);
                                }
                               else
                                {
                                    proxy2 = new ClientProxyService(binding2, address2);
                                    Console.WriteLine(proxy2.TestCommunication());
                                    CloseProxy(proxy2);
                                }
                            }
                            catch
                            {
                                Console.WriteLine("Greska pri konekciji");
                                serverPovukaoSert3 = true;
                            }
                            

                            break;
                        case 4:
                            string myName = WindowsIdentity.GetCurrent().Name.Split('\\')[1];
                            X509Certificate2 certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, myName);
                            if (certificate == null)
                            {
                                Console.WriteLine("Sertifikat je vec povucen");
                            }
                            else
                            {
                               // Thread.Sleep(1000);
                                Console.WriteLine(proxy.AddToRevocationList(certificate));
                                string msg = certificate.Thumbprint + " " + WindowsIdentity.GetCurrent().Name.Split('\\')[1];
                                Writer.WriteMsg(msg);
                            }

                            break;
                        case 5:
                            try
                            {
                                if (serverPovukaoSert5)
                                {
                                    srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, srvCertCN);//podesavanje serverskog sertifikataeateUpnIdentity("wcfservice"));
                                    EndpointAddress address2nova = new EndpointAddress(new Uri("net.tcp://localhost:9998/IWcfService"),
                                  new X509CertificateEndpointIdentity(srvCert));

                                    proxy2 = new ClientProxyService(binding2, address2nova);

                                    string name = WindowsIdentity.GetCurrent().Name.Split('\\')[1];
                                    X509Certificate2 cert = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, name);
                                    if (cert == null)
                                    {                                      
                                        break;
                                    }

                                    Console.WriteLine("Starting to ping server...");
                                    Random r = new Random();
                                    try
                                    {
                                        int brojac = 0;
                                        while (brojac < 10)
                                        {
                                            brojac++;
                                            Thread.Sleep(r.Next(1, 10) * 1000); //sleep 1-10s

                                            proxy2.PingServer(DateTime.Now);

                                        }
                                    }
                                    catch
                                    {                                      
                                       // Console.WriteLine("Greska pri konekciji - ovdje sam");
                                    }


                                    CloseProxy(proxy2);
                                }
                                else
                                {
                                    proxy2 = new ClientProxyService(binding2, address2); //nije podigao host ili povukao sertifikat

                                    string name = WindowsIdentity.GetCurrent().Name.Split('\\')[1];
                                    X509Certificate2 cert = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, name);
                                    if (cert == null)
                                    {
                                        //Console.WriteLine("Nema sertifikata");
                                        break;
                                    }

                                    Console.WriteLine("Starting to ping server...");
                                    Random r = new Random();
                                    try
                                    {
                                        int brojac = 0;
                                        while (brojac < 10)
                                        {
                                            brojac++;
                                            Thread.Sleep(r.Next(1, 10) * 1000); //sleep 1-10s
                                   
                                            proxy2.PingServer(DateTime.Now);
                                          
                                        }
                                    }
                                    catch
                                    {
                                     
                                        //Console.WriteLine("Greska pri konekciji - 12");
                                       
                                    }


                                    CloseProxy(proxy2);
                                }
                            }
                            catch
                            {
                               
                                Console.WriteLine("Greska pri konekciji.");
                                serverPovukaoSert5 = true;


                            }
                            break;
                     
                    }
                } while (option != 0);
             
            }


            

            Console.ReadLine();


        }


        public static void Obavijesti()
        {
            while (true)
            {
                int n = 0;
               // Thread.Sleep(2000);
                try
                {
                    List<string> lista = Reader.Read();
                    foreach (string item in lista)
                    {
                        n++;
                        try
                        {
                            if (n - 1 == brojac)
                            {

                                Console.WriteLine("Klijent " + item.Split(' ')[1] + " je povukao sertifikat");
                                brojac++;

                            }
                        }
                        catch
                        {

                        }
                       
                        
                    }
                }catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.Read();
                }
                
            }
        }

        public static void CloseProxy(ClientProxyService proxy)
        {
            proxy.Close();
            using (StreamWriter sw = new StreamWriter("..//..//..//Lista//Diskonektovani.txt", true))
            {
                string name = WindowsIdentity.GetCurrent().Name.Split('\\')[1];
                sw.WriteLine(name);
            }
        }

    }
}
