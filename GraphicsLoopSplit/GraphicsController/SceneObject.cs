using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImpunityEngine
{
    public partial class SceneObject
    {
        public string Name { get; set; }
        public string Tag { get; set; }
        public virtual int ID { get; set; }
        public int MeshID { get; set; }
        public int ShaderID { get; set; }
        public int ParentID { get; set; }
        public bool isChild { get; set; }
        public Transform transform = new Transform(true);
        public SceneObject Parent { get; set; }
        public List<SceneObject> Children = new List<SceneObject>();
        public List<ImpunityScript> Imps = new List<ImpunityScript>();
        public Material material = new Material(MaterialType.DefaultTextureless);

        public SceneObject()
        {
            Name = "SceneObject";
            Tag = string.Empty;
            ID = -1;
            MeshID = 0;
            ShaderID = 0;

        }

        public virtual void Start()
        {
            //Start everything up
            foreach (var imp in Imps)
            {
                imp.Start();
            }
        }
        public virtual void Update()
        {

            foreach (var imp in Imps)
            {
                imp.Update();
            }


            foreach (var child in Children)
            {
                child.transform.position = transform.position;
                child.transform.rotation = transform.rotation;
                child.transform.SetTransform(child.ID);
            }

            if (ID < 0)
                return;
            transform.SetTransform(ID);
        }
    }
}
