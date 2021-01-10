using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WCFClient
{
    public class Reader
    {
        public static List<string> Read()
        {          
            List<string> lista = new List<string>();
            string line = "";
            var stream = File.Open("..//..//..//Lista//Poruke.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            
            using(var sr = new StreamReader(stream))
            {
                   while ((line = sr.ReadLine()) != null)
                    {
                        lista.Add(line);
                    }

            }
         
           
            return lista;
        }
    }
}
