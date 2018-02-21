using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;

using System.IO;

using ImpunityEngine;
using ImpunityEngine.OpenGLExamples;
using ImpunityEngine.SceneManipulation;
using System.Collections;
using UserClasses;
using ImpunityEngine.Persistence;


namespace SceneEditLauncher
{
    class CommandInterpreter
    {
        
        private Stack RecentCommands = new Stack();
        public void InterpretOpeningArgs(string[] args) // broken up by spaces, basically
        {
            if (args.Length == 0)
                return;
            //First, add this to our list of recent commands


            foreach (var item in args)
            {
                Console.WriteLine(item);
            }

            if (args[0] == "openGLexamples")
            {
                SpaghettiScene();
            }
            else if (args[0] == "editor")
            {
                CLE cle = new CLE();
                cle.Run();
            }
            else
            {
                Console.WriteLine("ERROR: COMMAND " + args[0] + " UNRECOGNIZED");
            }
        }

        //Most actions taken will result in a save file string log.... thing
        public void ProcessInput(string input)
        {

            if (input == "^")
            {
                //use the last command
                if (RecentCommands.Count > 1)
                {
                    var thing = RecentCommands.Peek();
                    input = (string)thing;
                }
            }
            else
            {
                if (RecentCommands.Count > 3)
                {
                    RecentCommands.Pop();
                }
                RecentCommands.Push(input);
            }
            string[] args = input.Split(' ');
            foreach (var item in args)
            {
                //  Console.WriteLine(item);
                item.Replace(" ", string.Empty);
            }

            bool creation = false;
            bool setPosition = false;
            bool listObjects = false;
            for (int i = 0; i < args.Length; i++)
            {
                string word = args[i].ToLower();
                if (creation == true)
                {
                    creation = false;
                    Create(args[i], i, args);
                }
                else if (setPosition == true)
                {
                    setPosition = false;
                    SetPosition(i, args);
                }
                else if (listObjects == true)
                {
                    listObjects = false;
                    ShowList(i, args);
                    Console.WriteLine("List time");
                }
                else if (word == "create" || word == "crt")
                {
                    //Create the next thing that comes up
                    creation = true;
                }
                else if (word == "setposition" || word == "setpos")
                {
                    setPosition = true;

                }
                else if (word == "list")
                {
                    //Console.WriteLine("List?");
                    listObjects = true;
                }
                else if (word == "setambient" || word == "setamb")
                {
                    Console.WriteLine("Setting ambient level.");
                    SetAmbientLevel(i, args);
                }
                else if (word == "setdiffuse" || word == "setdiff")
                {
                    Console.WriteLine("Setting diffuse level.");
                    SetDiffuseLevel(i, args);
                }
                else if (word == "setspecular" || word == "setspec")
                {
                    Console.WriteLine("Setting specular level.");
                    SetSpecularLevel(i, args);
                }
                else if (word == "setmaxdist" || word == "setmaxdistance" || word == "smd")
                {
                    Console.WriteLine("Setting max distance");
                    SetMaxDist(i, args);
                }
                else if (word == "enable" || word == "setactive")
                {
                    Console.WriteLine("Enabling");
                    SetEnabled(i, args);
                }
                else if (word == "cutoff" || word == "spotlightangle" || word == "slightangle")
                {
                    Console.WriteLine("Setting cutoff");
                    SetCutOff(i, args);
                }
                else if (word == "save")
                {
                    Console.WriteLine("Saving...");
                    SaveScene(i, args);
                }
                else if (word == "scene" || word == "load")
                {
                    Console.WriteLine("Loading...");
                    LoadScene(i, args);
                }
                else if (word == "quit" || word == "quir" || word == "quiy" || word == "q")
                {
                    Console.WriteLine("Have a nice life, bro.");
                    CommandLineEditor.shouldRun = false;
                    Console.WriteLine("Beans.");
                }
                else if (word == "select" || word == "sel")
                {
                    //select a particular sceneObject
                    SelectSceneObject(i, args);
                }
                else if (word == "rotation" || word == "r")
                {
                    //rotate an object on an axis
                    RotateObject(i, args);
                }
                else if (word == "grab" || word == "g")
                {
                    GrabObject(i, args);
                }
                else if (word == "scale" || word == "s")
                {
                    Console.WriteLine("Scaling...");
                    ScaleObject(i, args);
                }
                else if (word == "swapdiffusemap" || word == "swapdiff")
                {

                    //SwapDiffuseMap
                    SwapDiffuseMap(i, args);
                }
                else if (word == "tiling" || word == "tile")
                {
                    //SetDiffuseTiling(i, args);
                    SetMaterialTiling(i, args);
                }
                else if (word == "offset")
                {
                    SetMaterialOffset(i, args);
                }
                else if (word == "mat" || word == "material")
                {
                    ConfigureMaterial(i, args);
                }
                else if (word == "imp")
                {
                    //add an impunityClass to the sceneObject
                    AddImpX(i, args);
                }
                else if (word == "p" || word == "pause") {
                    PauseEngine();
                }
                else if (word == "+" || word == "++")
                {
                    //select the sceneObject above this one
                }
                else if (word == "-" || word == "--")
                {
                    //select the sceneObject below this one
                }
                else
                {
                    // Console.WriteLine("Teapots.");
                }
            }
        }
        void PauseEngine() {
            if (CommandLineEditor.paused == true)
            {
                CommandLineEditor.paused = false;
            }
            else
            {
                CommandLineEditor.paused = true;
            }
        }
        //case sensitive, of course
        void AddImpX(int index, string[] args)
        {
            Console.WriteLine("Adding imp 2.0");
            if (index + 1 > args.Length - 1)
                return;

            if (SceneMaster.SelectedSceneObject == null)
                return;


            string className = args[index + 1];

            //we need to do this from UserClasses!
            AssemblyManager.ListAssemblies(className);

            #region deprecated assembly search
            //foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            //{

            //    if (asm.FullName.ToString().ToLower().Contains("mscorlib") || asm.FullName.ToString().ToLower().Contains("system") ||
            //        asm.FullName.ToString().ToLower().Contains("microsoft"))
            //        continue;

            //    Console.WriteLine("=========***" + asm.FullName.ToString() + "***==========");
            //    foreach (Type t in asm.GetTypes())
            //    {
            //     //   Console.WriteLine(t.FullName.ToString());
            //        if (t.Name.ToString() == className)
            //        {
            //            Console.WriteLine("EUREKA!"); //Excellent!
            //            var inst = (ImpunityClass)Activator.CreateInstance(t);
            //            SceneMaster.SelectedSceneObject.Imps.Add(inst);
            //            inst.Start();
            //        //    return;
            //        }

            //    }
            //}
            #endregion
        }
        void AddImp(int index, string[] args)
        {
            Console.WriteLine("Adding imp.");
            //we're expecting
            //args[index + 1] ---- the name of the user-created imp to add
            if (index + 1 > args.Length - 1)
                return;

           // string assembly = "UserClasses.dll";
        //    assembly = DirectoryManager.PathToGCBin() + "\\" + assembly;
       //     Console.WriteLine("Assembly dir: " + assembly);
            string className = args[index + 1];
           // Type userImp = Type.GetType(className);
            try
            {
                if (SceneMaster.SelectedSceneObject == null)
                    return;

                // Object instance = new Activator.CreateInstanceFrom(userImp);
                //ImpunityClass instance = (ImpunityClass)Activator.CreateInstance(userImp);
                //instance.sceneObject = SceneMaster.SelectedSceneObject;
                //SceneMaster.SelectedSceneObject.Imps.Add(instance);
                //instance.Start();

                // object objClassInstance = GetInstance(className);
                // ImpunityClass ic = (ImpunityClass)objClassInstance;
                //SceneMaster.SelectedSceneObject.Imps.Add(ic);

                //ImpunityClass instance = (ImpunityClass)System.Reflection.Assembly.GetExecutingAssembly().CreateInstance(className);
                //SceneMaster.SelectedSceneObject.Imps.Add(instance);

                //ObjectHandle handle = Activator.CreateInstance(null, className);
                //ImpunityClass p = (ImpunityClass)handle.Unwrap();
                ////p.Tag = "Samuel";
                //p.sceneObject = SceneMaster.SelectedSceneObject;

                //   Console.WriteLine(p);
           //     var path = @"D:\Plugin.dll";
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();

                foreach (Type t in assembly.GetTypes())
                {
                    
                    Console.WriteLine(t.FullName.ToString());
                    if (t.Name.ToString() == className) {
                        Console.WriteLine("EUREKA!"); //Excellent!
                        var inst = (ImpunityClass)Activator.CreateInstance(t);
                        SceneMaster.SelectedSceneObject.Imps.Add(inst);
                       // inst.Start();
                        return;
                    }
                    
                }
                string prefix = "SceneEditLauncher.";
                var type = assembly.GetType(prefix + className);
              //  var method = type.GetMethod("Run");
                var instance = Activator.CreateInstance(type);


            }
            catch (Exception err)
            {
                Console.WriteLine("Failed to add imp: " + err.Message);
            }
        }
        public object GetInstance(string strFullyQualifiedName)
        {
            Type type = Type.GetType(strFullyQualifiedName);
            if (type != null)
                return Activator.CreateInstance(type);
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = asm.GetType(strFullyQualifiedName);
                if (type != null)
                    return Activator.CreateInstance(type);
            }
            return null;
        }

