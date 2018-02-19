using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImpunityEngine
{
    public class Skybox
    {
        public int index { get; set; }
        public List<string> images { get; set; }
        public Skybox() { }
        public Skybox(int id, List<string> directories) {

            images = directories;
            index = id;
        }
    }
}
