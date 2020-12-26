using Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
            string srvCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);//naziv je dodjeljen po korisniku koji je pokrenuo proces,pozivamo fju ParseName kako bi dobili naziv serverkog sertifikata

            NetTcpBinding binding2 = new NetTcpBinding();
            binding2.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            
            string address2 = "net.tcp://localhost:9998/IWcfService";
            //connect to cms
            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:9999/ICertificateManager";

            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;


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


                            string cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

                            List<string> Lista = new List<string>();
                            bool nadjeno = false;
                            string myName = WindowsIdentity.GetCurrent().Name.Split('\\')[1];
                            X509Certificate2 certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, myName);
                            using (StreamReader sr = new StreamReader("..//..//..//Lista//RevocationList.txt"))
                            {
                                string line;
                                while ((line = sr.ReadLine()) != null)
                                {
                                    Lista.Add(line);
                                }
                            }

                            foreach (string item in Lista)
                            {
                                if (item == certificate.Thumbprint)
                                {
                                    nadjeno = true;
                                    break;
                                }
                            }
                            if (nadjeno)
                            {
                                Console.WriteLine("Nemate sertifikat.");
                                break;
                            }
                            else
                            {
                                host.Credentials.ClientCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.ChainTrust;//definisanje tipa validacije

                                host.Credentials.ServiceCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, srvCertCN);//dodajemo sertifikat na host, metoda GetCertificateFromStorage nam vraca sertifikat i podesava ga u Certificate (host.Credentials.ServiceCertificate.Certificat)

                                host.Open();

                                Console.WriteLine("Server podignut");
                                break;
                            }


                           
                            

                        case 4:
                            string myName1 = WindowsIdentity.GetCurrent().Name.Split('\\')[1];
                            X509Certificate2 certificate1 = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, myName1);
                            Console.WriteLine(proxy.AddToRevocationList(certificate1) ); 
                            break;

                    }
                } while (option != 0);

            }

            Console.ReadLine();

        }
    }
}
