using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImpunityEngine.Interoperability;

namespace ImpunityEngine
{
    public class DirectionalLight : Component
    {

        public Vector3 direction { get; set; }
        public Vector3 ambient { get; set; }
        public Vector3 diffuse { get; set; }
        public Vector3 specular { get; set; }
        public bool dynamic = true;
        public DirectionalLight(int id)
        {
            ID = id;
            enabled = true;
            guid = Guid.NewGuid();

            direction = new Vector3(0.0f);
            ambient = new Vector3(0.2f);
            diffuse = new Vector3(0.5f);
            specular = new Vector3(1.0f);
        }
        public DirectionalLight(int id, Vector3 indirection)
        {
            ID = id;
            enabled = true;
            guid = Guid.NewGuid();

            direction = indirection;
            ambient = new Vector3(0.2f);
            diffuse = new Vector3(0.5f);
            specular = new Vector3(1.0f);
        }


        public override void Update(SceneObject sceneObject)
        {
            if (dynamic != true)
                return;

            Vector3 forward = sceneObject.Forward();
            SetDirection(forward);
        }

        

        //public static DirectionalLight FindLightByID(int id)
        //{
        //    foreach (var light in Control.AllSceneObjects)
        //    {
        //        if (light is DirectionalLight)
        //        {
        //            DirectionalLight d = (DirectionalLight)light;
        //            if (d.LightID == id)
        //                return d;
        //        }
        //    }
        //    throw new NullReferenceException($"Directional light ID \"{id}\" not found");
        //}
        //public static DirectionalLight FindLightByGuid(Guid gid)
        //{

        //    foreach (var light in Control.AllSceneObjects)
        //    {
        //        if (light.guid == gid)
        //        {
        //            return (DirectionalLight)light;
        //        }
        //    }
        //    throw new NullReferenceException();
        //}

        public void SetDirection(Vector3 vec) {
            direction = vec;
            Bridge.SetDirLightDirection(ID, vec);
        }
        public void SetAmbient(Vector3 color)
        {
            ambient = color;
            Bridge.SetDLightAmbient(ID, ambient);
        }
        public void SetDiffuse(Vector3 color)
        {
            diffuse = color;
            Bridge.SetDLightDiffuse(ID, diffuse);
        }
        public void SetSpecular(Vector3 color)
        {
            specular = color;
            Bridge.SetDLightSpecular(ID, specular);
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

            Bridge.SetDLightEnabled(ID, value);
        }
    }
}
