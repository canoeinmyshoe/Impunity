using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImpunityEngine
{
    public partial struct  Vector3
    {
        public static float Distance(Vector3 a, Vector3 b)
        {
            //call the c++ dll - it's faster
            return 0;
        }

        public static Vector3 MultiplyFloat(Vector3 vec, float flo)
        {
            vec.x *= flo;
            vec.y *= flo;
            vec.z *= flo;
            return vec;
        }

        public void Multiply(float flo) {
            x *= flo;
            y *= flo;
            z *= flo;
        }
        public void Add(Vector3 add) {
            x += add.x;
            y += add.y;
            z += add.z;
        }
    }
}
