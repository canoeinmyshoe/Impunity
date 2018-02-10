using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;

namespace SceneEditLauncher
{
    class Program
    {
        static void Main(string[] args)
        {
            //From here, we'll access the graphics controller in a particular way
            //However, what we should do is keep all our assets embedded
            //so you can start by embedding the GraphicsController.exe in here
            //at this point, I've totally forgotten how to open another prog!!!
            //We'll simply navigate by ../'s

            string[] farts = new string[1];
            farts[0] = "sup";
            //GraphicsProgram.Start(farts);

           // string dir = DirectoryManager.PathToGCBin();
            //GraphicsController.exe
           // dir += "\\GraphicsController.exe";

            //    Process secondProc = new Process();
            //secondProc.StartInfo.FileName = dir;
            // secondProc.Start("Fart");

           // Console.WriteLine("Process Directory: " + dir);

            //I guess it turns out we don't need the directory..
            Process.Start("ImpunityEngine.exe", "editor secondArg ThirdArg");

            //ProcessStartInfo processInfo = new ProcessStartInfo("GraphicsController.exe", "Hey hey!");
            //processInfo.CreateNoWindow = true;
            //processInfo.WindowStyle = ProcessWindowStyle.Hidden;
            
        //    Process.Start()

           // Console.ReadKey();
        }
    }
}
