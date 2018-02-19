using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImpunityEngine.Interoperability;

namespace ImpunityEngine
{
    public partial class SceneObject
    {
        public void SetTransform()
        {
            Bridge.SetTransformMatrix(ID, transform);
        }
        public Vector3 Right()
        {
            //get the right direction of the matrix
            //Bridge.GetRight(ID);
            StringBuilder cmessage = new StringBuilder(256);//256 chars at most
          //  int i = Bridge.LoadModelFromDir(path, path.Length, cmessage, 256);
            int i = Bridge.GetRight(cmessage, ID);

            if (i == 1)
                return new Vector3(0);
          //  Console.WriteLine("GetRight() Result: " + message);
            string[] dimensions = cmessage.ToString().Split(',');
         
           float  x = Convert.ToSingle(dimensions[0]);
            float y = Convert.ToSingle(dimensions[1]);
           float  z = Convert.ToSingle(dimensions[2]);

            return new Vector3(x,y,z);
        }
        public Vector3 Up() {

            StringBuilder cmessage = new StringBuilder(256);//256 chars at most
                                                            //  int i = Bridge.LoadModelFromDir(path, path.Length, cmessage, 256);
            int i = Bridge.GetUp(cmessage, ID);

            if (i == 1)
                return new Vector3(0);
            //  Console.WriteLine("GetRight() Result: " + message);
            string[] dimensions = cmessage.ToString().Split(',');
            float x = Convert.ToSingle(dimensions[0]);
            float y = Convert.ToSingle(dimensions[1]);
            float z = Convert.ToSingle(dimensions[2]);

            return new Vector3(x, y, z);
        }
        public Vector3 Forward()
        {

            StringBuilder cmessage = new StringBuilder(256);//256 chars at most
                                                            //  int i = Bridge.LoadModelFromDir(path, path.Length, cmessage, 256);
            int i = Bridge.GetForward(cmessage, ID);

            if (i == 1)
                return new Vector3(0);
            //  Console.WriteLine("GetRight() Result: " + message);
            string[] dimensions = cmessage.ToString().Split(',');
          
            float x = Convert.ToSingle(dimensions[0]);
            float y = Convert.ToSingle(dimensions[1]);
            float z = Convert.ToSingle(dimensions[2]);


            return new Vector3(x, y, z);
        }
        public void SetMaterialAmbient(Vector3 ambientColor) {

            if (Bridge.SetMaterialAmbient(ID, ambientColor) != 1)
                material.ambient = ambientColor;

        }
        public void SetMaterialDiffuse(Vector3 diffuseColor)
        {

            if (Bridge.SetMaterialDiffuse(ID, diffuseColor) != 1)
                material.diffuse = diffuseColor;

        }
        public void SetMaterialSpecular(Vector3 specularColor)
        {

            if (Bridge.SetMaterialSpecular(ID, specularColor) != 1)
                material.specular = specularColor;

        }
        public void SetShininess(float shininess) {
        //    material.shininess = shininess;
            //Bridge.SetShininess(ID, shininess);
            //only set material if the result is not 1
            if (Bridge.SetShininess(ID, shininess) != 1) {
                material.shininess = shininess;
            }
        }
        public void SetMaterialOffset(float x, float y) {
            material.xOffset = x;
            material.yOffset = y;
            Bridge.SetMaterialOffset(ID, x, y);
        }
        public void SetMaterialTiling(float x, float y) {
            material.xTiling = x;
            material.yTiling = y;
            Bridge.SetMaterialTiling(ID, x, y);
        }
        public void SetDiffuseMap(int textureID) {

            Texture tex = Texture.FindByID(textureID);

            int result = Bridge.SwapDiffuseMap(this, tex);
            if (result != 0)
                return;

            material.diffuseMap = tex;
        }
        public void SetDiffuseMap(Texture texture)
        {

          //  Texture tex = Texture.FindByID(textureID);

            int result = Bridge.SwapDiffuseMap(this, texture);
            if (result != 0)
                return;

            material.diffuseMap = texture;
        }
        //Case-insensitive look-up
        public static SceneObject Find(string sceneObjectName)
        {
            foreach (var so in Control.AllSceneObjects)
            {
                if (so.Name.ToLower() == sceneObjectName.ToLower())
                {
                    return so;
                }
            }
            throw new NullReferenceException($"SceneObject \"{sceneObjectName}\" not found.");
        }
        public static SceneObject FindByID(int id)
        {
            foreach (var so in Control.AllSceneObjects)
            {
                if (so.ID == id)
                {
                    return so;
                }
            }
            throw new NullReferenceException($"SceneObject of id \"{id}\" not found.");
        }
        public static SceneObject FindByGuid(Guid gid)
        {
            foreach (var so in Control.AllSceneObjects)
            {
                if (so.guid == gid)
                    return so;
            }
            throw new NullReferenceException($"SceneObject of guid \"{gid}\" not found.");
        }
    }
}
