﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

//using ImpunityEngine.Persistence;
using ImpunityEngine.Interoperability;
using ImpunityEngine;
namespace ImpunityEngine.SceneManipulation
{
    //This could potentially be our save file... with a list of sceneObjects and lights...
    public static class SceneMaster
    {
        public static SceneObject SelectedSceneObject;


        public static void SetPointLightSpecular(Vector3 color)
        {
            if (SelectedSceneObject == null)
                return;
            try
            {
                Type type = typeof(PointLight);
                PointLight p = (PointLight)SelectedSceneObject.GetComponent(type);
                p.SetSpecular(color);
            }
            catch { return; }
        }
        public static void SetPointLightDiffuse(Vector3 color)
        {
            if (SelectedSceneObject == null)
                return;
            try
            {
                Type type = typeof(PointLight);
                PointLight p = (PointLight)SelectedSceneObject.GetComponent(type);
                p.SetDiffuse(color);
            }
            catch { return; }
        }
        public static void SetPointLightAmbient(Vector3 color)
        {
            if (SelectedSceneObject == null)
                return;
            try
            {
                Type type = typeof(PointLight);
                PointLight p = (PointLight)SelectedSceneObject.GetComponent(type);
                p.SetAmbient(color);
            }
            catch { return; }
        }

        public static void TestGetPointLightComponent(Type type)
        {
           // Type t = typeof(PointLight);
            PointLight p = (PointLight)SceneMaster.SelectedSceneObject.GetComponent(type);
        }
        public static SceneObject CreateEmptySceneObject()
        {
            Console.WriteLine("Creating empty SceneObject to which the point light shall be attached as component...");
            int id = Bridge.CreateEmptySceneObject();
            SceneObject so = new SceneObject(id);
            Control.AllSceneObjects.Add(so);
            return so;
        }
        public static SceneObject CreateEmptySceneObject(Guid guid)
        {
            Console.WriteLine("Creating empty SceneObject to which the point light shall be attached as component...");
            int id = Bridge.CreateEmptySceneObject();
            SceneObject so = new SceneObject(id);
            so.guid = guid;
            Control.AllSceneObjects.Add(so);
            return so;
        }
        public static void CreatePointLight()
        {
            int i = Bridge.CreatePointLight(0, 0, 0);
            PointLight plight = new PointLight(i, new Vector3(0));
            int id = Bridge.CreateEmptySceneObject();
            SceneObject so = new SceneObject(id);
            so.Components.Add(plight);
            so.Name = "Point Light";
            Control.AllSceneObjects.Add(so);
        }
        public static PointLight CreatePointLight(Guid guid)
        {
            int i = Bridge.CreatePointLight(0, 0, 0);
            PointLight plight = new PointLight(i, new Vector3(0));
            plight.guid = guid;
            return plight;
        }

        public static void CreateDirectionalLight()
        {


            //What we will do now is 
            //1. Create an empty SceneObject in the c++ world
            //2. Get its ID, and give it to a new sceneObject in c# world
            //3. Create a d light in c++ world, get its ID
            //4. Create a d light in c# world, give it the ID
            //5. Add this d light to the empty SceneObject's list of Components


            float x = 0;
            float y = 0;
            float z = 0;
            int id = Bridge.CreateDirectionalLight(x, y, z);
            Console.WriteLine("C#: Directional Light ID: " + id);
            DirectionalLight dlight = new DirectionalLight(id, new Vector3(x, y, z));
            Console.WriteLine("Added a new directional light.");
            //  SelectedSceneObject = dlight;
            //   Control.AllSceneObjects.Add(dlight);
            SceneObject so = CreateEmptySceneObject();
            so.Components.Add(dlight);
            so.Name = "Directional Light";

        }
        public static DirectionalLight CreateDirectionalLight(Guid guid)
        {
            float x = 0;
            float y = 0;
            float z = 0;
            int id = Bridge.CreateDirectionalLight(x, y, z);
   //         Console.WriteLine("C#: Directional Light ID: " + id);
            DirectionalLight dlight = new DirectionalLight(id, new Vector3(x, y, z));
         //   Console.WriteLine("Added a new directional light.");
            dlight.guid = guid;
            //SelectedSceneObject = dlight;
            //   Control.AllSceneObjects.Add(dlight);
            return dlight;
        }

