using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using ImpunityEngine.Interoperability;
namespace ImpunityEngine
{
    public  class Camera 
    {

        public Transform transform = new Transform(true);
        public float Zoom;
        public float NearClippingPlane = 0.1f;
        public float FarClippingPlane = 1000.0f;
        private int ID = -66666;

        public void Update()
        {
            //tell bridge the transform
            SetTransform();
        }

        public void SetTransform()
        {
            //   Bridge.SetCameraTransformMatrix(ID, transform);
            Bridge.SetCameraTransformMatrix(ID, transform);
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

            float x = Convert.ToSingle(dimensions[0]);
            float y = Convert.ToSingle(dimensions[1]);
            float z = Convert.ToSingle(dimensions[2]);

            return new Vector3(x, y, z);
        }
        public Vector3 Up()
        {

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

    }
}
