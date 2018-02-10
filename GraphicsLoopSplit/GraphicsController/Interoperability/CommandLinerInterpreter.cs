using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImpunityEngine.OpenGLExamples;
using ImpunityEngine.SceneManipulation;

namespace ImpunityEngine
{
    public class CommandLinerInterpreter
    {

        public  void InterpretOpeningArgs(string[] args)
        {
            if (args.Length == 0)
                return;

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
            string[] args = input.Split(' ');
            //foreach (var item in args)
            //{
            //    Console.WriteLine(item);
            //}

            bool creation = false;
            bool setPosition = false;
            for (int i = 0; i < args.Length; i++)
            {
                if (creation == true)
                {
                    creation = false;
                    Create(args[i]);
                }
                else if (setPosition == true)
                {
                    setPosition = false;
                    SetPosition(i, args);
                }
                else if (args[i] == "create")
                {
                    //Create the next thing that comes up
                    creation = true;
                }
                else if (args[i] == "setPosition")
                {
                    setPosition = true;
                }
            }
        }

        void Create(string arg)
        {
            if (arg == "pointlight")
            {
                Console.WriteLine("Creating point light.");
                SceneMaster.CreatePointLight();
            }
        }
        void SetPosition(int index, string[] args)
        {
            //we're expecting a vec3 after the first arg (a name or an ID). Is there actually a following vec3?
            if (index + 3 > args.Length - 1) { Console.WriteLine("ERROR: INVALID COMMAND"); return; }
            //what is the position?
            float x = Convert.ToSingle(args[index + 1]);
            float y = Convert.ToSingle(args[index + 2]);
            float z = Convert.ToSingle(args[index + 3]);
            Vector3 position = new Vector3(x, y, z);

            Console.WriteLine("Parsed vector3: " + x + ", " + y + ", " +  z);

            //the user may have specied a light type(or other) and the end of this line


            bool useID = false;
            int result;
            if (int.TryParse(args[index], out result))
            {
                //find and set by ID
                Console.WriteLine("Setting position of sceneObject " + result);
                SetByID(result, position);
                useID = true;
            }


            if (index + 4 <= args.Length - 1)
            {
                if (args[index + 4] == "pointLight" || args[index + 4] == "pLight")
                {
                    //set the pLight position...which is done in the class itself, if found
                    try
                    {
                        if (useID == true)
                            PointLight.FindLightByID(result).SetPosition(position);
                    }
                    catch { Console.WriteLine("Point light " + (index + 4) + " not found"); }
                    return;
                }
                else if (args[index + 4] == "directionalLight" || args[index + 4] == "dLight")
                {

                }
                else if (args[index + 4] == "spotLight" || args[index + 4] == "sLight")
                {

                }
            }

            if (useID == true)
            {
                SetByID(result, position);
            }

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