        public static void CreateSpotLight()
        {
            float x, y, z, dx, dy, dz;
            x = 0; y = 0; z = 0; dx = 0; dy = 0; dz = 0;
            int id = Bridge.CreateSpotLight(x, y, z, dx, dy, dz);
          
            SpotLight spot = new SpotLight(id, new Vector3(x, y, z), new Vector3(dx, dy, dz));
            
            SceneObject so = CreateEmptySceneObject();
            so.Name = "SpotLight";
            so.Components.Add(spot);
        }
        public static SpotLight CreateSpotLight(Guid guid)
        {
            float x, y, z, dx, dy, dz;
            x = 0; y = 0; z = 0; dx = 0; dy = 0; dz = 0;
            int id = Bridge.CreateSpotLight(x, y, z, dx, dy, dz);
       //     Console.WriteLine("C#: Spot Light ID: " + id);
            SpotLight spot = new SpotLight(id, new Vector3(x, y, z), new Vector3(dx, dy, dz));
            //     Console.WriteLine("Added new spotlight!");
            spot.guid = guid;
            //  Control.AllSceneObjects.Add(spot);
            return spot;
        }

        //Should be call loadModelFromDirectory
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

        public static void CreateCubeMap(string imgDirectory)
        {
            string[] imageDirectories = Directory.GetFiles(imgDirectory);


            List<string> images = new List<string>();

            foreach (var item in imageDirectories)
            {
                images.Add(item);
            }


            int result = Bridge.CreateSkyBox(images[0], images[1], images[2], images[3], images[4], images[5]);

            if (result == -1)
                return;

            Skybox box = new Skybox(result, images);
            Control.Skyboxes.Add(box);
        }



       public static void DrawLabel(string labelText, float xposition, float yposition, float scale, float r, float g, float b) 
      {
            Bridge.DrawLabel(labelText, xposition, yposition, scale, r, g, b);
      }

