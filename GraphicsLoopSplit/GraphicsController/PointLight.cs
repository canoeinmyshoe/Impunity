using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImpunityEngine
{
    public class PointLight : SceneObject
    {

        public int LightID { get; set; }
        public Vector3 position { get; set; }
        public Vector3 ambient { get; set; }
        public Vector3 diffuse { get; set; }
        public Vector3 specular { get; set; }
        public float constant { get; set; }
        public float linear { get; set; }
        public float quadratic { get; set; }
        public bool enabled { get; set; }

        public PointLight(int id)
        {
            LightID = id;//index in the c++ vector of pointlights....
            ID = -1;//should never correspond to an actual SceneObject in the c++ vector
        }

        public override void Update()
        {
          //  base.Update();
            //do nothing

            //set the light transform in a different way, as it corresponds to a different vector in c++

        }

        public static PointLight FindLightByID(int id)
        {
            foreach (var light in Control.AllSceneObjects)
            {
                if (light is PointLight)
                {
                    if (light.ID == id)
                        return (PointLight)light;
                }
            }
            throw new NullReferenceException($"Point light ID \"{id}\" not found");
        }

        //We need methods for setting other values, too.
    }
}
