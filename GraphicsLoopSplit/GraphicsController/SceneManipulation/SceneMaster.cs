using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

using ImpunityEngine.Persistence;
using ImpunityEngine.Interoperability;
namespace ImpunityEngine.SceneManipulation
{
    //This could potentially be our save file... with a list of sceneObjects and lights...
    public static class SceneMaster
    {
        public static SceneObject SelectedSceneObject;

        public static void CreatePointLight()
        {
            int i = Bridge.CreatePointLight(0, 0, 0);
            Console.WriteLine("C#: Point light ID: " + i);
            PointLight plight = new PointLight(i, new Vector3(0));
            Console.WriteLine("Wow!");
            SelectedSceneObject = plight;
            Control.AllSceneObjects.Add(plight);
        }

        public static void CreateDirectionalLight()
        {
            float x = 0;
            float y = 0;
            float z = 0;
            int id = Bridge.CreateDirectionalLight(x, y, z);
            Console.WriteLine("C#: Directional Light ID: " + id);
            DirectionalLight dlight = new DirectionalLight(id, new Vector3(x, y, z));
            Console.WriteLine("Added a new directional light.");
            SelectedSceneObject = dlight;
            Control.AllSceneObjects.Add(dlight);
        }

        public static void CreateSpotLight()
        {
            float x, y, z, dx, dy, dz;
            x = 0; y = 0; z = 0;dx = 0;dy = 0;dz = 0;
            int id = Bridge.CreateSpotLight(x, y, z, dx, dy, dz);
            Console.WriteLine("C#: Spot Light ID: " + id);
            SpotLight spot = new SpotLight(id, new Vector3(x, y, z), new Vector3(dx, dy, dz));
            Console.WriteLine("Added new spotlight!");
            Control.AllSceneObjects.Add(spot);

            int count = 0;
            foreach (var item in Control.AllSceneObjects)
            {
                if (item is SpotLight)
                    count += 1;
            }

            Console.WriteLine("Spotlight count: " + count);
        }

        public static void LoadFromDirectory(string path)
        {
            //  var path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            //int start = path.IndexOf("\\") + 1;
            //path = path.Substring(start, path.Length - start);

            //path += "\\Models\\tree02\\tree.obj";

            //   Console.WriteLine("Path: " + path);
            

            StringBuilder cmessage = new StringBuilder(10000);//256 chars at most

            int i = Bridge.LoadModelFromDir(path, path.Length, cmessage, 10000);


            string message = cmessage.ToString();
            Console.WriteLine("Result: " + message);

            //Parse the string and create the corresponding sceneObjects
            try
            {
                NativeTranslator translator = new NativeTranslator();
                List<SceneObject> sceneObs = translator.ParseNativeData(message, path);
                
            }
            catch (Exception err)
            {
                Console.WriteLine("ERROR: " + err.ToString());
            }
            
            cmessage.Clear();
        }


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

            XmlSerializer xs = new XmlSerializer(typeof(SceneFile));
            TextWriter tw = new StreamWriter(@"c:\data\" + filename + ".imp");

            xs.Serialize(tw, sceneFile);
            Console.WriteLine("Saved file " + filename);
            tw.Dispose();

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
            if (so is PointLight)
            {
                scene.AllPointLights.Add(SerializePointLight(so));
            }
            else if (so is DirectionalLight)
            {
                scene.AllDirectionalLights.Add(SerializeDirectionalLight(so));
            }
            else if (so is SpotLight)
            {
                scene.AllSpotLights.Add(SerializeSpotLight(so));
            }
            else
            {
                scene.AllSceneObjects.Add(SerializeSceneObject(so));
            }
        }

