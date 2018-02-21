using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImpunityEngine;

namespace UserClasses
{
    public class TestClass : ImpunityClass
    {

        //These need an empty constructor to work
        public TestClass() {
            //do nothing
        }

        //use start as the ctor
        public override void Start(SceneObject sceneObject)
        {
            // base.Start();
            Console.WriteLine("The test class has started!");
            sceneObject.transform.position.y += 0.1f;
        }

        public override void Update(SceneObject sceneObject)
        {
            sceneObject.transform.rotation.y += 0.1f;
            if (Input.GetKey(GLFWKeys.SPACE) == true) {
                Console.WriteLine("Space bar!");
            }

        }

    }
}
