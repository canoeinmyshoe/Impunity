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
        }
    }
}
