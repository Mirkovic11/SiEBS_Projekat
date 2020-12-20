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
        static string RSADecrypt(string xmlPrivateKey, string m_strDecryptString)
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            provider.FromXmlString(xmlPrivateKey);
            byte[] rgb = Convert.FromBase64String(m_strDecryptString);
            byte[] bytes = provider.Decrypt(rgb, false);
            return new UnicodeEncoding().GetString(bytes);
        }
        /// <summary>     
        /// RSA encryption     
        /// </summary>     
        /// <param name="xmlPublicKey"></param>     
        /// <param name="m_strEncryptString"></param>     
        /// <returns></returns>     
        static string RSAEncrypt(string xmlPublicKey, string m_strEncryptString)
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            provider.FromXmlString(xmlPublicKey);
            byte[] bytes = new UnicodeEncoding().GetBytes(m_strEncryptString);
            return Convert.ToBase64String(provider.Encrypt(bytes, false));
        }

        static void Main(string[] args)
        {
            String certificateName = "";//Certificate name
            string keyPublic = "";
            string keyPrivate = "";
            certificateName = "cao4";
            //X509Certificate2 privateCert = new X509Certificate2(pfxFilePath, pfxPassword, X509KeyStorageFlags.Exportable);  
            //// Create a certificate in the personal certificate directory in the local store
            ////makecert Win10 corresponds to the directory C:\Program Files (x86)\Windows Kits\8.0\bin\x64, different versions of Windows makecert directory may be different           
            DataCertificate.CreateCertWithPrivateKey(certificateName, "C:\\Program Files (x86)\\Windows Kits\\10\\bin\\10.0.17763.0\\x86\\makecert.exe");
            //Get the certificate
            X509Certificate2 c1 = DataCertificate.GetCertificateFromStore(certificateName);
            keyPublic = c1.PublicKey.Key.ToXmlString(false); // public key  
            keyPrivate = c1.PrivateKey.ToXmlString(true); // private key
            String cypher = RSAEncrypt(keyPublic, "programmer"); // Encryption  
            String plain = RSADecrypt(keyPrivate, cypher); // decrypt  

            System.Diagnostics.Debug.Assert(plain == "programmer");
            ////First time Read the generated cer certificate from the personal certificate directory in the local store Copy to the specified folder 
            DataCertificate.ExportToCerFile(certificateName, "d:\\Aleksa\\" + certificateName + ".cer");
            X509Certificate2 c2 = DataCertificate.GetCertFromCerFile("d:\\Aleksa\\" + certificateName + ".cer");
            string keyPublic2 = c2.PublicKey.Key.ToXmlString(false);
            bool b = keyPublic2 == keyPublic;
            if (!b)
                keyPublic2 = keyPublic;
            String cypher2 = RSAEncrypt(keyPublic2, "Programmer 2"); // Encryption  
            String plain2 = RSADecrypt(keyPrivate, cypher2); // Decrypt, there is no private key in cer, so use the private key obtained earlier to decrypt 

            //Generate a certificate to get the public key private key to re-call after encryption and decryption
            ////X509Certificate2 c2 = DataCertificate.GetCertFromCerFile("d:\\mycert\\" + certificateName + ".cer");
            //keyPublic = "<RSAKeyValue><Modulus>tZ6JUEO8A1gPE0+b2/QdxxgDmaKJvwD9hwXmxRaUc6OKlZ4QRNliLCDMvrgaXPF2S3QiqGmQnoeNPWF90IsaB/wxc4ayglEX8wl2yV8PARcQhx+V+wIJsBD0vVNk3sMBAfZrVrVJOZNkjUcAFmPYnPxkq/7y4HJFG+TCRNMVsRjreywK+CLrZFnYvUiw/xbNA6fJ03nkFDNoPsXAXVOUJv9J8FXiTirPx21aGJddy7AFlvNytgOru5F6pJYI76fe5YYGYYUVHjgIOCmkjcolwYt6AUmlmKG1bsGvd0Imacf5rKu32E8J8avcojnKyqpOGOPssMxq3/s0YsX6yJfHtQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
            //keyPrivate = "<RSAKeyValue><Modulus>tZ6JUEO8A1gPE0+b2/QdxxgDmaKJvwD9hwXmxRaUc6OKlZ4QRNliLCDMvrgaXPF2S3QiqGmQnoeNPWF90IsaB/wxc4ayglEX8wl2yV8PARcQhx+V+wIJsBD0vVNk3sMBAfZrVrVJOZNkjUcAFmPYnPxkq/7y4HJFG+TCRNMVsRjreywK+CLrZFnYvUiw/xbNA6fJ03nkFDNoPsXAXVOUJv9J8FXiTirPx21aGJddy7AFlvNytgOru5F6pJYI76fe5YYGYYUVHjgIOCmkjcolwYt6AUmlmKG1bsGvd0Imacf5rKu32E8J8avcojnKyqpOGOPssMxq3/s0YsX6yJfHtQ==</Modulus><Exponent>AQAB</Exponent><P>1tSbdgI4XdOdZGbPgwSOWqp0FzRe1GaKB/6zBtXVjR2iehF8BhrWbiYL2WGz2BgGQT7kyA240keywKzihmm7F+K0L0BTb/6t8ZIyFvIfj3u41P5BJ5ikQ/6ZLVBC2ZvrQwHz6ycxAt8gPbJPndfAjc3OMUQmBvalo9PUwGkX3FM=</P><Q>2Gyd1Pioi6QiDM/cdgCB97nkh/GlcnMlsBp654YheoTiaq/AiEeaNwQiSRsFmzWU5PsEKoUpOtKadCqhdSqvU47OdU+gD7aXyLgsIVbtLWdzFX4Bq06a7P1fYN2HPvOjPec2oEU6HRX9/XBJzXAGslw3JFUZlhWJRajbk6Lzitc=</Q><DP>0ft6APzmj39aJlr/lfaMFj7pvgyobD/Vxz7DSnkUhRxkRaB1c5oj4gI6Lr57BUtmQbvx70DKWG9QX1gdCniqMQycRls/swZiiu71GsyK4LpzzWy/zq46UWO34TzEOuNWL2bnPgBOvZnOb7+sZoIOagyx8CHGcaP//4P8Ph36/pU=</DP><DQ>OfxIAWKqDdfpA5PBlqAmMlBNCZtV36c4Rsmhely2pZPq8fiq1hiRGgJyiTHDO8WMYhlbEWViGY+JsGwnnDPWi8WsTUQLN4qNekrWEAyxOUQJUo3TNqm12p88KcDQ1q4CY7iKK0DBBD/7MCcgrvk/4hPQ9lwSoeKdR9upERJMvDs=</DQ><InverseQ>TMnaTY0Qcg5qhEB1+Mb5/QPDQzOpO+tlduALO23Kuubm+lYQ5sZUyHGv25lb0RAUkFqDFeyA0M9Y8Y8lCuXkjzU6depFZHay9a9OG8WJ7g5AcSuNltjbqC2y542dgnBU9gzUogySrVGAHdBFkQhwukH6tSgqIVR4PblI3Vr54uQ=</InverseQ><D>L2nM5SRZr/HMNblhsgE/yNsPDYuuNCv5A8fZn/guFyZJppeWHbM2etixOtTrJPpwbHBMH/U3KPuwNqb95nR5/j2rV0KB1Z2ACBWfaiCj1SAFU5E+YUH973XtvoNH4RO9bpq7GO7Ix/wfkvZHIpE8WndVfMVY+Jk8S3Tj9n24uvua4HHsBxy1TeRdRHojWWVZm79aH+CFhGIemSHSVEKp+GfPhIUzMGXwfA6l4M4FSpb+XNbB7un29JjTNB8TV49qVY4QelpsS/J9v6eMg0GDYBJIcG0AkghrFWBGjVyHGUJJsZhGVYAxdJow4BWo+Y129LYtcZtznGhH4VsjpTNsfQ==</D></RSAKeyValue>";
            //string cypher2 = RSAEncrypt(keyPublic, "Programmer 2"); // Encryption  
            //string plain2 = RSADecrypt(keyPrivate, cypher2); // Decrypt, there is no private key in cer, so use the private key obtained earlier to decrypt 

            //System.Diagnostics.Debug.Assert(plain2 == "programmer 2");
            // generate a pfx and delete it from the store
            DataCertificate.ExportToPfxFile(certificateName, "d:\\Aleksa\\" + certificateName + ".pfx", "111", true);
            X509Certificate2 c3 = DataCertificate.GetCertificateFromPfxFile("d:\\Aleksa\\" + certificateName + ".pfx", "111");

            String keyPublic3 = c3.PublicKey.Key.ToXmlString(false); // public key  
            String keyPrivate3 = c3.PrivateKey.ToXmlString(true); // private key  

            String cypher3 = RSAEncrypt(keyPublic3, "programmer 3"); // encryption  
            String plain3 = RSADecrypt(keyPrivate3, cypher3); // decrypt  
            System.Diagnostics.Debug.Assert(plain3 == "programmer 3");


        }
    }
}
