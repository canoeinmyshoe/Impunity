using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImpunityEngine
{
    public class Light : SceneObject
    {
        //Has all the props of the parent
        public int LightID { get; set; }

        public override void Update()
        {
            //Do Light-specific stuff
        }

    }
}
