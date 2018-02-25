using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Serialization;
using System.IO;
using ImpunityEngine.SceneManipulation;
using UserClasses;


namespace ImpunityEngine.Persistence
{
    public class DIskManager
    {

        #region Loading A Scene From Disk

        public static void LoadSceneFile(string filename)
        {

            if (!File.Exists(filename))
            {
                Console.WriteLine("No such file " + filename);
                return;
            }

            SceneFile scene;
            Type[] userTypes = AssemblyManager.UserTypes();
            using (var sr = new StreamReader(filename))
            {
                XmlSerializer xs = new XmlSerializer(typeof(SceneFile), userTypes);
                scene = (SceneFile)xs.Deserialize(sr);
                sr.Close();
            }

            //1st, we take a pass at loading everything from disk/blob
            Console.WriteLine("Scene Load Results: ");
            foreach (var so in scene.AllSceneObjects)
            {
                Console.WriteLine(so.Name);
                LoadSceneObject(so);
            }
           
         
          
            //2nd, we go through every item and set the data to their correct values
            foreach (var item in scene.AllSceneObjects)
            {
                DetailSceneObject(item, scene.AllSceneObjects);
            }
          

            foreach (var item in scene.AllSkyboxes)
            {
                SceneMaster.CreateCubeMap(item.images);
            }
        }

        private static void DetailSceneObject(SerializableSceneObject ser, List<SerializableSceneObject> allSer)
        {
            if (ser.ID < 0) { return; }
              //  return;
            //Find the SceneObject by Guid
            SceneObject so;
            try { so = SceneObject.FindByGuid(ser.guid); } catch { Console.WriteLine("WARNING: Failed to find SceneObject " + ser.guid.ToString()); return; }

            so.Name = ser.Name;
            so.Tag = ser.Tag;
            so.isStatic = ser.isStatic;
            so.transform = ser.transform;

            foreach (var imp in ser.Imps)//unknown if this works
            {
                so.Imps.Add(imp);
            }
            //foreach (var type in ser.impTypes)
            //{
            //    ///shit....who ever loads a scene will need userClasses as a reference!!!!
            //    ///Therefore, the SceneEditLauncher will need to be responsible for loading a scene.
            //    ///The topmost layer
            //}
            foreach (var component in ser.Components)
            {
                if (component is SerializablePointLight)
                {
                    so.Components.Add(LoadPointLight(component as SerializablePointLight));
                }
                else if (component is SerializableSpotLight)
                {
                    so.Components.Add(LoadSpotLight(component as SerializableSpotLight));
                }
                else if (component is SerializableDirectionalLight)
                {
                    so.Components.Add(LoadDirectionalLight(component as SerializableDirectionalLight));
                }
                else
                {
                    so.Components.Add(component);
                }
            }


            if (ser.material.diffuseMap.FullPath.ToLower() != so.material.diffuseMap.FullPath.ToLower() && ser.material.diffuseMap.FullPath.ToLower() != "unknown")
            {
                Console.WriteLine("Texture discrepancy.");

                Console.WriteLine("**************$$$$$$$$$$$$$$$$$$$$$$$$$$$***********************ATTENTION: Loading texture from directory.");
                if (Control.HasTexture(ser.material.diffuseMap.FullPath))
                {
                    Texture texture = Texture.FindByFullPath(ser.material.diffuseMap.FullPath);
                    so.SetDiffuseMap(texture);
                }
                else
                {

                    Texture texture = SceneMaster.LoadTextureFromDirectoryPerScene(ser.material.diffuseMap.FullPath);
                    so.SetDiffuseMap(texture);
                }

            }
            //We can also manage complex child/parent relationships here
            //if (ser.material.diffuseMap.FullPath.ToLower() != "unknown" && ser.material.diffuseMap.FullPath.Length > 2)
            //{
            //    //Load the texture. Swap the Texture.
            //    Console.WriteLine("**************$$$$$$$$$$$$$$$$$$$$$$$$$$$***********************ATTENTION: Loading texture from directory.");
            //    Texture texture = LoadTextureFromDirectoryPerScene(ser.material.diffuseMap.FullPath);
            //    so.SetDiffuseMap(texture);
            //}

            so.SetMaterialTiling(ser.material.xTiling, ser.material.yTiling);
            so.SetMaterialOffset(ser.material.xOffset, ser.material.yOffset);
            so.SetMaterialAmbient(ser.material.ambient);
            so.SetMaterialDiffuse(ser.material.diffuse);
            so.SetMaterialSpecular(ser.material.specular);
            so.SetShininess(ser.material.shininess);
        }
  

