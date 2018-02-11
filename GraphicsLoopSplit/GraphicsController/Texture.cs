using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImpunityEngine
{
    public class Texture
    {
        public string Path { get; set; }
        public int ID { get; set; }
        public TextureType Type;

        public Texture()
        {
            Path = "Nowhere";
            ID = 0;
            Type = 0;
        }
        public Texture(TextureType type)
        {
            Path = "Nowhere";
            ID = 0;
            Type = 0;
        }
        public Texture(string path, int id, TextureType type)
        {
            Path = path;
            ID = id;
            Type = type;
        }

        public static Texture Find(string desired)
        {
            foreach (var texture in Control.AllTextures)
            {
                if (texture.Path.ToLower() == desired.ToLower())
                {
                    return texture;
                }
            }
            throw new NullReferenceException($"Texture: \"{desired}\" not found");
        }

        public static Texture FindByID(int desired)
        {
            foreach (var texture in Control.AllTextures)
            {
                if (texture.ID== desired)
                {
                    return texture;
                }
            }
            throw new NullReferenceException($"Texture ID: \"{desired}\" not found");
        }
    }

    public enum TextureType
    {
        None = 0,
        DiffuseMap = 1,
        SpecularMap = 2
    }
}
