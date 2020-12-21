﻿using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WCFClient
{
    

        public class ClientProxy : ChannelFactory<ICommunication>, ICommunication, IDisposable
        {
            ICommunication factory;

            public ClientProxy(NetTcpBinding binding, string address) : base(binding, address)
            {
                factory = this.CreateChannel();
            }

            public ClientProxy(NetTcpBinding binding, EndpointAddress address) : base(binding, address)
            {

                factory = this.CreateChannel();
                //zabraniti izvrsavanje autentifikacije putem NTLM protokola
                //Credentials.Windows.AllowNtlm = false;
            }

            public void AddUser(string username, string password)
            {

                try
                {
                    factory.AddUser(username, password);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: {0}", e.Message);
                }
            }

        }
}