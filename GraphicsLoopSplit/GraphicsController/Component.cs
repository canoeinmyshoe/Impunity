using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImpunityEngine
{
    public class Component : IUpdatable
    {
        public int ID { get; set; }//The component's index in its respective c++ vector
        public Guid guid { get; set; }
        public bool enabled { get; set; }

        public virtual void Update(SceneObject sceneObject)
        {
            throw new NotImplementedException();
        }
    }
}
