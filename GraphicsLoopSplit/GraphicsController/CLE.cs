using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;


namespace ImpunityEngine
{
    public class CLE//The Command Line Editor
    {
        //In one thread, the command line.
        private bool shouldRun = true;
        //In another, the c# logic for rendering the scene, putting things in the scene,
        //configuring the sceneObjects, and saving their configuration so it can be loaded
        public void Run()
        {
            //You'll have to break out the web server for thread info!
            
            Thread CommandListenerT = new Thread(() =>
            {
                ListenForCommands();
            });
            CommandListenerT.Start();

            //And now a separate thread for rendering and doing stuff
            Thread GraphicsThreadT = new Thread(() => 
            {

            });
            GraphicsThreadT.Start();
        }

        private void ListenForCommands()
        {
            Console.WriteLine("Impunity Command Line Editor 1.0");
            CommandLinerInterpreter listener = new CommandLinerInterpreter();
            while (shouldRun == true)
            {
                string input = Console.ReadLine();
                //processCommandLineInput(input);
                listener.ProcessInput(input);
            }
        }


    }
}
