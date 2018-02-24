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
        #region Delegate Declarations
        Dictionary<string, Delegate> Methods;
        delegate void _addImp(string[] args);
        delegate void _configureMaterial(string[] args) ;
        delegate void _setMaterialOffset(string[] args);
        delegate void _setMaterialTiling(string[] args);
        delegate void _swapDiffuseMap(string[] args);
        delegate void _scaleObject(string[] args);
        delegate void _grabObject(string[] args);
        delegate void _rotateObject(string[] args);
        delegate void _selectSceneObject(string[] args);
        delegate void _loadScene(string[] args);
        delegate void _saveScene(string[] args);
        delegate void _setCutOff( string[] args);
        delegate void _setEnabled(string[] args);
        delegate void _setMaxDist(string[] args);
        delegate void _setAmbientLevel(string[] args);
        delegate void _setDiffuseLevel(string[] args);
        delegate void _setSpecularLevel(string[] args);
        delegate void _showList(string[] args);
        delegate void _create(string[] args);
        delegate void _createCubeMap(string[] args);
        delegate void _loadTexture(string[] args);
        delegate void _loadModel(string[] args);
        delegate void _setPosition(string[] args);
        delegate void _pauseEngine(string[] args);
        delegate void _quitEditor(string[] args);
        #endregion

        public CommandInterpreter() {
            CreateDelegates();
        }

        private void CreateDelegates() {

            Methods = new Dictionary<string, Delegate>();
            _addImp a = AddImp;
            Methods.Add("imp", a);
            _configureMaterial c = ConfigureMaterial;
            Methods.Add("material", c); Methods.Add("mat", c);
            _setMaterialOffset s = SetMaterialOffset;
            Methods.Add("offset", s);
            _setMaterialTiling ss = SetMaterialTiling;
            Methods.Add("tile", ss); Methods.Add("tiling", ss);
            _swapDiffuseMap sw = SwapDiffuseMap;
            Methods.Add("swap", sw); Methods.Add("swapdiff", sw); Methods.Add("swapdiffuse", sw);
            _scaleObject sc = ScaleObject;
            Methods.Add("scale", sc); Methods.Add("s", sc);
            _grabObject gr = GrabObject;
            Methods.Add("grab", gr); Methods.Add("g", gr);
            _rotateObject ro = RotateObject;
            Methods.Add("rotate", ro); Methods.Add("r", ro);
            _selectSceneObject sel = SelectSceneObject;
            Methods.Add("select", sel); Methods.Add("sel", sel);
            _loadScene lod = LoadScene;
            Methods.Add("scene", lod); Methods.Add("load", lod);
            _saveScene sav = SaveScene;
            Methods.Add("save", sav);
            _setCutOff cut = SetCutOff;
            Methods.Add("cutoff", cut);
            _setEnabled en = SetEnabled;
            Methods.Add("enable", en); Methods.Add("setactive", en);
            _setMaxDist dis = SetMaxDist;
            Methods.Add("maxdist", dis); Methods.Add("maxdistance", dis);
            _setAmbientLevel amb = SetAmbientLevel;
            Methods.Add("amb", amb); Methods.Add("ambient", amb);
            _setDiffuseLevel diff = SetDiffuseLevel;
            Methods.Add("diff", diff); Methods.Add("diffuse", diff);
            _setSpecularLevel spec = SetSpecularLevel;
            Methods.Add("spec", spec); Methods.Add("specular", spec);
            _showList lis = ShowList;
            Methods.Add("list", lis);
            _create crt = Create;
            Methods.Add("crt", crt); Methods.Add("create", crt);
            _createCubeMap cmp = CreateCubeMap;
            Methods.Add("cubemape", cmp); Methods.Add("skybox", cmp);
            _loadTexture tex = LoadTexture;
            Methods.Add("tex", tex); Methods.Add("texture", tex);
            _loadModel mod = LoadModel;
            Methods.Add("model", mod);
            _setPosition sp = SetPosition;
            Methods.Add("setpos", sp); Methods.Add("setposition", sp);
            _pauseEngine pe = PauseEngine;
            Methods.Add("pause", pe); Methods.Add("p", pe);
            _quitEditor qe = QuitEditor;
            Methods.Add("quit", qe); Methods.Add("q", qe);
        }
   
        public void ProcessInput(string input)
        {

            //First, we should split up the args by semi colon - no?
            string[] clauses = input.Split(';');
            //then foreach clause, the below, with clauses[i] instead of input in the latter
            //that way, it can read scripts
            Console.WriteLine("Clauses: " + clauses.Length);
            foreach (var clause in clauses)
            {
              //  Console.WriteLine("FUCK");
                string[] args = clause.Split(' ');
                if (Methods.ContainsKey(args[0]))
                {
                    var method = Methods[args[0]];
                    method.DynamicInvoke(new object[] { args });
                       Console.WriteLine("Completed command.");
                  //  return;
                }
                else
                {
                    Console.WriteLine($"No such method \"{args[0]}\"");
                }
            }

          

        }

        #region Methods referred to by the delegates
        void QuitEditor(string[] args) {
            Console.WriteLine("Have a nice life, bro.");
            CommandLineEditor.shouldRun = false;
        }
        void PauseEngine(string[] args) {
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
        void AddImp(string[] args)
        {
            int index = 0;
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

        void ConfigureMaterial(string[] args)
        {
            int index = 0;

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

        void SetMaterialOffset(string[] args)
        {
            int index = 0;
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
        void SetMaterialTiling( string[] args)
        {
            int index = 0;
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
        void SwapDiffuseMap(string[] args)
        {
            int index = 0;
            //we're expecting
            //args[index + 1]  texture ID

            if (index + 1 > args.Length - 1)
                return;

            int result;
            if (!int.TryParse(args[index + 1], out result))
                return;

            //We need to load the texture first!
            try
            {
                SceneMaster.SwapDiffuseMap(result);
            }
            catch (Exception)
            {

                throw;
            }
          
        }

        //TODO: ScaleObject
        //we might not want to scale on a single axis...
        void ScaleObject(string[] args)
        {
            int index = 0;
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
        void GrabObject(string[] args)
        {
            int index = 0;
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
        void RotateObject(string[] args)
        {
            int index = 0;
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

        void SelectSceneObject( string[] args)
        {
            int index = 0;
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

        void LoadScene(string[] args)
        {
            int index = 0;
            //we are expecting 
            //args[index + 1] -- filepath of scene file
            if (index + 1 > args.Length - 1)
                return;

            // SceneMaster.LoadSceneFile(args[index + 1]);
            DIskManager.LoadSceneFile(args[index + 1]); 
        }
        void SaveScene(string[] args)
        {
            int index = 0;
            //we are expecting
            //args[index + 1]  to be a file name
            if (index + 1 > args.Length - 1)
                return;

            // SceneMaster.SaveSceneAs(args[index + 1]);
            DIskManager.SaveSceneAs(args[index + 1]);

        }
        void SetCutOff(string[] args)
        {
            int index = 0;
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
        void SetEnabled(string[] args)
        {
            int index = 0;
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

        void SetMaxDist(string[] args)
        {
            int index = 0;
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

        void SetAmbientLevel(string[] args)
        {
            int index = 0;
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
        void SetDiffuseLevel(string[] args)
        {
            int index = 0;
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
        void SetSpecularLevel( string[] args)
        {
            int index = 0;
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


        void ShowList( string[] args)
        {
            int index = 0;
            Console.WriteLine("Key: " + args[index]);

            if (index > args.Length - 1)
                return;

            Console.WriteLine("Index: " + index);

            string key = args[index + 1].ToLower();
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
        void Create(string[] args)
        {
            string key = args[1].ToLower();
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
                //LoadModel(index, args);
            }
            else if (key == "texture" || key == "image" || key == "img" || key == "tex")
            {

                Console.WriteLine("Loading texture...");
             //   LoadTexture(index, args);
            }
            else if (key == "cubemap" || key == "skybox")
            {
                Console.WriteLine("Creating cube map...");
                //CreateCubeMap(index, args);
            }
            else if (key == "sceneobject" || key == "so") {
                CreateEmptySceneObject();
            }
        }

        void CreateCubeMap(string[] args) {
            //if (index + 1 > args.Length - 1)
            //    return;

            SceneMaster.CreateCubeMap();
        }
        void CreateEmptySceneObject()
        {

            SceneObject so = new SceneObject(true);
            so.Name = "Empty SceneObject";
            Control.AllSceneObjects.Add(so);
            Console.WriteLine("New scene object created.");
        }
        void LoadTexture(string[] args)
        {
            int index = 0;
            //We're expecting
            //args[index + 1] --- directory of texture
            if (index + 1 > args.Length + 1)
                return;

            SceneMaster.LoadTextureFromDirectory(args[index + 1]);
        }
        void LoadModel(string[] args)//index of the key
        {
            int index = 0;
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


        void SetPosition(string[] args)
        {
            int index = 0;
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
        #endregion

    }
    enum SelectionTypes
    {
        regular = 0,
        pointlight = 1,
        spotlight = 2,
        directionallight = 3
    }
}
