#ifndef MODEL_H
#define MODEL_H

 
#include <GL/glew.h>

#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
//SOIL
#include <SOIL.h>

#include <assimp/Importer.hpp>
#include <assimp/scene.h>
#include <assimp/postprocess.h>

#include "Mesh.h"
#include "Shader.h"

#include <string>
#include <fstream>
#include <sstream>
#include <iostream>
#include <map>
#include <vector>
using namespace std;

//unsigned int TextureFromFile(const char *path, const string &directory, bool gamma = false);
GLuint CreateTexture(const char * path, const string &directory, GLuint wrap, GLuint filter);

extern vector<Texture> LoadedTextures;

 
class Model
{
public:
	/*  Model Data */
	vector<Texture> textures_loaded;	// stores all the textures loaded so far, optimization to make sure textures aren't loaded more than once.
	vector<Mesh> meshes;
	string directory;
	bool gammaCorrection;
	string name;
	string filePath;

	/*  Functions   */
	//param-less ctor for convenience
	Model() {}
	// constructor, expects a filepath to a 3D model.
	Model(string const &path, bool gamma = false) : gammaCorrection(gamma)
	{
		loadModel(path);
	}

	//for initializing a model created with empty ctor
	void Load(string const &path, bool gamma = false)
	{
		gammaCorrection = gamma;
		loadModel(path);
	}


	// draws the model, and thus all its meshes
	void Draw(Shader shader)
	{
		for (unsigned int i = 0; i < meshes.size(); i++)
			meshes[i].Draw(shader);
	}
	void Draw(Shader * shader)
	{
		for (unsigned int i = 0; i < meshes.size(); i++)
			meshes[i].Draw(shader);
	}

private:
	/*  Functions   */
	// loads a model with supported ASSIMP extensions from file and stores the resulting meshes in the meshes vector.
	void loadModel(string const &path)
	{
		//we must now process these as sceneObjects, with a parent scene object
	//	cout << "Here comes the PAIN" << endl;
		// read file via ASSIMP
		Assimp::Importer importer;
		const aiScene* scene = importer.ReadFile(path, aiProcess_Triangulate | aiProcess_FlipUVs | aiProcess_CalcTangentSpace);
		// check for errors
		if (!scene || scene->mFlags & AI_SCENE_FLAGS_INCOMPLETE || !scene->mRootNode) // if is Not Zero
		{
			cout << "ERROR::ASSIMP:: " << importer.GetErrorString() << endl;
			return;
		}
		// retrieve the directory path of the filepath
		directory = path.substr(0, path.find_last_of('\\'));

		int siz = path.find_last_of('.') - path.find_last_of('\\');
		string nam = path.substr(path.find_last_of('\\') + 1, siz - 1);
		filePath = path;

		name = nam;

		//cout << "Directory: " << directory << endl;

		// process ASSIMP's root node recursively
		processNode(scene->mRootNode, scene);
	}

	// processes a node in a recursive fashion. Processes each individual mesh located at the node and repeats this process on its children nodes (if any).
	void processNode(aiNode *node, const aiScene *scene)
	{
		// process each mesh located at the current node
		//Add in SceneObjects
		for (unsigned int i = 0; i < node->mNumMeshes; i++)
		{
			// the node object only contains indices to index the actual objects in the scene. 
			// the scene contains all the data, node is just to keep stuff organized (like relations between nodes).
			aiMesh* mesh = scene->mMeshes[node->mMeshes[i]];
			Mesh m = processMesh(mesh, scene);
			meshes.push_back(m);

			/*aiString n = mesh->mName;
			const char * name = n.C_Str();*/
//		SceneObject objecto(name, m);

		}
		// after we've processed all of the meshes (if any) we then recursively process each of the children nodes
		for (unsigned int i = 0; i < node->mNumChildren; i++)
		{
			processNode(node->mChildren[i], scene);
		}

	}