        private static void LoadSceneObject(SerializableSceneObject ser)
        {

            string pth = ser.modelPath;
            if (pth.Length > 16)//It's probably a Model. Try to load it.
            {
                Console.WriteLine("Loading Model...");
                try
                {

                    SceneMaster.LoadFromDirectory(ser.modelPath, ser.guid, ser.ChildGuids);
                    //   Console.WriteLine("Child Guid count: " + ser.ChildGuids.Count);
                }
                catch
                {
                    //make an empty sceneObject with the model name
                    SceneMaster.CreateEmptySceneObject(ser.guid);
                }
            }
            else if(ser.ID > -1) //it's a child ignore it for now
            {
                //this will not work for loading empty scene objects.
                Console.WriteLine("M-T sceneObject");
                SceneMaster.CreateEmptySceneObject(ser.guid);
            }
        }
        private static PointLight LoadPointLight(SerializablePointLight ser)
        {
            PointLight pl = SceneMaster.CreatePointLight(ser.guid);
            pl.SetAmbient(ser.ambient);
            pl.SetDiffuse(ser.diffuse);
            pl.SetSpecular(ser.specular);
            pl.constant = ser.constant;
            pl.linear = ser.linear;
            pl.quadratic = ser.quadratic;
            pl.SetMaxDistance(ser.maxDistance);
            pl.dynamic = ser.dynamic;
            pl.SetEnabled(ser.enabled);
            return pl;
        }
        private static SpotLight LoadSpotLight(SerializableSpotLight ser)
        {
            SpotLight sp = SceneMaster.CreateSpotLight(ser.guid);
            sp.SetAmbient(ser.ambient);
            sp.SetDiffuse(ser.diffuse);
            sp.SetSpecular(ser.specular);
            sp.SetCutOff(ser.cutOff);
            sp.outerCutOff = ser.outerCutOff;
            sp.SetMaxDistance(ser.maxDistance);
            sp.cutOffRatio = ser.cutOffRatio;
            sp.SetEnabled(ser.enabled);
            return sp;
        }
        private static DirectionalLight LoadDirectionalLight(SerializableDirectionalLight ser)
        {
           DirectionalLight dl = SceneMaster.CreateDirectionalLight(ser.guid);
            dl.SetDirection(ser.direction);
            dl.SetAmbient(ser.ambient);
            dl.SetDiffuse(ser.diffuse);
            dl.SetSpecular(ser.specular);
            dl.SetEnabled(ser.enabled);
            return dl;
        }
     
        #endregion


