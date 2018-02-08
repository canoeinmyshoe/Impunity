using System;

namespace GCCore
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("GCCore debug mode");
            Console.ReadKey();

            Control c = new Control();
            // c.RunMain();
            // c.ManualLoop();
            c.GetFloat();
        }
    }
}
