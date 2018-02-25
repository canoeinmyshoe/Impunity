using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace ImpunityEngine
{
    public class SerializableSceneObject
    {
        public string Name { get; set; }
        public string Tag { get; set; }
        public virtual int ID { get; set; }
        public int MeshID { get; set; }
        public int ShaderID { get; set; }
        public int ParentID { get; set; }
        public bool isChild { get; set; }
        public bool isStatic { get; set; }
        public Transform transform = new Transform(true);
        public int[] ChildIDs;
        public List<ImpunityClass> Imps = new List<ImpunityClass>();
    //    public List<string> impTypes = new List<string>();
        public Material material = new Material(MaterialType.DefaultTextureless);
        public string modelPath { get; set; }
        public Guid guid { get; set; }
        public List<Guid> ChildGuids { get; set; }
        public bool enabled { get; set; }
        public List<Component> Components = new List<Component>();

    }

    public class SerializableDirectionalLight : SerializableSceneObject
    {
        public int LightID { get; set; }
        public Vector3 direction { get; set; }
        public Vector3 ambient { get; set; }
        public Vector3 diffuse { get; set; }
        public Vector3 specular { get; set; }
 //       public bool enabled { get; set; }
    }
    public class SerializablePointLight : Component
    {
        public Vector3 ambient { get; set; }
        public Vector3 diffuse { get; set; }
        public Vector3 specular { get; set; }
        public float constant { get; set; }
        public float linear { get; set; }
        public float quadratic { get; set; }
        public float maxDistance { get; set; }
        public bool dynamic { get; set; }
    }
    public class SerializableSpotLight : SerializableSceneObject
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
      //  public bool enabled { get; set; }
        public float cutOffRatio = 0.8333f;
    }
}
