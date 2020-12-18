using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WCFService
{
    public class SecurityService : ICommunication
    {
        public static Dictionary<string, User> UserAccountsDB = new Dictionary<string, User>();

        /// <summary>
        /// Add new user to UserAccountsDB. Dictionary Key is "username"
        /// </summary>
        public void AddUser(string username, string password)
        {
            if (!UserAccountsDB.ContainsKey(username))
            {
                User newUser = new User(username, password);
                UserAccountsDB.Add(username, newUser);
            }
            else
            {
                Console.WriteLine("User koga zelite da dodate vec postoji!");
            }

            //identitet klijenta koji je pozvao metodu
            IIdentity identity = Thread.CurrentPrincipal.Identity;

            Console.WriteLine("Tip autentifikacije: " + identity.AuthenticationType);
            WindowsIdentity windowsIdentity = identity as WindowsIdentity;
            Console.WriteLine("Ime korisnika koji je pozvao metodu: " + windowsIdentity.Name);
            Console.WriteLine("Jedinstveni identifikator: " + windowsIdentity.User);
            Console.WriteLine("Grupe korisnika: ");
            foreach (IdentityReference group in windowsIdentity.Groups)
            {
                SecurityIdentifier sid = (SecurityIdentifier)group.Translate(typeof(SecurityIdentifier));
                string name = (sid.Translate(typeof(NTAccount))).ToString();
                Console.WriteLine(name);
            }


        }

    }
}
