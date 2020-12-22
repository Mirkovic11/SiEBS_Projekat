using Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;


namespace CertificateManagerService
{
    public sealed class DataCertificate : ICertificateManager
    {
        public void createTrustedRootCA(string trustedRootName)
        {
            Process p = new Process();
            string certPath = "C:\\Program Files (x86)\\Windows Kits\\10\\bin\\10.0.17763.0\\x86\\makecert.exe";
            string command = "-n \"CN = " + trustedRootName + "\" -r -sv " + trustedRootName + ".pvk " + trustedRootName + ".cer";
            ProcessStartInfo startInfo = new ProcessStartInfo(certPath, command);
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
    }
}
