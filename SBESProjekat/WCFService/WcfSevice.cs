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
        
        private System.Timers.Timer disconnectTimer = new System.Timers.Timer();


        //[PrincipalPermission(SecurityAction.Demand, Role = "Zlatna")]
        public void PingServer(DateTime dt, string name, string cn)
        {
            int brojac = 0;
            List<string> korisnici = new List<string>();
            try { 
                using(StreamReader sr=new StreamReader("..//..//..//Lista//JavljanjeKlijenata.txt"))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        korisnici.Add(line);
                    }

                }
            }catch(Exception e)
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
                sw.WriteLine("<"+brojac+">:<"+dt+">:<"+cn+">");
            }
                /*

                            if (disconnectTimer.Enabled)
                            {
                                disconnectTimer.Stop();

                                disconnectTimer.Start();
                            }


                            disconnectTimer.Interval = 10000;
                            disconnectTimer.Enabled = true;
                            disconnectTimer.AutoReset = false;*/

                Console.WriteLine("Klijent"+name+" se javio "+dt.ToString()+"\n");
        }


        //[PrincipalPermission(SecurityAction.Demand, Role = "Zlatna")]
        public string TestCommunication()
        {
            return "Komunikacija je uspostavljena!";
        }
    }
}
