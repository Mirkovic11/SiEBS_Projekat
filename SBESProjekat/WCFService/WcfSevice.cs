using Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WCFService
{
    public class WcfSevice : IWcfService
    {




        public void PingServer(DateTime dt, string name, string cn, string grupa)
        {


            List<string> grupe = new List<string>();
            try
            {
                //Console.WriteLine("Ispis onog sto dobijem: " + grupa);
                string samo = grupa.Split('=')[1];
               // Console.WriteLine("Ispis nakon splitovanja po = : " + samo);
                string[] gr = samo.Split('_');
                for (int i = 0; i < gr.Count(); i++)
                {
                    grupe.Add(gr[i]);
                    //Console.WriteLine("Nakon splitovanja po _ : " + gr[i] + "\n");
                }

            }
            catch
            {
                grupe.Add(grupa);
               // Console.WriteLine("Jedina grupa kojoj pripada: " + grupa);

            }

            bool postoji = false;
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
                        sw.WriteLine("<" + brojac + ">:<" + DateTime.Now + ">:<" + cn + ">");
                    }


                    Console.WriteLine("Klijent " + name + " se javio " + DateTime.Now + "\n");


                }
                else
                {
                    Console.WriteLine("Ne pripada grupi");
                }

        }



        public string TestCommunication()
        {
            return "Komunikacija je uspostavljena!";
        }
    }
}