        #region Saving A Scene To Disk
        public static void SaveSceneAs(string filename)
        {
            //  SceneFile scene = new SceneFile();
            // scene.AllSceneObjects = Control.AllSceneObjects;

            //We need a SceneObject analogue to save/Load

            SceneFile sceneFile = new SceneFile();
            // List<SerializableSceneObject> scene = new List<SerializableSceneObject>();
            foreach (var so in Control.AllSceneObjects)
            {
                ProcessSceneObject(so, sceneFile);
            }
       
            foreach (var box in Control.Skyboxes)
            {
                sceneFile.AllSkyboxes.Add(box);
            }
            string savePath = @"c:\data\" + filename + ".imp";

            // CLE.blockInput = true;
            if (File.Exists(savePath))
            {
                Console.WriteLine("Overwrite file: " + filename + "?");
                string input = Console.ReadLine();
                if (input == "y" || input == "yes")
                {
                    Console.WriteLine("Overwriting.");
                    File.Delete(savePath);
                }
                else
                {
                    Console.WriteLine("Cancelled save");

                    return;
                }
            }
            //CLE.blockInput = false;

            Type[] userTypes = AssemblyManager.UserTypes();
            //Impunity classes have cyclical relationships with their SceneObjects!

            XmlSerializer xs = new XmlSerializer(typeof(SceneFile), userTypes);
      
            TextWriter tw = new StreamWriter(@"c:\data\" + filename + ".imp");

            xs.Serialize(tw, sceneFile);
            Console.WriteLine("Saved file " + filename);
            tw.Close();

        }
        private static void ProcessSceneObject(SceneObject so, SceneFile scene)
        {
            scene.AllSceneObjects.Add(SerializeSceneObject(so));
        }
    
        private static SerializableSceneObject SerializeSceneObject(SceneObject so)
        {
            SerializableSceneObject sp = new SerializableSceneObject();


            sp.Name = so.Name;
            sp.Tag = so.Tag;
            sp.ID = so.ID;
            sp.MeshID = so.MeshID;
            sp.ShaderID = so.ShaderID;
            sp.ParentID = so.ParentID;
            sp.modelPath = so.modelPath;
            sp.guid = so.guid;

            sp.isChild = so.isChild;
            sp.isStatic = so.isStatic;
            sp.transform = so.transform;
            sp.ChildIDs = new int[so.Children.Count];
            sp.ChildGuids = new List<Guid>();
            for (int i = 0; i < so.Children.Count; i++)
            {
                sp.ChildIDs[i] = so.Children[i].ID;
                sp.ChildGuids.Add(so.Children[i].guid);
            }
    


            sp.Imps = so.Imps;

            //sp.Components = so.Components;
            foreach (var component in so.Components)
            {
                if (component is PointLight)
                {
                    sp.Components.Add(SerializePointLight(component as PointLight));
                }
                else if (component is SpotLight)
                {
                    sp.Components.Add(SerializeSpotLight(component as SpotLight));
                }
                else if (component is DirectionalLight)
                {
                    sp.Components.Add(SerializeDirectionalLight(component as DirectionalLight));
                }
                else // it's a user-created ImpunityClass
                {
                    sp.Components.Add(component);
                }
            }
            
            sp.material = so.material;
            sp.enabled = so.enabled;

            return sp;
        }

        public static SerializablePointLight SerializePointLight(PointLight light)
        {
            SerializablePointLight sl = new SerializablePointLight();
            sl.ID = light.ID;
            sl.guid = light.guid;
            sl.enabled = light.enabled;

            sl.ambient = light.ambient;
            sl.diffuse = light.diffuse;
            sl.specular = light.specular;
            sl.constant = light.constant;
            sl.linear = light.linear;
            sl.quadratic = light.quadratic;
            sl.maxDistance = light.maxDistance;
            sl.dynamic = light.dynamic;

            return sl;
        }
        public static SerializableSpotLight SerializeSpotLight(SpotLight light)
        {
            SerializableSpotLight sl = new SerializableSpotLight();
            sl.ID = light.ID;
            sl.guid = light.guid;
            sl.enabled = light.enabled;

            sl.ambient = light.ambient;
            sl.diffuse = light.diffuse;
            sl.specular = light.specular;
            sl.cutOff = light.cutOff;
            sl.outerCutOff = light.outerCutOff;
            sl.cutOffRatio = light.cutOffRatio;
            sl.maxDistance = light.maxDistance;
            return sl;
        }
        public static SerializableDirectionalLight SerializeDirectionalLight(DirectionalLight light)
        {
            SerializableDirectionalLight sl = new SerializableDirectionalLight();
            sl.ID = light.ID;
            sl.guid = light.guid;
            sl.enabled = light.enabled;

            sl.ambient = light.ambient;
            sl.diffuse = light.diffuse;
            sl.specular = light.specular;
            sl.direction = light.direction;
            sl.dynamic = light.dynamic;
            return sl;
        }



        #endregion
    }


    [Serializable]
    public class SceneFile
    {
        public List<SerializableSceneObject> AllSceneObjects = new List<SerializableSceneObject>();
        public List<SerializablePointLight> AllPointLights = new List<SerializablePointLight>();
        public List<SerializableDirectionalLight> AllDirectionalLights = new List<SerializableDirectionalLight>();
        public List<SerializableSpotLight> AllSpotLights = new List<SerializableSpotLight>();
        public List<Texture> AllTextures = new List<Texture>();
        public List<Skybox> AllSkyboxes = new List<Skybox>();
    }
}