        void ConfigureMaterial(int index, string[] args)
        {

            //We're expecting
            //args[index + 1] ---- the quality of the material to change
            //args[index + >=2] ----- the value[s] by which to change the quality
            if (index + 2 > args.Length - 1)
                return;

            string quality = args[index + 1].ToLower();

            if (quality == "shine" || quality == "shiny" || quality == "shininess")
            {
                //the next arg will be a float
                float shininess = 0;
                try
                {
                    shininess = Convert.ToSingle(args[index + 2]);
                }
                catch { return; }
                SceneMaster.SetShininess(shininess);
                return;
            }

            //the next 3 args will constitute a vector3
            if (index + 4 > args.Length - 1)
                return;

            float x = 0; float y = 0; float z = 0;
            try
            {
                x = Convert.ToSingle(args[index + 2]);
                y = Convert.ToSingle(args[index + 3]);
                z = Convert.ToSingle(args[index + 4]);
            }
            catch { return; }

            if (quality == "amb" || quality == "ambient")
            {
                SceneMaster.SetMaterialAmbient(x, y, z);
            }
            else if (quality == "diff" || quality == "diffuse")
            {
                SceneMaster.SetMaterialDiffuse(x, y, z);
            }
            else if (quality == "spec" || quality == "specular")
            {
                SceneMaster.SetMaterialSpecular(x, y, z);
            }

        }



