using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

using ImpunityEngine.OpenGLExamples;
using ImpunityEngine.SceneManipulation;
using System.Collections;

namespace ImpunityEngine
{
    public class CommandLinerInterpreter
    {
        private Stack RecentCommands = new Stack();
        public  void InterpretOpeningArgs(string[] args) // broken up by spaces, basically
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
                else if (word == "scene" || word == "koad") {
                    Console.WriteLine("Loading...");
                    LoadScene(i, args);
                }
                else
                {
                    // Console.WriteLine("Teapots.");
                }
            }
        }

        void LoadScene(int index, string[] args) {
            //we are expecting 
            //args[index + 1] -- filepath of scene file
            if (index + 1 > args.Length - 1)
                return;

            SceneMaster.LoadSceneFile(args[index + 1]);
        }
        void SaveScene(int index, string[] args) {
            //we are expecting
            //args[index + 1]  to be a file name
            if (index + 1 > args.Length - 1)
                return;

            SceneMaster.SaveSceneAs(args[index + 1]);

        }
        void SetCutOff(int index, string[] args) {
            //we are expecting 
            //args[index + 1] -- spotLight ID
            //args[index + 2] -- angle

            if (index + 2 > args.Length - 1)
                return;

            int result;
            if (!int.TryParse(args[index + 1], out result))
                return;
            float angle = 0;
            try { angle = Convert.ToSingle(args[index + 2]); }catch { return; }

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
                    Console.WriteLine("Setting point light enabled="+enableState.ToString());
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
            if (!int.TryParse(args[index + 1],out result))
                return;
            float x = 0,y= 0, z = 0;
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
                        Console.WriteLine("Point Light ID: " + pl.LightID + "==Guid:"+pl.guid);
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
                        Console.WriteLine("Directional Light ID: " + pl.LightID + "--Guid:"+pl.guid);
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

            Console.WriteLine("Parsed vector3: " + x + ", " + y + ", " +  z);

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
                    catch(Exception ERR) { Console.WriteLine("The Point light " + (result) + " not found: " + ERR.Message ); }
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
                    catch (NullReferenceException nex){ Console.WriteLine(nex.Message); }
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
            catch (NullReferenceException nex){ Console.WriteLine(nex.Message); }

        }

        void SpaghettiScene()
        {
            Control c = new Control();
            OpenGLExampleController ogl = new OpenGLExampleController();
            ogl.DrawObjects();
        }


    }
}
