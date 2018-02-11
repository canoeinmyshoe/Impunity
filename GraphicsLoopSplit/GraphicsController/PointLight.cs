using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImpunityEngine.Interoperability;
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
        public float maxDistance { get; set; }
     //   public bool enabled { get; set; }

        public PointLight(int id)
        {
            LightID = id;//index in the c++ vector of pointlights....
            ID = -1;//should never correspond to an actual SceneObject in the c++ vector
            Name = "PointLight" + id;
            Tag = "none";
            MeshID = -1;
            ShaderID = -1;
            ParentID = -1;
            isChild = false;
            

            //defualt values
            position = new Vector3(0);
            ambient = new Vector3(0.2f, 0.2f, 0.2f);
            diffuse = new Vector3(0.5f, 0.5f, 0.5f);
            specular = new Vector3(1.0f);
            constant = 1.0f;
            linear = 0.09f;
            quadratic = 0.032f;
            maxDistance = 10.0f;
            enabled = true;
            isStatic = false;
        }
        public PointLight(int id, Vector3 inposition)
        {
            LightID = id;//index in the c++ vector of pointlights....
            ID = -1;//should never correspond to an actual SceneObject in the c++ vector

            //defualt values
            position = inposition;
            ambient = new Vector3(0.2f, 0.2f, 0.2f);
            diffuse = new Vector3(0.5f, 0.5f, 0.5f);
            specular = new Vector3(1.0f);
            constant = 1.0f;
            linear = 0.09f;
            quadratic = 0.032f;
            maxDistance = 10.0f;
            enabled = true;
            isStatic = false;
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
                    PointLight p = (PointLight)light;
                    if (p.LightID == id) {
                        return p;
                    }
                }
            }
            throw new NullReferenceException($"Point light ID \"{id}\" not found");
        }
        public void SetPosition()//the light would do this if it's dynamic
        {
            //This method will tell c++ to Directly set the pointLight's Position
            //Bridge.SetPointLightPosition(int id, float x, float y, float z);
            Bridge.SetPointLightPosition(LightID, position);
        }
        public void SetPosition(Vector3 pos)
        {
            //This method will tell c++ to Directly set the pointLight's Position
            //Bridge.SetPointLightPosition(int id, float x, float y, float z);
            position = pos;
            Bridge.SetPointLightPosition(LightID, position);
        }
        public void SetPosition(float x, float y, float z)
        {
            //This method will tell c++ to Directly set the pointLight's Position
            //Bridge.SetPointLightPosition(int id, float x, float y, float z);
            position = new Vector3(x, y, z);
            Bridge.SetPointLightPosition(LightID, x, y, z);
        }
        public void SetAmbient(Vector3 amb)
        {
            ambient = amb;
            Bridge.SetPLightAmbient(LightID, ambient);
        }
        public void SetDiffuse(Vector3 col)
        {
            diffuse = col;
            Bridge.SetPLightDiffuse(LightID, diffuse);
        }
        public void SetSpecular(Vector3 col)
        {
            specular = col;
            Bridge.SetPLightSpecular(LightID, specular);
        }
        public void SetMaxDistance(float radius)
        {
            maxDistance = radius;
            Bridge.SetMaxDistancePLight(LightID, radius);
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

            Bridge.SetPLightEnabled(LightID, value);
        }

        //We need methods for setting other values, too.
    }
}
