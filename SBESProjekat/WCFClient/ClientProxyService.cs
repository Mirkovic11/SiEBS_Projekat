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
            }
            else
            {
                Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.ChainTrust;//definisanje tipa validacije
                this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

                this.Credentials.ClientCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);
                factory = this.CreateChannel();
            }          
            //zabraniti izvrsavanje autentifikacije putem NTLM protokola
            //Credentials.Windows.AllowNtlm = false;

        }

        public string TestCommunication()
            {
                return factory.TestCommunication();
            }



        }


}
