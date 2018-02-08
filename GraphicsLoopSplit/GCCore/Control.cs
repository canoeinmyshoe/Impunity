using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;

namespace GCCore
{
    public class Control
    {
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int main();

        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Initiate();

        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int OnlyRender();

        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CloseAllWindows();
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int GiveFloat(float x);

        public void GetFloat()
        {
            int i = GiveFloat(3.14f);
        }

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
