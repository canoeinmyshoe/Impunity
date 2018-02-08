using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;

using GraphicsController.OpenGLExamples;
namespace GraphicsController
{
    class Program
    {
        static void Main(string[] args)
        {
            //based on args, we'll either load a StartUpScene,
            //Or Attempt to load a save file
            //AS either dev or player


        //    bool test = false;
        //    Debug.Assert(test, "Message");

            Control c = new Control();
            // c.RunMain();
            // c.ManualLoop();
            // c.LoopThread();//Success
            //Now carry on building out the c++ side with examples from the book

            OpenGLExampleController ogl = new OpenGLExampleController();
            //   ogl.DiffuseLightingLesson();
            // ogl.CoordSysLesson();
            // ogl.CoordSysRender();
            //   ogl.CamLesson();
            // ogl.LightingLessonOne();
            //Console.ReadKey();

            //So far so good. Continue in the book with materials
            // ogl.MaterialLesson();
            //ogl.LightMaps();
            //ogl.LightCasters();
            //     ogl.MultiLight();
            //     ogl.ModelLoad();
            // ogl.CreateTree();
            
            ogl.DrawObjects();
        }
    }
}
