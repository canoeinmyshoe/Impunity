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
     //   public bool enabled { get; set; }
        public float cutOffRatio = 0.8333f;

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
            guid = Guid.NewGuid();
        }
        public SpotLight(int id, Vector3 inposition, Vector3 indirection)
        {
            LightID = id;
            ID = -1;
            guid = Guid.NewGuid();
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

        public void SetAmbient(Vector3 color)
        {
            ambient = color;
            Bridge.SetSLightAmbient(LightID, ambient);
        }
        public void SetDiffuse(Vector3 color)
        {
            diffuse = color;
            Bridge.SetSLightDiffuse(LightID, diffuse);
        }
        public void SetSpecular(Vector3 color)
        {
            specular = color;
            Bridge.SetSLightSpecular(LightID, specular);
        }
        public void SetEnabled(bool enable)
        {
            int value = 0;
            if (enable == true)
            {
                value = 1;
                enabled = true;
            }
            else
            {
                value = 0;
                enabled = false;
            }
            Bridge.SetSLightEnabled(LightID, value);
        }

        public void SetMaxDistance(float radius)
        {
            maxDistance = radius;
            Bridge.SetMaxDistanceSLight(LightID, radius);
        }

        public void SetCutOff(float angle)
        {
            cutOff = angle;
            outerCutOff = cutOff / cutOffRatio;
            //Console.WriteLine("Outer cutoff: " + outerCutOff);
            Bridge.SetSpotLightCutOff(LightID, cutOff, outerCutOff);
        }
    }
}
