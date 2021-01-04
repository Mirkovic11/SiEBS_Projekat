using Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace CertificateManagerService
{
    public sealed class DataCertificate : ICertificateManager
    {
     

        public void createCertificateWithallKeys(string trustedRootName, string certificateName)
        {
            Process p = new Process();
            string makecertPath = "C:\\Program Files (x86)\\Windows Kits\\10\\bin\\10.0.17763.0\\x86\\makecert.exe";
            string username = Thread.CurrentPrincipal.Identity.Name.Split('\\')[1];
            string groups = GetUserGroups((Thread.CurrentPrincipal.Identity as WindowsIdentity));
            //string commmand ="-sv "+certificateName+".pvk -iv "+trustedRootName+".pvk -n \"CN = "+username+ "\" -pe -ic " + trustedRootName+".cer "+certificateName+".cer -sr localmachine -ss My -sky exchange";
            string commmand ="-sv "+certificateName+".pvk -iv "+trustedRootName+".pvk -n \"CN = "+username + ",OU=" + groups + "\" -pe -ic " + trustedRootName+".cer "+certificateName+".cer -sr localmachine -ss My -sky exchange";
            
            Console.WriteLine(commmand);
            ProcessStartInfo startInfo = new ProcessStartInfo(makecertPath, commmand);
            p.StartInfo = startInfo;
            try
            {
                p.Start();
            }
            catch(Exception e)
            {
                e.ToString();
            }
            p.WaitForExit();
            p.Close();



            //pfx
            Process pPfx = new Process();
            string pvk2pfxPath = "C:\\Program Files (x86)\\Windows Kits\\10\\bin\\10.0.17763.0\\x86\\pvk2pfx.exe";
            string pfxCommand = "/pvk "+certificateName+".pvk /pi 1234 /spc "+certificateName+".cer /pfx "+certificateName+".pfx";

            ProcessStartInfo newStartInfo = new ProcessStartInfo(pvk2pfxPath, pfxCommand);
            pPfx.StartInfo = newStartInfo;
            try
            {
                pPfx.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Replicate(certificateName, "1234");
        }

        public void createCertificateWithoutPrivateKey(string trustedRootName, string certificateName)
        {
            Process p = new Process();
            string makecertPath = "C:\\Program Files (x86)\\Windows Kits\\10\\bin\\10.0.17763.0\\x86\\makecert.exe";
            string username = Thread.CurrentPrincipal.Identity.Name.Split('\\')[1];
            string groups = GetUserGroups((Thread.CurrentPrincipal.Identity as WindowsIdentity));
            string command = "-iv "+trustedRootName+".pvk -n \"CN = "+username+ ",OU=" + groups + "\" -ic " + trustedRootName+".cer "+certificateName+".cer -sr localmachine -ss My -sky exchange";
            

            ProcessStartInfo startInfo = new ProcessStartInfo(makecertPath, command);
            p.StartInfo = startInfo;
            try
            {
                p.Start();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Replicate(username, "");

        }

        public void createTrustedRootCA(string trustedRootName)
        {
            Process p = new Process();
            string makecertPath = "C:\\Program Files (x86)\\Windows Kits\\10\\bin\\10.0.17763.0\\x86\\makecert.exe";
            string command = "-n \"CN = " + trustedRootName + "\" -r -sv " + trustedRootName + ".pvk " + trustedRootName + ".cer";

            ProcessStartInfo startInfo = new ProcessStartInfo(makecertPath, command);
            p.StartInfo = startInfo;
            try
            {
                p.Start();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }

        public string AddToRevocationList(X509Certificate2 cert)
        {
            List<string> Lista = new List<string>();
            bool nadjeno = false;
            //if(!File.Exists("RevocationList.txt"))
          //  {
             //   File.Create("RevocationList.txt");
           // }
            using (StreamReader sr = new StreamReader("..//..//..//Lista//RevocationList.txt"))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    Lista.Add(line);
                }
            }
            
            foreach (string item in Lista)
            {
                if (item == cert.Thumbprint)
                {
                    nadjeno = true;
                    break;
                }
            }
            using (StreamWriter sw = new StreamWriter("..//..//..//Lista//RevocationList.txt", true))
            {
                
                if(!nadjeno)
                {
                    sw.WriteLine(cert.Thumbprint);
                    ReplicateRevocationList(cert);
                    return "Sertifikat povucen!";

                }
                else
                {
                    return "Sertifikat je vec povucen.\n";
                }
                  
            }


        }

        private string GetUserGroups(WindowsIdentity windowsIdentity)
        {
            string groups = "";
            foreach (IdentityReference group in windowsIdentity.Groups)
            {
                SecurityIdentifier sid = (SecurityIdentifier)group.Translate(typeof(SecurityIdentifier));
                var name = sid.Translate(typeof(NTAccount)).ToString();
                name = Formatter.ParseName(name);



                if (name == "RegionEast" || name == "RegionWest"
                    || name == "RegionNorth" || name == "RegionSouth")
                {
                    if (groups != "")
                        groups += "_" + name;
                    else
                        groups = name;
                }
            }

            return groups;
        }

        private void Replicate(string userName, string password)
        {
            try
            {
                X509Certificate2 certificate;
                if (password == "")
                    certificate = new X509Certificate2(userName + ".cer");
                else
                    certificate = new X509Certificate2(userName + ".cer", password);

                NetTcpBinding binding = new NetTcpBinding();
                binding.Security.Mode = SecurityMode.Transport;
                binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
                binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
                string address = "net.tcp://localhost:9997/IBackup";
                using (BackupProxy proxy = new BackupProxy(binding, address))
                {

                    proxy.CopyCert(certificate.Subject + ", thumbprint: " + certificate.Thumbprint);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void ReplicateRevocationList(X509Certificate2 c)
        {
            try
            {

                NetTcpBinding binding = new NetTcpBinding();
                binding.Security.Mode = SecurityMode.Transport;
                binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
                binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
                string address = "net.tcp://localhost:9997/IBackup";
                using (BackupProxy proxy = new BackupProxy(binding, address))
                {

                    proxy.CopyRevokedCert(c.Subject + ", thumbprint: " + c.Thumbprint);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }


    } 
}
