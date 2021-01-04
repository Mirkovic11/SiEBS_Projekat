using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    [ServiceContract]
    public interface IBackup
    {
        [OperationContract]
        void CopyCert(string cert);

        [OperationContract]
        void CopyRevokedCert(string cert);

    }
}
