using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImpunityEngine
{
    public partial struct Vector3
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
        public Vector3(float ex, float wy, float ze)
        {
            x = ex;
            y = wy;
            z = ze;
        }
        public Vector3(float b)
        {

            x = b;
            y = b;
            z = b;
        }

        #region Methods

        #region Static Methods

        #endregion

        #endregion

    }
}
