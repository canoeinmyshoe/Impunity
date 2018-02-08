using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 

using GraphicsController;

namespace GraphicsController.ExampleImpunityScripts
{
    public class Character : ImpunityScript
    {
  //      public SceneObject sceneObject { get; set; }
        public Character(SceneObject so)
        {
            sceneObject = so;
            sceneObject.Imps.Add(this);
            Start();
        }
        public override void Start()
        {
            //Do a bunch of stuff at startup
            Random random = new Random();
            float min = -24.0f;
            float max = 24.0f;
            float randx = (float)random.NextDouble() *(max-min);
            

            sceneObject.transform.position = new Vector3(-randx, 1.0f, 8.0f);
        }
        public override void Update()
        {
            //base.Update();
            sceneObject.transform.position.x += 0.01f;
            sceneObject.transform.rotation.y += 0.1f;
            //sceneObject.transform.rotation.x += 0.1f;
         //   sceneObject.transform.rotation.z += 0.1f;
        }
    }
}
