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

        //Will eventually be components
        public List<ImpunityClass> Imps = new List<ImpunityClass>();
        public Material material = new Material(MaterialType.DefaultTextureless);


        public bool enabled { get; set; }
        public string modelPath { get; set; }
        public Guid guid { get; set; }
        private bool updated = false;

        //Now for one of the most important parts of the Scene Object: components!
        public List<Component> Components = new List<Component>();
        //This is where lights, classes, and many other components will lives

        public SceneObject(int id)//Whenever a sceneObject is made, we have to have a guarantee that it has an existing counterpart
        {
            Name = "SceneObject";
            Tag = string.Empty;
            ID = id;
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
        public SceneObject()//Whenever a sceneObject is made, we have to have a guarantee that it has an existing counterpart - However, to make life easier, it will for right now
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
        public virtual void Update() //eventually, will not be virtual
        {
            //will eventually be a component
            foreach (var imp in Imps)
            {
                imp.Update(this);
            }

            if (isStatic == true)
                return;

            //Update all of the components
            foreach (var item in Components)
            {
                if (item.enabled != true)
                    return;
                item.Update(this);
            }

            if (updated == true)
            {
                updated = false;
                return;
            }



            UpdateChildren();



            if (ID < 0)
                return;

            SetTransformTwo();
        }

        public void UpdateChildren() {
            foreach (var child in Children)
            {
                child.UpdateChildren();
                child.SetTransformTwo();
                child.updated = true; // don't let it set the
                //transform again after we multiply matrices

            Bridge.SetChildMatrix(ID, child.ID);
            }
        }
    }
}
