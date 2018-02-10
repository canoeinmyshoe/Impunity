using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using ImpunityEngine.Interoperability;

namespace ImpunityEngine
{
    public class CLE//The Command Line Editor
    {
        //In one thread, the command line.
        private bool shouldRun = false;
        private List<string> Commands = new List<string>();
        
        //In another, the c# logic for rendering the scene, putting things in the scene,
        //configuring the sceneObjects, and saving their configuration so it can be loaded
        public bool Run()
        {

            if (shouldRun == true)
                return false;

            shouldRun = true;

            Console.WriteLine("Well?");

            Thread CommandListenerT = new Thread(() =>
            {
                ListenForCommands();
            });
            CommandListenerT.Start();
            Console.WriteLine("Test.");
            RunImpunity();
            
            return shouldRun;
        }

        private void ListenForCommands()
        {
            Console.WriteLine("Impunity Command Line Editor 1.0");
            while (shouldRun == true)
            {
                string input = Console.ReadLine();
                //processCommandLineInput(input);
                //  listener.ProcessInput(input);
                Commands.Add(input);
                Console.WriteLine("Gee, that's something...");
            }
        }

        private void RunImpunity()
        {
            Console.WriteLine("Starting Impunity.");
            //ONLY initiate and loop the engine. Other than that, the Interpreter and the Listener share a list of commands (to be created)
            int success = Bridge.InitiateEngine();
            CommandLinerInterpreter listener = new CommandLinerInterpreter();

            while (shouldRun == true)
            {
                for (int i = 0; i < Commands.Count; i++)
                {
                    listener.ProcessInput(Commands[i]);
                    Commands.Remove(Commands[i]);
                }
                //change any positions, variables, etc
                //add forces to rigidbodies, create, etc
                foreach (var so in Control.AllSceneObjects)
                {
                    so.Update();
                }

                int r = Bridge.RenderAll();
            }
        }


    }
}
