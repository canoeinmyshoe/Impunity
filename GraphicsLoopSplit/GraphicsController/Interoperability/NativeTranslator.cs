using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicsController.Interoperability
{
    public class NativeTranslator
    {

        public Texture ParseTextureData(string text)
        {
            text = text.Replace(" ", string.Empty);
            int start1 = text.IndexOf("{") + 1;
            int end1 = text.LastIndexOf("}");
            string all = text.Substring(start1, end1 - start1);
            Texture t = ParseTexture(all);
            return t;
        }
        public List<SceneObject> ParseNativeData(string text)
        {
            List<SceneObject> NewSceneObjects = new List<SceneObject>();
            Console.WriteLine("Parsing through c++ data.");
            //kill any accidental whitespace
            text = text.Replace(" ", string.Empty);
            //get that which is in between brackets
            int start1 = text.IndexOf("{")+1;
            int end1 = text.LastIndexOf("}");
            string all = text.Substring(start1, end1 - start1);
            Console.WriteLine("All Chunks:" + all);
            string[] chunks = all.Split(',');
            for (int i = 0; i < chunks.Length; i++)
            {
                if (chunks[i].Length < 3)
                    continue;
                //if(chunk.contains("t:") - It's a texture, don't create a sceneObject for it.
                //instead, create a texture struct and add that to the list of loaded textures
                if (chunks[i].Contains("t:"))
                {
                    Console.WriteLine("Texture detected");
                    try
                    {
                        Control.AllTextures.Add(ParseTexture(chunks[i]));
                    }
                    catch { }
                    continue;
                }

                SceneObject so =  ProcessChunk(chunks[i]);
                NewSceneObjects.Add(so);
            }

            for (int i = 0; i < NewSceneObjects.Count; i++)
            {
                if (NewSceneObjects[i].isChild != true)
                    continue;

                for (int p = 0; p < NewSceneObjects.Count; p++)
                {
                    if (NewSceneObjects[p].isChild == true)
                        continue;
                    if (NewSceneObjects[p].ID == NewSceneObjects[i].ParentID)
                    {
                        NewSceneObjects[i].Parent = NewSceneObjects[p];
                        NewSceneObjects[p].Children.Add(NewSceneObjects[i]);
                        break;
                    }
                }
            }
            

            //now report for debugging
            //foreach (var so in NewSceneObjects)
            //{
            //    foreach (var child in so.Children)
            //    {
            //        Console.WriteLine($"{so.Name}'s child: {child.Name}");
            //    }
            //}
            
            return NewSceneObjects;
        }

        public SceneObject ProcessChunk(string chunk)
        {
            Console.WriteLine("Chunk: " + chunk);
            //this will be something like n:name_i:3_p:1
            SceneObject so = new SceneObject();
            if (chunk.Contains("n:"))
            {
                int start = chunk.IndexOf("n:") + 2;
                string substr = chunk.Substring(start, chunk.Length - start);
                int end = substr.IndexOf("*");
                string name = substr.Substring(0, end);
                Console.WriteLine("Parsed Name: " + name);
                so.Name = name;
            }

            if (chunk.Contains("i:"))
            {
                int start = chunk.IndexOf("i:") + 2;
                string substr = chunk.Substring(start, chunk.Length - start);
            //    Console.WriteLine("SUBSTRING: " + substr);
                if (substr.Contains("*"))
                {
                    int end = substr.IndexOf("*") ;
                    string term = substr.Substring(0, end);
                    try { so.ID = int.Parse(term); } catch { Console.WriteLine("ID FALURE!"); }
                }
                else
                {
                    try { so.ID = int.Parse(substr); } catch { Console.WriteLine("ID FAILURE2!"); };
                }

                Console.WriteLine("Parsed ID: " + so.ID);
            }
            else
            {
                Console.WriteLine("NO ID FOUND");
            }

            if (chunk.Contains("p:"))
            {
             //   Console.WriteLine("Parent found");

                int start = chunk.IndexOf("p:") + 2;
                string substr = chunk.Substring(start, chunk.Length - start);
                //    Console.WriteLine("SUBSTRING: " + substr);
                if (substr.Contains("*"))
                {
                    int end = substr.IndexOf("*");
                    string term = substr.Substring(0, end);
                    try { so.ParentID = int.Parse(term);
                        so.isChild = true;
                    } catch { Console.WriteLine("PARENT ID FALURE!"); }
                }
                else
                {
                    try { so.ParentID = int.Parse(substr);
                        so.isChild = true;
                    } catch { Console.WriteLine("PARENT ID FAILURE2!"); };
                }

                Console.WriteLine("Parsed Parent ID: " + so.ParentID);
            }

            return so;
        }

        public Texture ParseTexture(string text)
        {
            Console.WriteLine("Texture data: " + text);
            Texture texture = new Texture(TextureType.None);

            if (text.Contains("t:"))
            {
                //this is the filename of the texture
                //read until the next *
                int start = text.IndexOf("t:") + 2;
                string substr = text.Substring(start, text.Length - start);
                int end = substr.IndexOf("*");
                string name = substr.Substring(0, end);
                Console.WriteLine("Parsed texture path: " + name);
                texture.Path = name;
                texture.Type = TextureType.DiffuseMap;
            }
            else
            {
                Console.WriteLine("NO PATH FOUND");
            }
            if (text.Contains("i:"))
            {
                //This is the index (in Main.cp/LoadedTextures) of the texture
                int start = text.IndexOf("i:") + 2;
                string substr = text.Substring(start, text.Length - start);
                //    Console.WriteLine("SUBSTRING: " + substr);
                if (substr.Contains("*"))
                {
                    int end = substr.IndexOf("*");
                    string term = substr.Substring(0, end);
                    try
                    {
                        texture.ID = int.Parse(term);
                    }
                    catch { Console.WriteLine("TEXTURE ID FALURE!"); }
                }
                else
                {
                    try
                    {
                       texture.ID= int.Parse(substr);
                    }
                    catch { Console.WriteLine("PARENT ID FAILURE2!"); };
                }

                Console.WriteLine("Parsed texture ID: " + texture.ID);
            }
            return texture;

        }




    }


}