        private static SerializablePointLight SerializePointLight(SceneObject so)
        {
            SerializablePointLight sp = new SerializablePointLight();
            PointLight pl = (PointLight)so;
            sp.LightID = pl.LightID;
            sp.position = pl.position;
            sp.ambient = pl.ambient;
            sp.diffuse = pl.diffuse;
            sp.specular = pl.specular;
            sp.constant = pl.constant;
            sp.linear = pl.linear;
            sp.quadratic = pl.quadratic;
            sp.maxDistance = pl.maxDistance;
            sp.enabled = pl.enabled;

            sp.modelPath = so.modelPath;
            sp.Name = pl.Name;
            sp.Tag = pl.Tag;
            sp.ID = pl.ID;
            sp.MeshID = pl.MeshID;
            sp.ShaderID = pl.ShaderID;
            sp.ParentID = pl.ParentID;

            sp.isChild = pl.isChild;
            sp.isStatic = pl.isStatic;
            sp.transform = pl.transform;
            sp.ChildIDs = new int[pl.Children.Count];
            for (int i = 0; i < pl.Children.Count; i++)
            {
                sp.ChildIDs[i] = pl.Children[i].ID;
            }
            sp.Imps = pl.Imps;
            sp.material = pl.material;

            return sp;
        }
        private static SerializableDirectionalLight SerializeDirectionalLight(SceneObject so)
        {
            SerializableDirectionalLight sp = new SerializableDirectionalLight();
            DirectionalLight pl = (DirectionalLight)so;
            sp.LightID = pl.LightID;
         //   sp.position = pl.position;
            sp.ambient = pl.ambient;
            sp.diffuse = pl.diffuse;
            sp.specular = pl.specular;
       //     sp.constant = pl.constant;
            //sp.linear = pl.linear;
            //sp.quadratic = pl.quadratic;
            //sp.maxDistance = pl.maxDistance;
            sp.enabled = pl.enabled;
            sp.modelPath = so.modelPath;

            sp.Name = pl.Name;
            sp.Tag = pl.Tag;
            sp.ID = pl.ID;
            sp.MeshID = pl.MeshID;
            sp.ShaderID = pl.ShaderID;
            sp.ParentID = pl.ParentID;

            sp.isChild = pl.isChild;
            sp.isStatic = pl.isStatic;
            sp.transform = pl.transform;
            sp.ChildIDs = new int[pl.Children.Count];
            for (int i = 0; i < pl.Children.Count; i++)
            {
                sp.ChildIDs[i] = pl.Children[i].ID;
            }
            sp.Imps = pl.Imps;
            sp.material = pl.material;

            return sp;
        }
        private static SerializableSpotLight SerializeSpotLight(SceneObject so)
        {
            SerializableSpotLight sp = new SerializableSpotLight();
            SpotLight pl = (SpotLight)so;
            sp.LightID = pl.LightID;
            sp.position = pl.position;
            sp.ambient = pl.ambient;
            sp.diffuse = pl.diffuse;
            sp.specular = pl.specular;
            //sp.constant = pl.constant;
            //sp.linear = pl.linear;
            //sp.quadratic = pl.quadratic;
            sp.maxDistance = pl.maxDistance;
            sp.enabled = pl.enabled;

            sp.cutOff = pl.cutOff;
            sp.outerCutOff = pl.outerCutOff;
            sp.cutOffRatio = pl.cutOffRatio;
            sp.modelPath = so.modelPath;

            sp.Name = pl.Name;
            sp.Tag = pl.Tag;
            sp.ID = pl.ID;
            sp.MeshID = pl.MeshID;
            sp.ShaderID = pl.ShaderID;
            sp.ParentID = pl.ParentID;

            sp.isChild = pl.isChild;
            sp.isStatic = pl.isStatic;
            sp.transform = pl.transform;
            sp.ChildIDs = new int[pl.Children.Count];
            for (int i = 0; i < pl.Children.Count; i++)
            {
                sp.ChildIDs[i] = pl.Children[i].ID;
            }
            sp.Imps = pl.Imps;
            sp.material = pl.material;

            return sp;
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

            sp.isChild = so.isChild;
            sp.isStatic = so.isStatic;
            sp.transform = so.transform;
            sp.ChildIDs = new int[so.Children.Count];
            for (int i = 0; i < so.Children.Count; i++)
            {
                sp.ChildIDs[i] = so.Children[i].ID;
            }
            sp.Imps = so.Imps;
            sp.material = so.material;

            return sp;
        }

    }




    [Serializable]
    public class SceneFile
    {
        public List<SerializableSceneObject> AllSceneObjects = new List<SerializableSceneObject>();
        public List<SerializablePointLight> AllPointLights = new List<SerializablePointLight>();
        public List<SerializableDirectionalLight> AllDirectionalLights = new List<SerializableDirectionalLight>();
        public List<SerializableSpotLight> AllSpotLights = new List<SerializableSpotLight>();
       // public List<Texture> AllTextures = new List<Texture>();
        //Thus far, this is all the information we need to load a new scene
    }

}
