using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;

using ImpunityEngine;
using UserClasses;

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

            //CLE cle = new CLE();
            //cle.Run();

            CommandLineEditor editor = new CommandLineEditor();
            editor.Run();

            //The SceneEditLauncher class will need its own version of the CLE
            //So that it can add user created classes to a sceneObject

            //So this can work!! Exellent.
            //However, it MUST be this project which acts as middle man between the user project
            //and the impunity engine
            //TestClass tc = new TestClass();
            //Control.AllSceneObjects[0].Imps.Add(tc);
        }
    }
}
