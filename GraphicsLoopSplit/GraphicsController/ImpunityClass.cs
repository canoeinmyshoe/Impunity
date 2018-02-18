using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImpunityEngine
{
    public abstract class ImpunityClass
    {
        public SceneObject sceneObject { get; set; }
        public bool enabled = true;
        public ImpunityClass() {
            sceneObject = null;
        }
        public ImpunityClass(SceneObject so)
        {
            sceneObject = so;
        }
        public virtual void Start()
        {
            var s = Resource1.MyName;
            //Do a bunch of stuff at startup
        }
        public virtual void Update()
        {
            //do a bunch of stuff every frame
        }
    }
}
