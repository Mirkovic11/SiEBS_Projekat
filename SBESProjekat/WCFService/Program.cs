﻿using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WCFService
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

            Console.WriteLine("Korisnik koji je pokrenuo server: " + WindowsIdentity.GetCurrent().Name);


            ServiceHost host = new ServiceHost(typeof(SecurityService));
            host.AddServiceEndpoint(typeof(ICommunication), binding, address);

            host.Open();
            Console.WriteLine("Servis je pokrenut");

            Console.ReadLine();


        }
    }
}
