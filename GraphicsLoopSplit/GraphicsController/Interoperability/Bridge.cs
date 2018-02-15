using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;
using System.Threading;

namespace ImpunityEngine.Interoperability
{
    public class Bridge//Contains all the C++ pInvoke stuff
    {
        #region Older Methods
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

        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetPointLightPosition(int id, float x, float y, float z);
        public static int SetPointLightPosition(int id, Vector3 position)
        {
            return SetPointLightPosition(id, position.x, position.y, position.z);
        }

        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetDirLightDirection(int id, float x, float y, float z);
        public static int SetDirLightDirection(int id, Vector3 direction)
        {
            return SetDirLightDirection(id, direction.x, direction.y, direction.z);
        }
        #endregion

        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetSpotLightPosition(int id, float x, float y, float z);
        public static int SetSpotLightPosition(int id, Vector3 position)
        {
            return SetSpotLightPosition(id, position.x, position.y, position.z);
        }



        #region Point Light Values
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetPLightAmbient(int id, float x, float y, float z);
        public static int SetPLightAmbient(int id, Vector3 color)
        {
            return SetPLightAmbient(id, color.x, color.y, color.z);
        }
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetPLightDiffuse(int id, float x, float y, float z);
        public static int SetPLightDiffuse(int id, Vector3 color)
        {
            return SetPLightDiffuse(id, color.x, color.y, color.z);
        }
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetPLightSpecular(int id, float x, float y, float z);
        public static int SetPLightSpecular(int id, Vector3 color)
        {
            return SetPLightSpecular(id, color.x, color.y, color.z);
        }
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetMaxDistancePLight(int id, float radius);

        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetPLightEnabled(int id, int enabled);
        #endregion

        #region SpotLight values
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetSLightAmbient(int id, float x, float y, float z);
        public static int SetSLightAmbient(int id, Vector3 color)
        {
            return SetSLightAmbient(id, color.x, color.y, color.z);
        }
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetSLightDiffuse(int id, float x, float y, float z);
        public static int SetSLightDiffuse(int id, Vector3 color)
        {
            return SetSLightDiffuse(id, color.x, color.y, color.z);
        }
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetSLightSpecular(int id, float x, float y, float z);
        public static int SetSLightSpecular(int id, Vector3 color)
        {
            return SetSLightSpecular(id, color.x, color.y, color.z);
        }
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetSpotLightDirection(int id, float x, float y, float z);
        public static int SetSpotLightDirection(int id, Vector3 direction)
        {
            return SetSpotLightDirection(id, direction.x, direction.y, direction.z);
        }
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetSLightEnabled(int id, int enabled);
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetMaxDistanceSLight(int id, float radius);
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetSpotLightCutOff(int id, float x, float ox);
        #endregion

        #region Directional Light values
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetDLightAmbient(int id, float x, float y, float z);
        public static int SetDLightAmbient(int id, Vector3 color)
        {
            return SetDLightAmbient(id, color.x, color.y, color.z);
        }
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetDLightDiffuse(int id, float x, float y, float z);
        public static int SetDLightDiffuse(int id, Vector3 color)
        {
            return SetDLightDiffuse(id, color.x, color.y, color.z);
        }
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetDLightSpecular(int id, float x, float y, float z);
        public static int SetDLightSpecular(int id, Vector3 color)
        {
            return SetDLightSpecular(id, color.x, color.y, color.z);
        }

        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetDLightEnabled(int id, int enabled);
        #endregion

        #region SceneObject Selection
        [DllImport("graphicsPipeline.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SelectSceneObject(int index, int type);
        #endregion

    }

}

