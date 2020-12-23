using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCFService
{
    public class WcfSevice : IWcfService
    {
        public string TestCommunication()
        {
            return "Komunikacija je uspostavljena!";
        }
    }
}
