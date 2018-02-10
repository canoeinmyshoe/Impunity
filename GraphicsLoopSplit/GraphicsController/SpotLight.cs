using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImpunityEngine.Interoperability;
namespace ImpunityEngine
{
    public class SpotLight : SceneObject
    {
        public int LightID { get; set; }
        public Vector3 position { get; set; }
        public Vector3 ambient { get; set; }
        public Vector3 diffuse { get; set; }
        public Vector3 specular { get; set; }
        public Vector3 direction { get; set; }
        public float cutOff { get; set; }
        public float outerCutOff { get; set; }
        public float maxDistance { get; set; }
        public bool enabled { get; set; }

        public SpotLight(int id)
        {
            LightID = id;
            ID = -1;
            position = new Vector3(0);
            direction = new Vector3(0);
            cutOff = 12.5f;
            outerCutOff = 15.0f;
            ambient = new Vector3(0.2f);
            diffuse = new Vector3(0.5f);
            specular = new Vector3(1.0f);
            maxDistance = 25.0f;
            isStatic = false;
        }
        public SpotLight(int id, Vector3 inposition, Vector3 indirection)
        {
            LightID = id;
            ID = -1;
            position = inposition;
            direction = indirection;
            cutOff = 12.5f;
            outerCutOff = 15.0f;
            ambient = new Vector3(0.2f);
            diffuse = new Vector3(0.5f);
            specular = new Vector3(1.0f);
            maxDistance = 25.0f;
            isStatic = false;
        }



        public override void Update()
        {
            //  base.Update();
            //do nothing

            //set the light transform in a different way, as it corresponds to a different vector in c++
        }


        public static SpotLight FindLightByID(int id)
        {
            foreach (var light in Control.AllSceneObjects)
            {
                if (light is SpotLight)
                {
                    SpotLight sl = (SpotLight)light;
                    if (sl.LightID == id)
                        return sl;
                }
            }
            throw new NullReferenceException($"Spot light ID \"{id}\" not found");
        }

        public void SetPosition(Vector3 pos)
        {
            position = pos;
            Bridge.SetSpotLightPosition(LightID, position);
        }

    }
}
