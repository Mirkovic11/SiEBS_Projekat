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

            Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.ChainTrust;//definisanje tipa validacije
            this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;


            this.Credentials.ClientCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);




            factory = this.CreateChannel();
            //zabraniti izvrsavanje autentifikacije putem NTLM protokola
            //Credentials.Windows.AllowNtlm = false;


        }

        public string TestCommunication()
            {
                return factory.TestCommunication();
            }



        }


}
