using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImpunityEngine
{
    public static class Camera 
    {

        public static Transform transform = new Transform(true);
        public static float Zoom;
        public static float NearClippingPlane = 0.1f;
        public static float FarClippingPlane = 1000.0f;

        public static void Update()
        {
            //tell bridge the transform

        }

       
    }
}
