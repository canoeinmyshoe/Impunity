using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImpunityEngine.Interoperability;
namespace ImpunityEngine
{
    public class SpotLight : Component
    {


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
            ID = id;
            guid = Guid.NewGuid();
            enabled = true;


            direction = new Vector3(0);
            cutOff = 12.5f;
            outerCutOff = 15.0f;
            ambient = new Vector3(0.2f);
            diffuse = new Vector3(0.5f);
            specular = new Vector3(1.0f);
            maxDistance = 25.0f;
         
        }
        public SpotLight(int id, Vector3 inposition, Vector3 indirection)
        {
            ID = id;
            guid = Guid.NewGuid();
            enabled = true;
            
            direction = indirection;
            cutOff = 12.5f;
            outerCutOff = 15.0f;
            ambient = new Vector3(0.2f);
            diffuse = new Vector3(0.5f);
            specular = new Vector3(1.0f);
            maxDistance = 25.0f;
        }

        public override void Update(SceneObject sceneObject)
        {
            //  base.Update();
            //do nothing
            Vector3 forward = sceneObject.Forward();
            SetDirection(forward);

            SetPosition(sceneObject.transform.position);
        }


        //public static SpotLight FindLightByID(int id)
        //{
        //    foreach (var light in Control.AllSceneObjects)
        //    {
        //        if (light is SpotLight)
        //        {
        //            SpotLight sl = (SpotLight)light;
        //            if (sl.LightID == id)
        //                return sl;
        //        }
        //    }
        //    throw new NullReferenceException($"Spot light ID \"{id}\" not found");
        //}
        //public static SpotLight FindLightByGuid(Guid gid)
        //{

        //    foreach (var light in Control.AllSceneObjects)
        //    {
        //        if (light.guid == gid)
        //        {
        //            return (SpotLight)light;
        //        }
        //    }
        //    throw new NullReferenceException();
        //}

        public void SetPosition(Vector3 pos)
        {
           // position = pos;
            Bridge.SetSpotLightPosition(ID, pos);
        }
        public void SetDirection(Vector3 dir)
        {
            direction = dir;
            Bridge.SetSpotLightDirection(ID, direction);
        }
        public void SetAmbient(Vector3 color)
        {
            ambient = color;
            Bridge.SetSLightAmbient(ID, ambient);
        }
        public void SetDiffuse(Vector3 color)
        {
            diffuse = color;
            Bridge.SetSLightDiffuse(ID, diffuse);
        }
        public void SetSpecular(Vector3 color)
        {
            specular = color;
            Bridge.SetSLightSpecular(ID, specular);
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
            Bridge.SetSLightEnabled(ID, value);
        }

        public void SetMaxDistance(float radius)
        {
            maxDistance = radius;
            Bridge.SetMaxDistanceSLight(ID, radius);
        }

        public void SetCutOff(float angle)
        {
            cutOff = angle;
            outerCutOff = cutOff / cutOffRatio;
            //Console.WriteLine("Outer cutoff: " + outerCutOff);
            Bridge.SetSpotLightCutOff(ID, cutOff, outerCutOff);
        }


    }
}
