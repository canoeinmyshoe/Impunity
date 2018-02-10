using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImpunityEngine
{
    public class DirectionalLight : SceneObject
    {
        public int LightID { get; set; }
        public Vector3 direction { get; set; }
        public Vector3 ambient { get; set; }
        public Vector3 diffuse { get; set; }
        public Vector3 specular { get; set; }
        public bool enabled { get; set; }

        public DirectionalLight(int id)
        {
            LightID = id;
            ID = -1;
            direction = new Vector3(0.0f);
            ambient = new Vector3(0.2f);
            diffuse = new Vector3(0.5f);
            specular = new Vector3(1.0f);
            enabled = true;
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
        }

        public static DirectionalLight FindLightByID(int id)
        {
            foreach (var light in Control.AllSceneObjects)
            {
                if (light is DirectionalLight)
                {
                    if (light.ID == id)
                        return (DirectionalLight)light;
                }
            }
            throw new NullReferenceException($"Directional light ID \"{id}\" not found");
        }

    }
}
