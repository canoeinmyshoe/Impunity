using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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
                List<SceneObject> sceneObs = translator.ParseNativeData(message);
            }
            catch (Exception err)
            {
                Console.WriteLine("ERROR: " + err.ToString());
            }
            
            cmessage.Clear();
        }
    }
}
