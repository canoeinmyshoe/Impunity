using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicsController
{
    public struct Material
    {
        public Vector3 ambient;
        public Vector3 diffuse;
        public Vector3 specular;
        public float shininess;
        public float xTiling;
        public float yTiling;
        public float xOffset;
        public float yOffset;
        public Texture diffuseMap { get; set; }
        public Texture specularMap { get; set; }

        public Material(MaterialType type)
        {
            ambient = new Vector3(0.1f);
            diffuse = new Vector3(0.8f);
            specular = new Vector3(1.0f);
            shininess = 32.0f;
            xTiling = 0;
            yTiling = 0;
            xOffset = 0;
            yOffset = 0;
            diffuseMap = new Texture(TextureType.None);
            specularMap = new Texture(TextureType.None);
        }
    }

    public enum MaterialType
    {
        DefaultTextureless = 1,
        DefaultTextured = 2
    }
}
