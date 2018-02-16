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
