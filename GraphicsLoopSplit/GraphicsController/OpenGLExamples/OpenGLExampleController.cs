using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;
using System.Threading;

using ImpunityEngine.Interoperability;
using ImpunityEngine.ExampleImpunityScripts;

namespace ImpunityEngine.OpenGLExamples
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack =1)]
    public struct TestStruct{
        //c# to c++: CHECK
        //c++ to c#: 
        [MarshalAsAttribute(UnmanagedType.LPStr)]
       public string Name;

       // [MarshalAs(UnmanagedType.I4)]
       // public Int32 ID;
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct TestStruct2
    {
        public IntPtr fromPlus;//string from c++ to c#

        [MarshalAsAttribute(UnmanagedType.LPStr)]
        public string toPlus;//string from c# to c++
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack =1)]
    public struct TestStruct3
    {
        public char[] Name;
    }
    
    
    public class OpenGLExampleController
    {
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int EntireCoordinateSystemLesson();
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int InitiateCoordinateSystemLesson();
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int RenderCoordinateSystemLesson();

        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int EntireCameraLesson();
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int InitiateCameraLesson();

        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int RenderCameraLesson();

        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int EntireLightingLesson();

        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int InitiateLightingLesson();
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int RenderLightingLesson();
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int EntireDiffuseLesson();
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int InitiateDiffuseLesson();

        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int RenderDiffuseLesson();
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int EntireMaterialLesson();

        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int InitiateMaterialLesson();
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int RenderMaterialLesson();
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int LightingMapLesson();
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int InitiateLightMapLesson();
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int RenderLightMapLesson();
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int LightCasterLesson();
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int InitiateLightCasterLesson();
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int RenderLightCasterLesson();
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int EntireMultilightLesson();

        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ModelLoadingLesson();
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int InitiateEngine();
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int RenderAll();
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GiveStruct(TestStruct test);//it might be best to say "out ref someStruct" in the params here... but then what goes in the c++ code?
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr StringTest(string incoming);

        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GiveString(string incoming);

        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int BuildString(StringBuilder str, int len);

        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int GiveFloat(float x);

        

        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr StringToWChar();

        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int FreeWChar(IntPtr text);

        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int LoadModelFromDir(string modelDirectory, int modelDirLen, StringBuilder str, int len);
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CreatePointLight(float x, float y, float z);
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CreateDirectionalLight(float x, float y, float z);
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CreateSpotLight(float x, float y, float z, float dx, float dy, float dz);



        //These lights need to return data that gets interpreted into its analogous c# counterpart
        void MakeSpotLight(float x, float y, float z,float dx, float dy, float dz)
        {
            int id = CreateSpotLight(x, y, z, dx, dy, dz);
            Console.WriteLine("C#: Spot Light ID: " + id);
        }
        void MakeDirectionalLight(float x, float y, float z)
        {
            int id = CreateDirectionalLight(x, y, z);
            Console.WriteLine("C#: Directional Light ID: " + id);
        }
        void MakePointLight(float x, float y, float z)
        {
            int i = CreatePointLight(x, y, z);
            Console.WriteLine("C#: Point light ID: " + i);
        }
        void BuildStrings()
        {
            StringBuilder sb = new StringBuilder(256);//256 chars at most
            int i = BuildString(sb, 256);
            string message = sb.ToString();
            Console.WriteLine("Result: " + message);

            
        }
        void LoadATexture()
        {
            var path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            int start = path.IndexOf("\\") + 1;
            path = path.Substring(start, path.Length - start);
            path += "\\GrTextures\\awesomeface.png";

            //get the name of the texture
            int nstart = path.LastIndexOf("\\") + 1;
            string tname = path.Substring(nstart);

            Console.WriteLine("Texture name: " + tname);

            //get the directory of the texture
            int dend = nstart;
            string tdirectory = path.Substring(0, dend);
            Console.WriteLine("Texture directory: " + tdirectory);
            StringBuilder cmessage = new StringBuilder(10000);//256 chars at most

            int t = Bridge.LoadTextureFromDir(tname, tdirectory, cmessage, 256);
            

            string message = cmessage.ToString();
            Console.WriteLine("Texture creation result: " + message);

            //Now parse the data you got into a texture to be added to Control.AllTextures
            if (message != "{already-loaded}")
            {
                NativeTranslator translator = new NativeTranslator();
                try
                {
                    Texture tex = translator.ParseTextureData(message);
                    Control.AllTextures.Add(tex);
                }
                catch { }
            }
            else
            {
                Console.WriteLine($"{tname} has already been loaded.");
            }

            //try
            //{
            //    NativeTranslator translator = new NativeTranslator();
            //    List<SceneObject> sceneObs = translator.ParseNativeData(message);

            //    foreach (var item in sceneObs)
            //    {
            //        Control.AllSceneObjects.Add(item);
            //    }
            //}
            //catch (Exception err)
            //{
            //    Console.WriteLine("ERROR: " + err.ToString());
            //}


            cmessage.Clear();
        }
        void LoadFromDir()
        {
            var path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            int start = path.IndexOf("\\") + 1;
            path = path.Substring(start, path.Length - start);

            path += "\\GrModels\\tree02\\tree.obj";

         //   Console.WriteLine("Path: " + path);

        StringBuilder cmessage = new StringBuilder(10000);//256 chars at most

        int i = LoadModelFromDir(path, path.Length, cmessage, 10000);


            string message = cmessage.ToString();
            Console.WriteLine("Result: " + message);

            //this would be where we parse the string and create the appropriate 
            //corresponding SceneObject

            try
            {
                //we need to get data from the textures as well!
                NativeTranslator translator = new NativeTranslator();
                List<SceneObject> sceneObs = translator.ParseNativeData(message);

                //  List<SceneObject> sceneObs = Interoperability.NativeTranslator.ParseNativeData(message);
            
            }
            catch (Exception err)
            {
                Console.WriteLine("ERROR: " + err.ToString());
            }
           
            //foreach (var item in sceneObs)
            //{
            //    Control.AllSceneObjects.Add(item);
            //}
            
            cmessage.Clear();
        }
        
        void SendFloat()
        {
            int i = GiveFloat(3.14f);
        }
        public void DrawObjects()
        {
            Console.WriteLine("Creating lights.");
            ////Create some point lights
            //MakePointLight(0, 1.3f, -2.8f);
            //MakePointLight(0, -1.3f, 2.8f);
            //MakePointLight(0, -2.3f, 3.8f);
            //MakePointLight(1.0f, 1.0f, 1.0f);

            MakeDirectionalLight(0.5f, 0.2f, 0.5f);

            //MakeSpotLight(3.0f, 3.0f, 3.0f, 0.25f, 0.25f, 0.25f);
            //MakeSpotLight(3.0f, 3.0f, 3.0f, 0.25f, 0.25f, 0.25f);
            //MakeSpotLight(3.0f, -3.0f, -3.0f, 0.25f, 0.25f, -0.25f);

            int i = InitiateEngine();
            SendFloat();
            BuildStrings();

            int amount = 8;

            DateTime startTime = DateTime.Now;
            for (int l = 0; l < amount; l++)//max is around 10 mill vertices (the struct, not the point)
            {
                //foreach SceneObject SO in SceneObjects .... SO.Update()
                LoadFromDir();
            }
            DateTime endTime = DateTime.Now;

            var totalTime = endTime.Subtract(startTime).TotalMinutes;
            Console.WriteLine("=============================================");
            Console.WriteLine("TOTAL TIME: " + totalTime.ToString());

            //foreach (var item in Control.AllSceneObjects)
            //{
            //    Console.WriteLine("SceneObject: " + item.Name + "\r\n" +
            //        "ID: " + item.ID + "\r\n" + 
            //        "");
            //    item.transform.position = new Vector3(1.0f, 1.0f, 1.0f);
            //    item.transform.SetTransform();

            //}


            for (int o = 0; o < Control.AllSceneObjects.Count; o++)
            {
                //Console.WriteLine("SceneObject: " + Control.AllSceneObjects[o].Name + "\r\n" +
                //   "ID: " + Control.AllSceneObjects[o].ID + "\r\n" +
                //   "");
                Control.AllSceneObjects[o].transform.position = new Vector3(1.0f * o, 1.0f, 1.0f);
                Control.AllSceneObjects[o].transform.SetTransform(Control.AllSceneObjects[o].ID);
            }

            
            LoadATexture();
            LoadATexture();

            for (int o = 0; o < Control.AllTextures.Count; o++)
            {
                Console.WriteLine("Texture: " + Control.AllTextures[o].Path + "\r\n" +
                    "Id: " + Control.AllTextures[o].ID);
            }


            //try out an impunity class
            Character character = new Character(Control.AllSceneObjects[0]);
        Console.WriteLine("Added character script to " + Control.AllSceneObjects[0].Name);

            //load a preconfigured Scene file
            //for right now, the goal is to load/use an untextured model
            //and programmatically assign a diff map from c#
            //which has also been created from c#
            //step 1: create a texture - DONE!
            //step 2: tell a mesh to use that texture - DONE!
            //step 3: tell the mesh to tile that texture - DONE!
            //step 4: tell the mesh to offset that texture - 


            //Let's start by loading a model without any textures
            LoadUntexturedModel();

            string desiredSO = "untexturedWall-0";
            //Now, put the smiley face onto the untextured model's mesh

            //The magic number here is 2. Let's make a method to look up a texture
            //all these methods should be in scene object
            SceneObject found = SceneObject.Find(desiredSO);
            Texture tex = Texture.FindByID(2);

             int tresult = Bridge.SwapDiffuseMap(found, tex);

            ////if success, assign the texture to the scene object
            if (tresult != 1)//resultTypes
            {
                found.material.diffuseMap = tex;
            }

            int tilingResult = Bridge.SetMaterialTiling(found, 6.0f, 6.0f);
            Bridge.SetMaterialOffset(found, 0.2f, 0.8f);

            //random stuff - changes a tree's leaves to smileys
            Bridge.SwapDiffuseMap(found.ID-2, 2);//2 is id of smiley


            //for (int o = 0; o < Control.AllSceneObjects.Count; o++)
            //{
            //    Console.WriteLine("SceneObject: " + Control.AllSceneObjects[o].Name + "\r\n" +
            //       "ID: " + Control.AllSceneObjects[o].ID + "\r\n" +
            //       "");
            //}

            for (int p = 0; p < Control.AllTextures.Count; p++)
            {
                Console.WriteLine($"{Control.AllTextures[p].Path}\r\n" +
                    $"{Control.AllTextures[p].ID}");
            }

            //tell c++ to report what textures it has loaded
            Bridge.ReportTextures();

            for (int o = 0; o < Control.AllSceneObjects.Count; o++)
            {
                Console.WriteLine("SceneObject: " + Control.AllSceneObjects[o].Name + "\r\n" +
                   "ID: " + Control.AllSceneObjects[o].ID + "\r\n" +
                  $"Texture path: {Control.AllSceneObjects[o].material.diffuseMap.Path}");
            }


            while (true)
            {
                //change any positions, variables, etc
                //add forces to rigidbodies, create, etc
                foreach (var so in Control.AllSceneObjects)
                {
                    so.Update();
                }

                int r = RenderAll();
            }
        }
        void LoadUntexturedModel()
        {
            var path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            int start = path.IndexOf("\\") + 1;
            path = path.Substring(start, path.Length - start);

            path += "\\GrModels\\Misfits\\untexturedWall.obj";

            //   Console.WriteLine("Path: " + path);

            StringBuilder cmessage = new StringBuilder(10000);//256 chars at most

            int i = LoadModelFromDir(path, path.Length, cmessage, 10000);


            string message = cmessage.ToString();
            Console.WriteLine("Result: " + message);

            //this would be where we parse the string and create the appropriate 
            //corresponding SceneObject

            try
            {
                NativeTranslator translator = new NativeTranslator();
                List<SceneObject> sceneObs = translator.ParseNativeData(message);

                //  List<SceneObject> sceneObs = Interoperability.NativeTranslator.ParseNativeData(message);
                foreach (var item in sceneObs)
                {
                    Control.AllSceneObjects.Add(item);
                    item.transform.position = new Vector3(-3.0f, 1.0f, 2.0f);
                }
            }
            catch (Exception err)
            {
                Console.WriteLine("ERROR: " + err.ToString());
            }

            //foreach (var item in sceneObs)
            //{
            //    Control.AllSceneObjects.Add(item);
            //}

            cmessage.Clear();
        }

        public void ModelLoad()
        {
            int i = ModelLoadingLesson();
        }
        public void MultiLight()
        {
            Console.WriteLine("Starting multilight lesson");
            int i = EntireMultilightLesson();
        }
        public void LightCasters()
        {
            Console.WriteLine("Starting light caster scene");
            //  int i = LightCasterLesson();
            int i = InitiateLightCasterLesson();
            while (true)
            {
                int r = RenderLightCasterLesson();
            }
        }

        public void LightMaps()
        {
            Console.WriteLine("Starting lighting map lesson");
            int i = InitiateLightMapLesson();
            while (true)
            {
                int r = RenderLightMapLesson();
            }
        }

        public void MaterialLesson()
        {
            //   int i = EntireMaterialLesson();
            Console.WriteLine("Starting materials lesson.");
            int i = InitiateMaterialLesson();
            while (true)
            {
                int r = RenderMaterialLesson();
            }
        }

        public void DiffuseLightingLesson()
        {
            // int i = EntireDiffuseLesson();
            int i = InitiateDiffuseLesson();
            while (true)
            {
                int r = RenderDiffuseLesson();
            }
        }

        public void LightingLessonOne()
        {
            // int i = EntireLightingLesson();
            int i = InitiateLightingLesson();
            //Console.ReadKey();
            while (true)
            {
                int r = RenderLightingLesson();
            }
        }

        public void CamLesson()
        {
            //  int i = EntireCameraLesson();
            Console.WriteLine("w00t!");
            int i = InitiateCameraLesson();
            while(true)
            {
                int p = RenderCameraLesson();
            }
        }


        public void CoordSysLesson()
        {
            int i = EntireCoordinateSystemLesson();
        }

        public void CoordSysRender()
        {
            Console.WriteLine("Hurray!");
            int i = InitiateCoordinateSystemLesson();
            while (true)
            {
                RenderCoordinateSystemLesson();
            }
        }

        public void StructTest()
        {

            //TestStruct^ test = new TestStruct { };
            //test.Name = "This is a test struct";
            //int i = GiveStruct(test);
            //Console.WriteLine("Result: " + test.Name);
            //IntPtr ptr = LookupInfo(id);
            //return (Info)(Marshal.PtrToStructure(ptr, typeof(Info));
            //     IntPtr ptr = GiveStruct();
            //TestStruct ts = (TestStruct)(Marshal.PtrToStructure(ptr, typeof(TestStruct)));//This is the line that throws an exception

            //IntPtr ptr = IntPtr.Zero;
            //int i = GiveStruct(out ptr);
            TestStruct test;
            test.Name = "Hey hey, kids!";
            //int i = GiveStruct(test);
            IntPtr response = GiveStruct(test);
            Console.WriteLine("C# result: " + test.Name);


            try
            {
                TestStruct ts = (TestStruct)(Marshal.PtrToStructure(response, typeof(TestStruct)));//This is the line that throws an exception
                                                                                                   // TestStruct ts = (TestStruct)(Marshal.PtrToStructure(ptr, typeof(TestStruct)));//This is the line that throws an exception
                Console.WriteLine("C# struct results: " + ts.Name);
                //   Console.WriteLine("Name: " + ts.Name);
                //  Console.WriteLine("ID: " + ts.ID);
            }
            catch (Exception err)
            {
                Console.WriteLine(err);
            }

        }
    }
}