	Mesh processMesh(aiMesh *mesh, const aiScene *scene)
	{
		// data to fill
		vector<Vertex> vertices;
		vector<unsigned int> indices;
		vector<Texture> textures;

		// Walk through each of the mesh's vertices
		for (unsigned int i = 0; i < mesh->mNumVertices; i++)
		{
			Vertex vertex;
			glm::vec3 vector; // we declare a placeholder vector since assimp uses its own vector class that doesn't directly convert to glm's vec3 class so we transfer the data to this placeholder glm::vec3 first.
							  // positions
			vector.x = mesh->mVertices[i].x;
			vector.y = mesh->mVertices[i].y;
			vector.z = mesh->mVertices[i].z;
			vertex.Position = vector;
			// normals
			vector.x = mesh->mNormals[i].x;
			vector.y = mesh->mNormals[i].y;
			vector.z = mesh->mNormals[i].z;
			vertex.Normal = vector;
			// texture coordinates
			if (mesh->mTextureCoords[0]) // does the mesh contain texture coordinates?
			{
				glm::vec2 vec;
				// a vertex can contain up to 8 different texture coordinates. We thus make the assumption that we won't 
				// use models where a vertex can have multiple texture coordinates so we always take the first set (0).
				vec.x = mesh->mTextureCoords[0][i].x;
				vec.y = mesh->mTextureCoords[0][i].y;
				vertex.TexCoords = vec;
			}
			else 
			{
				vertex.TexCoords = glm::vec2(0.0f, 0.0f);
			}
				
			//If there aren't any tangents or bitangents, the importer will crash


			// tangent
			vector.x = mesh->mTangents[i].x;
			vector.y = mesh->mTangents[i].y;
			vector.z = mesh->mTangents[i].z;
			vertex.Tangent = vector;
			// bitangent
			vector.x = mesh->mBitangents[i].x;
			vector.y = mesh->mBitangents[i].y;
			vector.z = mesh->mBitangents[i].z;
			vertex.Bitangent = vector;
			vertices.push_back(vertex);
		}
		// now wak through each of the mesh's faces (a face is a mesh its triangle) and retrieve the corresponding vertex indices.
		for (unsigned int i = 0; i < mesh->mNumFaces; i++)
		{
			aiFace face = mesh->mFaces[i];
			// retrieve all indices of the face and store them in the indices vector
			for (unsigned int j = 0; j < face.mNumIndices; j++)
				indices.push_back(face.mIndices[j]);
		}
		// process materials
		aiMaterial* material = scene->mMaterials[mesh->mMaterialIndex];
		// we assume a convention for sampler names in the shaders. Each diffuse texture should be named
		// as 'texture_diffuseN' where N is a sequential number ranging from 1 to MAX_SAMPLER_NUMBER. 
		// Same applies to other texture as the following list summarizes:
		// diffuse: texture_diffuseN
		// specular: texture_specularN
		// normal: texture_normalN

		// 1. diffuse maps
		vector<Texture> diffuseMaps = loadMaterialTextures(material, aiTextureType_DIFFUSE, "texture_diffuse");
		textures.insert(textures.end(), diffuseMaps.begin(), diffuseMaps.end());
		// 2. specular maps
		vector<Texture> specularMaps = loadMaterialTextures(material, aiTextureType_SPECULAR, "texture_specular");
		textures.insert(textures.end(), specularMaps.begin(), specularMaps.end());
		// 3. normal maps
		std::vector<Texture> normalMaps = loadMaterialTextures(material, aiTextureType_HEIGHT, "texture_normal");
		textures.insert(textures.end(), normalMaps.begin(), normalMaps.end());
		// 4. height maps
		std::vector<Texture> heightMaps = loadMaterialTextures(material, aiTextureType_AMBIENT, "texture_height");
		textures.insert(textures.end(), heightMaps.begin(), heightMaps.end());

		// return a mesh object created from the extracted mesh data
		Mesh m(vertices, indices, textures);
		m.Name = mesh->mName.C_Str();
		/*aiString as = mesh->mName;*/
		std::string nem = m.Name;
		
		//get the color of the material
		aiColor4D dcolor(0.0f, 0.0f, 0.0f, 0.0f);
		aiGetMaterialColor(material, AI_MATKEY_COLOR_DIFFUSE, &dcolor);
		aiColor4D scolor(0.0f, 0.0f, 0.0f, 0.0f);
		aiGetMaterialColor(material, AI_MATKEY_COLOR_SPECULAR, &scolor);
		
		m.diffuseColor = glm::vec4(dcolor.r, dcolor.g, dcolor.b, dcolor.a);
		m.specularColor = glm::vec4(scolor.r, scolor.g, scolor.b, scolor.a);

		cout << "***NEW MESH***" << endl << "Mesh: " << m.Name
			<< endl << "texture count: " << m.textures.size() << endl
			<< "diffuse color: " << m.diffuseColor.x << ", " << m.diffuseColor.y << ", " << m.diffuseColor.z << endl;


	/*	return Mesh(vertices, indices, textures);*/
		return m;
	}

