using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WCFService
{
    public class ServiceProxy : ChannelFactory<ICertificateManager>, ICertificateManager, IDisposable
    {
        ICertificateManager factory;

        public ServiceProxy(NetTcpBinding binding, string address) : base(binding, address)
        {
            factory = this.CreateChannel();
        }

        public ServiceProxy(NetTcpBinding binding, EndpointAddress address) : base(binding, address)
        {

            factory = this.CreateChannel();
            //zabraniti izvrsavanje autentifikacije putem NTLM protokola
            //Credentials.Windows.AllowNtlm = false;
        }

        public void createCertificateWithallKeys(string trustedRootName, string certificateName)
        {
            factory.createCertificateWithallKeys(trustedRootName, certificateName);
        }

        public void createCertificateWithoutPrivateKey(string trustedRootName, string certificateName)
        {
            factory.createCertificateWithoutPrivateKey(trustedRootName, certificateName);
        }

        public void createTrustedRootCA(string trustedRootName)
        {
            factory.createTrustedRootCA(trustedRootName);
        }
    }
}
