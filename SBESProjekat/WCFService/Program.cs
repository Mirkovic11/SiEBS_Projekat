using Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WCFService
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Korisnik koji je pokrenuo server: " + WindowsIdentity.GetCurrent().Name);

            LogData.InitializeServerEventLog();
            Thread thread = new Thread(new ThreadStart(ClosingClientConnection));//xD
            thread.Start();

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

            

            bool otvoreno = false;
            using (ServiceProxy proxy = new ServiceProxy(binding, address)) //da radim ono za NTLM ovde bih prosledio endpointAddress
            {
                
                int option = 0;
                string certName = "";
                ServiceHost host=null;

                do
                {
                    Console.WriteLine("--------------------------------------------------------------\n" +
                        "Izaberite opciju:" +
                        "\n1 za kreiranje sertifikata sa svim kljucevima\n" +
                        "2 za kreiranje sertifikata bez privatnog kljuca\n" +
                        "3 za podizanje servera\n" +
                        "4 za povlacenje sertifikata" +
                        "\n0 izlaz\n--------------------------------------------------------------");
               
               
               
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
                            if (host == null || host.State == CommunicationState.Closed )
                            {

                                
                            host = new ServiceHost(typeof(WcfSevice));
                            host.AddServiceEndpoint(typeof(IWcfService), binding2, address2);

                            string cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

                            List<string> Lista = new List<string>();
                            bool nadjeno = false;
                            string myName = WindowsIdentity.GetCurrent().Name.Split('\\')[1];
                            X509Certificate2 certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, myName);

                            if (certificate == null)
                            {
                                Console.WriteLine("Nemate sertifikat");
                            }
                            else
                            {
                                host.Credentials.ClientCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.ChainTrust;//definisanje tipa validacije
                                host.Credentials.ServiceCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, srvCertCN);//dodajemo sertifikat na host, metoda GetCertificateFromStorage nam vraca sertifikat i podesava ga u Certificate (host.Credentials.ServiceCertificate.Certificat)
                                host.Open();

                                ServiceSecurityAuditBehavior audit = new ServiceSecurityAuditBehavior();
                                audit.AuditLogLocation = AuditLogLocation.Application;
                                host.Description.Behaviors.Remove<ServiceSecurityAuditBehavior>();
                                host.Description.Behaviors.Add(audit);

                                Console.WriteLine("Server podignut");
                            }
                            }else
                            {
                                //Console.WriteLine("Vec sam otvoren");
                                string cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

                                List<string> Lista = new List<string>();
                                bool nadjeno = false;
                                string myName = WindowsIdentity.GetCurrent().Name.Split('\\')[1];
                                X509Certificate2 certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, myName);

                                if (certificate == null)
                                {
                                    Console.WriteLine("Nemate sertifikat");
                                }
                                else
                                {
                                    host.Credentials.ClientCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.ChainTrust;//definisanje tipa validacije
                                    host.Credentials.ServiceCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, srvCertCN);//dodajemo sertifikat na host, metoda GetCertificateFromStorage nam vraca sertifikat i podesava ga u Certificate (host.Credentials.ServiceCertificate.Certificat)
                                    host.Open();

                                    ServiceSecurityAuditBehavior audit = new ServiceSecurityAuditBehavior();
                                    audit.AuditLogLocation = AuditLogLocation.Application;
                                    host.Description.Behaviors.Remove<ServiceSecurityAuditBehavior>();
                                    host.Description.Behaviors.Add(audit);

                                    Console.WriteLine("Server podignut");
                                }
                            }

                            break;
                        case 4:
                            string myName1 = WindowsIdentity.GetCurrent().Name.Split('\\')[1];
                            X509Certificate2 certificate1 = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, myName1);
                            if (certificate1 == null)
                            {
                                Console.WriteLine("Sertifikat je vec povucen");
                            }
                            else
                            { 
                                Console.WriteLine(proxy.AddToRevocationList(certificate1));
                                if (host == null)
                                {                                  
                                    break;
                                }
                                
                                 host.Close();
                               
                            }
                            break;
                          
                    }
                    
                } while (option != 0);
                host.Close();
            }
           
            Console.ReadLine();

        }
        public static void ClosingClientConnection()
        {

            while (true)
            {
                using (StreamReader sr = new StreamReader("..//..//..//Lista//Diskonektovani.txt"))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string message = String.Format("Client {0} closed connection with server.", line);
                        EventLogEntryType evntType = EventLogEntryType.SuccessAudit;

                        LogData.WriteEntryServer(message, evntType, Convert.ToInt32(IDType.Disconnected));
                    }
                    
                }
                using (FileStream fs = File.Create("..//..//..//Lista//Diskonektovani.txt"))
                {

                }
                Thread.Sleep(2000);
            }

        }

        
    }
}
