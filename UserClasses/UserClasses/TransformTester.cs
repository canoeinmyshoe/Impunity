using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImpunityEngine;

namespace UserClasses
{
    public class TransformTester : ImpunityClass
    {
        public TransformTester() { }

        public override void Start(SceneObject sceneObject)
        {
            // base.Start(sceneObject);
            Vector3 right = sceneObject.Right();
            Console.WriteLine(right.x + ", " + right.y + ", " + right.z);
        }

        public override void Update(SceneObject sceneObject)
        {
           // base.Update(sceneObject);

            Vector3 _right = sceneObject.Right();
            _right.Multiply(0.001f);

            sceneObject.transform.position.Add(_right);
            //sceneObject.transform.position.x += _right.x;
            //sceneObject.transform.position.y += _right.y;
            //sceneObject.transform.position.z += _right.z;

        }

    }
}
