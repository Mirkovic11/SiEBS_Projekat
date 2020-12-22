using Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CertificateManagerService
{
    class Program
    {
       
        static void Main(string[] args)
        {
            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:9999/Contracts";


            Console.WriteLine("Korisnik koji je pokrenuo server: " + WindowsIdentity.GetCurrent().Name);


            ServiceHost host = new ServiceHost(typeof(DataCertificate));
            host.AddServiceEndpoint(typeof(ICertificateManager), binding, address);

            host.Open();
            Console.WriteLine("CertificateManagerService je pokrenut");
            Console.ReadLine();

        }
    }
}
