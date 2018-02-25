using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImpunityEngine;
using ImpunityEngine.SceneManipulation;
namespace UserClasses
{
    public class AssemblyManager
    {
        public static void ListAssemblies(string className)
        {
            //string prefix = "UserClasses.";
           // className = prefix + className;
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (asm.FullName.ToString().ToLower().Contains("mscorlib") || asm.FullName.ToString().ToLower().Contains("system") ||
              asm.FullName.ToString().ToLower().Contains("microsoft"))
                    continue;

                if (!asm.FullName.ToString().ToLower().Contains("userclasses"))
                    continue;

                Console.WriteLine("=========***" + asm.FullName.ToString() + "***==========");
                foreach (Type t in asm.GetTypes())
                {
                //      Console.WriteLine(t.FullName.ToString() + "   vs.   " +className);
                    if (t.Name.ToString() == className)
                    {
                        Console.WriteLine("EUREKA!"); //Excellent!

                        var inst = (ImpunityClass)Activator.CreateInstance(t);


                     //   inst.sceneObject = SceneMaster.SelectedSceneObject;
                        inst.Start(SceneMaster.SelectedSceneObject);
                        //   SceneMaster.SelectedSceneObject.Imps.Add(inst);


                        SceneMaster.SelectedSceneObject.Components.Add(inst);
                        return;
                    }

                }
            }

        }

        public static Type[] UserTypes() {

            List<Type> utypes = new List<Type>();
          
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (asm.FullName.ToString().ToLower().Contains("mscorlib") || asm.FullName.ToString().ToLower().Contains("system") ||
              asm.FullName.ToString().ToLower().Contains("microsoft"))
                    continue;

                if (!asm.FullName.ToString().ToLower().Contains("userclasses"))
                    continue;

                Console.WriteLine("=========***" + asm.FullName.ToString() + "***==========");
                foreach (Type t in asm.GetTypes())
                {

                    Console.WriteLine("Adding " + t.Name + " to Type[]");
                    utypes.Add(t);
                }
            }

            Type[] result = new Type[utypes.Count];
            for (int i = 0; i < utypes.Count; i++)
            {
                result[i] = utypes[i];
            }

            return result;
           // throw new NotImplementedException();
        }


    }
}
