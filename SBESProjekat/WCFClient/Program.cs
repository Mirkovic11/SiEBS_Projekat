using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WCFClient
{
    class Program
    {
        static void Main(string[] args)
        {

            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:9999/Contracts";

            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            //za NTLM dodatno, meni nece raditi
            //EndpointAddress endpointAddress = new EndpointAddress(new Uri(address), EndpointIdentity.CreateUpnIdentity("wcfservice"));

            Console.WriteLine("Korisnik koji je pokrenuo klijenta: " + WindowsIdentity.GetCurrent().Name);
           

            using (ClientProxy proxy = new ClientProxy(binding, address)) //da radim ono za NTLM ovde bih prosledio endpointAddress
            {
                Console.WriteLine("Unesi ime sertifikata koji zelis da napravis");
                string certName = Console.ReadLine();
                proxy.createTrustedRootCA("TestCA");
                //proxy.createCertificateWithallKeys("TestCA", certName);
                proxy.createCertificateWithoutPrivateKey("TestCA", certName);
                
            }

            Console.ReadLine();


        }
    }
}
