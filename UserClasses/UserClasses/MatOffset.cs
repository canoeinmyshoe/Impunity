using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImpunityEngine;

namespace UserClasses
{
    public class MatOffset : ImpunityClass
    {

        public override void Update(SceneObject sceneObject)
        {
            float x = sceneObject.material.xOffset;
            float y = sceneObject.material.yOffset;
            sceneObject.SetMaterialOffset(x + 0.001f, y);
        }
    }
}
