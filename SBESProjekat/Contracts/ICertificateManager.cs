using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    [ServiceContract]
    public interface ICertificateManager
    {
        [OperationContract]
        void createTrustedRootCA(string trustedRootName);

        [OperationContract]
        void createCertificateWithallKeys(string trustedRootName, string certificateName);

        [OperationContract]
        void createCertificateWithoutPrivateKey(string trustedRootName, string certificateName);

    }
}
