using Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
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
            string commmand ="-sv "+certificateName+".pvk -iv "+trustedRootName+".pvk -n \"CN = "+username+ "\" -pe -ic " + trustedRootName+".cer "+certificateName+".cer -sr localmachine -ss My -sky exchange";

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
        }

        public void createCertificateWithoutPrivateKey(string trustedRootName, string certificateName)
        {
            Process p = new Process();
            string makecertPath = "C:\\Program Files (x86)\\Windows Kits\\10\\bin\\10.0.17763.0\\x86\\makecert.exe";
            string username = Thread.CurrentPrincipal.Identity.Name.Split('\\')[1];
            string command = "-iv "+trustedRootName+".pvk -n \"CN = "+username+"\" -ic "+trustedRootName+".cer "+certificateName+".cer -sr localmachine -ss My -sky exchange";
            

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
    }
}