        void SetMaterialOffset(int index, string[] args)
        {
            //we're expecting
            //args[index + 1] ---- x offset
            //args[index + 2] ----- y offset
            if (index + 2 > args.Length + 1)
                return;

            float x = 0; float y = 0;
            try
            {
                x = Convert.ToSingle(args[index + 1]);
                y = Convert.ToSingle(args[index + 2]);
            }
            catch { return; }
            SceneMaster.SetMaterialOffset(x, y);
        }
        void SetMaterialTiling(int index, string[] args)
        {
            //we're expecting
            //args[index + 1] ---- x tiling
            //args[index + 2] ----- y tiling
            if (index + 2 > args.Length + 1)
                return;

            float x = 0; float y = 0;
            try
            {
                x = Convert.ToSingle(args[index + 1]);
                y = Convert.ToSingle(args[index + 2]);
            }
            catch { return; }
            SceneMaster.SetMaterialTiling(x, y);
        }
        //Swaps diffuse map of selected object, if possible
        void SwapDiffuseMap(int index, string[] args)
        {
            //we're expecting
            //args[index + 1]  texture ID

            if (index + 1 > args.Length - 1)
                return;

            int result;
            if (!int.TryParse(args[index + 1], out result))
                return;

            //We need to load the texture first!
            SceneMaster.SwapDiffuseMap(result);
        }

        //TODO: ScaleObject
        //we might not want to scale on a single axis...
        void ScaleObject(int index, string[] args)
        {
            //it could be
            //args[index + 1] -- scale factor
            //or it could be
            //args[index + 1] -- axis, and then scale


            if (index + 1 > args.Length)
                return;

            if (args.Length == 2)
            {
                float scaleFactor = 0;
                try
                {
                    scaleFactor = Convert.ToSingle(args[index + 1]);
                }
                catch { return; }
                SceneMaster.ScaleEntireObject(scaleFactor);
                return;
            }


            if (index + 2 > args.Length)
                return;


            float factor = 0;
            try
            {
                factor = Convert.ToSingle(args[index + 2]);
            }
            catch { return; }

            string key = args[index + 1].ToLower(); // the axis

            SceneMaster.ScaleObject(factor, key);
        }
        void GrabObject(int index, string[] args)
        {
            if (index + 2 > args.Length)
                return;

            float distance = 0;
            try
            {
                distance = Convert.ToSingle(args[index + 2]);
            }
            catch { return; }

            string key = args[index + 1].ToLower(); // the axis

            SceneMaster.GrabObject(distance, key);
        }
        void RotateObject(int index, string[] args)
        {
            //we are expecting
            //args[index + 1]  ---- axis, eg "z"
            //args[index + 2] ---- degrees, eg "90"

            if (index + 2 > args.Length)
                return;

            float degree = 0;
            try
            {
                degree = Convert.ToSingle(args[index + 2]);
            }
            catch { return; }

            string key = args[index + 1].ToLower(); // the axis

            //tell c++ to rotate the object by degree degrees around key axis
            //and set the c# value, too
            SceneMaster.RotateObject(degree, key);

        }

