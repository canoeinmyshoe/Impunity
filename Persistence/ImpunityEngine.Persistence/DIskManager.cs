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
        //private static void DetailPointLight(SerializablePointLight ser, List<SerializablePointLight> allSer)
        //{

        //    //Find the PointLight by Guid
        //    PointLight so;
        //    try { so = PointLight.FindLightByGuid(ser.guid); } catch { Console.WriteLine("WARNING: Failed to find PointLight " + ser.guid.ToString()); return; }

        //    so.Name = ser.Name;
        //    so.Tag = ser.Tag;
        //    so.isStatic = ser.isStatic;
        //    so.transform = ser.transform;
        //    foreach (var imp in ser.Imps)//unknown if this works
        //    {
        //        so.Imps.Add(imp);
        //    }
        //    //we will have to set custom material/texture/shader details 
        //    //one property at a time
        //    //so don't worry about that, as we don't even have methods
        //    //to set material values yet

        //    //We can also manage complex child/parent relationships here

        //    //point light variables
        //    so.SetPosition(ser.position);
        //    so.SetAmbient(ser.ambient);
        //    so.SetDiffuse(ser.diffuse);
        //    so.SetSpecular(ser.specular);
        //    so.constant = ser.constant;
        //    so.linear = ser.linear;
        //    so.quadratic = ser.quadratic;
        //    so.SetMaxDistance(ser.maxDistance);
        //    so.SetEnabled(ser.enabled);

        //    //you actually need to call the methods to set this...
        //}


        //private static void DetailSpotLight(SerializableSpotLight ser, List<SerializableSpotLight> allSer)
        //{

        //    //Find the PointLight by Guid
        //    SpotLight so;
        //    try { so = SpotLight.FindLightByGuid(ser.guid); } catch { Console.WriteLine("WARNING: Failed to find spot light " + ser.guid.ToString()); return; }

        //    so.Name = ser.Name;
        //    so.Tag = ser.Tag;
        //    so.isStatic = ser.isStatic;
        //    so.transform = ser.transform;
        //    foreach (var imp in ser.Imps)//unknown if this works
        //    {
        //        so.Imps.Add(imp);
        //    }
        //    //we will have to set custom material/texture/shader details 
        //    //one property at a time
        //    //so don't worry about that, as we don't even have methods
        //    //to set material values yet

        //    //We can also manage complex child/parent relationships here

        //    //point light variables
        //    so.SetPosition(ser.position);
        //    so.SetAmbient(ser.ambient);
        //    so.SetDiffuse(ser.diffuse);
        //    so.SetSpecular(ser.specular);

        //    so.cutOffRatio = ser.cutOffRatio;
        //    so.SetDirection(ser.direction);
        //    so.SetCutOff(ser.cutOff);

        //    so.SetMaxDistance(ser.maxDistance);
        //    so.SetEnabled(ser.enabled);

        //    //you actually need to call the methods to set this...
        //}


        //private static void DetailDirLight(SerializableDirectionalLight ser, List<SerializableDirectionalLight> allSer)
        //{

        //    //Find the PointLight by Guid
        //    DirectionalLight so;
        //    try { so = DirectionalLight.FindLightByGuid(ser.guid); } catch { Console.WriteLine("WARNING: Failed to find spot light " + ser.guid.ToString()); return; }

        //    so.Name = ser.Name;
        //    so.Tag = ser.Tag;
        //    so.isStatic = ser.isStatic;
        //    so.transform = ser.transform;
        //    foreach (var imp in ser.Imps)//unknown if this works
        //    {
        //        so.Imps.Add(imp);
        //    }
        //    //we will have to set custom material/texture/shader details 
        //    //one property at a time
        //    //so don't worry about that, as we don't even have methods
        //    //to set material values yet

        //    //We can also manage complex child/parent relationships here

        //    //point light variables
        //    // so.SetPosition(ser.position);
        //    so.SetAmbient(ser.ambient);
        //    so.SetDiffuse(ser.diffuse);
        //    so.SetSpecular(ser.specular);

        //    //so.cutOffRatio = ser.cutOffRatio;
        //    so.SetDirection(ser.direction);
        //    //  so.SetCutOff(ser.cutOff);

        //    //  so.SetMaxDistance(ser.maxDistance);
        //    so.SetEnabled(ser.enabled);

        //    //you actually need to call the methods to set this...
        //}

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
            return pl;
        }
        private static void LoadSpotLight(SerializableSpotLight ser)
        {
            SceneMaster.CreateSpotLight(ser.guid);
        }
        private static void LoadDirectionalLight(SerializableDirectionalLight ser)
        {
            SceneMaster.CreateDirectionalLight(ser.guid);
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

            //FileStream writeStream;
            //try
            //{
            //    writeStream = new FileStream("c:\\csharp.net-informations.dat", FileMode.Create);
            //    BinaryWriter writeBinary = new BinaryWriter(writeStream);

            //    //Divide all SceneObjects... by #
            //    //Divid all properties... by @

            //    writeBinary.Write("====Impunity 1.0 Save File====#");
            //    foreach (SceneObject so in Control.AllSceneObjects) {
            //        //Write all the prop names, all their stuff, then a delimiting * character
            //        writeBinary.Write($"Name:{so.Name}*Tag:{so.Tag}*ID:{so.ID}" +
            //            $"*MeshID:{so.MeshID}*ShaderID:{so.ShaderID}*ParentID:{so.ParentID}" +
            //            $"*isChild:{so.isChild.ToString()}*isStatic:{so.isStatic.ToString()}" +
            //            $"*transform.position:{so.transform.position}");
            //    }

            //    writeBinary.Close();
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("ERROR: " + ex.Message);
            //}


        }
        private static void ProcessSceneObject(SceneObject so, SceneFile scene)
        {
            scene.AllSceneObjects.Add(SerializeSceneObject(so));
        }
        //private static SerializablePointLight SerializePointLight(SceneObject so)
        //{
        //    SerializablePointLight sp = new SerializablePointLight();
        //    PointLight pl = (PointLight)so;
        //    sp.LightID = pl.LightID;
        //    sp.position = pl.position;
        //    sp.ambient = pl.ambient;
        //    sp.guid = so.guid;
        //    sp.diffuse = pl.diffuse;
        //    sp.specular = pl.specular;
        //    sp.constant = pl.constant;
        //    sp.linear = pl.linear;
        //    sp.quadratic = pl.quadratic;
        //    sp.maxDistance = pl.maxDistance;
        //    sp.enabled = pl.enabled;

        //    sp.modelPath = so.modelPath;
        //    sp.Name = pl.Name;
        //    sp.Tag = pl.Tag;
        //    sp.ID = pl.ID;
        //    sp.MeshID = pl.MeshID;
        //    sp.ShaderID = pl.ShaderID;
        //    sp.ParentID = pl.ParentID;

        //    sp.isChild = pl.isChild;
        //    sp.isStatic = pl.isStatic;
        //    sp.transform = pl.transform;
        //    sp.ChildIDs = new int[pl.Children.Count];
        //    sp.ChildGuids = new List<Guid>();
        //    for (int i = 0; i < so.Children.Count; i++)
        //    {
        //        sp.ChildIDs[i] = so.Children[i].ID;
        //        sp.ChildGuids.Add(so.Children[i].guid);
        //    }
        //    sp.Imps = so.Imps;
        //    //foreach (var imp in so.Imps)
        //    //{
        //    //    string type = imp.GetType().ToString();
        //    //    int start = type.IndexOf(".") + 1;
        //    //    string className = type.Substring(start);
        //    //    sp.impTypes.Add(className);
        //    //}

        //    sp.material = pl.material;

        //    return sp;
        //}

        //private static SerializableDirectionalLight SerializeDirectionalLight(SceneObject so)
        //{
        //    SerializableDirectionalLight sp = new SerializableDirectionalLight();
        //    DirectionalLight pl = (DirectionalLight)so;
        //    sp.LightID = pl.LightID;
        //    //   sp.position = pl.position;
        //    sp.ambient = pl.ambient;
        //    sp.diffuse = pl.diffuse;
        //    sp.guid = so.guid;
        //    sp.specular = pl.specular;
        //    //     sp.constant = pl.constant;
        //    //sp.linear = pl.linear;
        //    //sp.quadratic = pl.quadratic;
        //    //sp.maxDistance = pl.maxDistance;
        //    sp.enabled = pl.enabled;
        //    sp.modelPath = so.modelPath;
        //    sp.enabled = pl.enabled;
        //    sp.direction = pl.direction;

        //    sp.Name = pl.Name;
        //    sp.Tag = pl.Tag;
        //    sp.ID = pl.ID;
        //    sp.MeshID = pl.MeshID;
        //    sp.ShaderID = pl.ShaderID;
        //    sp.ParentID = pl.ParentID;

        //    sp.isChild = pl.isChild;
        //    sp.isStatic = pl.isStatic;
        //    sp.transform = pl.transform;
        //    sp.ChildIDs = new int[pl.Children.Count];
        //    sp.ChildGuids = new List<Guid>();
        //    for (int i = 0; i < so.Children.Count; i++)
        //    {
        //        sp.ChildIDs[i] = so.Children[i].ID;
        //        sp.ChildGuids.Add(so.Children[i].guid);
        //    }
        //    //foreach (var imp in so.Imps)
        //    //{
        //    //    string type = imp.GetType().ToString();
        //    //    int start = type.IndexOf(".") + 1;
        //    //    string className = type.Substring(start);
        //    //    sp.impTypes.Add(className);
        //    //}
        //    sp.Imps = so.Imps;

        //    sp.material = pl.material;

        //    return sp;
        //}


        //private static SerializableSpotLight SerializeSpotLight(SceneObject so)
        //{
        //    SerializableSpotLight sp = new SerializableSpotLight();
        //    SpotLight pl = (SpotLight)so;
        //    sp.LightID = pl.LightID;
        //    sp.position = pl.position;
        //    sp.ambient = pl.ambient;
        //    sp.diffuse = pl.diffuse;
        //    sp.guid = so.guid;
        //    sp.specular = pl.specular;
        //    //sp.constant = pl.constant;
        //    //sp.linear = pl.linear;
        //    //sp.quadratic = pl.quadratic;
        //    sp.maxDistance = pl.maxDistance;
        //    sp.enabled = pl.enabled;

        //    sp.cutOff = pl.cutOff;
        //    sp.outerCutOff = pl.outerCutOff;
        //    sp.cutOffRatio = pl.cutOffRatio;
        //    sp.modelPath = so.modelPath;
        //    sp.direction = pl.direction;

        //    sp.Name = pl.Name;
        //    sp.Tag = pl.Tag;
        //    sp.ID = pl.ID;
        //    sp.MeshID = pl.MeshID;
        //    sp.ShaderID = pl.ShaderID;
        //    sp.ParentID = pl.ParentID;
        //    sp.enabled = pl.enabled;

        //    sp.isChild = pl.isChild;
        //    sp.isStatic = pl.isStatic;
        //    sp.transform = pl.transform;
        //    sp.ChildIDs = new int[pl.Children.Count];
        //    sp.ChildGuids = new List<Guid>();
        //    for (int i = 0; i < so.Children.Count; i++)
        //    {
        //        sp.ChildIDs[i] = so.Children[i].ID;
        //        sp.ChildGuids.Add(so.Children[i].guid);
        //    }
        //    //foreach (var imp in so.Imps)
        //    //{
        //    //    string type = imp.GetType().ToString();
        //    //    int start = type.IndexOf(".") + 1;
        //    //    string className = type.Substring(start);
        //    //    sp.impTypes.Add(className);
        //    //}
        //    sp.Imps = so.Imps;

        //    sp.material = pl.material;

        //    return sp;
        //}

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
            //   sp.Imps = so.Imps;
            //It's not quite as simple as that. There's no way to know what the user will call the classes
            //Therefore, we save a list of strings which are the type!
            //And load from the strings afterwards.

            //don't do this. Ask some DTO for the string of the XML
            //actually, just have the topmost layer do the saving/loading
            //foreach (var imp in so.Imps)
            //{
            //    string type = imp.GetType().ToString();
            //    int start = type.IndexOf(".") + 1;
            //    string className = type.Substring(start);
            //    sp.impTypes.Add(className);
            //}
            sp.Imps = so.Imps;

            //sp.Components = so.Components;
            foreach (var component in so.Components)
            {
                if (component is PointLight)
                {
                    sp.Components.Add(SerializePointLight(component as PointLight));
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
