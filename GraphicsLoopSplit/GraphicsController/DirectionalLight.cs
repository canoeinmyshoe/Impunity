using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImpunityEngine.Interoperability;

namespace ImpunityEngine
{
    public class DirectionalLight : SceneObject
    {
        public int LightID { get; set; }
        public Vector3 direction { get; set; }
        public Vector3 ambient { get; set; }
        public Vector3 diffuse { get; set; }
        public Vector3 specular { get; set; }
      //  public bool enabled { get; set; }

        public DirectionalLight(int id)
        {
            LightID = id;
            ID = -1;
            direction = new Vector3(0.0f);
            ambient = new Vector3(0.2f);
            diffuse = new Vector3(0.5f);
            specular = new Vector3(1.0f);
            enabled = true;
            isStatic = false;
            guid = Guid.NewGuid();
        }
        public DirectionalLight(int id, Vector3 indirection)
        {
            LightID = id;
            ID = -1;
            direction = indirection;
            ambient = new Vector3(0.2f);
            diffuse = new Vector3(0.5f);
            specular = new Vector3(1.0f);
            enabled = true;
            isStatic = false;
            guid = Guid.NewGuid();
        }

        public override void Update()
        {
          //  base.Update();

            //Update the directional light's transform
        }

        public static DirectionalLight FindLightByID(int id)
        {
            foreach (var light in Control.AllSceneObjects)
            {
                if (light is DirectionalLight)
                {
                    DirectionalLight d = (DirectionalLight)light;
                    if (d.LightID == id)
                        return d;
                }
            }
            throw new NullReferenceException($"Directional light ID \"{id}\" not found");
        }

        public void SetDirection(Vector3 vec) {
            Bridge.SetDirLightDirection(LightID, vec);
        }
        public void SetAmbient(Vector3 color)
        {
            ambient = color;
            Bridge.SetDLightAmbient(LightID, ambient);
        }
        public void SetDiffuse(Vector3 color)
        {
            diffuse = color;
            Bridge.SetDLightDiffuse(LightID, diffuse);
        }
        public void SetSpecular(Vector3 color)
        {
            specular = color;
            Bridge.SetDLightSpecular(LightID, specular);
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

            Bridge.SetDLightEnabled(LightID, value);
        }
    }
}
