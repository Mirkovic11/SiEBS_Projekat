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

namespace WCFClient
{
    
        public class ClientProxyService : ChannelFactory<IWcfService>, IWcfService, IDisposable
        {
            IWcfService factory;

            public ClientProxyService(NetTcpBinding binding, string address) : base(binding, address)
            {
                factory = this.CreateChannel();
            }

        public ClientProxyService(NetTcpBinding binding, EndpointAddress address) : base(binding, address)
        {
            
                string cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);


                List<string> Lista = new List<string>();
                bool nadjeno = false;
                string myName = WindowsIdentity.GetCurrent().Name.Split('\\')[1];

                X509Certificate2 certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, myName);

                if (certificate == null)
                {
                    Console.WriteLine("nemate sertifikat");
                }
                else
                {
                    Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.ChainTrust;//definisanje tipa validacije

                    this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;


                    this.Credentials.ClientCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);

                    factory = this.CreateChannel();
                }
            

        }

        public void PingServer(DateTime dt)
        {
            try
            {
                factory.PingServer(dt);

            }catch(Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }

        public string TestCommunication()
            {
                return factory.TestCommunication();
            }



        }


}
