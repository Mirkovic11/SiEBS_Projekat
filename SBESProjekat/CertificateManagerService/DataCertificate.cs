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
    public sealed class DataCertificate
    {
        #region Generate a certificate
        /// <summary>     
        /// Generate a certificate based on the specified certificate name and makecert full path (including the public and private keys and save them in the MY store)     
        /// </summary>     
        /// <param name="subjectName"></param>     
        /// <param name="makecertPath"></param>     
        /// <returns></returns>
        //The parameter is: makecert -r -pe -n "cn=MyCA" -$ commercial -a sha1 -b 08/05/2010 -e 01/01/2012 -cy authority -ss my -sr currentuser
        //The meaning of each part:
        //-r: self-signed
        //-pe: Marks the generated private key as exportable. This will include the private key in the certificate.
        //-n "cn=MyCA": The subject name of the certificate, the .net comes with the X509Store class in the library, which can be found in the store according to the certificate subject name.
        //store reference: X509Store class 
        //-$ commercial: Indicates the commercial use of the certificate. . .
        //-a: Specify the signature algorithm. Must be md5 (default) or sha1.
        //-b 08/05/2010: The start time of the certificate validity period, the default is the date the certificate was created. The format is: mm/dd/yyyy
        //-e 01/01/2012: Specify the end time of the validity period. The default is 12/31/2039 11:59:59 GMT. Same format as above
        //-ss my: The certificate is generated to my personal store area
        //-sr currentuser: Keeps the current personal user area of ​​the computer. Other users cannot see the certificate after logging in to the system. .
        public static bool CreateCertWithPrivateKey(string subjectName, string makecertPath)
        {
            subjectName = "CN=" + subjectName;
            //string param = " -pe -ss my -n \"" + subjectName + "\" ";
            string param = " -r -pe -ss my -n \"" + subjectName + "\" " + " -a sha256 -b 01/01/2019 -e 01/01/4019";
            try
            {
                Process p = Process.Start(makecertPath, param);
                p.WaitForExit();
                p.Close();
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }
        #endregion


        /// <summary>     
        /// Find the certificate with the subject subjectName from the personal MY area of ​​the WINDOWS certificate store.     
        /// and export to a pfx file with a password for it     
        /// and remove the certificate from the personal area (if isDelFromstor is true)     
        /// </summary>     
        /// <param name="subjectName">Certificate subject, excluding CN=</param>     
        /// <param name="pfxFileName">pfx filename</param>     
        /// <param name="password">pfx file password</param>     
        /// <param name="isDelFromStore">Remove from storage</param>     
        /// <returns></returns>     
        public static bool ExportToPfxFile(string subjectName, string pfxFileName, string password, bool isDelFromStore)
        {
            subjectName = "CN=" + subjectName;
            X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadWrite);
            X509Certificate2Collection storecollection = (X509Certificate2Collection)store.Certificates;
            foreach (X509Certificate2 x509 in storecollection)
            {
                if (x509.Subject == subjectName)
                {
                    Debug.Print(string.Format("certificate name: {0}", x509.Subject));

                    byte[] pfxByte = x509.Export(X509ContentType.Pfx, password);
                    using (FileStream fileStream = new FileStream(pfxFileName, FileMode.Create))
                    {
                        // Write the data to the file, byte by byte.     
                        for (int i = 0; i < pfxByte.Length; i++)
                            fileStream.WriteByte(pfxByte[i]);
                        // Set the stream position to the beginning of the file.     
                        fileStream.Seek(0, SeekOrigin.Begin);
                        // Read and verify the data.     
                        for (int i = 0; i < fileStream.Length; i++)
                        {
                            if (pfxByte[i] != fileStream.ReadByte())
                            {
                                fileStream.Close();
                                return false;
                            }
                        }
                        fileStream.Close();
                    }
                    if (isDelFromStore == true)
                        store.Remove(x509);
                }

            }

            store.Close();
            store = null;
            storecollection = null;
            return true;
        }



        public static X509Certificate2 GetCertificateFromPfxFile(string pfxFileName, string password)
        {
            try
            {
                return new X509Certificate2(pfxFileName, password, X509KeyStorageFlags.Exportable);
            }
            catch (Exception e)
            {
                return null;
            }
        }


        public static X509Certificate2 GetCertificateFromStore(string subjectName)
        {
            subjectName = "CN=" + subjectName;
            X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadWrite);
            X509Certificate2Collection storecollection = (X509Certificate2Collection)store.Certificates;
            foreach (X509Certificate2 x509 in storecollection)
            {
                if (x509.Subject == subjectName)
                {
                    return x509;
                }
            }
            store.Close();
            store = null;
            storecollection = null;
            return null;
        }

        /// <summary>     
        /// Return the certificate entity based on the public key certificate     
        /// </summary>     
        /// <param name="cerPath"></param>     
        public static X509Certificate2 GetCertFromCerFile(string cerPath)
        {
            try
            {
                return new X509Certificate2(cerPath);
            }
            catch (Exception e)
            {
                return null;
            }
        }


        public static bool ExportToCerFile(string subjectName, string cerFileName)
        {
            subjectName = "CN=" + subjectName;
            X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadWrite);
            X509Certificate2Collection storecollection = (X509Certificate2Collection)store.Certificates;
            foreach (X509Certificate2 x509 in storecollection)
            {
                if (x509.Subject == subjectName)
                {
                    Debug.Print(string.Format("certificate name: {0}", x509.Subject));
                    //byte[] pfxByte = x509.Export(X509ContentType.Pfx, password);     
                    byte[] cerByte = x509.Export(X509ContentType.Cert);
                    using (FileStream fileStream = new FileStream(cerFileName, FileMode.Create))
                    {
                        // Write the data to the file, byte by byte.     
                        for (int i = 0; i < cerByte.Length; i++)
                            fileStream.WriteByte(cerByte[i]);
                        // Set the stream position to the beginning of the file.     
                        fileStream.Seek(0, SeekOrigin.Begin);
                        // Read and verify the data.     
                        for (int i = 0; i < fileStream.Length; i++)
                        {
                            if (cerByte[i] != fileStream.ReadByte())
                            {
                                fileStream.Close();
                                return false;
                            }
                        }
                        fileStream.Close();
                    }
                }
            }
            store.Close();
            store = null;
            storecollection = null;
            return true;
        }



    }
}
