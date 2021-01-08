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
    public enum IDType { GenerateSuccess = 0, RevokeSuccess, ReplicateSuccess, GenerateFailure, RevokeFailure, ReplicateFailure };
    public sealed class DataCertificate : ICertificateManager
    {
        private string message = string.Empty;
        private EventLogEntryType evntType;
        public void createCertificateWithallKeys(string trustedRootName, string certificateName)
        {
            Process p = new Process();
            string makecertPath = "C:\\Program Files (x86)\\Windows Kits\\10\\bin\\10.0.17763.0\\x86\\makecert.exe";
            string username = Thread.CurrentPrincipal.Identity.Name.Split('\\')[1];
            string groups = GetUserGroups((Thread.CurrentPrincipal.Identity as WindowsIdentity));
            //string commmand ="-sv "+certificateName+".pvk -iv "+trustedRootName+".pvk -n \"CN = "+username+ "\" -pe -ic " + trustedRootName+".cer "+certificateName+".cer -sr localmachine -ss My -sky exchange";
            string commmand ="-sv "+certificateName+".pvk -iv "+trustedRootName+".pvk -n \"CN = "+username + ",OU=" + groups + "\" -pe -ic " + trustedRootName+".cer "+certificateName+".cer -sr localmachine -ss My -sky exchange";
            
            //Console.WriteLine(commmand);
            ProcessStartInfo startInfo = new ProcessStartInfo(makecertPath, commmand);
            p.StartInfo = startInfo;
            try
            {
                p.Start();
            }
            catch(Exception e)
            {
                message = String.Format("Certificate cannot be generated to {0}.Error: {1}", username, e.Message);
                evntType = EventLogEntryType.FailureAudit;
                LogData.WriteEntryCMS(message, evntType, Convert.ToInt32(IDType.GenerateFailure));
                return;
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
                message = String.Format("Certificate with PVK cannot be generated to {0}.Error: {1}", username, e.Message);
                evntType = EventLogEntryType.FailureAudit;
                LogData.WriteEntryCMS(message, evntType, Convert.ToInt32(IDType.GenerateFailure));
                return;
            }

            message = String.Format("Certificate with PVK generated to {0}.", (Thread.CurrentPrincipal.Identity as WindowsIdentity).Name);
            evntType = EventLogEntryType.SuccessAudit;
            LogData.WriteEntryCMS(message, evntType, Convert.ToInt32(IDType.GenerateSuccess));

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
                message = String.Format("Certificate without PVK cannot be generated to {0}.Error: {1}", username, e.Message);
                evntType = EventLogEntryType.FailureAudit;
                LogData.WriteEntryCMS(message, evntType, Convert.ToInt32(IDType.GenerateFailure));
                return;
            }

            message = String.Format("Certificate without PVK generated to {0}.", username);
            evntType = EventLogEntryType.SuccessAudit;
            LogData.WriteEntryCMS(message, evntType, Convert.ToInt32(IDType.GenerateSuccess));

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
                message = String.Format("Root certificate {0} cannot be generated.Error: {1}", trustedRootName, e.Message);
                evntType = EventLogEntryType.FailureAudit;
                LogData.WriteEntryCMS(message, evntType, Convert.ToInt32(IDType.GenerateFailure));
                return;
            }
            message = String.Format("Root certificate {0} generated.", trustedRootName);
            evntType = EventLogEntryType.SuccessAudit;
            LogData.WriteEntryCMS(message, evntType, Convert.ToInt32(IDType.GenerateSuccess));


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

                    message = String.Format("Client {0}  with certificate[Subject {1}] has been revoked.Revocation list updated.", (Thread.CurrentPrincipal.Identity as WindowsIdentity).Name, cert.Subject);
                    evntType = EventLogEntryType.SuccessAudit;
                    LogData.WriteEntryCMS(message, evntType, Convert.ToInt32(IDType.RevokeSuccess));

                    return "Sertifikat povucen!";

                }
                else
                {
                    /*message = String.Format("Client {0}  with certificate[Subject {1}] has already been revoked", (Thread.CurrentPrincipal.Identity as WindowsIdentity).Name, cert.Subject);
                    evntType = EventLogEntryType.FailureAudit;
                    LogData.WriteEntryCMS(message, evntType, Convert.ToInt32(IDType.RevokeFailure));*/
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
                    message = String.Format("Client {0} with certificate[Subject {1}] successfully replicated.", userName, certificate.Subject);
                    evntType = EventLogEntryType.SuccessAudit;
                    LogData.WriteEntryCMS(message, evntType, Convert.ToInt32(IDType.ReplicateSuccess));
                    proxy.CopyCert(certificate.Subject + ", thumbprint: " + certificate.Thumbprint);
                }
            }
            catch (Exception e)
            {
                message = String.Format("Error with replicating client {0} certificate.Error: {1}", userName, e.Message);
                evntType = EventLogEntryType.FailureAudit;
                LogData.WriteEntryCMS(message, evntType, Convert.ToInt32(IDType.ReplicateFailure));
                Console.WriteLine("Error while trying to replicate certificate {0}. ERROR = {1}", userName, e.Message);
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
                    message = String.Format("Revocation list successfully replicated.");
                    evntType = EventLogEntryType.SuccessAudit;
                    LogData.WriteEntryCMS(message, evntType, Convert.ToInt32(IDType.ReplicateSuccess));
                    proxy.CopyRevokedCert(c.Subject + ", thumbprint: " + c.Thumbprint);
                }
            }
            catch (Exception e)
            {
                message = String.Format("Error with replicating revocation list.Error: {0}", e.Message);
                evntType = EventLogEntryType.FailureAudit;
                LogData.WriteEntryCMS(message, evntType, Convert.ToInt32(IDType.ReplicateFailure));
                Console.WriteLine("Error while trying to replicate certificate {0}. ERROR = {1}", c.Subject, e.Message);
            }
        }


    } 
}
