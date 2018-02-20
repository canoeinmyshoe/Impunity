using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImpunityEngine.SceneManipulation;
using ImpunityEngine;

namespace UserClasses
{
    public class MatOffset : ImpunityClass
    {
        private float xIncrementer = 0;
        public override void Update(SceneObject sceneObject)
        {
            float x = sceneObject.material.xOffset;
            float y = sceneObject.material.yOffset;
            sceneObject.SetMaterialOffset(x + xIncrementer, y);

            float halfScreen = Screen.Width / 2.0f;
            string message = "Here's some text";
            int mlen = message.Length;
            float factor = 20.0f;
            float xposition = halfScreen - (mlen * factor) / 2.0f;
            SceneMaster.DrawLabel("Hello from c#", xposition, 65.0f, 1.0f, 1.0f, 1.0f, 1.0f);

            IncrementIndefinitely();
        }

        private void IncrementIndefinitely()
        {
            if (xIncrementer > 200.0f)
                xIncrementer = 0;
            xIncrementer += 0.000001f;
        }
    }
}
