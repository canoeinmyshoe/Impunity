
using System;
using ImpunityEngine.Interoperability;

namespace ImpunityEngine
{
    public struct Transform
    {
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
        public Vector3 localScale;
        public Vector3 localRotation;

        public Transform(bool hey)
        {
            position = new Vector3(0);
            rotation = new Vector3(0);
            scale = new Vector3(1.0f);
            localScale = new Vector3(1.0f);
            localRotation = new Vector3(0);
        }

        public void Translate()//?
        {

        }
        public void Rotate(float x, float y, float z)
        {
            rotation.x += x;
            rotation.y += y;
            rotation.z += z;
        }
        public void Stretch(float factor)
        {
            scale.x *= factor;
            scale.y *= factor;
            scale.z *= factor;
        }

    }
}
