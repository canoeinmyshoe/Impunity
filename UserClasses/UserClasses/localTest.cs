using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImpunityEngine;
namespace UserClasses
{
    public class localTest : ImpunityClass
    {
        private bool goUp = true;
        public override void Update(SceneObject sceneObject)
        {
            sceneObject.transform.rotation.y += 0.1f;

            UpNDown(sceneObject);
            GoForward(sceneObject);
        }
        private void GoForward(SceneObject sceneObject) {

            Vector3 forward = sceneObject.Forward();
            forward.Multiply(0.0008f);
            sceneObject.transform.position.Add(forward);
        }
        private void UpNDown(SceneObject sceneObject) {
            float degree = -sceneObject.transform.localRotation.x;
            if (goUp == true)
            {
                if (degree > 90)
                    goUp = false;

                sceneObject.transform.localRotation.x += 0.03f;
            }
            else
            {
                if (degree < 50)
                    goUp = true;

                sceneObject.transform.localRotation.x -= 0.03f;
            }
        }
    }
}
