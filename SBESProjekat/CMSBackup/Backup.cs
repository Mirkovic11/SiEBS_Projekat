using Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSBackup
{
    public class Backup : IBackup
    {
        public void CopyCert(string cert)
        {
            using (StreamWriter sw = new StreamWriter("CertListBackup.txt", true))
            {
                
                    sw.WriteLine(cert);
                
            }
            Console.WriteLine("Certificate replicated...");
        }

        public void CopyRevokedCert(string cert)
        {
            using (StreamWriter sw = new StreamWriter("RevocationListBackup.txt", true))
            {

                sw.WriteLine(cert);

            }
            Console.WriteLine("Certificate replicated...");
        }
    }
}