        public static void CreateCubeMap()
        {
            // string[] imageDirectories = Directory.GetFiles(imgDirectory);

            //we need to put these images in a particular order
            //1. right
            //2. left 
            //3. top
            //4. bottom
            //5. front
            //6. back
            List<string> images = new List<string>();
            Console.WriteLine("Enter directory for right face.");
            string input = Console.ReadLine();
            images.Add(input);
            Console.WriteLine("Enter directory for left face.");
            input = Console.ReadLine();
            images.Add(input);
            Console.WriteLine("Enter directory for top face.");
            input = Console.ReadLine();
            images.Add(input);
            Console.WriteLine("Enter directory for bottom face.");
            input = Console.ReadLine();
            images.Add(input);
            Console.WriteLine("Enter directory for front face.");
            input = Console.ReadLine();
            images.Add(input);
            Console.WriteLine("Enter directory for back face.");
            input = Console.ReadLine();
            images.Add(input);

            string output = string.Empty;
            foreach (var item in images)
            {
                if (item.Length < 2)
                    return;
                output += item + ",";
            }
            // Bridge.CreateSkyBox(output);

          int result =  Bridge.CreateSkyBox(images[0], images[1], images[2], images[3], images[4], images[5]);

            if (result == -1)
                return;

            Skybox box = new Skybox(result, images);
            Control.Skyboxes.Add(box);
        }
        public static void CreateCubeMap(List<string> images)
        {

            int result = Bridge.CreateSkyBox(images[0], images[1], images[2], images[3], images[4], images[5]);

            if (result == -1)
                return;

            Skybox box = new Skybox(result, images);
            Control.Skyboxes.Add(box);
        }
        public static void LoadTextureFromDirectory(string path) {
            //get the name of the texture
            int nstart = path.LastIndexOf("\\") + 1;
            string tname = path.Substring(nstart);
            //get the directory of the texture
            int dend = nstart;
            string tdirectory = path.Substring(0, dend);
            Console.WriteLine("Texture directory: " + tdirectory);
            StringBuilder cmessage = new StringBuilder(10000);//256 chars at most
            int t = Bridge.LoadTextureFromDir(tname, tdirectory, cmessage, 256);
            string message = cmessage.ToString();
            Console.WriteLine("Texture creation result: " + message);
            if (message != "{already-loaded}")
            {
                NativeTranslator translator = new NativeTranslator();
                try
                {
                    Texture tex = translator.ParseTextureData(message);
                    tex.FullPath = path;
                    Control.AllTextures.Add(tex);
                }
                catch { }
            }
            else
            {
                Console.WriteLine($"{tname} has already been loaded.");
            }
        }
        public static Texture LoadTextureFromDirectoryPerScene(string path)
        {
            //get the name of the texture
            int nstart = path.LastIndexOf("\\") + 1;
            string tname = path.Substring(nstart);
            //get the directory of the texture
            int dend = nstart;
            string tdirectory = path.Substring(0, dend);
            Console.WriteLine("Texture directory: " + tdirectory);
            StringBuilder cmessage = new StringBuilder(10000);//256 chars at most
            int t = Bridge.LoadTextureFromDir(tname, tdirectory, cmessage, 256);
            string message = cmessage.ToString();
            Console.WriteLine("Texture creation result: " + message);

            Texture texture;
            if (message != "{already-loaded}")
            {
                NativeTranslator translator = new NativeTranslator();
                try
                {
                   texture = translator.ParseTextureData(message);
                    texture.FullPath = path;
                    Control.AllTextures.Add(texture);
                }
                catch { throw new Exception("Failed to load texture from directory."); }
            }
            else
            {
                Console.WriteLine($"{tname} has already been loaded.");
                //Find the texture in Control and return it
                try
                {
                    texture = Texture.FindByFullPath(path);
                    return texture;
                }
                catch { throw new NullReferenceException("Failed to load texture from pool."); }
            }



            return texture;
        }
        public static void SwapDiffuseMap(int textureID) {

            if (SelectedSceneObject == null)
                return;

            SelectedSceneObject.SetDiffuseMap(textureID);
         //   Texture tex = Texture.FindByID(textureID);
            //Again, this is a crutch for a method that should be in the sceneObject itself

         //   int result = Bridge.SwapDiffuseMap(SelectedSceneObject, tex);
        }
        public static void SetMaterialTiling(float x, float y) {
            if (SelectedSceneObject == null)
                return;

            //   Bridge.SetMaterialTiling(SelectedSceneObject, x, y);
            //This is a crutch. Call it from the sceneObject itself
            SelectedSceneObject.SetMaterialTiling(x, y);
        }
        public static void SetMaterialOffset(float x, float y)
        {
            if (SelectedSceneObject == null)
                return;

            //   Bridge.SetMaterialTiling(SelectedSceneObject, x, y);
            //This is a crutch. Call it from the sceneObject itself
            SelectedSceneObject.SetMaterialOffset(x, y);
        }
        public static void SetShininess(float shininess) {
            if (SelectedSceneObject == null)
                return;

            SelectedSceneObject.SetShininess(shininess);
        }
        public static void SetMaterialAmbient(float x, float y, float z) {
            if (SelectedSceneObject == null)
                return;

            SelectedSceneObject.SetMaterialAmbient(new Vector3(x, y, z));

        }
        public static void SetMaterialDiffuse(float x, float y, float z)
        {
            if (SelectedSceneObject == null)
                return;

            SelectedSceneObject.SetMaterialDiffuse(new Vector3(x, y, z));

        }
        public static void SetMaterialSpecular(float x, float y, float z)
        {
            if (SelectedSceneObject == null)
                return;

            SelectedSceneObject.SetMaterialSpecular(new Vector3(x, y, z));

        }

