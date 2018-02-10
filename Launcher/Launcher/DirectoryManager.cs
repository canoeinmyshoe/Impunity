using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SceneEditLauncher
{
    public class DirectoryManager
    {

        public static string PathToGCBin()
        {

            //1. reflect
            string path = System.IO.Path.GetDirectoryName(
          System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);

            Console.WriteLine(path);

            return "";
        }
    }
}
