using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicsController
{
    public class Light
    {
        //Has all the props of the parent
        public int LightID { get; set; }

        public virtual void Update()
        {
            //Do Light-specific stuff
        }

    }
}
