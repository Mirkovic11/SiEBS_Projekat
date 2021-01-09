
using Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CMSBackup
{
    class Program
    {
        static void Main(string[] args)
        {
           

            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:9997/IBackup";

            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;

            Console.WriteLine("Korisnik koji je pokrenuo Backup server: " + WindowsIdentity.GetCurrent().Name);


            ServiceHost host = new ServiceHost(typeof(Backup));
            host.AddServiceEndpoint(typeof(IBackup), binding, address);
            host.Open();



            Console.WriteLine("Backup service je pokrenut");
            Console.ReadLine();

        }
    }
}
