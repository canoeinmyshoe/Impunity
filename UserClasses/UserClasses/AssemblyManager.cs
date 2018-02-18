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
            string prefix = "UserClasses.";
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
                      Console.WriteLine(t.FullName.ToString());
                    if (t.Name.ToString() == prefix + className)
                    {
                        Console.WriteLine("EUREKA!"); //Excellent!
                        var inst = (ImpunityClass)Activator.CreateInstance(t);
                        SceneMaster.SelectedSceneObject.Imps.Add(inst);
                        inst.Start();
                        //    return;
                    }

                }
            }

        }
    }
}
