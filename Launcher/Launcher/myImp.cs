using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImpunityEngine;
namespace SceneEditLauncher
{
    public class myImp : ImpunityClass
    {
        private float testFloat;
        //These need an empty constructor to work
        public myImp()
        {
            //do nothing
        }

        //use start as the ctor
        public override void Start()
        {
            // base.Start();
            Console.WriteLine("The test class has started!");
        }

    }
}
