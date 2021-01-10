
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WCFClient
{
    public class Writer
    { 
        public static void WriteMsg(string msg)
        {
            var stream = File.Open("..//..//..//Lista//Poruke.txt", FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            using(var sw = new StreamWriter(stream))
            {
              sw.WriteLine(msg);              
            }         
        }
    }
}
