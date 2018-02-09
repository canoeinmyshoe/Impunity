using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;
using System.Threading;

namespace GraphicsController.Interoperability
{
    public class Bridge//Contains all the C++ pInvoke stuff
    {
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int EntireCoordinateSystemLesson();
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int InitiateCoordinateSystemLesson();
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int RenderCoordinateSystemLesson();

        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int EntireCameraLesson();
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int InitiateCameraLesson();

        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int RenderCameraLesson();

        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int EntireLightingLesson();

        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int InitiateLightingLesson();
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int RenderLightingLesson();
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int EntireDiffuseLesson();
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int InitiateDiffuseLesson();

        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int RenderDiffuseLesson();
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int EntireMaterialLesson();

        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int InitiateMaterialLesson();
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int RenderMaterialLesson();
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int LightingMapLesson();
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int InitiateLightMapLesson();
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int RenderLightMapLesson();
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int LightCasterLesson();
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int InitiateLightCasterLesson();
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int RenderLightCasterLesson();
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int EntireMultilightLesson();

        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ModelLoadingLesson();
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int InitiateEngine();
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int RenderAll();
     
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr StringTest(string incoming);

        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GiveString(string incoming);

        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int BuildString(StringBuilder str, int len);

        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int GiveFloat(float x);

        

        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr StringToWChar();

        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int FreeWChar(IntPtr text);

        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int LoadModelFromDir(string modelDirectory, int modelDirLen, StringBuilder str, int len);
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CreatePointLight(float x, float y, float z);
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CreateDirectionalLight(float x, float y, float z);
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CreateSpotLight(float x, float y, float z, float dx, float dy, float dz);
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetTransform(
            int ID,
            float px, float py, float pz,
            float rx, float ry, float rz,
            float sx, float sy, float sz);

      

        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int LoadTextureFromDir(string textureName, string textureDirectory, StringBuilder str, int len);

        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SwapDiffuseMap(int sceneObjectID, int textureID);
        public static int SwapDiffuseMap(SceneObject sceneObject, Texture texture)
        {
            return SwapDiffuseMap(sceneObject.ID, texture.ID);
        }

        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetMaterialTiling(int sceneObjectID,float xtiling, float ytiling);
        public static int SetMaterialTiling(SceneObject sceneObject, float xtiling, float ytiling)
        {
            return SetMaterialTiling(sceneObject.ID, xtiling, ytiling);
        }

        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetMaterialOffset(int sceneObjectID, float xOffset, float yOffset);
        public static int SetMaterialOffset(SceneObject sceneObject, float xOffset, float yOffset)
        {
            return SetMaterialOffset(sceneObject.ID, xOffset, yOffset);
        }

        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ReportTextures();
    }

}

