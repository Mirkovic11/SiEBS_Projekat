using Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Claims;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WCFService
{

    public enum IDType { Connected = 0, Disconnected };
    public class WcfSevice : IWcfService
    {
        private EventLogEntryType evntType;
        public WcfSevice()
        {
            string message = String.Format("Client {0} established connection with server.", ServiceSecurityContext.Current.PrimaryIdentity.Name);
            EventLogEntryType evntType = EventLogEntryType.SuccessAudit;
            LogData.WriteEntryServer(message, evntType, Convert.ToInt32(IDType.Connected));
        }

        public void PingServer(DateTime dt)
        {
            

                bool postoji = false;


                X509Certificate2 cC = getClientCertificate();
            if(cC == null)
            {
                Console.WriteLine("Sertifikat ne postoji");
                return;
            }

                string CN = cC.SubjectName.Name.Split(',')[0];
                string grupa = cC.SubjectName.Name.Split(',')[1];
                string name = CN.Split('=')[1];



                List<string> grupe = new List<string>();
                try
                {

                    string samo = grupa.Split('=')[1];

                    string[] gr = samo.Split('_');
                    for (int i = 0; i < gr.Count(); i++)
                    {
                        grupe.Add(gr[i]);

                    }

                }
                catch
                {
                    grupe.Add(grupa);


                }


                foreach (string g in grupe)
                {
                    if (g == "RegionEast" || g == "RegionWest" || g == "RegionNorth" || g == "RegionSouth")
                    {
                        postoji = true;
                        break;
                    }
                }


                if (postoji)
                {
                    int brojac = 0;
                    List<string> korisnici = new List<string>();
                    try
                    {
                        using (StreamReader sr = new StreamReader("..//..//..//Lista//JavljanjeKlijenata.txt"))
                        {
                            string line;
                            while ((line = sr.ReadLine()) != null)
                            {
                                korisnici.Add(line);
                            }

                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Nema kreiran txt");

                    }

                    foreach (string item in korisnici)
                    {
                        brojac++;
                    }

                    brojac++;
                    using (StreamWriter sw = new StreamWriter("..//..//..//Lista//JavljanjeKlijenata.txt", true))
                    {
                        sw.WriteLine("<" + brojac + ">:<" + DateTime.Now + ">:<" + CN + ">");
                    }


                    Console.WriteLine("Klijent " + name + " se javio " + DateTime.Now + "\n");


                }
                else
                {
                    Console.WriteLine("Klijent " + name + " nije autorizovan za zeljenu akciju jer ne pripada grupama kojima je to omoguceno!");
                }
            
        }



        public string TestCommunication()
        {
            X509Certificate2 clientCert = getClientCertificate();
            int commaIndex = clientCert.SubjectName.Name.IndexOf(',');
            string commonName = clientCert.SubjectName.Name.Remove(commaIndex); //CN=username

            Console.WriteLine("Klijent koji je testirao komunikaciju: "+ commonName.Substring(3));
            return "Komunikacija je uspostavljena!";
        }

        private X509Certificate2 getClientCertificate()
        {
            try
            {
                return ((X509CertificateClaimSet)
                        OperationContext.Current.ServiceSecurityContext.AuthorizationContext.ClaimSets[0]).X509Certificate;
            }
            catch
            {
                return null;
            }
        }
    }
}
