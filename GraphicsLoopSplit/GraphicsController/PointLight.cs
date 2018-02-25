using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImpunityEngine.Interoperability;
namespace ImpunityEngine
{
    public class PointLight : Component
    {
        
        public Vector3 ambient { get; set; }
        public Vector3 diffuse { get; set; }
        public Vector3 specular { get; set; }
        public float constant { get; set; }
        public float linear { get; set; }
        public float quadratic { get; set; }
        public float maxDistance { get; set; }
        //   public bool enabled { get; set; }
        public bool dynamic { get; set; }

        public PointLight(int id)
        {
            ID = id;
            guid = Guid.NewGuid();
            enabled = true;

            //defualt values
            //We now inherit position from a sceneObject's transform
            //    position = new Vector3(0);
            ambient = new Vector3(0.2f, 0.2f, 0.2f);
            diffuse = new Vector3(0.5f, 0.5f, 0.5f);
            specular = new Vector3(1.0f);
            constant = 1.0f;
            linear = 0.09f;
            quadratic = 0.032f;
            maxDistance = 10.0f;

            dynamic = true;
            
        }
        public PointLight(int id, Vector3 inposition)
        {
            ID = id;
            guid = Guid.NewGuid();
            enabled = true;
            //defualt values
            //  position = inposition;
            ambient = new Vector3(0.2f, 0.2f, 0.2f);
            diffuse = new Vector3(0.5f, 0.5f, 0.5f);
            specular = new Vector3(1.0f);
            constant = 1.0f;
            linear = 0.09f;
            quadratic = 0.032f;
            maxDistance = 10.0f;
            dynamic = true;
        }

        public  override void Update(SceneObject sceneObject)
        {
            //use the sceneObject passed in to update the variables associated with the pointlight

            if (dynamic != true)
                return;

            SetPosition(sceneObject.transform);
        }

        //The methods for finding a pointlight
        //public static PointLight FindLightByID(int id)
        //{
        //    foreach (var light in Control.AllSceneObjects)
        //    {
        //        if (light is PointLight)
        //        {
        //            PointLight p = (PointLight)light;
        //            if (p.LightID == id) {
        //                return p;
        //            }
        //        }
        //    }
        //    throw new NullReferenceException($"Point light ID \"{id}\" not found");
        //}
        //public static PointLight FindLightByGuid(Guid gid) {

        //    foreach (var light in Control.AllSceneObjects)
        //    {
        //        if (light.guid == gid)
        //        {
        //            return (PointLight)light;
        //        }
        //    }
        //    throw new NullReferenceException();
        //}


        public void SetPosition(Transform transfom)//the light would do this if it's dynamic
        {
            //This method will tell c++ to Directly set the pointLight's Position
            //Bridge.SetPointLightPosition(int id, float x, float y, float z);
            Bridge.SetPointLightPosition(ID, transfom.position);
        }
        public void SetPosition(Vector3 pos)
        {
            //This method will tell c++ to Directly set the pointLight's Position
            //Bridge.SetPointLightPosition(int id, float x, float y, float z);
          
            Bridge.SetPointLightPosition(ID, pos);
        }
        public void SetPosition(float x, float y, float z)
        {
            //This method will tell c++ to Directly set the pointLight's Position
            //Bridge.SetPointLightPosition(int id, float x, float y, float z);
            //position = new Vector3(x, y, z);
            Bridge.SetPointLightPosition(ID, x, y, z);
        }

        public void SetAmbient(Vector3 amb)
        {
            ambient = amb;
            Bridge.SetPLightAmbient(ID, ambient);
        }
        public void SetDiffuse(Vector3 col)
        {
            diffuse = col;
            Bridge.SetPLightDiffuse(ID, diffuse);
        }
        public void SetSpecular(Vector3 col)
        {
            specular = col;
            Bridge.SetPLightSpecular(ID, specular);
        }
        public void SetMaxDistance(float radius)
        {
            maxDistance = radius;
            Bridge.SetMaxDistancePLight(ID, radius);
        }
        public void SetEnabled(bool enable)
        {
            int value = 0;
            if (enable == true){
                value = 1;
                enabled = true;
            }
            else {
                value = 0;
                enabled = false;
            }

            Bridge.SetPLightEnabled(ID, value);
        }

        //We need methods for setting other values, too.
    }
}
