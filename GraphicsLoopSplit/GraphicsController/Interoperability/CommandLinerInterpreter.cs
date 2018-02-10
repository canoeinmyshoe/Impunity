using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImpunityEngine.OpenGLExamples;

namespace ImpunityEngine
{
    public class CommandLinerInterpreter
    {

        public  void InterpretOpeningArgs(string[] args)
        {
            if (args.Length == 0)
                return;

            foreach (var item in args)
            {
                Console.WriteLine(item);
            }

            if (args[0] == "openGLexamples")
            {
                SpaghettiScene();
            }
            else if (args[0] == "editor")
            {
                CLE cle = new CLE();
                cle.Run();
            }
            else
            {
                Console.WriteLine("ERROR: COMMAND " + args[0] + " UNRECOGNIZED");
            }
        }

        //Most actions taken will result in a save file string log.... thing
        public void ProcessInput(string input)
        {

        }
        
        void RunEditor()
        {
            Control c = new Control();
            //OpenGLExampleController ogl = new OpenGLExampleController();
            //ogl.DrawObjects();
            CLE GT = new CLE();
            //This thread initiates the c++/openGL affair, and continuosly loops while polling
            //for input from the command line. This allows us to set postions or add scripts, pause, play
            //and most importantly, create a .ImpunityScene file, from which we can load
            //allowing us to create a growing colleciton of scenes, which would constitute...
            //anything from a game to a web browser. Buttons and text coming soon.
        }

        void SpaghettiScene()
        {
            Control c = new Control();
            OpenGLExampleController ogl = new OpenGLExampleController();
            ogl.DrawObjects();
        }


    }
}