	// checks all material textures of a given type and loads the textures if they're not loaded yet.
	// the required info is returned as a Texture struct.
	vector<Texture> loadMaterialTextures(aiMaterial *mat, aiTextureType type, string typeName)
	{
		vector<Texture> textures;
		for (unsigned int i = 0; i < mat->GetTextureCount(type); i++)
		{
			aiString str;
			mat->GetTexture(type, i, &str);

			//This must also check LoadedTextures for the presence of the texture

			// check if texture was loaded before and if so, continue to next iteration: skip loading a new texture
			bool skip = false;
			for (unsigned int j = 0; j < textures_loaded.size(); j++)
			{
				if (std::strcmp(textures_loaded[j].path.data(), str.C_Str()) == 0)
				{
					textures.push_back(textures_loaded[j]);
					skip = true; // a texture with the same filepath has already been loaded, continue to next one. (optimization)
					break;
				}
			}
			if (!skip)
			{   // if texture hasn't been loaded already, load it
				Texture texture;
			//	texture.id = TextureFromFile(str.C_Str(), this->directory);
				//example use: texture = CreateTexture(iDir, GL_REPEAT, GL_LINEAR);
				texture.id = CreateTexture(str.C_Str(), this->directory, GL_REPEAT, GL_LINEAR);
				texture.type = typeName;
				texture.path = str.C_Str();//May as well be the name of the texture- does not include the directory
				 // store it as texture loaded for entire model, to ensure we won't unnecesery load duplicate textures.
				//Store it in a place where it can be accessed by other models, if need be

				//try c
				/*const char * test = "123";
				int testInt = 0;
				sscanf(test, "%d", &testInt);*/

				/*size_t sz = LoadedTextures.size();
				string ss = std::to_string(sz);
				const char * mid = ss.c_str();
				int indx = 0;
				sscanf(mid, "%d", &indx);*/
				//texture.index = indx;
				texture.index = LoadedTextures.size();
				LoadedTextures.push_back(texture);
				textures.push_back(texture);
				textures_loaded.push_back(texture);

				cout << "Created texture!" << endl;
			}
			else {
				cout << "C++: SKIPPING TEXTURE" << endl;
			}
		}
		return textures;
	}
};


//unsigned int TextureFromFile(const char *path, const string &directory, bool gamma)
//{
//	string filename = string(path);
//	filename = directory + '/' + filename;
//
//	unsigned int textureID;
//	glGenTextures(1, &textureID);
//
//	int width, height, nrComponents;
//	unsigned char *data = stbi_load(filename.c_str(), &width, &height, &nrComponents, 0);
//	if (data)
//	{
//		GLenum format;
//		if (nrComponents == 1)
//			format = GL_RED;
//		else if (nrComponents == 3)
//			format = GL_RGB;
//		else if (nrComponents == 4)
//			format = GL_RGBA;
//
//		glBindTexture(GL_TEXTURE_2D, textureID);
//		glTexImage2D(GL_TEXTURE_2D, 0, format, width, height, 0, format, GL_UNSIGNED_BYTE, data);
//		glGenerateMipmap(GL_TEXTURE_2D);
//
//		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
//		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
//		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR);
//		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
//
//		stbi_image_free(data);
//	}
//	else
//	{
//		std::cout << "Texture failed to load at path: " << path << std::endl;
//		stbi_image_free(data);
//	}
//
//	return textureID;
//}

GLuint CreateTexture(const char * path, const string &directory, GLuint wrap, GLuint filter)
{
	string filename = string(path);
	filename = directory + '/' + filename;
	const char * dir = filename.c_str();

	GLuint texture;
	glGenTextures(1, &texture);
	glBindTexture(GL_TEXTURE_2D, texture); // All upcoming GL_TEXTURE_2D operations now have effect on this texture object
										   // Set the texture wrapping parameters
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, wrap);	// Set texture wrapping to GL_REPEAT (usually basic wrapping method)
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, wrap);
	// Set texture filtering parameters
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, filter);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, filter);
	// Load image, create texture and generate mipmaps

	int w, h;//width, height
	//unsigned char* image = SOIL_load_image(iDir, &w, &h, 0, SOIL_LOAD_RGB);
	unsigned char* image;
	try
	{
		image = SOIL_load_image(dir, &w, &h, 0, SOIL_LOAD_RGBA);
	}
	catch (const std::exception&)
	{
		std::cout << "Failed to load texture: " << directory << std::endl;
		return 0;
	}

	glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, w, h, 0, GL_RGBA, GL_UNSIGNED_BYTE, image);
	glGenerateMipmap(GL_TEXTURE_2D);
	SOIL_free_image_data(image);
	glBindTexture(GL_TEXTURE_2D, 0); // Unbind texture when done, so we won't accidentily mess up our texture.

	return texture;
}
#endif