        void SelectSceneObject(int index, string[] args)
        {
            //we are expecting 
            //args[index + 1] ---- ID of sceneObject to select
            //args[index + 2] ---- type of sceneObject to select
            if (index + 2 > args.Length - 1)
                return;
            Console.WriteLine("Selecting scene object: " + args[index + 1]);
            int result;
            if (!int.TryParse(args[index + 1], out result))
                return;
            string key = args[index + 2].ToLower();
            if (key == "sceneobject" || key == "so")
            {
                //tell/get selection to c++
                SceneMaster.SelectSceneObject(result, (int)SelectionTypes.regular);
            }
            else if (key == "pointlight" || key == "plight")
            {
                SceneMaster.SelectSceneObject(result, (int)SelectionTypes.pointlight);
            }
            else if (key == "spotlight" || key == "slight")
            {
                SceneMaster.SelectSceneObject(result, (int)SelectionTypes.spotlight);
            }
            else if (key == "directionallight" || key == "dlight")
            {
                SceneMaster.SelectSceneObject(result, (int)SelectionTypes.directionallight);
            }
        }

        void LoadScene(int index, string[] args)
        {
            //we are expecting 
            //args[index + 1] -- filepath of scene file
            if (index + 1 > args.Length - 1)
                return;

            // SceneMaster.LoadSceneFile(args[index + 1]);
            DIskManager.LoadSceneFile(args[index + 1]); 
        }
        void SaveScene(int index, string[] args)
        {
            //we are expecting
            //args[index + 1]  to be a file name
            if (index + 1 > args.Length - 1)
                return;

            // SceneMaster.SaveSceneAs(args[index + 1]);
            DIskManager.SaveSceneAs(args[index + 1]);

        }
        void SetCutOff(int index, string[] args)
        {
            //we are expecting 
            //args[index + 1] -- spotLight ID
            //args[index + 2] -- angle

            if (index + 2 > args.Length - 1)
                return;

            int result;
            if (!int.TryParse(args[index + 1], out result))
                return;
            float angle = 0;
            try { angle = Convert.ToSingle(args[index + 2]); } catch { return; }

            try
            {
                SpotLight.FindLightByID(result).SetCutOff(angle);
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to set spot light cutOff");
            }
        }
        void SetEnabled(int index, string[] args)
        {
            //we're expecting either an ID or a name args[index + 1]
            //followed by a true or false args[index + 2]
            //followed by optional further clarification of light type args[index + 3]


            if (index + 2 > args.Length - 1) { Console.WriteLine("ERROR: INVALID COMMAND"); return; }


            //the user may have specied a light type(or other) and the end of this line
            Console.WriteLine("ID or Name: " + args[index + 1]);

            bool useID = false;
            int result;
            if (int.TryParse(args[index + 1], out result))
            {
                useID = true;
            }
            bool enableState = false;
            if (args[index + 2].ToLower() == "true" || args[index + 2].ToLower() == "t")
            {
                enableState = true;
            }

            if (index + 3 <= args.Length - 1)
            {
                if (args[index + 3].ToLower() == "pointlight" || args[index + 3].ToLower() == "plight")
                {
                    //set the pLight position...which is done in the class itself, if found
                    Console.WriteLine("Setting point light enabled=" + enableState.ToString());
                    try
                    {
                        if (useID == true)
                        {
                            PointLight plight = PointLight.FindLightByID(result);
                            plight.SetEnabled(enableState);
                            //  PointLight.FindLightByID(result).SetPosition(position);
                        }
                    }
                    catch (Exception ERR) { Console.WriteLine("The Point light " + (result) + " not found: " + ERR.Message); }
                    return;
                }
                else if (args[index + 3].ToLower() == "directionallight" || args[index + 3].ToLower() == "dlight")
                {
                    Console.WriteLine("Setting enable directional light");
                    if (useID == true)
                    {
                        try
                        {
                            DirectionalLight dlight = DirectionalLight.FindLightByID(result);
                            //   dlight.SetDirection(position);
                            dlight.SetEnabled(enableState);
                        }
                        catch (NullReferenceException nex) { Console.WriteLine(nex.Message); }
                    }
                    return;
                }
                else if (args[index + 3].ToLower() == "spotlight" || args[index + 3].ToLower() == "slight")
                {
                    Console.WriteLine("Setting enable spotlight");
                    try
                    {
                        SpotLight spot = SpotLight.FindLightByID(result);
                        //   spot.SetPosition(position);
                        spot.SetEnabled(enableState);
                    }
                    catch (NullReferenceException nex) { Console.WriteLine(nex.Message); }
                    return;
                }
            }

            return;

            if (useID == true)
            {
                //   SetByID(result, position);
            }
            else
            {
                // SetByName(args[index], position);
            }

        }

