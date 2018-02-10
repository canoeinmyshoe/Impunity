using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;

namespace ImpunityEngine
{
    //This is where all the openGL method declarations can go
    public class Control
    {
        public static List<SceneObject> AllSceneObjects = new List<SceneObject>();
        public static List<Texture> AllTextures = new List<Texture>();

        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int main();

        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Initiate();

        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int OnlyRender();

        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CloseAllWindows();

        public void RunMain()
        {
            int i = main();
        }

        public void ManualLoop()
        {
            int i = Initiate();
            while (true)
            {
                int r = OnlyRender();
                if (r == 2)
                {
                    //close down
                    int q = CloseAllWindows();
                }
            }
        }

        public void LoopThread()
        {
            Thread Runner = new Thread(() =>
            {
                ManualLoop();
            });
            Runner.Start();
            Console.WriteLine("Began running.");
        }

        
    }
}
