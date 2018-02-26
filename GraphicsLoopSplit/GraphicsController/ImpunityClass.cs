using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImpunityEngine
{
    public abstract class ImpunityClass : Component
    {
      //  public SceneObject sceneObject { get; set; }
       
        public ImpunityClass() {
            //  sceneObject = null;
            ID = -1;
            enabled = true;
            guid = Guid.NewGuid();
        }
        public ImpunityClass(SceneObject so)
        {
           // sceneObject = so;
        }
        public virtual void Start(SceneObject sceneObject)
        {
           // var s = Resource1.MyName;
            //Do a bunch of stuff at startup
        }
        public override void Update(SceneObject sceneObject)
        {
            //do a bunch of stuff every frame
        }
    }
}
