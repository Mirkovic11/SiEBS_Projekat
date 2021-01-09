
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using System.Text;
using System.Threading.Tasks;

namespace WCFClient
{
    public class Writer
    {
        
        public static void WriteMsg(string msg)
        {
            using (StreamWriter sw = new StreamWriter("..//..//..//Lista//Poruke.txt", true))
            {
                
                    sw.WriteLine(msg);

                
              
            }
        }

        public static List<string> Read()
        {
            List<string> lista = new List<string>();
            string line = "";
            using (StreamReader sr = new StreamReader("..//..//..//Lista//Poruke.txt"))
            {
                
                    while ((line = sr.ReadLine()) != null)
                    {
                        lista.Add(line);
                    }
                
                
                
            }
            return lista;
        }
        public static void Delete()
        {
            using (FileStream fs = File.Create("..//..//..//Lista//Poruke.txt"))
            {

            }
        }
    }
}
