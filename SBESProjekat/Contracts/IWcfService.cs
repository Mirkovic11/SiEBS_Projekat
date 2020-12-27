using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    [ServiceContract]
    public interface IWcfService
    {
        [OperationContract]
        string TestCommunication();

        [OperationContract]
        void PingServer(DateTime dt, string name, string cn);
    }
}
