using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
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
