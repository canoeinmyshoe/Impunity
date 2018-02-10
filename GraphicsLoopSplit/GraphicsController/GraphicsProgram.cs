using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;

using ImpunityEngine.OpenGLExamples;
namespace ImpunityEngine
{
    public class GraphicsProgram
    {
        //Another project will launch this one with some args. 
        static void Main(string[] args)
        {
            //based on args, we'll either load a StartUpScene,
            //Or Attempt to load a save file
            //AS either dev or player


            //    bool test = false;
            //    Debug.Assert(test, "Message");
           CommandLinerInterpreter Reader = new CommandLinerInterpreter();
            Reader.InterpretOpeningArgs(args);

            //if(runDebugMode == true)
            Console.WriteLine("End of the line.");
            Console.ReadKey();
        }
        
    }
}
