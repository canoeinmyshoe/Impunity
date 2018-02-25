using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImpunityEngine.Interoperability
{
    public class NativeTranslator
    {
        public List<SceneObject> ParseNativeData(string text, string filePath, Guid parentGuid, List<Guid> ChildGuids)
        {
            List<SceneObject> NewSceneObjects = new List<SceneObject>();
            Console.WriteLine("Parsing through c++ data with GUIDs.");
            //kill any accidental whitespace
            text = text.Replace(" ", string.Empty);
            //get that which is in between brackets
            int start1 = text.IndexOf("{") + 1;
            int end1 = text.LastIndexOf("}");
            string all = text.Substring(start1, end1 - start1);
            Console.WriteLine("All Chunks:" + all);
            string[] chunks = all.Split(',');
            int sosAdded = 0;
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
                        //don't add it if we already have it
                        // Texture texture = ParseTexture(chunks[i]);

                        Control.AllTextures.Add(ParseTexture(chunks[i]));
                    }
                    catch { }
                    continue;
                }
                SceneObject so = ProcessChunk(chunks[i]);
                //We need to add to control, not local...

                if (i == 0)
                {
                    //MODEL_PATH: c:\\..appdata\\something
                    so.modelPath = filePath;
                    so.guid = parentGuid;
                }
                else if (sosAdded - 1 <= ChildGuids.Count - 1)
                {
                    so.guid = ChildGuids[sosAdded - 1];
                }
                else
                {
                    Console.WriteLine("No dice. " + ChildGuids.Count + " vs. " + (sosAdded - 1) + " ==> i=" + sosAdded);
                }

                Control.AllSceneObjects.Add(so);
                NewSceneObjects.Add(so);
                sosAdded += 1;
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
        public Texture ParseTextureData(string text)
        {
            text = text.Replace(" ", string.Empty);
            int start1 = text.IndexOf("{") + 1;
            int end1 = text.LastIndexOf("}");
            string all = text.Substring(start1, end1 - start1);
            Texture t = ParseTexture(all);
            return t;
        }
        public List<SceneObject> ParseNativeData(string text, string filePath)
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
                        //don't add it if we already have it
                       // Texture texture = ParseTexture(chunks[i]);
                       
                        Control.AllTextures.Add(ParseTexture(chunks[i]));
                    }
                    catch { }
                    continue;
                }
                SceneObject so =  ProcessChunk(chunks[i]);
                //We need to add to control, not local...

                if (i == 0)
                {
                    //MODEL_PATH: c:\\..appdata\\something
                    so.modelPath = filePath;
                }

                Control.AllSceneObjects.Add(so);
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
            SceneObject so = new SceneObject(-1);
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

            if (chunk.Contains("maX:"))
            {
                //material.ambient.x
                string key = "maX:";
                int keyLen = key.Length;
                int subStart = chunk.IndexOf(key) + keyLen;
                string subStr = chunk.Substring(subStart);

                //now, get another substring ending at first occurance of *
                string value = subStr.Substring(0, subStr.IndexOf("*"));

                try
                {
                    float ambx = Convert.ToSingle(value);
                    so.material.ambient.x = ambx;
                }
                catch { }
            }
            if (chunk.Contains("maY:"))
            {
                //material.ambient.x
                string key = "maY:";
                int keyLen = key.Length;
                int subStart = chunk.IndexOf(key) + keyLen;
                string subStr = chunk.Substring(subStart);

                //now, get another substring ending at first occurance of *
                string value = subStr.Substring(0, subStr.IndexOf("*"));

                try
                {
                    float val = Convert.ToSingle(value);
                    so.material.ambient.y = val;
                }
                catch { }
            }
            if (chunk.Contains("maZ:"))
            {
                //material.ambient.x
                string key = "maZ:";
                int keyLen = key.Length;
                int subStart = chunk.IndexOf(key) + keyLen;
                string subStr = chunk.Substring(subStart);

                //now, get another substring ending at first occurance of *
                string value = subStr.Substring(0, subStr.IndexOf("*"));

                try
                {
                    float val = Convert.ToSingle(value);
                    so.material.ambient.z = val;
                }
                catch { }
            }
            if (chunk.Contains("mdX:"))
            {
                //material.ambient.x
                string key = "mdX:";
                int keyLen = key.Length;
                int subStart = chunk.IndexOf(key) + keyLen;
                string subStr = chunk.Substring(subStart);

                //now, get another substring ending at first occurance of *
                string value = subStr.Substring(0, subStr.IndexOf("*"));

                try
                {
                    float val = Convert.ToSingle(value);
                    so.material.diffuse.x = val;
                }
                catch { }
            }
            if (chunk.Contains("mdY:"))
            {
                //material.ambient.x
                string key = "mdY:";
                int keyLen = key.Length;
                int subStart = chunk.IndexOf(key) + keyLen;
                string subStr = chunk.Substring(subStart);

                //now, get another substring ending at first occurance of *
                string value = subStr.Substring(0, subStr.IndexOf("*"));

                try
                {
                    float val = Convert.ToSingle(value);
                    so.material.diffuse.y = val;
                }
                catch { }
            }
            if (chunk.Contains("mdZ:"))
            {
                //material.ambient.x
                string key = "mdZ:";
                int keyLen = key.Length;
                int subStart = chunk.IndexOf(key) + keyLen;
                string subStr = chunk.Substring(subStart);

                //now, get another substring ending at first occurance of *
                string value = subStr.Substring(0, subStr.IndexOf("*"));

                try
                {
                    float val = Convert.ToSingle(value);
                    so.material.diffuse.z = val;
                }
                catch { }
            }
            if (chunk.Contains("msX:"))
            {
                //material.ambient.x
                string key = "msX:";
                int keyLen = key.Length;
                int subStart = chunk.IndexOf(key) + keyLen;
                string subStr = chunk.Substring(subStart);

                //now, get another substring ending at first occurance of *
                string value = subStr.Substring(0, subStr.IndexOf("*"));

                try
                {
                    float val = Convert.ToSingle(value);
                    so.material.specular.x = val;
                }
                catch { }
            }
            if (chunk.Contains("msY:"))
            {
                //material.ambient.x
                string key = "msY:";
                int keyLen = key.Length;
                int subStart = chunk.IndexOf(key) + keyLen;
                string subStr = chunk.Substring(subStart);

                //now, get another substring ending at first occurance of *
                string value = subStr.Substring(0, subStr.IndexOf("*"));

                try
                {
                    float val = Convert.ToSingle(value);
                    so.material.specular.y = val;
                }
                catch { }
            }
            if (chunk.Contains("msZ:"))
            {
                //material.ambient.x
                string key = "msZ:";
                int keyLen = key.Length;
                int subStart = chunk.IndexOf(key) + keyLen;
                string subStr = chunk.Substring(subStart);

                //now, get another substring ending at first occurance of *
                string value = subStr.Substring(0, subStr.IndexOf("*"));

                try
                {
                    float val = Convert.ToSingle(value);
                    so.material.specular.z = val;
                }
                catch { }
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
                    int id;
                    if (int.TryParse(term, out id))
                    {
                        texture.ID = id;
                    }
                    else
                    {
                        Console.WriteLine("WARNING: Texture ID parse failed. TERM:" + term);
                    }

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
            //bool OnSceneObject = false;
            //int soID = 0;
            if (text.Contains("ty:"))
            {
                int start = text.IndexOf("ty:") + 3;
                string substr = text.Substring(start, text.Length - start);
                int end = substr.IndexOf("*");
                string term = substr.Substring(0, end);
                Console.WriteLine("Texture type: " + term);
            }
            if (text.Contains("tg:"))
            {
                int start = text.IndexOf("tg:") + 3;
                string substr = text.Substring(start, text.Length - start);
                int end = substr.IndexOf("*");
                string term = substr.Substring(0, end);
                Console.WriteLine("SceneObject to which texture has been added: " + term);
                int attempt;
                if (int.TryParse(term, out attempt))
                {
                    //put the texture on the sceneObject... but how?!
                    //since the sceneObject has already been added to control, we can add it here
                    //So we need to find the gameObject with ID whatever and set its texture
                    try
                    {
                        SceneObject.FindByID(attempt).material.diffuseMap = texture;
                        Console.WriteLine("Set sceneObject's diffuse map to " + texture.Path);
                        //soID = attempt;
                        //OnSceneObject = true;
                    }
                    catch (NullReferenceException nre)
                    {
                        Console.WriteLine("WARNING: Failed to apply texture to sceneObject: " + nre.Message);
                        //OnSceneObject = false;
                    }
                }
            }
            else {
                Console.WriteLine("WARNING: No sceneObject data available for this texture.");
            }

            //bool alreadyLoaded = false;
            ////do we already have this texture loaded?
            //Texture analogue;
            //foreach (var tex in Control.AllTextures)
            //{
            //    if (tex.ID == texture.ID) {
            //        alreadyLoaded = true;
            //        analogue = tex;
            //        if (OnSceneObject == true)
            //            Control.AllSceneObjects[soID].material.diffuseMap = analogue;
            //        break;
            //    }
            //}
            //if (alreadyLoaded != true)
            //{
            //    Control.AllTextures.Add(texture);
            //}

            return texture;

        }




    }


}
