﻿using System;
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
        delegate void _testGetComponent(string[] args);
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
            //_setCutOff cut = SetCutOff;
            //Methods.Add("cutoff", cut);
            //_setEnabled en = SetEnabled;
            //Methods.Add("enable", en); Methods.Add("setactive", en);
            //_setMaxDist dis = SetMaxDist;
            //Methods.Add("maxdist", dis); Methods.Add("maxdistance", dis);
            //_setAmbientLevel amb = SetAmbientLevel;
            //Methods.Add("amb", amb); Methods.Add("ambient", amb);
            //_setDiffuseLevel diff = SetDiffuseLevel;
            //Methods.Add("diff", diff); Methods.Add("diffuse", diff);
            //_setSpecularLevel spec = SetSpecularLevel;
            //Methods.Add("spec", spec); Methods.Add("specular", spec);
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
            _testGetComponent tgc = GetComponent;
            Methods.Add("getcomponent", tgc); Methods.Add("getcomp", tgc);
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


        private void ConfigureDirectionalLight(string[] args)
        {
            string key = args[1].ToLower();//this would have been dlight or directionallight
            string varType = args[2].ToLower();
            string firstValue = args[3].ToLower();

            if (SceneMaster.SelectedSceneObject == null)
                return;
            DirectionalLight sl;
            try
            {
                sl = (DirectionalLight)SceneMaster.SelectedSceneObject.GetComponent(typeof(DirectionalLight));
            }
            catch { return; }

            if (varType == "amb" || varType == "ambient")
            {
                if (6 > args.Length)
                    return;
                Vector3 color = StringToVec3(args[3], args[4], args[5]);
                sl.SetAmbient(color);
            }
            else if (varType == "diff" || varType == "diffuse")
            {
                if (6 > args.Length)
                    return;
                Vector3 color = StringToVec3(args[3], args[4], args[5]);
                sl.SetDiffuse(color);
            }
            else if (varType == "spec" || varType == "specular")
            {
                if (6 > args.Length)
                    return;
                Vector3 color = StringToVec3(args[3], args[4], args[5]);
                sl.SetSpecular(color);
            }
            else if (varType == "enabled" || varType == "setactive")
            {
                try
                {
                    bool value = false;
                    if (firstValue == "true")
                    {
                        value = true;
                    }
                    else
                    {
                        value = false;
                    }
                    sl.SetEnabled(value);
                }
                catch { return; }
            }
        }
        private void ConfigureSpotLight(string[] args)
        {
            string key = args[1].ToLower();//this would have been slight or spotlight
            string varType = args[2].ToLower();
            string firstValue = args[3].ToLower();

            if (SceneMaster.SelectedSceneObject == null)
                return;
            SpotLight sl;
            try
            {
                sl = (SpotLight)SceneMaster.SelectedSceneObject.GetComponent(typeof(SpotLight));
            }
            catch { return; }

            if (varType == "amb" || varType == "ambient")
            {
                if (6 > args.Length)
                    return;
                Vector3 color = StringToVec3(args[3], args[4], args[5]);
                sl.SetAmbient(color);
            }
            else if (varType == "diff" || varType == "diffuse")
            {
                if (6 > args.Length)
                    return;
                Vector3 color = StringToVec3(args[3], args[4], args[5]);
                sl.SetDiffuse(color);
            }
            else if (varType == "spec" || varType == "specular")
            {
                if (6 > args.Length)
                    return;
                Vector3 color = StringToVec3(args[3], args[4], args[5]);
                sl.SetSpecular(color);
            }
            else if (varType == "distance" || varType == "dist" || varType == "maxdistance" || varType == "maxdist")
            {
                try
                {
                    float x = Convert.ToSingle(firstValue);
                    sl.SetMaxDistance(x);
                }
                catch { return; }
            }
            else if (varType == "cutoff" || varType == "angle" || varType == "spread")
            {
                try
                {
                    float x = Convert.ToSingle(firstValue);
                    sl.SetCutOff(x);
                }
                catch { return; }
            }
            else if (varType == "enabled" || varType == "setactive" || varType == "enable")
            {
                try
                {
                    bool value = false;
                    if (firstValue == "true" ||firstValue == "t")
                    {
                        value = true;
                    }
                    else
                    {
                        value = false;
                    }
                    sl.SetEnabled(value);
                }
                catch { return; }
            }
        }
        void GetComponent(string[] args)
        {
            //args[0] getcomponent
            //args[1] component type
            //args[2] the variable to change
            //args[3 - 6] the value to which the variable is to be changed

            //eg.  "getcomp plight amb .1 .1 .1"
            if (3 > args.Length - 1)
                return;
            if (SceneMaster.SelectedSceneObject == null)
                return;
            Console.WriteLine("Getting component of type " + args[1]);

            string key = args[1].ToLower();
            string varType = args[2].ToLower();
            string firstValue = args[3].ToLower();
            
            if (key == "plight" || key == "pointlight")
            {
                ConfigurePointLight(args);
            }
            else if (key == "slight" || key == "spotlight")
            {
                ConfigureSpotLight(args);
            }
            else if (key == "dlight" || key == "directionallight")
            {
                ConfigureDirectionalLight(args);
            }
        }
        private void ConfigurePointLight(string[] args)
        {
            string key = args[1].ToLower();
            string varType = args[2].ToLower();
            string firstValue = args[3].ToLower();

            if (varType == "amb" || varType == "ambient")
            {
                if (6 > args.Length)
                    return;
                Vector3 color = StringToVec3(args[3], args[4], args[5]);
                SceneMaster.SetPointLightAmbient(color);
            }
            else if (varType == "diff" || varType == "diffuse")
            {
                if (6 > args.Length)
                    return;
                Vector3 color = StringToVec3(args[3], args[4], args[5]);
                SceneMaster.SetPointLightDiffuse(color);
            }
            else if (varType == "spec" || varType == "specular")
            {
                if (6 > args.Length)
                    return;
                Vector3 color = StringToVec3(args[3], args[4], args[5]);
                SceneMaster.SetPointLightSpecular(color);
            }
            else if (varType == "distance" || varType == "dist" || varType == "maxdistance" || varType == "maxdist")
            {
                try
                {
                    float x = Convert.ToSingle(firstValue);
                    PointLight p = (PointLight)SceneMaster.SelectedSceneObject.GetComponent(typeof(PointLight));
                    p.SetMaxDistance(x);
                }
                catch { return; }
            }
            else if (varType == "enabled" || varType == "setactive")
            {
                try
                {
                    bool value = false;
                    if (firstValue == "true")
                    {
                        value = true;
                    }
                    else {
                        value = false;
                    }
                    PointLight p = (PointLight)SceneMaster.SelectedSceneObject.GetComponent(typeof(PointLight));
                    p.SetEnabled(value);
                }
                catch { return; }
            }
        }
        private Vector3 StringToVec3(string ex, string wy, string ze)
        {
            float x = 0; float y = 0; float z = 0;
            try
            {
                x = Convert.ToSingle(ex); y = Convert.ToSingle(wy);z = Convert.ToSingle(ze);
                return new Vector3(x, y, z);
            }
            catch (Exception)
            {
                throw new FormatException("Failed to convert string to vector3");
            }
        }
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
            else if (args.Length == 4) {
                //we assume the user has specified with an 'L' that they want to scale on a local
                //axis
                string keyywird = args[args.Length - 1].ToLower();
                if (keyywird == "l" || keyywird == "local")
                {
                    Console.WriteLine("Scaling on local axis");
                    float scaleFactor = 0;
                    try
                    {
                        scaleFactor = Convert.ToSingle(args[2]);
                    }
                    catch { return; }
                    SceneMaster.ScaleObjectLocal(scaleFactor, args[1]);
                }

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



            if (args.Length == 4)
            {
                //we assume the user has specified with an 'L' that they want to scale on a local
                //axis
                string keyywird = args[args.Length - 1].ToLower();
                if (keyywird == "l" || keyywird == "local")
                {
                    Console.WriteLine("rotating on local axis");
                
                    SceneMaster.RotateObjectLocal(degree, key);
                    return;
                }

            }

            //tell c++ to rotate the object by degree degrees around key axis
            //and set the c# value, too
            SceneMaster.RotateObject(degree, key);

        }



        void SelectSceneObject( string[] args)
        {
            int index = 0;
            //we are expecting 
            //args[index + 1] ---- ID of sceneObject to select
            //args[index + 2] ---- type of sceneObject to select - DEPRECATED
            if (index + 1 > args.Length - 1)
                return;
            Console.WriteLine("Selecting scene object: " + args[index + 1]);
            int result;
            if (!int.TryParse(args[index + 1], out result))
                return;

            SceneMaster.SelectSceneObject(result);
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
                   //     PointLight pl = (PointLight)light;
                   //     Console.WriteLine("Point Light ID: " + pl.LightID + "==Guid:" + pl.guid);
                    }
                }
            }
            else if (key == "directionallight" || key == "dlight")
            {
                foreach (var light in Control.AllSceneObjects)
                {
                    if (light is DirectionalLight)
                    {
                   //     DirectionalLight pl = (DirectionalLight)light;
                 //       Console.WriteLine("Directional Light ID: " + pl.LightID + "--Guid:" + pl.guid);
                    }
                }
            }
            else if (key == "spotlight" || key == "slight")
            {
                foreach (var light in Control.AllSceneObjects)
                {
                    if (light is SpotLight)
                    {
                        //SpotLight pl = (SpotLight)light;
                        //Console.WriteLine("Spot Light ID: " + pl.LightID + "--Guid:" + pl.guid);
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

        }


        void CreateCubeMap(string[] args) {
            //if (index + 1 > args.Length - 1)
            //    return;

            SceneMaster.CreateCubeMap();
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
                       //     PointLight plight = PointLight.FindLightByID(result);
                            //plight.SetPosition(position);
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
                   //         DirectionalLight dlight = DirectionalLight.FindLightByID(result);
                      //      dlight.SetDirection(position);
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
            //            SpotLight spot = SpotLight.FindLightByID(result);
              //          spot.SetPosition(position);
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


        #endregion

    }
  
}
