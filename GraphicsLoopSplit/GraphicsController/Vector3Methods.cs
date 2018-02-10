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
    }
}
