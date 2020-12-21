using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CertificateManagerService
{
    class Program
    {

        static void Main(string[] args)
        {
            String certificateName = "";//Certificate name
            
            certificateName = "cao4";
            //X509Certificate2 privateCert = new X509Certificate2(pfxFilePath, pfxPassword, X509KeyStorageFlags.Exportable);  
            //// Create a certificate in the personal certificate directory in the local store
            ////makecert Win10 corresponds to the directory C:\Program Files (x86)\Windows Kits\8.0\bin\x64, different versions of Windows makecert directory may be different           
            DataCertificate.CreateCertWithPrivateKey(certificateName, "C:\\Program Files (x86)\\Windows Kits\\10\\bin\\10.0.17763.0\\x86\\makecert.exe");
            //Get the certificate
            X509Certificate2 c1 = DataCertificate.GetCertificateFromStore(certificateName);
            
            

            
            ////First time Read the generated cer certificate from the personal certificate directory in the local store Copy to the specified folder 
            DataCertificate.ExportToCerFile(certificateName, "d:\\Aleksa\\" + certificateName + ".cer");
            X509Certificate2 c2 = DataCertificate.GetCertFromCerFile("d:\\Aleksa\\" + certificateName + ".cer");
           
            
            // generate a pfx and delete it from the store
            DataCertificate.ExportToPfxFile(certificateName, "d:\\Aleksa\\" + certificateName + ".pfx", "111", true);
            X509Certificate2 c3 = DataCertificate.GetCertificateFromPfxFile("d:\\Aleksa\\" + certificateName + ".pfx", "111");

            


        }
    }
}