        public static void LoadFromDirectory(string path,Guid guid, List<Guid> kidguids)
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
                List<SceneObject> sceneObs = translator.ParseNativeData(message, path, guid, kidguids);
            }
            catch (Exception err)
            {
                Console.WriteLine("ERROR: " + err.ToString());
            }

            cmessage.Clear();
        }

        public static void LoadSceneFile(string filename)
        {

            if (!File.Exists(filename))
            {
                Console.WriteLine("No such file " + filename);
                return;
            }

            SceneFile scene;
            using (var sr = new StreamReader(filename))
            {
                XmlSerializer xs = new XmlSerializer(typeof(SceneFile));
                scene = (SceneFile)xs.Deserialize(sr);
                sr.Close();
            }

            //1st, we take a pass at loading everything from disk/blob
            Console.WriteLine("Scene Load Results: ");
            foreach (var item in scene.AllSceneObjects)
            {
                Console.WriteLine(item.Name);
                LoadSceneObject(item);
            }
            foreach (var item in scene.AllPointLights)
            {
             //   Console.WriteLine("Point Light: " + item.LightID);
                LoadPointLight(item);
            }
            foreach (var item in scene.AllSpotLights)
            {
            //    Console.WriteLine("Spot light: " + item.LightID);
                LoadSpotLight(item);
            }
            foreach (var item in scene.AllDirectionalLights)
            {
           //     Console.WriteLine("Directional light: " + item.LightID);
                LoadDirectionalLight(item);
            }
            //2nd, we go through every item and set the data to their correct values
            foreach (var item in scene.AllSceneObjects)
            {
                DetailSceneObject(item, scene.AllSceneObjects);
            }
            foreach (var item in scene.AllPointLights)
            {
              //DetailPointLight(item, scene.AllPointLights);
            }
            foreach (var item in scene.AllSpotLights)
            {
                //DetailSpotLight(item, scene.AllSpotLights);
            }

            foreach (var item in scene.AllDirectionalLights)
            {
                //DetailDirLight(item, scene.AllDirectionalLights);
            }
        }
        private static void DetailSceneObject(SerializableSceneObject ser, List<SerializableSceneObject> allSer)
        {
            if (ser.ID < 0)
                return;
            //Find the SceneObject by Guid
            SceneObject so;
            try { so = SceneObject.FindByGuid(ser.guid); } catch { Console.WriteLine("WARNING: Failed to find SceneObject " + ser.guid.ToString());return; }

            so.Name = ser.Name;
            so.Tag = ser.Tag;
            so.isStatic = ser.isStatic;
            so.transform = ser.transform;

            //foreach (var imp in ser.Imps)//unknown if this works
            //{
            //    so.Imps.Add(imp);
            //}
            //foreach (var type in ser.impTypes)
            //{
            //    ///shit....who ever loads a scene will need userClasses as a reference!!!!
            //    ///Therefore, the SceneEditLauncher will need to be responsible for loading a scene.
            //    ///The topmost layer
            //}
            

            if (ser.material.diffuseMap.FullPath.ToLower() != so.material.diffuseMap.FullPath.ToLower() && ser.material.diffuseMap.FullPath.ToLower() != "unknown") {
                Console.WriteLine("Texture discrepancy.");
               
                Console.WriteLine("**************$$$$$$$$$$$$$$$$$$$$$$$$$$$***********************ATTENTION: Loading texture from directory.");
                if (Control.HasTexture(ser.material.diffuseMap.FullPath))
                {
                    Texture texture = Texture.FindByFullPath(ser.material.diffuseMap.FullPath);
                    so.SetDiffuseMap(texture);
                }
                else {

                    Texture texture = LoadTextureFromDirectoryPerScene(ser.material.diffuseMap.FullPath);
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
        //    //foreach (var imp in ser.Imps)//unknown if this works
        //    //{
        //    //    so.Imps.Add(imp);
        //    //}
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
        //    //foreach (var imp in ser.Imps)//unknown if this works
        //    //{
        //    //    so.Imps.Add(imp);
        //    //}
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
        //    //foreach (var imp in ser.Imps)//unknown if this works
        //    //{
        //    //    so.Imps.Add(imp);
        //    //}
        //    //we will have to set custom material/texture/shader details 
        //    //one property at a time
        //    //so don't worry about that, as we don't even have methods
        //    //to set material values yet

        //    //We can also manage complex child/parent relationships here

        //    //point light variables
        //   // so.SetPosition(ser.position);
        //    so.SetAmbient(ser.ambient);
        //    so.SetDiffuse(ser.diffuse);
        //    so.SetSpecular(ser.specular);

        //    //so.cutOffRatio = ser.cutOffRatio;
        //    so.SetDirection(ser.direction);
        //  //  so.SetCutOff(ser.cutOff);

        //  //  so.SetMaxDistance(ser.maxDistance);
        //    so.SetEnabled(ser.enabled);

        //    //you actually need to call the methods to set this...
        //}

        
        private static void LoadSceneObject(SerializableSceneObject ser) {

            string pth = ser.modelPath;
            if (pth.Length > 3 )//It's a Model. Try to load it.
            {
                Console.WriteLine("Loading Model...");
                try {
                    
                    LoadFromDirectory(ser.modelPath,ser.guid, ser.ChildGuids);
                 //   Console.WriteLine("Child Guid count: " + ser.ChildGuids.Count);
                } catch { }
            }
            else //it's a child ignore it for now
            {
                Console.WriteLine("Model child");
            }
        }
        private static void LoadPointLight(SerializablePointLight ser)
        {
            CreatePointLight(ser.guid);
        }
        private static void LoadSpotLight(SerializableSpotLight ser) {
            CreateSpotLight(ser.guid);
        }
        private static void LoadDirectionalLight(SerializableDirectionalLight ser) {
            CreateDirectionalLight(ser.guid);
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
            //also add all the textures and material details
            foreach (var texture in Control.AllTextures)
            {
                //ProcessSceneTextures(texture, sceneFile);
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
        private static void ProcessTexture(Texture tex, SceneFile sceneFile)
        {

        }

        private static void ProcessSceneObject(SceneObject so, SceneFile scene)
        {
            if (so is PointLight)
            {
         //       scene.AllPointLights.Add(SerializePointLight(so));
            }
            else if (so is DirectionalLight)
            {
            //    scene.AllDirectionalLights.Add(SerializeDirectionalLight(so));
            }
            else if (so is SpotLight)
            {
             //   scene.AllSpotLights.Add(SerializeSpotLight(so));
            }
            else
            {
                scene.AllSceneObjects.Add(SerializeSceneObject(so));
            }
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


       // private static SerializableDirectionalLight SerializeDirectionalLight(SceneObject so)
       // {
       //     SerializableDirectionalLight sp = new SerializableDirectionalLight();
       //     DirectionalLight pl = (DirectionalLight)so;
       //     sp.LightID = pl.LightID;
       //  //   sp.position = pl.position;
       //     sp.ambient = pl.ambient;
       //     sp.diffuse = pl.diffuse;
       //     sp.guid = so.guid;
       //     sp.specular = pl.specular;
       ////     sp.constant = pl.constant;
       //     //sp.linear = pl.linear;
       //     //sp.quadratic = pl.quadratic;
       //     //sp.maxDistance = pl.maxDistance;
       //     sp.enabled = pl.enabled;
       //     sp.modelPath = so.modelPath;
       //     sp.enabled = pl.enabled;
       //     sp.direction = pl.direction;

       //     sp.Name = pl.Name;
       //     sp.Tag = pl.Tag;
       //     sp.ID = pl.ID;
       //     sp.MeshID = pl.MeshID;
       //     sp.ShaderID = pl.ShaderID;
       //     sp.ParentID = pl.ParentID;

       //     sp.isChild = pl.isChild;
       //     sp.isStatic = pl.isStatic;
       //     sp.transform = pl.transform;
       //     sp.ChildIDs = new int[pl.Children.Count];
       //     sp.ChildGuids = new List<Guid>();
       //     for (int i = 0; i < so.Children.Count; i++)
       //     {
       //         sp.ChildIDs[i] = so.Children[i].ID;
       //         sp.ChildGuids.Add(so.Children[i].guid);
       //     }
       //     //foreach (var imp in so.Imps)
       //     //{
       //     //    string type = imp.GetType().ToString();
       //     //    int start = type.IndexOf(".") + 1;
       //     //    string className = type.Substring(start);
       //     //    sp.impTypes.Add(className);
       //     //}

       //     sp.material = pl.material;

       //     return sp;
       // }


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



            sp.material = so.material;
            sp.enabled = so.enabled;

            return sp;
        }


        public static void SelectSceneObject(int index) {
            int result = Bridge.SelectSceneObject(index);
            if (result == 1)
                return;

            SelectedSceneObject = SceneObject.FindByID(index);
        }

        public static void GrabObject(float dist, string axis)
        {
            if (SelectedSceneObject == null)
                return;

            //Regular SceneObject
            if (axis == "x")
                SelectedSceneObject.transform.position.x += dist;
            else if (axis == "y")
                SelectedSceneObject.transform.position.y += dist;
            else if (axis == "z")
                SelectedSceneObject.transform.position.z += dist;

        }

        public static void RotateObject(float degree, string axis)
        {
            
            //add degree to the axis of the selected SceneObject
            //Just like blender!
            if (SelectedSceneObject == null)
                return;

            Console.WriteLine("Rotating selected by " + degree + " degrees on " + axis + " axis");

            if (SelectedSceneObject is PointLight)
            {
                Console.WriteLine("Point lights don't have a rotation...");
            }
            else if (SelectedSceneObject is SpotLight)
            {
                //SpotLight dl = (SpotLight)SelectedSceneObject;
                //if (axis == "x")
                //    dl.SetDirection(new Vector3(dl.direction.x + degree,
                //        dl.direction.y, dl.direction.z));
                //else if (axis == "y")
                //    dl.SetDirection(new Vector3(dl.direction.x,
                //       dl.direction.y + degree, dl.direction.z));
                //else if (axis == "z")
                //    dl.SetDirection(new Vector3(dl.direction.x,
                //        dl.direction.y, dl.direction.z + degree));
            }
            else if (SelectedSceneObject is DirectionalLight)
            {
              //  DirectionalLight dl = (DirectionalLight)SelectedSceneObject;
                //if (axis == "x")
                //    dl.SetDirection(new Vector3(dl.direction.x + degree,
                //        dl.direction.y, dl.direction.z));
                //else if (axis == "y")
                //    dl.SetDirection(new Vector3(dl.direction.x,
                //       dl.direction.y + degree, dl.direction.z));
                //else if (axis == "z")
                //    dl.SetDirection(new Vector3(dl.direction.x,
                //        dl.direction.y, dl.direction.z + degree));
            }
            else
            {
                //Regular SceneObject
                if (axis == "x")
                    SelectedSceneObject.transform.rotation.x += degree;
                else if (axis == "y")
                    SelectedSceneObject.transform.rotation.y += degree;
                else if (axis == "z")
                    SelectedSceneObject.transform.rotation.z += degree;

               // SelectedSceneObject.transform.SetTransform(SelectedSceneObject.ID);
            }
        }

        public static void RotateObjectLocal(float degree, string axis)
        {

            //add degree to the axis of the selected SceneObject
            //Just like blender!
            if (SelectedSceneObject == null)
                return;

            Console.WriteLine("Rotating selected by " + degree + " degrees on " + axis + " axis");

            if (SelectedSceneObject is PointLight)
            {
                Console.WriteLine("Point lights don't have a rotation...");
            }
            else if (SelectedSceneObject is SpotLight)
            {
                //SpotLight dl = (SpotLight)SelectedSceneObject;
                //if (axis == "x")
                //    dl.SetDirection(new Vector3(dl.direction.x + degree,
                //        dl.direction.y, dl.direction.z));
                //else if (axis == "y")
                //    dl.SetDirection(new Vector3(dl.direction.x,
                //       dl.direction.y + degree, dl.direction.z));
                //else if (axis == "z")
                //    dl.SetDirection(new Vector3(dl.direction.x,
                //        dl.direction.y, dl.direction.z + degree));
            }
            else if (SelectedSceneObject is DirectionalLight)
            {
                //DirectionalLight dl = (DirectionalLight)SelectedSceneObject;
                //if (axis == "x")
                //    dl.SetDirection(new Vector3(dl.direction.x + degree,
                //        dl.direction.y, dl.direction.z));
                //else if (axis == "y")
                //    dl.SetDirection(new Vector3(dl.direction.x,
                //       dl.direction.y + degree, dl.direction.z));
                //else if (axis == "z")
                //    dl.SetDirection(new Vector3(dl.direction.x,
                //        dl.direction.y, dl.direction.z + degree));
            }
            else
            {
                //Regular SceneObject
                if (axis == "x")
                    SelectedSceneObject.transform.localRotation.x += degree;
                else if (axis == "y")
                    SelectedSceneObject.transform.localRotation.y += degree;
                else if (axis == "z")
                    SelectedSceneObject.transform.localRotation.z += degree;

                // SelectedSceneObject.transform.SetTransform(SelectedSceneObject.ID);
            }
        }

        public static void ScaleObject(float factor, string axis)
        {
            if (SelectedSceneObject == null)
                return;

            if (SelectedSceneObject is PointLight)
            {
                return;
            }
            else if (SelectedSceneObject is SpotLight)
            {
                return;
            }
            else if (SelectedSceneObject is DirectionalLight)
            {
                return;
            }
            else
            {
                //Regular SceneObject
                if (axis == "x")
                    SelectedSceneObject.transform.scale.x *= factor;
                else if (axis == "y")
                    SelectedSceneObject.transform.scale.y *= factor;
                else if (axis == "z")
                    SelectedSceneObject.transform.scale.z *= factor;

                //   SelectedSceneObject.transform.Stretch(factor);

              //  SelectedSceneObject.transform.SetTransform(SelectedSceneObject.ID);
            }

        }
        public static void ScaleObjectLocal(float factor, string axis)
        {
            if (SelectedSceneObject == null)
                return;

            if (SelectedSceneObject is PointLight)
            {
                return;
            }
            else if (SelectedSceneObject is SpotLight)
            {
                return;
            }
            else if (SelectedSceneObject is DirectionalLight)
            {
                return;
            }
            else
            {
                //Regular SceneObject
                if (axis == "x")
                    SelectedSceneObject.transform.localScale.x *= factor;
                else if (axis == "y")
                    SelectedSceneObject.transform.localScale.y *= factor;
                else if (axis == "z")
                    SelectedSceneObject.transform.localScale.z *= factor;

               //SelectedSceneObject.transform.localScale.x *= factor;

            //    SelectedSceneObject.transform.SetTransform(SelectedSceneObject.ID);
            }

        }
        public static void ScaleEntireObject(float factor)
        {
            if (SelectedSceneObject == null)
                return;

            if (SelectedSceneObject is PointLight)
            {
                return;
            }
            else if (SelectedSceneObject is SpotLight)
            {
                return;
            }
            else if (SelectedSceneObject is DirectionalLight)
            {
                return;
            }
            else
            {

                SelectedSceneObject.transform.Stretch(factor);

             //   SelectedSceneObject.transform.SetTransform(SelectedSceneObject.ID);
            }

        }

    }




    [Serializable]
    public class SceneFile
    {
        public List<SerializableSceneObject> AllSceneObjects = new List<SerializableSceneObject>();
        public List<SerializablePointLight> AllPointLights = new List<SerializablePointLight>();
        public List<SerializableDirectionalLight> AllDirectionalLights = new List<SerializableDirectionalLight>();
        public List<SerializableSpotLight> AllSpotLights = new List<SerializableSpotLight>();
        public List<Texture> AllTextures = new List<Texture>();
    }

}
