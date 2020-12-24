using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public class CertManager
    {
        public static X509Certificate2 GetCertificateFromStorage(StoreName storeName, StoreLocation storeLocation, string subjectName)
        {
            X509Store store = new X509Store(storeName, storeLocation);//da l' valjda napravimo objekat u kome se skladiste sertifikati
            store.Open(OpenFlags.ReadOnly);//samo citamo sertifikate
                                           //po cemu trazimo //samo one koji su validni
            X509Certificate2Collection certCollection = store.Certificates.Find(X509FindType.FindBySubjectName, subjectName, true);//za pronalazenje sertifikata koji nam treba

            /// Check whether the subjectName of the certificate is exactly the same as the given "subjectName"
            foreach (X509Certificate2 c in certCollection)//prolazimo kroz svaki sertifikat
            {
                if (c.SubjectName.Name.Equals(string.Format("CN={0}", subjectName)))//provjeravamo da li je
                {
                    return c;
                }
            }

            return null;
        }
    }
}
