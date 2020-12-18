using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertificateManagerService
{
    class Program
    {
        static void Main(string[] args)
        {
            string cert = Console.ReadLine();
            string cmdText = "makecert - sv "+cert+".pvk"+" - iv TestCA.pvk - n \"CN=wcfservice\" - pe - ic TestCA.cer"+cert+".cer" +" - sr localmachine - ss My - sky exchange";
            string proba = "mkdir radi";


           
            



        }
    }
}
