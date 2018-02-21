using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImpunityEngine.Interoperability;

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
        public bool isStatic { get; set; }
        public Transform transform = new Transform(true);
        public SceneObject Parent { get; set; }
        public List<SceneObject> Children = new List<SceneObject>();
        public List<ImpunityClass> Imps = new List<ImpunityClass>();
        public Material material = new Material(MaterialType.DefaultTextureless);
        public bool enabled { get; set; }
        public string modelPath { get; set; }
        public Guid guid { get; set; }
        private bool updated = false;
        public SceneObject()
        {
            Name = "SceneObject";
            Tag = string.Empty;
            ID = -1;
            MeshID = 0;
            ShaderID = 0;
            isStatic = false;
            ParentID = -25;
            isChild = false;
            enabled = true;
            modelPath = "NA";
            guid = Guid.NewGuid();

            //this is a doozy if left unchecked
           // isStatic = true;
        }
        public SceneObject(bool ISSTATIC)
        {
            Name = "SceneObject";
            Tag = string.Empty;
            var rand = new Random();
            int rrr = rand.Next(6000);
            ID = -rrr;
            MeshID = 0;
            ShaderID = 0;
            isStatic = true;
            ParentID = -25;
            isChild = false;
            enabled = true;
            modelPath = "NA";
            guid = Guid.NewGuid();

            //this is a doozy if left unchecked
            // isStatic = true;
        }


        public virtual void Start()
        {
            //Start everything up
            foreach (var imp in Imps)
            {
                imp.Start(this);
            }
        }
        public virtual void Update()
        {

            foreach (var imp in Imps)
            {
                imp.Update(this);
            }

            if (isStatic == true)
                return;

            if (updated == true)
            {
                updated = false;
                return;
            }



            UpdateChildren();



            if (ID < 0)
                return;

            SetTransform();
        }

        public void UpdateChildren() {
            foreach (var child in Children)
            {
                child.UpdateChildren();
                child.SetTransform();
                child.updated = true; // don't let it set the
                //transform again after we multiply matrices

            Bridge.SetChildMatrix(ID, child.ID);
            }
        }
    }
}
