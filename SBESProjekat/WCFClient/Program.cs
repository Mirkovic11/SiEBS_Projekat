﻿using Contracts;
using System;
using System.Collections.Generic;
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
        static void Main(string[] args)
        {

            string srvCertCN = "wcfservice";
            X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, srvCertCN);//podesavanje serverskog sertifikataeateUpnIdentity("wcfservice"));

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

            

            Console.WriteLine("Korisnik koji je pokrenuo klijenta: " + WindowsIdentity.GetCurrent().Name);

           
            using (ClientProxy proxy = new ClientProxy(binding, address)) //da radim ono za NTLM ovde bih prosledio endpointAddress
            {
                int option = 0;
                string certName = "";

                //proxy.createTrustedRootCA("TestCAClient");

                do
                {
                    Console.WriteLine("Unesi 1 za kreiranje sertifikata sa svim kljucevima, 2 za kreiranje sertifikata bez privatnog kljuca, 3 konekcija sa serverom, 4 povlacenje sertifikata, 5 za javljanje serveru,0 za izlaz");
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

                                ClientProxyService proxy2 = new ClientProxyService(binding2, address2);
                                Console.WriteLine(proxy2.TestCommunication()); 
                            
                            }
                            catch(Exception e)
                            {
                                Console.WriteLine(e.ToString());
                                //Console.WriteLine("Ne radim");
                            }
                            break;
                        case 4:
                            string myName = WindowsIdentity.GetCurrent().Name.Split('\\')[1];
                            X509Certificate2 certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, myName);
                            Console.WriteLine(proxy.AddToRevocationList(certificate) ); 
                            break;
                        case 5:
                          //  try { 
                                ClientProxyService proxy3 = new ClientProxyService(binding2, address2);
                            
                                Console.WriteLine("Starting to ping server...");
                                string name=WindowsIdentity.GetCurrent().Name.Split('\\')[1];
                                
                                X509Certificate2 cert = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, name);
                                if (cert == null)
                                {
                                    Console.WriteLine("Nema sertifikata");
                                }
                                string CN = cert.SubjectName.Name.Split(',')[0];
                                Console.WriteLine(CN);
                                Random r = new Random();
                                try
                                {
                                    while (true)
                                    {
                                        Thread.Sleep(r.Next(1, 10) * 1000); //sleep 1-10s

                                        proxy3.PingServer(DateTime.Now,name,CN);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("evo me");
                                    Console.WriteLine(ex.Message);
                                }
                           /* }catch
                            {
                                Console.WriteLine("Ne radim!");
                            }*/
                                break;

                    }
                } while (option != 0);
             
            }


            

            Console.ReadLine();


        }
    }
}
