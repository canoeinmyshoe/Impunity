using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImpunityEngine;
namespace UserClasses
{
    public class CameraController : ImpunityClass
    {
        //Use this class for flying around in the scene editor
        public override void Update(SceneObject sceneObject)
        {
            base.Update(sceneObject);

            //Tell the camera what to do here
            if (Input.GetKey(GLFWKeys.KEY_W)) {

              //  Console.WriteLine("FUCK");
                Control.SceneCamera.transform.position.Add(Control.SceneCamera.Forward());
            }
        }

    }
}
