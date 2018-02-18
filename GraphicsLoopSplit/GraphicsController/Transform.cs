
using System;
using ImpunityEngine.Interoperability;

namespace ImpunityEngine
{
    public struct Transform
    {
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;

        public Transform(bool hey)
        {
            position = new Vector3(0);
            rotation = new Vector3(0);
            scale = new Vector3(1.0f);
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
        public void SetTransform(int ID)
        {
            //call the c++ dll
            Bridge.SetTransform(
                ID,
                position.x, position.y, position.z,
                rotation.x, rotation.y, rotation.z,
                scale.x, scale.y, scale.z
                );
        }
        
    }
}