        void SetMaxDist(int index, string[] args)
        {
            //we are expecting (after index) 
            //args[index + 1]  - ID of light
            //args[index + 2] - new ambient value
            //args[index + 3} - light type to search for
            if (index + 3 > args.Length - 1)
                return;
            int result;
            if (!int.TryParse(args[index + 1], out result))
                return;
            float x = 0;
            try
            {
                x = Convert.ToSingle(args[index + 2]);
            }
            catch { return; }

            string key = args[3].ToLower();
            if (key == "pointlight" || key == "plight")
            {
                try { PointLight.FindLightByID(result).SetMaxDistance(x); } catch { Console.WriteLine("Failed to set maxdist point light level."); }
                return;
            }
            else if (key == "spotlight" || key == "slight")
            {
                try { SpotLight.FindLightByID(result).SetMaxDistance(x); } catch { }
                return;
            }
            else if (key == "directionallight" || key == "dlight")
            {
                Console.WriteLine("Directional lights do not have a max distance property");
                //    try { DirectionalLight.FindLightByID(result).SetAmbient(ambient); } catch { }
                return;
            }
        }

        void SetAmbientLevel(int index, string[] args)
        {
            //we are expecting (after index) 
            //args[index + ]  - ID of light
            //args[index + 2-5] - new ambient value
            //args[index + 6] - type of light
            if (index + 5 > args.Length - 1)
                return;
            int result;
            if (!int.TryParse(args[index + 1], out result))
                return;
            float x = 0, y = 0, z = 0;
            try
            {
                x = Convert.ToSingle(args[index + 2]);
                y = Convert.ToSingle(args[index + 3]);
                z = Convert.ToSingle(args[index + 4]);
            }
            catch { return; }
            Vector3 ambient = new Vector3(x, y, z);
            string key = args[5].ToLower();
            if (key == "pointlight" || key == "plight")
            {
                try { PointLight.FindLightByID(result).SetAmbient(ambient); } catch { Console.WriteLine("Failed to set ambient point light level."); }
                return;
            }
            else if (key == "spotlight" || key == "slight")
            {
                try { SpotLight.FindLightByID(result).SetAmbient(ambient); } catch { }
                return;
            }
            else if (key == "directionallight" || key == "dlight")
            {
                try { DirectionalLight.FindLightByID(result).SetAmbient(ambient); } catch { }
                return;
            }
        }
        void SetDiffuseLevel(int index, string[] args)
        {
            //we are expecting (after index) 
            //args[index + ]  - ID of light
            //args[index + 2-5] - new ambient value
            //args[index + 6] - type of light
            if (index + 5 > args.Length - 1)
                return;
            int result;
            if (!int.TryParse(args[index + 1], out result))
                return;
            float x = 0, y = 0, z = 0;
            try
            {
                x = Convert.ToSingle(args[index + 2]);
                y = Convert.ToSingle(args[index + 3]);
                z = Convert.ToSingle(args[index + 4]);
            }
            catch { return; }
            Vector3 color = new Vector3(x, y, z);
            string key = args[5].ToLower();
            if (key == "pointlight" || key == "plight")
            {
                try { PointLight.FindLightByID(result).SetDiffuse(color); } catch { Console.WriteLine("Failed to set diffuse point light level."); }
                return;
            }
            else if (key == "spotlight" || key == "slight")
            {
                try { SpotLight.FindLightByID(result).SetDiffuse(color); } catch { }
                return;
            }
            else if (key == "directionallight" || key == "dlight")
            {
                try { DirectionalLight.FindLightByID(result).SetDiffuse(color); } catch { }
                return;
            }
        }
        void SetSpecularLevel(int index, string[] args)
        {
            //we are expecting (after index) 
            //args[index + ]  - ID of light
            //args[index + 2-5] - new ambient value
            //args[index + 6] - type of light
            if (index + 5 > args.Length - 1)
                return;
            int result;
            if (!int.TryParse(args[index + 1], out result))
                return;
            float x = 0, y = 0, z = 0;
            try
            {
                x = Convert.ToSingle(args[index + 2]);
                y = Convert.ToSingle(args[index + 3]);
                z = Convert.ToSingle(args[index + 4]);
            }
            catch { return; }
            Vector3 color = new Vector3(x, y, z);
            string key = args[5].ToLower();
            if (key == "pointlight" || key == "plight")
            {
                try { PointLight.FindLightByID(result).SetSpecular(color); } catch { Console.WriteLine("Failed to set specular point light level."); }
                return;
            }
            else if (key == "spotlight" || key == "slight")
            {
                try { SpotLight.FindLightByID(result).SetSpecular(color); } catch { }
                return;
            }
            else if (key == "directionallight" || key == "dlight")
            {
                try { DirectionalLight.FindLightByID(result).SetSpecular(color); } catch { }
                return;
            }
        }


