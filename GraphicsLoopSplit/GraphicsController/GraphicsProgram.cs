using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;

using GraphicsController.OpenGLExamples;
namespace GraphicsController
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
            Interoperability.CommandLinerInterpreter Reader = new Interoperability.CommandLinerInterpreter();
            Reader.InterpretOpeningArgs(args);

            //if(runDebugMode == true)
         
        }
        
    }
}
