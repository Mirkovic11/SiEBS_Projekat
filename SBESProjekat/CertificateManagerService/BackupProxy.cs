using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CertificateManagerService
{
    public class BackupProxy : ChannelFactory<IBackup>, IBackup, IDisposable
    {

        IBackup proxy;
        public BackupProxy(NetTcpBinding binding, string address)
            : base(binding, address)
        {
            proxy = this.CreateChannel();
        }

        public void CopyCert(string cert)
        {
            proxy.CopyCert(cert);
        }

        public void CopyRevokedCert(string cert)
        {
            proxy.CopyRevokedCert(cert);
        }

       

    }
}