        void ShowList(int index, string[] args)
        {

            Console.WriteLine("Key: " + args[index]);

            if (index > args.Length - 1)
                return;

            Console.WriteLine("Index: " + index);

            string key = args[index].ToLower();
            if (key == "sceneobjects" || key == "sos" || key == "sceneobjects")
            {
                foreach (var so in Control.AllSceneObjects)
                {
                    if (so.ID < 0)
                        continue;
                    Console.WriteLine($"Name:{so.Name}--ID:{so.ID}--Guid:{so.guid}");
                }
            }
            else if (key == "pointlight" || key == "plight")
            {
                foreach (var light in Control.AllSceneObjects)
                {
                    if (light is PointLight)
                    {
                        PointLight pl = (PointLight)light;
                        Console.WriteLine("Point Light ID: " + pl.LightID + "==Guid:" + pl.guid);
                    }
                }
            }
            else if (key == "directionallight" || key == "dlight")
            {
                foreach (var light in Control.AllSceneObjects)
                {
                    if (light is DirectionalLight)
                    {
                        DirectionalLight pl = (DirectionalLight)light;
                        Console.WriteLine("Directional Light ID: " + pl.LightID + "--Guid:" + pl.guid);
                    }
                }
            }
            else if (key == "spotlight" || key == "slight")
            {
                foreach (var light in Control.AllSceneObjects)
                {
                    if (light is SpotLight)
                    {
                        SpotLight pl = (SpotLight)light;
                        Console.WriteLine("Spot Light ID: " + pl.LightID + "--Guid:" + pl.guid);
                    }
                }
            }
            else if (key == "texture" || key == "tex")
            {
                foreach (var tex in Control.AllTextures)
                {
                    Console.WriteLine("Texture ID: " + tex.ID + "\r\n"
                        + "Texture path: " + tex.FullPath);
                }
            }

        }
        void Create(string arg, int index, string[] args)
        {
            string key = arg.ToLower();
            if (key == "pointlight" || key == "plight" || key == "plght")
            {
                Console.WriteLine("Creating point light.");
                SceneMaster.CreatePointLight();
            }
            else if (key == "directionallight" || key == "directionalight" || key == "dlight" || key == "dlght")
            {
                Console.WriteLine("Creating directional light");
                SceneMaster.CreateDirectionalLight();
            }
            else if (key == "spotlight" || key == "slight" || key == "slght")
            {
                Console.WriteLine("Creating spot light.");
                SceneMaster.CreateSpotLight();
            }
            else if (key == "model" || key == "mdl")
            {
                Console.WriteLine("Creating model");
                LoadModel(index, args);
            }
            else if (key == "texture" || key == "image" || key == "img" || key == "tex")
            {

                Console.WriteLine("Loading texture...");
                LoadTexture(index, args);
            }
            else if (key == "cubemap" || key == "skybox")
            {
                Console.WriteLine("Creating cube map...");
                CreateCubeMap(index, args);
            }
        }
        void CreateCubeMap(int index, string[] args) {
            //if (index + 1 > args.Length - 1)
            //    return;

            SceneMaster.CreateCubeMap();
        }
        void LoadTexture(int index, string[] args)
        {
            //We're expecting
            //args[index + 1] --- directory of texture
            if (index + 1 > args.Length + 1)
                return;

            SceneMaster.LoadTextureFromDirectory(args[index + 1]);
        }
        void LoadModel(int index, string[] args)//index of the key
        {
            //we're expecting the next argument to be a 3-D object file
            if (index + 1 > args.Length - 1)
                return;

            string path = args[index + 1];
            if (!File.Exists(path))
            {
                Console.WriteLine("ERROR: NO SUCH FILE!");
                return;
            }

            SceneMaster.LoadFromDirectory(path);

        }

