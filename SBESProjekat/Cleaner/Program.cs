using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cleaner
{
    class Program
    {
        static void Main(string[] args)
        {
            FileStream fs = File.Create("..//..//..//Lista//Poruke.txt");
            fs = File.Create("..//..//..//Lista//JavljanjeKlijenata.txt");

            //fs = File.Create("..//..//..//CMSBackup//bin//Debug//CertListBackup.txt");
            //fs = File.Create("..//..//..//CMSBackup//bin//Debug//RevocationListBackup.txt");

        }
    }
}