        void SetPosition(int index, string[] args)
        {
            //we're expecting a vec3 after the first arg (a name or an ID). Is there actually a following vec3?
            if (index + 3 > args.Length - 1) { Console.WriteLine("ERROR: INVALID COMMAND"); return; }
            //what is the position?
            float x, y, z;
            x = 0; y = 0; z = 0;
            try
            {
                z = Convert.ToSingle(args[index + 3]);
                x = Convert.ToSingle(args[index + 1]);
                y = Convert.ToSingle(args[index + 2]);
            }
            catch { Console.WriteLine("Input in invalid form."); return; }
            Vector3 position = new Vector3(x, y, z);

            Console.WriteLine("Parsed vector3: " + x + ", " + y + ", " + z);

            //the user may have specied a light type(or other) and the end of this line
            Console.WriteLine("ID or Name: " + args[index]);

            bool useID = false;
            int result;
            if (int.TryParse(args[index], out result))
            {

                useID = true;
            }


            if (index + 4 <= args.Length - 1)
            {
                if (args[index + 4].ToLower() == "pointlight" || args[index + 4].ToLower() == "plight")
                {
                    //set the pLight position...which is done in the class itself, if found
                    Console.WriteLine("Setting point light position!");
                    try
                    {
                        if (useID == true)
                        {
                            PointLight plight = PointLight.FindLightByID(result);
                            plight.SetPosition(position);
                            //  PointLight.FindLightByID(result).SetPosition(position);
                        }
                    }
                    catch (Exception ERR) { Console.WriteLine("The Point light " + (result) + " not found: " + ERR.Message); }
                    return;
                }
                else if (args[index + 4].ToLower() == "directionallight" || args[index + 4].ToLower() == "dlight")
                {
                    Console.WriteLine("Directional lights don't have a position. Setting rotation instead.");
                    if (useID == true)
                    {
                        try
                        {
                            DirectionalLight dlight = DirectionalLight.FindLightByID(result);
                            dlight.SetDirection(position);
                        }
                        catch (NullReferenceException nex) { Console.WriteLine(nex.Message); }
                    }
                    return;
                }
                else if (args[index + 4].ToLower() == "spotlight" || args[index + 4].ToLower() == "slight")
                {
                    Console.WriteLine("Setting position of spotlight");
                    try
                    {
                        SpotLight spot = SpotLight.FindLightByID(result);
                        spot.SetPosition(position);
                    }
                    catch (NullReferenceException nex) { Console.WriteLine(nex.Message); }
                    return;
                }
            }

            if (useID == true)
            {
                SetByID(result, position);
            }
            else
            {
                SetByName(args[index], position);
            }

        }
        void SetByName(string name, Vector3 position)
        {
            try
            {
                SceneObject so = SceneObject.Find(name);
                so.transform.position = position;
                return;
            }
            catch (NullReferenceException nex) { Console.WriteLine(nex.Message); }

        }
        void SetByID(int ID, Vector3 position)
        {
            try
            {
                SceneObject so = SceneObject.FindByID(ID);
                so.transform.position = position;
                return;
            }
            catch (NullReferenceException nex) { Console.WriteLine(nex.Message); }

        }

        void SpaghettiScene()
        {
            Control c = new Control();
            OpenGLExampleController ogl = new OpenGLExampleController();
            ogl.DrawObjects();
        }




    }
    enum SelectionTypes
    {
        regular = 0,
        pointlight = 1,
        spotlight = 2,
        directionallight = 3
    }
}
