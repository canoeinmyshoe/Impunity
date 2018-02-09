#pragma warning(disable:4996)

#pragma region Headers
#include <iostream>

// GLEW
#define GLEW_STATIC
#include <GL/glew.h>

// GLFW
#include <GLFW/glfw3.h>

//SOIL
#include <SOIL.h>

//openGL math lib
#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <glm/gtc/type_ptr.hpp>

//Assimp
#include <assimp/Importer.hpp>
#include <assimp/scene.h>
#include <assimp/postprocess.h>

//windows
#include <Windows.h>

//basic math
#include <math.h>


#include "Shader.h"
#include "Camera.h"
#include "Main.h"
#include "Mesh.h"
#include "Model.h"
#include "Lighting.h"

using namespace std;
#pragma endregion

#pragma region Prototypes
//prototypes
void key_callback(GLFWwindow* window, int key, int scancode, int action, int mode);
string exeDirectory();
GLuint CreateTexture(const char * iDir, GLuint wrap, GLuint filter);
GLuint CreateTextureX(const char * path, const string &directory, GLuint wrap, GLuint filter);
void mouse_callback(GLFWwindow* window, double xpos, double ypos);
void processInput(GLFWwindow *window);
void framebuffer_size_callback(GLFWwindow* window, int width, int height);
void scroll_callback(GLFWwindow* window, double xoffset, double yoffset);
static wchar_t * strTowchar(string str);
void DrawDebuggingLights();
//SceneObject InstantiateSceneObject();
#pragma endregion

#pragma region Time variables
// timing
float deltaTime = 0.0f;	// time between current frame and last frame
float lastFrame = 0.0f;
#pragma endregion

#pragma region Global Matrices
//global matrices
glm::mat4 ProjectionMatrix;
glm::mat4 ViewMatrix;
#pragma endregion

#pragma region Scene Variables

GLfloat screenWidth = 1000.0f;
GLfloat screenHeight = 800.0f;

#pragma region Camera
Camera * camera = new Camera(glm::vec3(0.0f, 0.0f, 3.0f));
float lastX = screenWidth / 2.0f;
float lastY = screenHeight / 2.0f;
bool firstMouse = true;
#pragma endregion



GLFWwindow * sceneWindow;
vector<PointLight> ScenePointLights;
vector<DirectionalLight> SceneDirectionalLights;
vector<SpotLight> SceneSpotLights;

#include "SceneObject.h"
//do we want a vector of sceneObjects or is there some other data structure we can use?

vector<Model> LoadedModels;
vector<Shader*> LoadedShaders;
vector<Texture> LoadedTextures;

GLfloat WorldAmbientLight;
vector<SceneObject> AllSceneObjects;
#pragma endregion

#pragma region Example Code Variables
// Window dimensions - Example Code
const GLuint WIDTH = 800, HEIGHT = 600;
GLuint VAO, VBO, EBO;
Shader * myShader;
GLuint texture;
GLuint texture2;
GLuint containerVAO, lightVAO;
glm::vec3 cubePositions[] = {
	glm::vec3(0.0f, 0.0f, 0.0f),
	glm::vec3(2.0f, 5.0f, -15.0f),
	glm::vec3(-1.5f, -2.2f, -2.5f),
	glm::vec3(-3.8f, -2.0f, -12.3f),
	glm::vec3(2.4f, -0.4f, -3.5f),
	glm::vec3(-1.7f, 3.0f, -7.5f),
	glm::vec3(1.3f, -2.0f, -2.5f),
	glm::vec3(1.5f, 2.0f, -2.5f),
	glm::vec3(1.5f, 0.2f, -1.5f),
	glm::vec3(-1.3f, 1.0f, -1.5f)
};
glm::vec3 lightPos(1.2f, 1.0f, 2.0f);
Shader * lightingShader;
Shader * lampShader;
Shader * treeShader;
Model * treeModel;
#pragma endregion



 int main() 
 {
	 return 0;
 }

extern "C"
{
	//and NOW we will change the OFFSET of the texture!
	__declspec(dllexport) int SetMaterialOffset(int sceneObjectID, float xOffset, float yOffset) 
	{
		if (sceneObjectID >= AllSceneObjects.size())
			return 1;

	//	cout << "C++: setting material offset of sceneObject " << AllSceneObjects[sceneObjectID].ID << endl;

		AllSceneObjects[sceneObjectID].material.xOffset = xOffset;
		AllSceneObjects[sceneObjectID].material.yOffset = yOffset;

		//Does it actually look any different? You will need to move it continually for a good test!

		return 0;
	}

	//We need a method to change the tiling in a SceneObject's material
	__declspec(dllexport) int SetMaterialTiling(int sceneObjectID, float xtiling, float ytiling) 
	{
		if (sceneObjectID >= AllSceneObjects.size())
			return 1;

		cout << "C++: Setting tile of a sceneObject" << endl;
		AllSceneObjects[sceneObjectID].material.xTiling = xtiling;
		AllSceneObjects[sceneObjectID].material.yTiling = ytiling;
		return 0;
	}
	__declspec(dllexport) int SwapDiffuseMap(int sceneObjectID, int textureIndex) 
	{
		//first , check if the index is out of range
		if (sceneObjectID >= AllSceneObjects.size())
			return 1;
		if (textureIndex >= LoadedTextures.size())
			return 1;
		if (AllSceneObjects[sceneObjectID].meshes.size() == 0)
			return 1;
		if (AllSceneObjects[sceneObjectID].meshes[0].textures.size() > 0) 
		{
			cout << "Removing pre-existing diffuse map." << endl;
			//add all the textures to a new vector. clear the old vector. Then add them back
			//IF the texture type isn't a diffuse map
			vector<Texture> tempTextures;
			for each (Texture tex in AllSceneObjects[sceneObjectID].meshes[0].textures)
			{
				tempTextures.push_back(tex);
			}
			AllSceneObjects[sceneObjectID].meshes[0].textures.clear();
			string comp = "texture_diffuse";
			for each (Texture tex in tempTextures)
			{
				if (tex.type != comp)
					AllSceneObjects[sceneObjectID].meshes[0].textures.push_back(tex);
			}
			//This should get rid of the pre-existing diffuse map while preserving other types
		}

		//now point the mesh to the pre-existing texture in the LoadedTextures array
		//we already checked if it's out of range
		cout << "Texture to swap: " << LoadedTextures[textureIndex].path << endl;
		cout << "SceneObject to give texture: " << AllSceneObjects[sceneObjectID].Name << endl;
		cout << "SceneObjectID: " << sceneObjectID << endl;

		Texture texxx = LoadedTextures[textureIndex];
		AllSceneObjects[sceneObjectID].meshes[0].textures.push_back(texxx);

		AllSceneObjects[sceneObjectID].material.useExtTexture = true;
		AllSceneObjects[sceneObjectID].material.extDiffuseIndex =textureIndex;


		cout << "~ ~ ~ ~ ~ ~ C++ DIFFUSE SWAP ASSURANCE TEST ~ ~ ~ ~ ~ ~ ~ ~ " << endl
			<< " texture path: " << AllSceneObjects[sceneObjectID].meshes[0].textures[0].path << endl
			<< " texture index: " << AllSceneObjects[sceneObjectID].meshes[0].textures[0].index << endl;



		return 0;//1 for failure to swap
	}
	__declspec(dllexport) int LoadTextureFromDir(char * textureName, char* textureDirectory, char * stringBuilder, int strLen) 
	{
		//c++ will need a string builder to tell c# the details of the loaded texture
		//we use the name to compare with pre-existing textures that have already been loaded
		string texPath = textureName;
	//	cout << "C++ name of texture: " << texPath << endl;

		bool skip = false;
		for (size_t i = 0; i < LoadedTextures.size(); i++)
		{
			if (LoadedTextures[i].path == texPath) 
			{
				//the texture has already been loaded
		//		cout << "The texture has already been loaded";
				skip = true;
				break;
			}
		}
		string returnData = "{";

		if (skip == true) 
		{
			returnData += "already-loaded}";
			strcpy(stringBuilder, returnData.c_str());
			return 0;
		}

		//else...
		string fullPath = std::string(textureDirectory) + texPath;
		const char * iDir = fullPath.c_str();
		GLuint textureID = CreateTexture(textureDirectory, GL_REPEAT, GL_LINEAR);
		//GLuint CreateTexture(const char * path, const string &directory, GLuint wrap, GLuint filter)
		//Load this texture the RIGHT way - the way that model does it
		//texture.id = CreateTexture(str.C_Str(), this->directory, GL_REPEAT, GL_LINEAR);

		Texture texture;
		/*texture.id = textureID;*/
		texture.id = CreateTexture(texPath.c_str(), std::string(textureDirectory), GL_REPEAT, GL_LINEAR);
		texture.index = LoadedTextures.size();
		string t_type = "texture_diffuse";
		texture.type = t_type;
		texture.path = texPath;
		LoadedTextures.push_back(texture);

		cout << "~ ~ ~ ~ ~ ~ C++ TEXTURE ASSURANCE TEST ~ ~ ~ ~ ~ ~ ~ ~ " << endl
			<< "PRE texture path: " << texture.path << endl
			<< "PRE texture index: " << texture.index << endl
			<< "POST texture path: " << LoadedTextures[texture.index].path << endl
			<< "POST texrure index: " << LoadedTextures[texture.index].index << endl;


		returnData += "t:" + texture.path + "*i:" + std::to_string(texture.index);
		returnData += "}";

		strcpy(stringBuilder, returnData.c_str());


		return 0;//but if you faill, return 1.... no...
	}
	__declspec(dllexport) int SetTransform(int ID, float px, float py, float pz, float rx, float ry, float rz, float sx, float sy, float sz) 
	{

		if (AllSceneObjects.size() - 1 < ID) 
		{
			return 1;
		}

		AllSceneObjects[ID].transform.position = glm::vec3(px, py, pz);
		AllSceneObjects[ID].transform.rotation = glm::vec3(rx, ry, rz);
		AllSceneObjects[ID].transform.scale = glm::vec3(sx, sy, sz);
		return 0;
	}
	__declspec(dllexport) int CreateSpotLight(float x, float y, float z, float dx, float dy, float dz) 
	{
		int id = SceneSpotLights.size();
		SpotLight sp;
		sp.position = glm::vec3(x, y, z);
		sp.direction = glm::vec3(dx, dy, dz);
		sp.cutOff = 12.5f; //degrees
		sp.outerCutOff = 15.0f;//degrees
		sp.ambient = glm::vec3(0.2f);//needs worldAmbient
		sp.diffuse = glm::vec3(0.5f);
		sp.specular = glm::vec3(1.0f);
		sp.maxDistance = 25.0f;
		SceneSpotLights.push_back(sp);
		return id;
	}
	__declspec(dllexport) int CreateDirectionalLight(float x, float y, float z) 
	{
		int id = SceneDirectionalLights.size();
		DirectionalLight dlight;
		dlight.direction = glm::vec3(x, y, z);
		dlight.ambient = glm::vec3(0.2f);//needs to refelct worldAmbient
		dlight.diffuse = glm::vec3(0.5f);
		dlight.specular = glm::vec3(1.0f);
		SceneDirectionalLights.push_back(dlight);
		return id;
	}
	__declspec(dllexport) int CreatePointLight(float x, float y, float z) 
	{

		//just assume its values are that of a white light. They can always be changed later

		int id = ScenePointLights.size();
		PointLight pointLight;
		pointLight.position = glm::vec3(x, y, z);
		pointLight.constant = 1.0f;
		pointLight.linear = 0.09f;
		pointLight.quadratic = 0.032f;
		pointLight.ambient = glm::vec3(0.2f, 0.2f, 0.2f);//needs to refelct worldAmbient
		pointLight.diffuse = glm::vec3(0.5f, 0.5f, 0.5f);
		pointLight.specular = glm::vec3(1.0f);//This is rather odd...
		pointLight.maxDistance = 10.0f;

		ScenePointLights.push_back(pointLight);
		//cout << "Created pointLight at " << x << ", " << y << ", " << z << endl;
		return id;
	}
	__declspec(dllexport) int LoadModelFromDir(char * modelDirectory, int modelDirLen, char * strIn, int strLen) //const char * modelDirectory
	{
		//In this method, we return the ID and name of all the objects loaded
		//we return the sceneObject data in the form of "0 Name , 1 Name p0, 2 Name p0"
		//where parent is denoted by pX

		//get the amount of loaded textures
		int totalTextures = LoadedTextures.size();

		string returnData = "{";

		string mdir = modelDirectory;
		

		const char * dir = mdir.c_str();

		Model model;
		bool found = false;
		//check to see if the model has been loaded by comparing
		for(int i = 0; i < LoadedModels.size(); i++)
			{

			if (LoadedModels[i].filePath == mdir) 
			{
			//	cout << "ModelDir: " << LoadedModels[i].filePath << "  v.   dir: " << mdir << endl;
				cout << "Model found at " << std::to_string(i) << endl;
				//Use this Model inseated
				model = LoadedModels[i];
				found = true;
				break;
			}
		}
		if (found != true) 
		{
			
			cout << "Model not found. Loading anew." << endl;
			model.Load(dir);
			LoadedModels.push_back(model);
		}


	//	Model newModel(dir);
		//now, just take the meshes from this and assign them to scene objects

		
		SceneObject so(AllSceneObjects.size());
		so.Name = model.name.c_str();
		AllSceneObjects.push_back(so);

		returnData += "n:" + so.Name + "*i:" + std::to_string(so.ID) + ",*";
		//so.usingShader = 0;//index in LoadedShaders[]
//		cout << "Amount of shaders loaded: " << LoadedShaders.size() << endl;
		//by default it gets one of the default shaders
		so.shader = LoadedShaders[0];
		so.shader = LoadedShaders[1];//experimental multilight shader
		so.ShaderNumber = 1;
		cout << "Using MultiLight shader ID " << 1 << endl;

		//Nameless meshes will ruin your day!
		for (unsigned int i = 0; i < model.meshes.size(); i++)//newModel.meshes.size()
		{
			SceneObject soo(AllSceneObjects.size());
			soo.shader = LoadedShaders[0];
			soo.shader = LoadedShaders[1];//experimental multilight shader
			soo.ShaderNumber = 1;
			
			//set up color data
			soo.material.diffuse = glm::vec3(model.meshes[i].diffuseColor.x, model.meshes[i].diffuseColor.y, model.meshes[i].diffuseColor.z);
			soo.material.specular = glm::vec3(model.meshes[i].specularColor.x, model.meshes[i].specularColor.y, model.meshes[i].specularColor.z);
			//debug the colors
			cout << "==================================" << endl;
			cout << "Material color: " << soo.material.diffuse.x << ", " << soo.material.diffuse.y << ", " << soo.material.diffuse.z << endl;
			cout << "==================================" << endl;

			soo.material.xTiling = 1.0f;
			soo.material.yTiling = 1.0f;
			
		//	soo.Name = newModel.meshes[i].Name;
			soo.meshes.push_back(model.meshes[i]);//newModel.meshes[i])
		//	soo.meshes[0].Report();
			
			string nam = so.Name + "-" + std::to_string(i);
			soo.Name = nam.c_str();
			//soo.Name = so.Name + "-" + std::to_string(i);
			returnData += "n:"+ std::string(soo.Name) + "*i:" + std::to_string(soo.ID) + "*p:" + std::to_string(so.ID) + ",*";


			//cout << "C++ SceneObject final name: " << soo.Name << endl;
			//cout << "C++ SceneObject final ID: " << soo.ID << endl;

			//so.Children.push_back(soo);
			AllSceneObjects.push_back(soo);
			//and this is where things go WRONG! This returned "SceneObject". Why would it do that?!
		/*	cout << "~C++ Index assurance result~ " << endl 
				<<"Name: " << AllSceneObjects[soo.ID].Name << endl
				<<"ID: "<<AllSceneObjects[soo.ID].ID << endl;*/
		}

	//	cout << "Child count: " << so.Children.size() << endl;
		if (totalTextures < LoadedTextures.size()) 
		{
			//append something like "t:texture_name_ti:texture_ID" foreach new texture
			int diff = LoadedTextures.size() - totalTextures;
			cout << "Loaded " << diff << " new textures" << endl;
			for (size_t i = totalTextures; i < LoadedTextures.size(); i++)
			{
				returnData += "t:" + LoadedTextures[i].path + "*";
				returnData += "i:" + std::to_string(LoadedTextures[i].index) + ",*";
			}
		}


		returnData += "}";
		strcpy(strIn, returnData.c_str());
		return 0;
	}


	__declspec(dllexport) int InitiateEngine() 
	{

		glfwInit();
		// Set all the required options for GLFW
		glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
		glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 3);
		glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);
		glfwWindowHint(GLFW_RESIZABLE, GL_FALSE);

		// Create a GLFWwindow object that we can use for GLFW's functions
		sceneWindow = glfwCreateWindow(screenWidth, screenHeight, "Scene Window", nullptr, nullptr);
		glfwMakeContextCurrent(sceneWindow);

		// Set the required callback functions
		glfwSetKeyCallback(sceneWindow, key_callback);
		glfwSetCursorPosCallback(sceneWindow, mouse_callback);
		glfwSetScrollCallback(sceneWindow, scroll_callback);

		// GLFW Options
		glfwSetInputMode(sceneWindow, GLFW_CURSOR, GLFW_CURSOR_DISABLED);

		// Set this to true so GLEW knows to use a modern approach to retrieving function pointers and extensions
		glewExperimental = GL_TRUE;
		// Initialize GLEW to setup the OpenGL Function pointers
		glewInit();

		// Define the viewport dimensions
		glViewport(0, 0, screenWidth, screenHeight);

		// OpenGL options
		glEnable(GL_DEPTH_TEST);

		//we calc in initiation because we may be full screen and res will vary
		ProjectionMatrix = glm::perspective(camera->Zoom, (GLfloat)screenWidth / (GLfloat)screenHeight, 0.1f, 1000.0f);





		string d = exeDirectory();
		string vShaderDir = d + "Shaders\\openglexamples\\vertexDiffuse.txt";
		const char * vSource = vShaderDir.c_str();
		cout << "Getting vert shader from dir: " << vShaderDir << endl;
		string fragDir = d + "Shaders\\openglexamples\\fragmentDiffuse.txt";
		const char *fragSource = fragDir.c_str();
		cout << "Getting frag shader from " << fragSource << endl;
		lightingShader = new Shader(vSource, fragSource);

		/*Shader treeShader(vSource, fragSource);*/
		treeShader = new Shader(vSource, fragSource);

		//lamp debugging shader
		vShaderDir = d + "Shaders\\openglexamples\\vertexSimpleLighting.txt";
		vSource = vShaderDir.c_str();
		fragDir = d + "Shaders\\openglexamples\\fragmentLightShader.txt";
		fragSource = fragDir.c_str();
		lampShader = new Shader(vSource, fragSource);

		//Use this section to test if shaders are actually compling/linking
		vShaderDir = d + "Shaders\\defaultShaders\\vertexDefaultSpecular.txt";
		vSource = vShaderDir.c_str();
		fragDir = d + "Shaders\\defaultShaders\\fragmentDefaultSpecular.txt";
		fragSource = fragDir.c_str();
		Shader * defShader = new Shader(vSource, fragSource);
		LoadedShaders.push_back(defShader);

		fragDir = d + "Shaders\\defaultShaders\\fragmentDefaultMultiLight.txt";
		fragSource = fragDir.c_str();
		Shader * defMultiLightShader = new Shader(vSource, fragSource);
		LoadedShaders.push_back(defMultiLightShader);
		//We might need to give these shaders names/ID's other attributes by which to recognize them

		cout << "Finished making shaders." << endl;



		//This seems like a decent place to initiate the model loading process

	/*	string mDir = d + "Models\\tree02\\tree.obj";
		const char * modelPath = mDir.c_str();
		treeModel = new Model(modelPath);*/


		// Set up vertex data (and buffer(s)) and attribute pointers
		GLfloat vertices[] = {
			-0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
			0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
			0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
			0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
			-0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
			-0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,

			-0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
			0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
			0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
			0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
			-0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
			-0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,

			-0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,
			-0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
			-0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
			-0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
			-0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,
			-0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,

			0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,
			0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
			0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
			0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
			0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,
			0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,

			-0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,
			0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,
			0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
			0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
			-0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
			-0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,

			-0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,
			0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,
			0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
			0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
			-0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
			-0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f
		};
		// First, set the container's VAO (and VBO)
		//GLuint VBO, containerVAO;
		glGenVertexArrays(1, &containerVAO);
		glGenBuffers(1, &VBO);

		glBindBuffer(GL_ARRAY_BUFFER, VBO);
		glBufferData(GL_ARRAY_BUFFER, sizeof(vertices), vertices, GL_STATIC_DRAW);

		glBindVertexArray(containerVAO);
		// Position attribute
		glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 6 * sizeof(GLfloat), (GLvoid*)0);
		glEnableVertexAttribArray(0);
		// Normal attribute
		glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 6 * sizeof(GLfloat), (GLvoid*)(3 * sizeof(GLfloat)));
		glEnableVertexAttribArray(1);
		glBindVertexArray(0);

		// Then, we set the light's VAO (VBO stays the same. After all, the vertices are the same for the light object (also a 3D cube))
		//	GLuint lightVAO;
		glGenVertexArrays(1, &lightVAO);
		glBindVertexArray(lightVAO);
		// We only need to bind to the VBO (to link it with glVertexAttribPointer), no need to fill it; the VBO's data already contains all we need.
		glBindBuffer(GL_ARRAY_BUFFER, VBO);
		// Set the vertex attributes (only position data for the lamp))
		glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 6 * sizeof(GLfloat), (GLvoid*)0); // Note that we skip over the normal vectors
		glEnableVertexAttribArray(0);
		glBindVertexArray(0);

		//enable blending for text display
		glEnable(GL_BLEND);
		glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
			

		return 0;
	}
	__declspec(dllexport) int RenderAll() 
	{
		// Calculate deltatime of current frame
		GLfloat currentFrame = glfwGetTime();
		deltaTime = currentFrame - lastFrame;
		lastFrame = currentFrame;

		// Check if any events have been activiated (key pressed, mouse moved etc.) and call corresponding response functions
		glfwPollEvents();
		processInput(sceneWindow);

		// Clear the colorbuffer
		glClearColor(0.1f, 0.1f, 0.1f, 1.0f);
		glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

		//Set Global Matrices
		//ProjectionMatrix = glm::mat4();
		ProjectionMatrix = glm::perspective(camera->Zoom, (GLfloat)screenWidth / (GLfloat)screenHeight, 0.1f, 1000.0f);
		ViewMatrix = camera->GetViewMatrix();



		// Use cooresponding shader when setting uniforms/drawing objects
		lightingShader->Use();

		lightingShader->SetVec3(1.0f, 0.5f, 0.31f, "objectColor");
		lightingShader->SetVec3(1.0f, 1.0f, 1.0f, "lightColor");
		lightingShader->SetVec3(lightPos.x, lightPos.y, lightPos.z, "lightPosition");
		lightingShader->SetVec3(camera->Position.x, camera->Position.y, camera->Position.z, "viewPosition");

		//// Create camera transformations
		//glm::mat4 view;
		//view = camera->GetViewMatrix();
	//	glm::mat4 projection = glm::perspective(camera->Zoom, (GLfloat)screenWidth / (GLfloat)screenHeight, 0.1f, 1000.0f);
		// Get the uniform locations
		lightingShader->setMat4("view", ViewMatrix);
		lightingShader->setMat4("projection", ProjectionMatrix);

		glm::mat4 model;
		///*glUniformMatrix4fv(modelLoc, 1, GL_FALSE, glm::value_ptr(model));*/
		model = glm::translate(model, glm::vec3(8.0f, -10.0f, 1.0f));
		lightingShader->setMat4("model", model);//This doesn't appear to actually translate it

		// Draw the container (using container's vertex attributes)
		glBindVertexArray(containerVAO);


		glDrawArrays(GL_TRIANGLES, 0, 36);
		glBindVertexArray(0);


		//ultimately, we want all these uniforms set by the sceneObject

	/*	treeShader->Use();
		treeShader->SetVec3(0.5f, 0.5f, 0.0, "objectColor");
		treeShader->SetVec3(1.0f, 1.0f, 1.0f, "lightColor");
		treeShader->SetVec3(lightPos.x, lightPos.y, lightPos.z, "lightPosition");*/
	//	treeShader->SetVec3(camera->Position.x, camera->Position.y, camera->Position.z, "viewPosition");


		//Start with transform
		//for starters, everything uses the same projection. There's no need to recalculate each round
	//	treeShader->setMat4("view", ViewMatrix);
	//	treeShader->setMat4("projection", ProjectionMatrix);
	//	glm::mat4 treeMat;
		//model = glm::translate(model, glm::vec3(10.0f, 2.0f, 3.0f));//WHY WONT IT TAKE!!!!!
	//	treeShader->setMat4("model", model);

		//Start off by moving ViewMatrix-setting to its new home in SceneObject

		//temporary flashlight, to be reconfigured in c# at a later time
		if (SceneSpotLights.size() > 0) 
		{
			SceneSpotLights[0].position = camera->Position;
			SceneSpotLights[0].direction = camera->Front;
		}

		for (GLuint i = 0; i < AllSceneObjects.size(); i++)
		{
			//int total = AllSceneObjects.size();
			//float ratio = (float)i / (float)total * (2.0f * 3.14f);
			//float angle = glm::sin(ratio);//point on unit circle
			//							  //get x positions
			//float radius = 10.0f;
			//float x = radius *  glm::cos(angle);
			//float y = radius *  glm::sin(angle);
			//AllSceneObjects[i].transform.position = glm::vec3(x, 0.0, y);

			/*if (i % 3 == 0) 
			{
				AllSceneObjects[i].transform.position = glm::vec3(5.0f * i, 0.0, 0.0);
			}*/
				
			//AllSceneObjects[i].transform.position = glm::vec3(2.0f * i, -1.0f, 0.0);
			
			//AllSceneObjects[i].Draw(treeShader);
			AllSceneObjects[i].Draw();
		}
		//and all the setting of shaders would be done inside the sceneObject class
		//we'll use the SceneObject's draw method instead of the Model's
		//Trust me, there's plenty more 


		
		DrawDebuggingLights();
	

		// Swap the screen buffers
		glfwSwapBuffers(sceneWindow);
		return 0;
	}




#pragma region Practice code
	__declspec(dllexport) int * BuildString(char *str, int len)//GREAT
	{
		//write up to len characters into str
		cout << "C++: Max length of string: " << len << endl;
		string message = "Hello from c++";
		/*
		str = new char[len];*/
		strcpy(str, message.c_str());

		return 0;
	}

	__declspec(dllexport) wchar_t * StringTest(const char * incoming)
	{
		cout << "C++ in: " << incoming << endl;
		static wchar_t weird[] = L"Hello from c++!";
		return weird;
	}

	__declspec(dllexport) int * GiveFloat(float x)
	{
		cout << "C++ float: " << x << endl;

		return 0;
	}

	__declspec(dllexport) int FreeWChar(wchar_t text[])
	{

		delete(text);
		return 0;
	}
	__declspec(dllexport) wchar_t * StringToWChar()
	{
		string returnData = "{Hello from c++";

		returnData += " test";
		returnData += " test2";
		returnData += " test3";
		returnData += " looooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooong test";
		returnData += " test4}";

		//from here below is the entire process of converting a c++ string to a c# consumable static wchar_t *

		//Convert string to wtring
		wstring predata;
		for (unsigned int i = 0; i < returnData.length(); i++)
		{
			predata += wchar_t(returnData[i]);
		}

		//convert wstring to locally allocated  static wchar_t *
		static wchar_t * result = const_cast<wchar_t*>(predata.c_str()); //This line is lopping off all chars after 4!


																		 //convert locally allocated wchar_t* to heap allocated wchar_t*
		size_t wize = wcslen(predata.c_str());
		cout << "C++ size_t: " << wize << endl;
		//now convert the size_t to a const int... easy, right?
		static const int isize = static_cast<int>(wize);
		const int finSize = isize;
		cout << "C++ cast result: " << isize << endl;

		const int s = sizeof(result);
		static wchar_t * array = new wchar_t[isize];


		for (unsigned int i = 0; i < isize; i++)
		{
			array[i] = result[i];
		}

		//try using the method
		//array = strTowchar(returnData);


		//after this is returned, there will be a bunch of ???????'s
		return array;
	}

#pragma region Example Methods from LearnOpenGL.com
	__declspec(dllexport) int ModelLoadingLesson()
	{
		//set up a box and light for reference

		glfwInit();
		// Set all the required options for GLFW
		glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
		glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 3);
		glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);
		glfwWindowHint(GLFW_RESIZABLE, GL_FALSE);

		// Create a GLFWwindow object that we can use for GLFW's functions
		sceneWindow = glfwCreateWindow(screenWidth, screenHeight, "Model Loading Lesson", nullptr, nullptr);
		glfwMakeContextCurrent(sceneWindow);

		// Set the required callback functions
		glfwSetKeyCallback(sceneWindow, key_callback);
		glfwSetCursorPosCallback(sceneWindow, mouse_callback);
		glfwSetScrollCallback(sceneWindow, scroll_callback);

		// GLFW Options
		glfwSetInputMode(sceneWindow, GLFW_CURSOR, GLFW_CURSOR_DISABLED);

		// Set this to true so GLEW knows to use a modern approach to retrieving function pointers and extensions
		glewExperimental = GL_TRUE;
		// Initialize GLEW to setup the OpenGL Function pointers
		glewInit();

		// Define the viewport dimensions
		glViewport(0, 0, screenWidth, screenHeight);

		// OpenGL options
		glEnable(GL_DEPTH_TEST);

		string d = exeDirectory();
		string vShaderDir = d + "Shaders\\openglexamples\\vertexDiffuse.txt";
		const char * vSource = vShaderDir.c_str();
		cout << "Getting vert shader from dir: " << vShaderDir << endl;
		string fragDir = d + "Shaders\\openglexamples\\fragmentDiffuse.txt";
		const char *fragSource = fragDir.c_str();
		cout << "Getting frag shader from " << fragSource << endl;
		lightingShader = new Shader(vSource, fragSource);

		Shader treeShader(vSource, fragSource);

		//lamp debugging shader
		vShaderDir = d + "Shaders\\openglexamples\\vertexSimpleLighting.txt";
		vSource = vShaderDir.c_str();
		fragDir = d + "Shaders\\openglexamples\\fragmentLightShader.txt";
		fragSource = fragDir.c_str();
		lampShader = new Shader(vSource, fragSource);

		//This seems like a decent place to initiate the model loading process

		string mDir = d + "Models\\tree02\\tree.obj";
		const char * modelPath = mDir.c_str();
		Model treeModel(modelPath);


		// Set up vertex data (and buffer(s)) and attribute pointers
		GLfloat vertices[] = {
			-0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
			0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
			0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
			0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
			-0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
			-0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,

			-0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
			0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
			0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
			0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
			-0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
			-0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,

			-0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,
			-0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
			-0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
			-0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
			-0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,
			-0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,

			0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,
			0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
			0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
			0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
			0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,
			0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,

			-0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,
			0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,
			0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
			0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
			-0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
			-0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,

			-0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,
			0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,
			0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
			0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
			-0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
			-0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f
		};
		// First, set the container's VAO (and VBO)
		//GLuint VBO, containerVAO;
		glGenVertexArrays(1, &containerVAO);
		glGenBuffers(1, &VBO);

		glBindBuffer(GL_ARRAY_BUFFER, VBO);
		glBufferData(GL_ARRAY_BUFFER, sizeof(vertices), vertices, GL_STATIC_DRAW);

		glBindVertexArray(containerVAO);
		// Position attribute
		glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 6 * sizeof(GLfloat), (GLvoid*)0);
		glEnableVertexAttribArray(0);
		// Normal attribute
		glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 6 * sizeof(GLfloat), (GLvoid*)(3 * sizeof(GLfloat)));
		glEnableVertexAttribArray(1);
		glBindVertexArray(0);

		// Then, we set the light's VAO (VBO stays the same. After all, the vertices are the same for the light object (also a 3D cube))
		//	GLuint lightVAO;
		glGenVertexArrays(1, &lightVAO);
		glBindVertexArray(lightVAO);
		// We only need to bind to the VBO (to link it with glVertexAttribPointer), no need to fill it; the VBO's data already contains all we need.
		glBindBuffer(GL_ARRAY_BUFFER, VBO);
		// Set the vertex attributes (only position data for the lamp))
		glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 6 * sizeof(GLfloat), (GLvoid*)0); // Note that we skip over the normal vectors
		glEnableVertexAttribArray(0);
		glBindVertexArray(0);


		// Game loop
		while (!glfwWindowShouldClose(sceneWindow))
		{
			// Calculate deltatime of current frame
			GLfloat currentFrame = glfwGetTime();
			deltaTime = currentFrame - lastFrame;
			lastFrame = currentFrame;

			// Check if any events have been activiated (key pressed, mouse moved etc.) and call corresponding response functions
			glfwPollEvents();
			processInput(sceneWindow);

			// Clear the colorbuffer
			glClearColor(0.1f, 0.1f, 0.1f, 1.0f);
			glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

			// Use cooresponding shader when setting uniforms/drawing objects
			lightingShader->Use();

			lightingShader->SetVec3(1.0f, 0.5f, 0.31f, "objectColor");
			lightingShader->SetVec3(1.0f, 1.0f, 1.0f, "lightColor");
			lightingShader->SetVec3(lightPos.x, lightPos.y, lightPos.z, "lightPosition");

			// Create camera transformations
			glm::mat4 view;
			view = camera->GetViewMatrix();
			glm::mat4 projection = glm::perspective(camera->Zoom, (GLfloat)WIDTH / (GLfloat)HEIGHT, 0.1f, 100.0f);
			// Get the uniform locations
			lightingShader->setMat4("view", view);
			lightingShader->setMat4("projection", projection);

			// Draw the container (using container's vertex attributes)
			glBindVertexArray(containerVAO);
			glm::mat4 model;
			/*glUniformMatrix4fv(modelLoc, 1, GL_FALSE, glm::value_ptr(model));*/
			glm::translate(model, glm::vec3(3.0f));
			lightingShader->setMat4("model", model);


			glDrawArrays(GL_TRIANGLES, 0, 36);
			glBindVertexArray(0);


			treeShader.Use();
			treeShader.SetVec3(0.5f, 0.5f, 0.0, "objectColor");
			treeShader.SetVec3(1.0f, 1.0f, 1.0f, "lightColor");
			treeShader.SetVec3(lightPos.x, lightPos.y, lightPos.z, "lightPosition");
			treeShader.setMat4("view", view);
			treeShader.setMat4("projection", projection);
			glm::mat4 treeMat;
			glm::translate(treeMat, glm::vec3(1.0f, 1.0f, 1.0f));
			treeShader.setMat4("model", model);
			treeModel.Draw(treeShader);





			// Also draw the lamp object, again binding the appropriate shader
			lampShader->Use();
			lampShader->setMat4("view", view);
			lampShader->setMat4("projection", projection);

			model = glm::mat4();
			model = glm::translate(model, lightPos);
			model = glm::scale(model, glm::vec3(0.05f)); // Make it a smaller cube
			lampShader->setMat4("model", model);

			// Draw the light object (using light's vertex attributes)
			glBindVertexArray(lightVAO);
			glDrawArrays(GL_TRIANGLES, 0, 36);
			glBindVertexArray(0);

			// Swap the screen buffers
			glfwSwapBuffers(sceneWindow);
		}

		// Terminate GLFW, clearing any resources allocated by GLFW.
		glfwTerminate();
		return 3;
	}
	__declspec(dllexport) int EntireMultilightLesson()
	{
		glfwInit();
		// Set all the required options for GLFW
		glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
		glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 3);
		glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);
		glfwWindowHint(GLFW_RESIZABLE, GL_FALSE);

		// Create a GLFWwindow object that we can use for GLFW's functions
		sceneWindow = glfwCreateWindow(screenWidth, screenHeight, "Specular Map With Lighting", nullptr, nullptr);
		glfwMakeContextCurrent(sceneWindow);

		// Set the required callback functions
		glfwSetKeyCallback(sceneWindow, key_callback);
		glfwSetCursorPosCallback(sceneWindow, mouse_callback);
		glfwSetScrollCallback(sceneWindow, scroll_callback);

		// GLFW Options
		glfwSetInputMode(sceneWindow, GLFW_CURSOR, GLFW_CURSOR_DISABLED);

		// Set this to true so GLEW knows to use a modern approach to retrieving function pointers and extensions
		glewExperimental = GL_TRUE;
		// Initialize GLEW to setup the OpenGL Function pointers
		glewInit();

		// Define the viewport dimensions
		glViewport(0, 0, screenWidth, screenHeight);

		// OpenGL options
		glEnable(GL_DEPTH_TEST);

		//create new shaders by copying from these

		string d = exeDirectory();
		string vShaderDir = d + "Shaders\\openglexamples\\vertexSpecularTextured.txt";
		const char * vSource = vShaderDir.c_str();
		cout << "Getting vert shader from dir: " << vShaderDir << endl;
		string fragDir = d + "Shaders\\openglexamples\\fragmentMultilight.txt";
		const char *fragSource = fragDir.c_str();
		cout << "Getting frag shader from " << fragSource << endl;
		lightingShader = new Shader(vSource, fragSource);

		//lamp debugging shader
		vShaderDir = d + "Shaders\\openglexamples\\vertexSimpleLighting.txt";
		vSource = vShaderDir.c_str();
		fragDir = d + "Shaders\\openglexamples\\fragmentLightShader.txt";
		fragSource = fragDir.c_str();
		lampShader = new Shader(vSource, fragSource);


		// Set up vertex data (and buffer(s)) and attribute pointers
		float vertices[] = {
			// positions          // normals           // texture coords
			-0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,
			0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 0.0f,
			0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
			0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
			-0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 1.0f,
			-0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,

			-0.5f, -0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   0.0f, 0.0f,
			0.5f, -0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   1.0f, 0.0f,
			0.5f,  0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   1.0f, 1.0f,
			0.5f,  0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   1.0f, 1.0f,
			-0.5f,  0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   0.0f, 1.0f,
			-0.5f, -0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   0.0f, 0.0f,

			-0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
			-0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
			-0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
			-0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
			-0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
			-0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,

			0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
			0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
			0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
			0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
			0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
			0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,

			-0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,
			0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 1.0f,
			0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
			0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
			-0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 0.0f,
			-0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,

			-0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f,
			0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 1.0f,
			0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
			0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
			-0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 0.0f,
			-0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f
		};
		// First, set the container's VAO (and VBO)
		//GLuint VBO, containerVAO;
		glGenVertexArrays(1, &containerVAO);
		glGenBuffers(1, &VBO);

		glBindBuffer(GL_ARRAY_BUFFER, VBO);
		glBufferData(GL_ARRAY_BUFFER, sizeof(vertices), vertices, GL_STATIC_DRAW);

		glBindVertexArray(containerVAO);
		// Position attribute
		glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 8 * sizeof(GLfloat), (GLvoid*)0);
		glEnableVertexAttribArray(0);
		// Normal attribute
		glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 8 * sizeof(GLfloat), (GLvoid*)(3 * sizeof(GLfloat)));
		glEnableVertexAttribArray(1);
		//Texture coordinate attribute
		glVertexAttribPointer(2, 2, GL_FLOAT, GL_FALSE, 8 * sizeof(GLfloat), (GLvoid*)(6 * sizeof(GLfloat)));
		glEnableVertexAttribArray(2);

		glBindVertexArray(0);





		// Then, we set the light's VAO (VBO stays the same. After all, the vertices are the same for the light object (also a 3D cube))
		//	GLuint lightVAO;
		glGenVertexArrays(1, &lightVAO);
		glBindVertexArray(lightVAO);
		// We only need to bind to the VBO (to link it with glVertexAttribPointer), no need to fill it; the VBO's data already contains all we need.
		glBindBuffer(GL_ARRAY_BUFFER, VBO);
		// Set the vertex attributes (only position data for the lamp))
		glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 8 * sizeof(GLfloat), (GLvoid*)0); // Note that we skip over the normal vectors
		glEnableVertexAttribArray(0);
		glBindVertexArray(0);

		glBindBuffer(GL_ARRAY_BUFFER, 0);


		//LOAD THE DIFFUSEMAP!
		string imaged = d + "textures\\container2.png";
		const char *iDir = imaged.c_str();

		// Load and create a texture 

		texture = CreateTexture(iDir, GL_REPEAT, GL_LINEAR);

		string hap = d + "textures\\container2_specular.png";
		iDir = hap.c_str();

		texture2 = CreateTexture(iDir, GL_REPEAT, GL_LINEAR);

		//cout << "Got texture from: " << iDir << endl;
		//enable blending for text display
		glEnable(GL_BLEND);
		glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);

		glm::vec3 pointLightPositions[] = {
			glm::vec3(-2.0f, 2.0f, 4.0f),
			glm::vec3(2.0f, -2.0f, 0.0),
			glm::vec3(0.0, 3.0f, -1.0f),
			glm::vec3(-1.0f, -1.0f, -2.0f)
		};
		//use these point light positions
		//GLfloat pointLightPositions[] = {
		//-2.0f, 2.0f, 4.0f, //1
		//2.0f, -2.0f, 0.0, //2
		//0.0, 3.0f, -1.0f, //3
		//-1.0f, -1.0f, -2.0f
		//};

		//Down in the renda' loop, we'll need to set a multitude of uniforms in the shada's
		while (!glfwWindowShouldClose(sceneWindow))
		{
			// Calculate deltatime of current frame
			GLfloat currentFrame = glfwGetTime();
			deltaTime = currentFrame - lastFrame;
			lastFrame = currentFrame;

			// Check if any events have been activiated (key pressed, mouse moved etc.) and call corresponding response functions
			glfwPollEvents();
			processInput(sceneWindow);

			// Clear the colorbuffer
			glClearColor(0.1f, 0.1f, 0.1f, 1.0f);
			glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

			// Use cooresponding shader when setting uniforms/drawing objects
			lightingShader->Use();

			glActiveTexture(GL_TEXTURE0);
			glBindTexture(GL_TEXTURE_2D, texture);
			lightingShader->SetInt("material.diffuseMap", 0);
			glActiveTexture(GL_TEXTURE1);
			glBindTexture(GL_TEXTURE_2D, texture2);
			lightingShader->SetInt("material.specularMap", 1);

			/*	glActiveTexture(GL_TEXTURE0);
			glBindTexture(GL_TEXTURE_2D, texture);
			lightingShader->SetInt("material.diffuseMap", 0);
			glActiveTexture(GL_TEXTURE1);
			glBindTexture(GL_TEXTURE_2D, texture2);
			lightingShader->SetInt("material.specularMap", 1);*/

			lightingShader->SetVec3(camera->Position.x, camera->Position.y, camera->Position.z, "viewPosition");

			//now materials require quite a few more uniforms
			lightingShader->SetVec3(1.0f, 0.5f, 0.31f, "material.ambient");
			lightingShader->SetVec3(1.0f, 0.5f, 0.31f, "material.diffuse");
			lightingShader->SetVec3(0.5f, 0.5f, 0.5f, "material.specular");
			lightingShader->SetFloat(32.0f, "material.shininess");

			//first, just set the light.direction uniform of the new shader
			//	lightingShader->SetVec3(-0.2f, -1.0f, -0.3f, "light.direction");//directional light
			//lightingShader->SetVec3(lightPos.x, lightPos.y, lightPos.z, "light.position");//point light

			//in the future, use vectors to avoid constants and raw ints
			//also, just assign an ambient/diff/spec value to the shader itself that all lights can share
			for (GLuint i = 0; i < 4; i++)
			{
				string eye = std::to_string(i);
				std::string s = "pointLights[" + eye + "].position";
				GLchar * pos = &s[0u];
				lightingShader->SetVec3(pointLightPositions[i].x, pointLightPositions[i].y, pointLightPositions[i].z, pos);
				s = "pointLights[" + eye + "].ambient";
				pos = &s[0u];
				lightingShader->SetVec3(0.2f, 0.2f, 0.2f, pos);
				s = "pointLights[" + eye + "].diffuse";
				pos = &s[0u];
				lightingShader->SetVec3(0.5f, 0.5f, 0.5f, pos);
				s = "pointLights[" + eye + "].specular";
				pos = &s[0u];
				lightingShader->SetVec3(-0.2f, -1.0f, -0.3f, pos);
				s = "pointLights[" + eye + "].constant";
				pos = &s[0u];
				lightingShader->SetFloat(1.0f, pos);
				s = "pointLights[" + eye + "].linear";
				pos = &s[0u];
				lightingShader->SetFloat(0.09f, pos);
				s = "pointLights[" + eye + "].quadratic";
				pos = &s[0u];
				lightingShader->SetFloat(0.032f, pos);
			}

			//point light variables
			/*	lightingShader->SetFloat(1.0f, "light.constant");
			lightingShader->SetFloat(0.09f, "light.linear");
			lightingShader->SetFloat(0.032f, "light.quadratic");*/

			//direction, ambient, diffuse, specular of directional Light
			lightingShader->SetVec3(0.2f, 0.2f, 0.2f, "directionalLight.ambient");
			lightingShader->SetVec3(0.5f, 0.5f, 0.5f, "directionalLight.diffuse");
			lightingShader->SetVec3(1.0f, 1.0f, 1.0f, "directionalLight.specular");
			lightingShader->SetVec3(-0.2f, -1.0f, -0.3f, "directionalLight.direction");

			//spot light variables
			lightingShader->SetVec3(camera->Position.x, camera->Position.y, camera->Position.z, "flashLight.position");
			lightingShader->SetVec3(camera->Front.x, camera->Front.y, camera->Front.z, "flashLight.direction");
			lightingShader->SetFloat(glm::cos(glm::radians(12.5f)), "flashLight.cutOff");
			lightingShader->SetFloat(glm::cos(glm::radians(15.0f)), "flashLight.outerCutOff");
			lightingShader->SetVec3(0.2f, 0.2f, 0.2f, "flashLight.ambient");
			lightingShader->SetVec3(0.5f, 0.5f, 0.5f, "flashLight.diffuse");
			lightingShader->SetVec3(1.0f, 1.0f, 1.0f, "flashLight.specular");


			// Create camera transformations
			glm::mat4 view;
			view = camera->GetViewMatrix();
			glm::mat4 projection = glm::perspective(camera->Zoom, (GLfloat)WIDTH / (GLfloat)HEIGHT, 0.005f, 1000.0f);
			// Get the uniform locations
			lightingShader->setMat4("view", view);
			lightingShader->setMat4("projection", projection);

			// Draw the container (using container's vertex attributes)
			glBindVertexArray(containerVAO);


			for (GLuint i = 0; i <10; i++)
			{

				glm::mat4 bmodel;
				bmodel = glm::translate(bmodel, cubePositions[i]);
				GLfloat angle = i * 20.0f;
				bmodel = glm::rotate(bmodel, angle, glm::vec3(1.0f, 0.3f, 0.5f));
				lightingShader->setMat4("model", bmodel);
				glDrawArrays(GL_TRIANGLES, 0, 36);

			}






			//	glDrawArrays(GL_TRIANGLES, 0, 36);
			glBindVertexArray(0);

			// Also draw the lamp object, again binding the appropriate shader
			lampShader->Use();
			lampShader->setMat4("view", view);
			lampShader->setMat4("projection", projection);

			// Draw the light object (using light's vertex attributes)
			glBindVertexArray(lightVAO);
			for (GLuint i = 0; i < 4; i++)
			{
				glm::mat4 model;
				model = glm::mat4();
				model = glm::translate(model, pointLightPositions[i]);
				model = glm::scale(model, glm::vec3(0.05f)); // Make it a smaller cube
				lampShader->setMat4("model", model);


				glDrawArrays(GL_TRIANGLES, 0, 36);
			}
			//glm::mat4 model;
			//model = glm::mat4();
			//model = glm::translate(model, lightPos);
			//model = glm::scale(model, glm::vec3(0.05f)); // Make it a smaller cube
			//lampShader->setMat4("model", model);



			glBindVertexArray(0);

			// Swap the screen buffers
			glfwSwapBuffers(sceneWindow);
		}

		return 0;
		return 0;
	}
	__declspec(dllexport) int InitiateLightCasterLesson()
	{
		glfwInit();
		// Set all the required options for GLFW
		glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
		glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 3);
		glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);
		glfwWindowHint(GLFW_RESIZABLE, GL_FALSE);

		// Create a GLFWwindow object that we can use for GLFW's functions
		sceneWindow = glfwCreateWindow(screenWidth, screenHeight, "Specular Map With Lighting", nullptr, nullptr);
		glfwMakeContextCurrent(sceneWindow);

		// Set the required callback functions
		glfwSetKeyCallback(sceneWindow, key_callback);
		glfwSetCursorPosCallback(sceneWindow, mouse_callback);
		glfwSetScrollCallback(sceneWindow, scroll_callback);

		// GLFW Options
		glfwSetInputMode(sceneWindow, GLFW_CURSOR, GLFW_CURSOR_DISABLED);

		// Set this to true so GLEW knows to use a modern approach to retrieving function pointers and extensions
		glewExperimental = GL_TRUE;
		// Initialize GLEW to setup the OpenGL Function pointers
		glewInit();

		// Define the viewport dimensions
		glViewport(0, 0, screenWidth, screenHeight);

		// OpenGL options
		glEnable(GL_DEPTH_TEST);


		string d = exeDirectory();
		string vShaderDir = d + "Shaders\\openglexamples\\vertexSpecularTextured.txt";
		const char * vSource = vShaderDir.c_str();
		cout << "Getting vert shader from dir: " << vShaderDir << endl;
		string fragDir = d + "Shaders\\openglexamples\\fragmentSpecularTexturedSpotlight.txt";
		const char *fragSource = fragDir.c_str();
		cout << "Getting frag shader from " << fragSource << endl;
		lightingShader = new Shader(vSource, fragSource);

		//lamp debugging shader
		vShaderDir = d + "Shaders\\openglexamples\\vertexSimpleLighting.txt";
		vSource = vShaderDir.c_str();
		fragDir = d + "Shaders\\openglexamples\\fragmentLightShader.txt";
		fragSource = fragDir.c_str();
		lampShader = new Shader(vSource, fragSource);


		// Set up vertex data (and buffer(s)) and attribute pointers
		float vertices[] = {
			// positions          // normals           // texture coords
			-0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,
			0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 0.0f,
			0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
			0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
			-0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 1.0f,
			-0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,

			-0.5f, -0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   0.0f, 0.0f,
			0.5f, -0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   1.0f, 0.0f,
			0.5f,  0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   1.0f, 1.0f,
			0.5f,  0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   1.0f, 1.0f,
			-0.5f,  0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   0.0f, 1.0f,
			-0.5f, -0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   0.0f, 0.0f,

			-0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
			-0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
			-0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
			-0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
			-0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
			-0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,

			0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
			0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
			0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
			0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
			0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
			0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,

			-0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,
			0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 1.0f,
			0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
			0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
			-0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 0.0f,
			-0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,

			-0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f,
			0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 1.0f,
			0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
			0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
			-0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 0.0f,
			-0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f
		};
		// First, set the container's VAO (and VBO)
		//GLuint VBO, containerVAO;
		glGenVertexArrays(1, &containerVAO);
		glGenBuffers(1, &VBO);

		glBindBuffer(GL_ARRAY_BUFFER, VBO);
		glBufferData(GL_ARRAY_BUFFER, sizeof(vertices), vertices, GL_STATIC_DRAW);

		glBindVertexArray(containerVAO);
		// Position attribute
		glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 8 * sizeof(GLfloat), (GLvoid*)0);
		glEnableVertexAttribArray(0);
		// Normal attribute
		glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 8 * sizeof(GLfloat), (GLvoid*)(3 * sizeof(GLfloat)));
		glEnableVertexAttribArray(1);
		//Texture coordinate attribute
		glVertexAttribPointer(2, 2, GL_FLOAT, GL_FALSE, 8 * sizeof(GLfloat), (GLvoid*)(6 * sizeof(GLfloat)));
		glEnableVertexAttribArray(2);

		glBindVertexArray(0);





		// Then, we set the light's VAO (VBO stays the same. After all, the vertices are the same for the light object (also a 3D cube))
		//	GLuint lightVAO;
		glGenVertexArrays(1, &lightVAO);
		glBindVertexArray(lightVAO);
		// We only need to bind to the VBO (to link it with glVertexAttribPointer), no need to fill it; the VBO's data already contains all we need.
		glBindBuffer(GL_ARRAY_BUFFER, VBO);
		// Set the vertex attributes (only position data for the lamp))
		glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 8 * sizeof(GLfloat), (GLvoid*)0); // Note that we skip over the normal vectors
		glEnableVertexAttribArray(0);
		glBindVertexArray(0);

		glBindBuffer(GL_ARRAY_BUFFER, 0);


		//LOAD THE DIFFUSEMAP!
		string imaged = d + "textures\\container2.png";
		const char *iDir = imaged.c_str();

		// Load and create a texture 

		texture = CreateTexture(iDir, GL_REPEAT, GL_LINEAR);

		string hap = d + "textures\\container2_specular.png";
		iDir = hap.c_str();

		texture2 = CreateTexture(iDir, GL_REPEAT, GL_LINEAR);

		//cout << "Got texture from: " << iDir << endl;
		//enable blending for text display
		glEnable(GL_BLEND);
		glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
		return 0;
	}
	__declspec(dllexport) int RenderLightCasterLesson()
	{
		// Calculate deltatime of current frame
		GLfloat currentFrame = glfwGetTime();
		deltaTime = currentFrame - lastFrame;
		lastFrame = currentFrame;

		// Check if any events have been activiated (key pressed, mouse moved etc.) and call corresponding response functions
		glfwPollEvents();
		processInput(sceneWindow);

		// Clear the colorbuffer
		glClearColor(0.1f, 0.1f, 0.1f, 1.0f);
		glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

		// Use cooresponding shader when setting uniforms/drawing objects
		lightingShader->Use();

		glActiveTexture(GL_TEXTURE0);
		glBindTexture(GL_TEXTURE_2D, texture);
		lightingShader->SetInt("material.diffuseMap", 0);
		glActiveTexture(GL_TEXTURE1);
		glBindTexture(GL_TEXTURE_2D, texture2);
		lightingShader->SetInt("material.specularMap", 1);

		/*	glActiveTexture(GL_TEXTURE0);
		glBindTexture(GL_TEXTURE_2D, texture);
		lightingShader->SetInt("material.diffuseMap", 0);
		glActiveTexture(GL_TEXTURE1);
		glBindTexture(GL_TEXTURE_2D, texture2);
		lightingShader->SetInt("material.specularMap", 1);*/

		lightingShader->SetVec3(camera->Position.x, camera->Position.y, camera->Position.z, "viewPosition");

		//now materials require quite a few more uniforms
		lightingShader->SetVec3(1.0f, 0.5f, 0.31f, "material.ambient");
		lightingShader->SetVec3(1.0f, 0.5f, 0.31f, "material.diffuse");
		lightingShader->SetVec3(0.5f, 0.5f, 0.5f, "material.specular");
		lightingShader->SetFloat(32.0f, "material.shininess");

		//first, just set the light.direction uniform of the new shader
		//	lightingShader->SetVec3(-0.2f, -1.0f, -0.3f, "light.direction");//directional light
		lightingShader->SetVec3(lightPos.x, lightPos.y, lightPos.z, "light.position");//point light
		lightingShader->SetVec3(0.2f, 0.2f, 0.2f, "light.ambient");
		lightingShader->SetVec3(0.5f, 0.5f, 0.5f, "light.diffuse");
		lightingShader->SetVec3(1.0f, 1.0f, 1.0f, "light.specular");

		//point light variables
		/*	lightingShader->SetFloat(1.0f, "light.constant");
		lightingShader->SetFloat(0.09f, "light.linear");
		lightingShader->SetFloat(0.032f, "light.quadratic");*/

		//spot light variables
		lightingShader->SetVec3(camera->Position.x, camera->Position.y, camera->Position.z, "light.position");
		lightingShader->SetVec3(camera->Front.x, camera->Front.y, camera->Front.z, "light.direction");
		lightingShader->SetFloat(glm::cos(glm::radians(12.5f)), "light.cutOff");
		lightingShader->SetFloat(glm::cos(glm::radians(15.0f)), "light.outerCutOff");


		// Create camera transformations
		glm::mat4 view;
		view = camera->GetViewMatrix();
		glm::mat4 projection = glm::perspective(camera->Zoom, (GLfloat)WIDTH / (GLfloat)HEIGHT, 0.005f, 1000.0f);
		// Get the uniform locations
		lightingShader->setMat4("view", view);
		lightingShader->setMat4("projection", projection);

		// Draw the container (using container's vertex attributes)
		glBindVertexArray(containerVAO);


		for (GLuint i = 0; i <10; i++)
		{

			glm::mat4 bmodel;
			bmodel = glm::translate(bmodel, cubePositions[i]);
			GLfloat angle = i * 20.0f;
			bmodel = glm::rotate(bmodel, angle, glm::vec3(1.0f, 0.3f, 0.5f));
			lightingShader->setMat4("model", bmodel);
			glDrawArrays(GL_TRIANGLES, 0, 36);

		}

		//	glDrawArrays(GL_TRIANGLES, 0, 36);
		glBindVertexArray(0);

		// Also draw the lamp object, again binding the appropriate shader
		lampShader->Use();
		lampShader->setMat4("view", view);
		lampShader->setMat4("projection", projection);

		glm::mat4 model;
		model = glm::mat4();
		model = glm::translate(model, lightPos);
		model = glm::scale(model, glm::vec3(0.05f)); // Make it a smaller cube
		lampShader->setMat4("model", model);


		// Draw the light object (using light's vertex attributes)
		glBindVertexArray(lightVAO);
		glDrawArrays(GL_TRIANGLES, 0, 36);
		glBindVertexArray(0);

		// Swap the screen buffers
		glfwSwapBuffers(sceneWindow);
		return 0;
	}
	__declspec(dllexport) int LightCasterLesson()
	{
		glfwInit();
		// Set all the required options for GLFW
		glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
		glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 3);
		glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);
		glfwWindowHint(GLFW_RESIZABLE, GL_FALSE);

		// Create a GLFWwindow object that we can use for GLFW's functions
		sceneWindow = glfwCreateWindow(screenWidth, screenHeight, "Specular Map With Lighting", nullptr, nullptr);
		glfwMakeContextCurrent(sceneWindow);

		// Set the required callback functions
		glfwSetKeyCallback(sceneWindow, key_callback);
		glfwSetCursorPosCallback(sceneWindow, mouse_callback);
		glfwSetScrollCallback(sceneWindow, scroll_callback);

		// GLFW Options
		glfwSetInputMode(sceneWindow, GLFW_CURSOR, GLFW_CURSOR_DISABLED);

		// Set this to true so GLEW knows to use a modern approach to retrieving function pointers and extensions
		glewExperimental = GL_TRUE;
		// Initialize GLEW to setup the OpenGL Function pointers
		glewInit();

		// Define the viewport dimensions
		glViewport(0, 0, screenWidth, screenHeight);

		// OpenGL options
		glEnable(GL_DEPTH_TEST);


		string d = exeDirectory();
		string vShaderDir = d + "Shaders\\openglexamples\\vertexSpecularTextured.txt";
		const char * vSource = vShaderDir.c_str();
		cout << "Getting vert shader from dir: " << vShaderDir << endl;
		string fragDir = d + "Shaders\\openglexamples\\fragmentSpecularTexturedSpotlight.txt";
		const char *fragSource = fragDir.c_str();
		cout << "Getting frag shader from " << fragSource << endl;
		lightingShader = new Shader(vSource, fragSource);

		//lamp debugging shader
		vShaderDir = d + "Shaders\\openglexamples\\vertexSimpleLighting.txt";
		vSource = vShaderDir.c_str();
		fragDir = d + "Shaders\\openglexamples\\fragmentLightShader.txt";
		fragSource = fragDir.c_str();
		lampShader = new Shader(vSource, fragSource);


		// Set up vertex data (and buffer(s)) and attribute pointers
		float vertices[] = {
			// positions          // normals           // texture coords
			-0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,
			0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 0.0f,
			0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
			0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
			-0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 1.0f,
			-0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,

			-0.5f, -0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   0.0f, 0.0f,
			0.5f, -0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   1.0f, 0.0f,
			0.5f,  0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   1.0f, 1.0f,
			0.5f,  0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   1.0f, 1.0f,
			-0.5f,  0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   0.0f, 1.0f,
			-0.5f, -0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   0.0f, 0.0f,

			-0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
			-0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
			-0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
			-0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
			-0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
			-0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,

			0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
			0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
			0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
			0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
			0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
			0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,

			-0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,
			0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 1.0f,
			0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
			0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
			-0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 0.0f,
			-0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,

			-0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f,
			0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 1.0f,
			0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
			0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
			-0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 0.0f,
			-0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f
		};
		// First, set the container's VAO (and VBO)
		//GLuint VBO, containerVAO;
		glGenVertexArrays(1, &containerVAO);
		glGenBuffers(1, &VBO);

		glBindBuffer(GL_ARRAY_BUFFER, VBO);
		glBufferData(GL_ARRAY_BUFFER, sizeof(vertices), vertices, GL_STATIC_DRAW);

		glBindVertexArray(containerVAO);
		// Position attribute
		glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 8 * sizeof(GLfloat), (GLvoid*)0);
		glEnableVertexAttribArray(0);
		// Normal attribute
		glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 8 * sizeof(GLfloat), (GLvoid*)(3 * sizeof(GLfloat)));
		glEnableVertexAttribArray(1);
		//Texture coordinate attribute
		glVertexAttribPointer(2, 2, GL_FLOAT, GL_FALSE, 8 * sizeof(GLfloat), (GLvoid*)(6 * sizeof(GLfloat)));
		glEnableVertexAttribArray(2);

		glBindVertexArray(0);





		// Then, we set the light's VAO (VBO stays the same. After all, the vertices are the same for the light object (also a 3D cube))
		//	GLuint lightVAO;
		glGenVertexArrays(1, &lightVAO);
		glBindVertexArray(lightVAO);
		// We only need to bind to the VBO (to link it with glVertexAttribPointer), no need to fill it; the VBO's data already contains all we need.
		glBindBuffer(GL_ARRAY_BUFFER, VBO);
		// Set the vertex attributes (only position data for the lamp))
		glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 8 * sizeof(GLfloat), (GLvoid*)0); // Note that we skip over the normal vectors
		glEnableVertexAttribArray(0);
		glBindVertexArray(0);

		glBindBuffer(GL_ARRAY_BUFFER, 0);


		//LOAD THE DIFFUSEMAP!
		string imaged = d + "textures\\container2.png";
		const char *iDir = imaged.c_str();

		// Load and create a texture 

		texture = CreateTexture(iDir, GL_REPEAT, GL_LINEAR);

		string hap = d + "textures\\container2_specular.png";
		iDir = hap.c_str();

		texture2 = CreateTexture(iDir, GL_REPEAT, GL_LINEAR);

		//cout << "Got texture from: " << iDir << endl;
		//enable blending for text display
		glEnable(GL_BLEND);
		glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);


		while (!glfwWindowShouldClose(sceneWindow))
		{
			// Calculate deltatime of current frame
			GLfloat currentFrame = glfwGetTime();
			deltaTime = currentFrame - lastFrame;
			lastFrame = currentFrame;

			// Check if any events have been activiated (key pressed, mouse moved etc.) and call corresponding response functions
			glfwPollEvents();
			processInput(sceneWindow);

			// Clear the colorbuffer
			glClearColor(0.1f, 0.1f, 0.1f, 1.0f);
			glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

			// Use cooresponding shader when setting uniforms/drawing objects
			lightingShader->Use();

			glActiveTexture(GL_TEXTURE0);
			glBindTexture(GL_TEXTURE_2D, texture);
			lightingShader->SetInt("material.diffuseMap", 0);
			glActiveTexture(GL_TEXTURE1);
			glBindTexture(GL_TEXTURE_2D, texture2);
			lightingShader->SetInt("material.specularMap", 1);

			/*	glActiveTexture(GL_TEXTURE0);
			glBindTexture(GL_TEXTURE_2D, texture);
			lightingShader->SetInt("material.diffuseMap", 0);
			glActiveTexture(GL_TEXTURE1);
			glBindTexture(GL_TEXTURE_2D, texture2);
			lightingShader->SetInt("material.specularMap", 1);*/

			lightingShader->SetVec3(camera->Position.x, camera->Position.y, camera->Position.z, "viewPosition");

			//now materials require quite a few more uniforms
			lightingShader->SetVec3(1.0f, 0.5f, 0.31f, "material.ambient");
			lightingShader->SetVec3(1.0f, 0.5f, 0.31f, "material.diffuse");
			lightingShader->SetVec3(0.5f, 0.5f, 0.5f, "material.specular");
			lightingShader->SetFloat(32.0f, "material.shininess");

			//first, just set the light.direction uniform of the new shader
			//	lightingShader->SetVec3(-0.2f, -1.0f, -0.3f, "light.direction");//directional light
			lightingShader->SetVec3(lightPos.x, lightPos.y, lightPos.z, "light.position");//point light
			lightingShader->SetVec3(0.2f, 0.2f, 0.2f, "light.ambient");
			lightingShader->SetVec3(0.5f, 0.5f, 0.5f, "light.diffuse");
			lightingShader->SetVec3(1.0f, 1.0f, 1.0f, "light.specular");

			//point light variables
			/*	lightingShader->SetFloat(1.0f, "light.constant");
			lightingShader->SetFloat(0.09f, "light.linear");
			lightingShader->SetFloat(0.032f, "light.quadratic");*/

			//spot light variables
			lightingShader->SetVec3(camera->Position.x, camera->Position.y, camera->Position.z, "light.position");
			lightingShader->SetVec3(camera->Front.x, camera->Front.y, camera->Front.z, "light.direction");
			lightingShader->SetFloat(glm::cos(glm::radians(12.5f)), "light.cutOff");
			lightingShader->SetFloat(glm::cos(glm::radians(15.0f)), "light.outerCutOff");


			// Create camera transformations
			glm::mat4 view;
			view = camera->GetViewMatrix();
			glm::mat4 projection = glm::perspective(camera->Zoom, (GLfloat)WIDTH / (GLfloat)HEIGHT, 0.005f, 1000.0f);
			// Get the uniform locations
			lightingShader->setMat4("view", view);
			lightingShader->setMat4("projection", projection);

			// Draw the container (using container's vertex attributes)
			glBindVertexArray(containerVAO);


			for (GLuint i = 0; i <10; i++)
			{

				glm::mat4 bmodel;
				bmodel = glm::translate(bmodel, cubePositions[i]);
				GLfloat angle = i * 20.0f;
				bmodel = glm::rotate(bmodel, angle, glm::vec3(1.0f, 0.3f, 0.5f));
				lightingShader->setMat4("model", bmodel);
				glDrawArrays(GL_TRIANGLES, 0, 36);

			}






			//	glDrawArrays(GL_TRIANGLES, 0, 36);
			glBindVertexArray(0);

			// Also draw the lamp object, again binding the appropriate shader
			lampShader->Use();
			lampShader->setMat4("view", view);
			lampShader->setMat4("projection", projection);

			glm::mat4 model;
			model = glm::mat4();
			model = glm::translate(model, lightPos);
			model = glm::scale(model, glm::vec3(0.05f)); // Make it a smaller cube
			lampShader->setMat4("model", model);


			// Draw the light object (using light's vertex attributes)
			glBindVertexArray(lightVAO);
			glDrawArrays(GL_TRIANGLES, 0, 36);
			glBindVertexArray(0);

			// Swap the screen buffers
			glfwSwapBuffers(sceneWindow);
		}

		return 0;
	}
	__declspec(dllexport) int InitiateLightMapLesson()
	{
		glfwInit();
		// Set all the required options for GLFW
		glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
		glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 3);
		glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);
		glfwWindowHint(GLFW_RESIZABLE, GL_FALSE);

		// Create a GLFWwindow object that we can use for GLFW's functions
		sceneWindow = glfwCreateWindow(screenWidth, screenHeight, "Specular Map With Lighting", nullptr, nullptr);
		glfwMakeContextCurrent(sceneWindow);

		// Set the required callback functions
		glfwSetKeyCallback(sceneWindow, key_callback);
		glfwSetCursorPosCallback(sceneWindow, mouse_callback);
		glfwSetScrollCallback(sceneWindow, scroll_callback);

		// GLFW Options
		glfwSetInputMode(sceneWindow, GLFW_CURSOR, GLFW_CURSOR_DISABLED);

		// Set this to true so GLEW knows to use a modern approach to retrieving function pointers and extensions
		glewExperimental = GL_TRUE;
		// Initialize GLEW to setup the OpenGL Function pointers
		glewInit();

		// Define the viewport dimensions
		glViewport(0, 0, screenWidth, screenHeight);

		// OpenGL options
		glEnable(GL_DEPTH_TEST);


		string d = exeDirectory();
		string vShaderDir = d + "Shaders\\openglexamples\\vertexSpecularTextured.txt";
		const char * vSource = vShaderDir.c_str();
		cout << "Getting vert shader from dir: " << vShaderDir << endl;
		string fragDir = d + "Shaders\\openglexamples\\fragmentSpecularTextured.txt";
		const char *fragSource = fragDir.c_str();
		cout << "Getting frag shader from " << fragSource << endl;
		lightingShader = new Shader(vSource, fragSource);

		//lamp debugging shader
		vShaderDir = d + "Shaders\\openglexamples\\vertexSimpleLighting.txt";
		vSource = vShaderDir.c_str();
		fragDir = d + "Shaders\\openglexamples\\fragmentLightShader.txt";
		fragSource = fragDir.c_str();
		lampShader = new Shader(vSource, fragSource);


		// Set up vertex data (and buffer(s)) and attribute pointers
		float vertices[] = {
			// positions          // normals           // texture coords
			-0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,
			0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 0.0f,
			0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
			0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
			-0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 1.0f,
			-0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,

			-0.5f, -0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   0.0f, 0.0f,
			0.5f, -0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   1.0f, 0.0f,
			0.5f,  0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   1.0f, 1.0f,
			0.5f,  0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   1.0f, 1.0f,
			-0.5f,  0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   0.0f, 1.0f,
			-0.5f, -0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   0.0f, 0.0f,

			-0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
			-0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
			-0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
			-0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
			-0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
			-0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,

			0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
			0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
			0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
			0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
			0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
			0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,

			-0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,
			0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 1.0f,
			0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
			0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
			-0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 0.0f,
			-0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,

			-0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f,
			0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 1.0f,
			0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
			0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
			-0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 0.0f,
			-0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f
		};
		// First, set the container's VAO (and VBO)
		//GLuint VBO, containerVAO;
		glGenVertexArrays(1, &containerVAO);
		glGenBuffers(1, &VBO);

		glBindBuffer(GL_ARRAY_BUFFER, VBO);
		glBufferData(GL_ARRAY_BUFFER, sizeof(vertices), vertices, GL_STATIC_DRAW);

		glBindVertexArray(containerVAO);
		// Position attribute
		glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 8 * sizeof(GLfloat), (GLvoid*)0);
		glEnableVertexAttribArray(0);
		// Normal attribute
		glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 8 * sizeof(GLfloat), (GLvoid*)(3 * sizeof(GLfloat)));
		glEnableVertexAttribArray(1);
		//Texture coordinate attribute
		glVertexAttribPointer(2, 2, GL_FLOAT, GL_FALSE, 8 * sizeof(GLfloat), (GLvoid*)(6 * sizeof(GLfloat)));
		glEnableVertexAttribArray(2);

		glBindVertexArray(0);





		// Then, we set the light's VAO (VBO stays the same. After all, the vertices are the same for the light object (also a 3D cube))
		//	GLuint lightVAO;
		glGenVertexArrays(1, &lightVAO);
		glBindVertexArray(lightVAO);
		// We only need to bind to the VBO (to link it with glVertexAttribPointer), no need to fill it; the VBO's data already contains all we need.
		glBindBuffer(GL_ARRAY_BUFFER, VBO);
		// Set the vertex attributes (only position data for the lamp))
		glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 8 * sizeof(GLfloat), (GLvoid*)0); // Note that we skip over the normal vectors
		glEnableVertexAttribArray(0);
		glBindVertexArray(0);

		glBindBuffer(GL_ARRAY_BUFFER, 0);


		//LOAD THE DIFFUSEMAP!
		string imaged = d + "textures\\container2.png";
		const char *iDir = imaged.c_str();

		// Load and create a texture 

		texture = CreateTexture(iDir, GL_REPEAT, GL_LINEAR);

		string hap = d + "textures\\container2_specular.png";
		iDir = hap.c_str();

		texture2 = CreateTexture(iDir, GL_REPEAT, GL_LINEAR);

		//cout << "Got texture from: " << iDir << endl;
		//enable blending for text display
		glEnable(GL_BLEND);
		glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
		return 0;
	}

	__declspec(dllexport) int RenderLightMapLesson()
	{
		// Calculate deltatime of current frame
		GLfloat currentFrame = glfwGetTime();
		deltaTime = currentFrame - lastFrame;
		lastFrame = currentFrame;

		// Check if any events have been activiated (key pressed, mouse moved etc.) and call corresponding response functions
		glfwPollEvents();
		processInput(sceneWindow);

		// Clear the colorbuffer
		glClearColor(0.1f, 0.1f, 0.1f, 1.0f);
		glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

		// Use cooresponding shader when setting uniforms/drawing objects
		lightingShader->Use();

		glActiveTexture(GL_TEXTURE0);
		glBindTexture(GL_TEXTURE_2D, texture);
		lightingShader->SetInt("material.diffuseMap", 0);
		glActiveTexture(GL_TEXTURE1);
		glBindTexture(GL_TEXTURE_2D, texture2);
		lightingShader->SetInt("material.specularMap", 1);

		/*	glActiveTexture(GL_TEXTURE0);
		glBindTexture(GL_TEXTURE_2D, texture);
		lightingShader->SetInt("material.diffuseMap", 0);
		glActiveTexture(GL_TEXTURE1);
		glBindTexture(GL_TEXTURE_2D, texture2);
		lightingShader->SetInt("material.specularMap", 1);*/

		lightingShader->SetVec3(camera->Position.x, camera->Position.y, camera->Position.z, "viewPosition");

		//now materials require quite a few more uniforms
		lightingShader->SetVec3(1.0f, 0.5f, 0.31f, "material.ambient");
		lightingShader->SetVec3(1.0f, 0.5f, 0.31f, "material.diffuse");
		lightingShader->SetVec3(0.5f, 0.5f, 0.5f, "material.specular");
		lightingShader->SetFloat(32.0f, "material.shininess");

		lightingShader->SetVec3(lightPos.x, lightPos.y, lightPos.z, "light.position");
		lightingShader->SetVec3(0.2f, 0.2f, 0.2f, "light.ambient");
		lightingShader->SetVec3(0.5f, 0.5f, 0.5f, "light.diffuse");
		lightingShader->SetVec3(1.0f, 1.0f, 1.0f, "light.specular");

		// Create camera transformations
		glm::mat4 view;
		view = camera->GetViewMatrix();
		glm::mat4 projection = glm::perspective(camera->Zoom, (GLfloat)WIDTH / (GLfloat)HEIGHT, 0.005f, 1000.0f);
		// Get the uniform locations
		lightingShader->setMat4("view", view);
		lightingShader->setMat4("projection", projection);

		// Draw the container (using container's vertex attributes)
		glBindVertexArray(containerVAO);
		glm::mat4 model;
		/*glUniformMatrix4fv(modelLoc, 1, GL_FALSE, glm::value_ptr(model));*/
		lightingShader->setMat4("model", model);


		glDrawArrays(GL_TRIANGLES, 0, 36);
		glBindVertexArray(0);

		// Also draw the lamp object, again binding the appropriate shader
		lampShader->Use();
		lampShader->setMat4("view", view);
		lampShader->setMat4("projection", projection);

		model = glm::mat4();
		model = glm::translate(model, lightPos);
		model = glm::scale(model, glm::vec3(0.05f)); // Make it a smaller cube
		lampShader->setMat4("model", model);

		// Draw the light object (using light's vertex attributes)
		glBindVertexArray(lightVAO);
		glDrawArrays(GL_TRIANGLES, 0, 36);
		glBindVertexArray(0);

		// Swap the screen buffers
		glfwSwapBuffers(sceneWindow);
		return 0;
	}
	__declspec(dllexport) int LightingMapLesson()
	{
		glfwInit();
		// Set all the required options for GLFW
		glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
		glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 3);
		glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);
		glfwWindowHint(GLFW_RESIZABLE, GL_FALSE);

		// Create a GLFWwindow object that we can use for GLFW's functions
		sceneWindow = glfwCreateWindow(screenWidth, screenHeight, "Specular Map With Lighting", nullptr, nullptr);
		glfwMakeContextCurrent(sceneWindow);

		// Set the required callback functions
		glfwSetKeyCallback(sceneWindow, key_callback);
		glfwSetCursorPosCallback(sceneWindow, mouse_callback);
		glfwSetScrollCallback(sceneWindow, scroll_callback);

		// GLFW Options
		glfwSetInputMode(sceneWindow, GLFW_CURSOR, GLFW_CURSOR_DISABLED);

		// Set this to true so GLEW knows to use a modern approach to retrieving function pointers and extensions
		glewExperimental = GL_TRUE;
		// Initialize GLEW to setup the OpenGL Function pointers
		glewInit();

		// Define the viewport dimensions
		glViewport(0, 0, screenWidth, screenHeight);

		// OpenGL options
		glEnable(GL_DEPTH_TEST);


		string d = exeDirectory();
		string vShaderDir = d + "Shaders\\openglexamples\\vertexSpecularTextured.txt";
		const char * vSource = vShaderDir.c_str();
		cout << "Getting vert shader from dir: " << vShaderDir << endl;
		string fragDir = d + "Shaders\\openglexamples\\fragmentSpecularTextured.txt";
		const char *fragSource = fragDir.c_str();
		cout << "Getting frag shader from " << fragSource << endl;
		lightingShader = new Shader(vSource, fragSource);

		//lamp debugging shader
		vShaderDir = d + "Shaders\\openglexamples\\vertexSimpleLighting.txt";
		vSource = vShaderDir.c_str();
		fragDir = d + "Shaders\\openglexamples\\fragmentLightShader.txt";
		fragSource = fragDir.c_str();
		lampShader = new Shader(vSource, fragSource);


		// Set up vertex data (and buffer(s)) and attribute pointers
		float vertices[] = {
			// positions          // normals           // texture coords
			-0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,
			0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 0.0f,
			0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
			0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
			-0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 1.0f,
			-0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,

			-0.5f, -0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   0.0f, 0.0f,
			0.5f, -0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   1.0f, 0.0f,
			0.5f,  0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   1.0f, 1.0f,
			0.5f,  0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   1.0f, 1.0f,
			-0.5f,  0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   0.0f, 1.0f,
			-0.5f, -0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   0.0f, 0.0f,

			-0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
			-0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
			-0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
			-0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
			-0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
			-0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,

			0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
			0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
			0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
			0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
			0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
			0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,

			-0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,
			0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 1.0f,
			0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
			0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
			-0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 0.0f,
			-0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,

			-0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f,
			0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 1.0f,
			0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
			0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
			-0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 0.0f,
			-0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f
		};
		// First, set the container's VAO (and VBO)
		//GLuint VBO, containerVAO;
		glGenVertexArrays(1, &containerVAO);
		glGenBuffers(1, &VBO);

		glBindBuffer(GL_ARRAY_BUFFER, VBO);
		glBufferData(GL_ARRAY_BUFFER, sizeof(vertices), vertices, GL_STATIC_DRAW);

		glBindVertexArray(containerVAO);
		// Position attribute
		glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 8 * sizeof(GLfloat), (GLvoid*)0);
		glEnableVertexAttribArray(0);
		// Normal attribute
		glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 8 * sizeof(GLfloat), (GLvoid*)(3 * sizeof(GLfloat)));
		glEnableVertexAttribArray(1);
		//Texture coordinate attribute
		glVertexAttribPointer(2, 2, GL_FLOAT, GL_FALSE, 8 * sizeof(GLfloat), (GLvoid*)(6 * sizeof(GLfloat)));
		glEnableVertexAttribArray(2);

		glBindVertexArray(0);





		// Then, we set the light's VAO (VBO stays the same. After all, the vertices are the same for the light object (also a 3D cube))
		//	GLuint lightVAO;
		glGenVertexArrays(1, &lightVAO);
		glBindVertexArray(lightVAO);
		// We only need to bind to the VBO (to link it with glVertexAttribPointer), no need to fill it; the VBO's data already contains all we need.
		glBindBuffer(GL_ARRAY_BUFFER, VBO);
		// Set the vertex attributes (only position data for the lamp))
		glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 8 * sizeof(GLfloat), (GLvoid*)0); // Note that we skip over the normal vectors
		glEnableVertexAttribArray(0);
		glBindVertexArray(0);

		glBindBuffer(GL_ARRAY_BUFFER, 0);


		//LOAD THE DIFFUSEMAP!
		string imaged = d + "textures\\container2.png";
		const char *iDir = imaged.c_str();

		// Load and create a texture 

		texture = CreateTexture(iDir, GL_REPEAT, GL_LINEAR);

		string hap = d + "textures\\container2_specular.png";
		iDir = hap.c_str();

		texture2 = CreateTexture(iDir, GL_REPEAT, GL_LINEAR);

		//cout << "Got texture from: " << iDir << endl;
		//enable blending for text display
		glEnable(GL_BLEND);
		glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);

		while (!glfwWindowShouldClose(sceneWindow))
		{
			// Calculate deltatime of current frame
			GLfloat currentFrame = glfwGetTime();
			deltaTime = currentFrame - lastFrame;
			lastFrame = currentFrame;

			// Check if any events have been activiated (key pressed, mouse moved etc.) and call corresponding response functions
			glfwPollEvents();
			processInput(sceneWindow);

			// Clear the colorbuffer
			glClearColor(0.1f, 0.1f, 0.1f, 1.0f);
			glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

			// Use cooresponding shader when setting uniforms/drawing objects
			lightingShader->Use();

			glActiveTexture(GL_TEXTURE0);
			glBindTexture(GL_TEXTURE_2D, texture);
			lightingShader->SetInt("material.diffuseMap", 0);
			glActiveTexture(GL_TEXTURE1);
			glBindTexture(GL_TEXTURE_2D, texture2);
			lightingShader->SetInt("material.specularMap", 1);

			/*	glActiveTexture(GL_TEXTURE0);
			glBindTexture(GL_TEXTURE_2D, texture);
			lightingShader->SetInt("material.diffuseMap", 0);
			glActiveTexture(GL_TEXTURE1);
			glBindTexture(GL_TEXTURE_2D, texture2);
			lightingShader->SetInt("material.specularMap", 1);*/

			lightingShader->SetVec3(camera->Position.x, camera->Position.y, camera->Position.z, "viewPosition");

			//now materials require quite a few more uniforms
			lightingShader->SetVec3(1.0f, 0.5f, 0.31f, "material.ambient");
			lightingShader->SetVec3(1.0f, 0.5f, 0.31f, "material.diffuse");
			lightingShader->SetVec3(0.5f, 0.5f, 0.5f, "material.specular");
			lightingShader->SetFloat(32.0f, "material.shininess");

			lightingShader->SetVec3(lightPos.x, lightPos.y, lightPos.z, "light.position");
			lightingShader->SetVec3(0.2f, 0.2f, 0.2f, "light.ambient");
			lightingShader->SetVec3(0.5f, 0.5f, 0.5f, "light.diffuse");
			lightingShader->SetVec3(1.0f, 1.0f, 1.0f, "light.specular");

			// Create camera transformations
			glm::mat4 view;
			view = camera->GetViewMatrix();
			glm::mat4 projection = glm::perspective(camera->Zoom, (GLfloat)WIDTH / (GLfloat)HEIGHT, 0.05f, 1000.0f);
			// Get the uniform locations
			lightingShader->setMat4("view", view);
			lightingShader->setMat4("projection", projection);

			// Draw the container (using container's vertex attributes)
			glBindVertexArray(containerVAO);
			glm::mat4 model;
			/*glUniformMatrix4fv(modelLoc, 1, GL_FALSE, glm::value_ptr(model));*/
			lightingShader->setMat4("model", model);


			glDrawArrays(GL_TRIANGLES, 0, 36);
			glBindVertexArray(0);

			// Also draw the lamp object, again binding the appropriate shader
			lampShader->Use();
			lampShader->setMat4("view", view);
			lampShader->setMat4("projection", projection);

			model = glm::mat4();
			model = glm::translate(model, lightPos);
			model = glm::scale(model, glm::vec3(0.05f)); // Make it a smaller cube
			lampShader->setMat4("model", model);

			// Draw the light object (using light's vertex attributes)
			glBindVertexArray(lightVAO);
			glDrawArrays(GL_TRIANGLES, 0, 36);
			glBindVertexArray(0);

			// Swap the screen buffers
			glfwSwapBuffers(sceneWindow);

		}

		// Terminate GLFW, clearing any resources allocated by GLFW.
		glfwTerminate();
		return 0;
	}
	__declspec(dllexport) int InitiateMaterialLesson()
	{
		glfwInit();
		// Set all the required options for GLFW
		glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
		glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 3);
		glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);
		glfwWindowHint(GLFW_RESIZABLE, GL_FALSE);

		// Create a GLFWwindow object that we can use for GLFW's functions



		string d = exeDirectory();
		string vShaderDir = d + "Shaders\\openglexamples\\vertexDiffuse.txt";
		const char * vSource = vShaderDir.c_str();
		cout << "Getting vert shader from dir: " << vShaderDir << endl;
		string fragDir = d + "Shaders\\openglexamples\\fragmentDiffuse2.txt";
		const char *fragSource = fragDir.c_str();
		cout << "Getting frag shader from " << fragSource << endl;
		lightingShader = new Shader(vSource, fragSource);

		//lamp debugging shader
		vShaderDir = d + "Shaders\\openglexamples\\vertexSimpleLighting.txt";
		vSource = vShaderDir.c_str();
		fragDir = d + "Shaders\\openglexamples\\fragmentLightShader.txt";
		fragSource = fragDir.c_str();
		lampShader = new Shader(vSource, fragSource);

		// Set up vertex data (and buffer(s)) and attribute pointers
		GLfloat vertices[] = {
			-0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
			0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
			0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
			0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
			-0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
			-0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,

			-0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
			0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
			0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
			0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
			-0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
			-0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,

			-0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,
			-0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
			-0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
			-0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
			-0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,
			-0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,

			0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,
			0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
			0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
			0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
			0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,
			0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,

			-0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,
			0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,
			0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
			0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
			-0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
			-0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,

			-0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,
			0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,
			0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
			0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
			-0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
			-0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f
		};
		// First, set the container's VAO (and VBO)
		//GLuint VBO, containerVAO;
		glGenVertexArrays(1, &containerVAO);
		glGenBuffers(1, &VBO);

		glBindBuffer(GL_ARRAY_BUFFER, VBO);
		glBufferData(GL_ARRAY_BUFFER, sizeof(vertices), vertices, GL_STATIC_DRAW);

		glBindVertexArray(containerVAO);
		// Position attribute
		glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 6 * sizeof(GLfloat), (GLvoid*)0);
		glEnableVertexAttribArray(0);
		// Normal attribute
		glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 6 * sizeof(GLfloat), (GLvoid*)(3 * sizeof(GLfloat)));
		glEnableVertexAttribArray(1);
		glBindVertexArray(0);

		// Then, we set the light's VAO (VBO stays the same. After all, the vertices are the same for the light object (also a 3D cube))
		//	GLuint lightVAO;
		glGenVertexArrays(1, &lightVAO);
		glBindVertexArray(lightVAO);
		// We only need to bind to the VBO (to link it with glVertexAttribPointer), no need to fill it; the VBO's data already contains all we need.
		glBindBuffer(GL_ARRAY_BUFFER, VBO);
		// Set the vertex attributes (only position data for the lamp))
		glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 6 * sizeof(GLfloat), (GLvoid*)0); // Note that we skip over the normal vectors
		glEnableVertexAttribArray(0);
		glBindVertexArray(0);

		return 0;
	}

	__declspec(dllexport) int RenderMaterialLesson()
	{
		GLfloat currentFrame = glfwGetTime();
		deltaTime = currentFrame - lastFrame;
		lastFrame = currentFrame;

		// Check if any events have been activiated (key pressed, mouse moved etc.) and call corresponding response functions
		glfwPollEvents();
		processInput(sceneWindow);

		// Clear the colorbuffer
		glClearColor(0.1f, 0.1f, 0.1f, 1.0f);
		glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

		// Use cooresponding shader when setting uniforms/drawing objects
		lightingShader->Use();

		lightingShader->SetVec3(camera->Position.x, camera->Position.y, camera->Position.z, "viewPosition");

		//now materials require quite a few more uniforms
		lightingShader->SetVec3(1.0f, 0.5f, 0.31f, "material.ambient");
		lightingShader->SetVec3(1.0f, 0.5f, 0.31f, "material.diffuse");
		lightingShader->SetVec3(0.5f, 0.5f, 0.5f, "material.specular");
		lightingShader->SetFloat(32.0f, "material.shininess");

		lightingShader->SetVec3(lightPos.x, lightPos.y, lightPos.z, "light.position");
		lightingShader->SetVec3(0.2f, 0.2f, 0.2f, "light.ambient");
		lightingShader->SetVec3(0.5f, 0.5f, 0.5f, "light.diffuse");
		lightingShader->SetVec3(1.0f, 1.0f, 1.0f, "light.specular");

		// Create camera transformations
		glm::mat4 view;
		view = camera->GetViewMatrix();
		glm::mat4 projection = glm::perspective(camera->Zoom, (GLfloat)WIDTH / (GLfloat)HEIGHT, 0.05f, 1000.0f);
		// Get the uniform locations
		lightingShader->setMat4("view", view);
		lightingShader->setMat4("projection", projection);

		// Draw the container (using container's vertex attributes)
		glBindVertexArray(containerVAO);
		glm::mat4 model;
		/*glUniformMatrix4fv(modelLoc, 1, GL_FALSE, glm::value_ptr(model));*/
		glm::translate(model, glm::vec3(-4.0f));
		lightingShader->setMat4("model", model);


		glDrawArrays(GL_TRIANGLES, 0, 36);
		glBindVertexArray(0);

		// Also draw the lamp object, again binding the appropriate shader
		lampShader->Use();
		lampShader->setMat4("view", view);
		lampShader->setMat4("projection", projection);

		model = glm::mat4();
		model = glm::translate(model, lightPos);
		model = glm::scale(model, glm::vec3(0.05f)); // Make it a smaller cube
		lampShader->setMat4("model", model);

		// Draw the light object (using light's vertex attributes)
		glBindVertexArray(lightVAO);
		glDrawArrays(GL_TRIANGLES, 0, 36);
		glBindVertexArray(0);

		// Swap the screen buffers
		glfwSwapBuffers(sceneWindow);
		return 0;
	}
	__declspec(dllexport) int EntireMaterialLesson()
	{
		glfwInit();
		// Set all the required options for GLFW
		glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
		glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 3);
		glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);
		glfwWindowHint(GLFW_RESIZABLE, GL_FALSE);

		// Create a GLFWwindow object that we can use for GLFW's functions
		sceneWindow = glfwCreateWindow(screenWidth, screenHeight, "Diffuse Lighting", nullptr, nullptr);
		glfwMakeContextCurrent(sceneWindow);

		// Set the required callback functions
		glfwSetKeyCallback(sceneWindow, key_callback);
		glfwSetCursorPosCallback(sceneWindow, mouse_callback);
		glfwSetScrollCallback(sceneWindow, scroll_callback);

		// GLFW Options
		glfwSetInputMode(sceneWindow, GLFW_CURSOR, GLFW_CURSOR_DISABLED);

		// Set this to true so GLEW knows to use a modern approach to retrieving function pointers and extensions
		glewExperimental = GL_TRUE;
		// Initialize GLEW to setup the OpenGL Function pointers
		glewInit();

		// Define the viewport dimensions
		glViewport(0, 0, screenWidth, screenHeight);

		// OpenGL options
		glEnable(GL_DEPTH_TEST);


		string d = exeDirectory();
		string vShaderDir = d + "Shaders\\openglexamples\\vertexDiffuse.txt";
		const char * vSource = vShaderDir.c_str();
		cout << "Getting vert shader from dir: " << vShaderDir << endl;
		string fragDir = d + "Shaders\\openglexamples\\fragmentDiffuse2.txt";
		const char *fragSource = fragDir.c_str();
		cout << "Getting frag shader from " << fragSource << endl;
		lightingShader = new Shader(vSource, fragSource);

		//lamp debugging shader
		vShaderDir = d + "Shaders\\openglexamples\\vertexSimpleLighting.txt";
		vSource = vShaderDir.c_str();
		fragDir = d + "Shaders\\openglexamples\\fragmentLightShader.txt";
		fragSource = fragDir.c_str();
		lampShader = new Shader(vSource, fragSource);

		// Set up vertex data (and buffer(s)) and attribute pointers
		GLfloat vertices[] = {
			-0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
			0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
			0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
			0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
			-0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
			-0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,

			-0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
			0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
			0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
			0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
			-0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
			-0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,

			-0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,
			-0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
			-0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
			-0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
			-0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,
			-0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,

			0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,
			0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
			0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
			0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
			0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,
			0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,

			-0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,
			0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,
			0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
			0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
			-0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
			-0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,

			-0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,
			0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,
			0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
			0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
			-0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
			-0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f
		};
		// First, set the container's VAO (and VBO)
		//GLuint VBO, containerVAO;
		glGenVertexArrays(1, &containerVAO);
		glGenBuffers(1, &VBO);

		glBindBuffer(GL_ARRAY_BUFFER, VBO);
		glBufferData(GL_ARRAY_BUFFER, sizeof(vertices), vertices, GL_STATIC_DRAW);

		glBindVertexArray(containerVAO);
		// Position attribute
		glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 6 * sizeof(GLfloat), (GLvoid*)0);
		glEnableVertexAttribArray(0);
		// Normal attribute
		glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 6 * sizeof(GLfloat), (GLvoid*)(3 * sizeof(GLfloat)));
		glEnableVertexAttribArray(1);
		glBindVertexArray(0);

		// Then, we set the light's VAO (VBO stays the same. After all, the vertices are the same for the light object (also a 3D cube))
		//	GLuint lightVAO;
		glGenVertexArrays(1, &lightVAO);
		glBindVertexArray(lightVAO);
		// We only need to bind to the VBO (to link it with glVertexAttribPointer), no need to fill it; the VBO's data already contains all we need.
		glBindBuffer(GL_ARRAY_BUFFER, VBO);
		// Set the vertex attributes (only position data for the lamp))
		glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 6 * sizeof(GLfloat), (GLvoid*)0); // Note that we skip over the normal vectors
		glEnableVertexAttribArray(0);
		glBindVertexArray(0);

		while (!glfwWindowShouldClose(sceneWindow))
		{
			// Calculate deltatime of current frame
			GLfloat currentFrame = glfwGetTime();
			deltaTime = currentFrame - lastFrame;
			lastFrame = currentFrame;

			// Check if any events have been activiated (key pressed, mouse moved etc.) and call corresponding response functions
			glfwPollEvents();
			processInput(sceneWindow);

			// Clear the colorbuffer
			glClearColor(0.1f, 0.1f, 0.1f, 1.0f);
			glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

			// Use cooresponding shader when setting uniforms/drawing objects
			lightingShader->Use();

			lightingShader->SetVec3(camera->Position.x, camera->Position.y, camera->Position.z, "viewPosition");

			//now materials require quite a few more uniforms
			lightingShader->SetVec3(1.0f, 0.5f, 0.31f, "material.ambient");
			lightingShader->SetVec3(1.0f, 0.5f, 0.31f, "material.diffuse");
			lightingShader->SetVec3(0.5f, 0.5f, 0.5f, "material.specular");
			lightingShader->SetFloat(32.0f, "material.shininess");

			lightingShader->SetVec3(lightPos.x, lightPos.y, lightPos.z, "light.position");
			lightingShader->SetVec3(0.2f, 0.2f, 0.2f, "light.ambient");
			lightingShader->SetVec3(0.5f, 0.5f, 0.5f, "light.diffuse");
			lightingShader->SetVec3(1.0f, 1.0f, 1.0f, "light.specular");

			// Create camera transformations
			glm::mat4 view;
			view = camera->GetViewMatrix();
			glm::mat4 projection = glm::perspective(camera->Zoom, (GLfloat)WIDTH / (GLfloat)HEIGHT, 0.05f, 1000.0f);
			// Get the uniform locations
			lightingShader->setMat4("view", view);
			lightingShader->setMat4("projection", projection);

			// Draw the container (using container's vertex attributes)
			glBindVertexArray(containerVAO);
			glm::mat4 model;
			/*glUniformMatrix4fv(modelLoc, 1, GL_FALSE, glm::value_ptr(model));*/
			lightingShader->setMat4("model", model);


			glDrawArrays(GL_TRIANGLES, 0, 36);
			glBindVertexArray(0);

			// Also draw the lamp object, again binding the appropriate shader
			lampShader->Use();
			lampShader->setMat4("view", view);
			lampShader->setMat4("projection", projection);

			model = glm::mat4();
			model = glm::translate(model, lightPos);
			model = glm::scale(model, glm::vec3(0.05f)); // Make it a smaller cube
			lampShader->setMat4("model", model);

			// Draw the light object (using light's vertex attributes)
			glBindVertexArray(lightVAO);
			glDrawArrays(GL_TRIANGLES, 0, 36);
			glBindVertexArray(0);

			// Swap the screen buffers
			glfwSwapBuffers(sceneWindow);

		}

		// Terminate GLFW, clearing any resources allocated by GLFW.
		glfwTerminate();
		return 0;
	}

	__declspec(dllexport) int InitiateDiffuseLesson()
	{
		glfwInit();
		// Set all the required options for GLFW
		glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
		glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 3);
		glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);
		glfwWindowHint(GLFW_RESIZABLE, GL_FALSE);

		// Create a GLFWwindow object that we can use for GLFW's functions
		sceneWindow = glfwCreateWindow(screenWidth, screenHeight, "Diffuse Lighting", nullptr, nullptr);
		glfwMakeContextCurrent(sceneWindow);

		// Set the required callback functions
		glfwSetKeyCallback(sceneWindow, key_callback);
		glfwSetCursorPosCallback(sceneWindow, mouse_callback);
		glfwSetScrollCallback(sceneWindow, scroll_callback);

		// GLFW Options
		glfwSetInputMode(sceneWindow, GLFW_CURSOR, GLFW_CURSOR_DISABLED);

		// Set this to true so GLEW knows to use a modern approach to retrieving function pointers and extensions
		glewExperimental = GL_TRUE;
		// Initialize GLEW to setup the OpenGL Function pointers
		glewInit();

		// Define the viewport dimensions
		glViewport(0, 0, screenWidth, screenHeight);

		// OpenGL options
		glEnable(GL_DEPTH_TEST);


		string d = exeDirectory();
		string vShaderDir = d + "Shaders\\openglexamples\\vertexDiffuse.txt";
		const char * vSource = vShaderDir.c_str();
		cout << "Getting vert shader from dir: " << vShaderDir << endl;
		string fragDir = d + "Shaders\\openglexamples\\fragmentDiffuse.txt";
		const char *fragSource = fragDir.c_str();
		cout << "Getting frag shader from " << fragSource << endl;
		lightingShader = new Shader(vSource, fragSource);

		//lamp debugging shader
		vShaderDir = d + "Shaders\\openglexamples\\vertexSimpleLighting.txt";
		vSource = vShaderDir.c_str();
		fragDir = d + "Shaders\\openglexamples\\fragmentLightShader.txt";
		fragSource = fragDir.c_str();
		lampShader = new Shader(vSource, fragSource);

		// Set up vertex data (and buffer(s)) and attribute pointers
		GLfloat vertices[] = {
			-0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
			0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
			0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
			0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
			-0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
			-0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,

			-0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
			0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
			0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
			0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
			-0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
			-0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,

			-0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,
			-0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
			-0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
			-0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
			-0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,
			-0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,

			0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,
			0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
			0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
			0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
			0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,
			0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,

			-0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,
			0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,
			0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
			0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
			-0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
			-0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,

			-0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,
			0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,
			0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
			0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
			-0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
			-0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f
		};
		// First, set the container's VAO (and VBO)
		//GLuint VBO, containerVAO;
		glGenVertexArrays(1, &containerVAO);
		glGenBuffers(1, &VBO);

		glBindBuffer(GL_ARRAY_BUFFER, VBO);
		glBufferData(GL_ARRAY_BUFFER, sizeof(vertices), vertices, GL_STATIC_DRAW);

		glBindVertexArray(containerVAO);
		// Position attribute
		glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 6 * sizeof(GLfloat), (GLvoid*)0);
		glEnableVertexAttribArray(0);
		// Normal attribute
		glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 6 * sizeof(GLfloat), (GLvoid*)(3 * sizeof(GLfloat)));
		glEnableVertexAttribArray(1);
		glBindVertexArray(0);

		// Then, we set the light's VAO (VBO stays the same. After all, the vertices are the same for the light object (also a 3D cube))
		//	GLuint lightVAO;
		glGenVertexArrays(1, &lightVAO);
		glBindVertexArray(lightVAO);
		// We only need to bind to the VBO (to link it with glVertexAttribPointer), no need to fill it; the VBO's data already contains all we need.
		glBindBuffer(GL_ARRAY_BUFFER, VBO);
		// Set the vertex attributes (only position data for the lamp))
		glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 6 * sizeof(GLfloat), (GLvoid*)0); // Note that we skip over the normal vectors
		glEnableVertexAttribArray(0);
		glBindVertexArray(0);
		return 0;
	}

	__declspec(dllexport) int RenderDiffuseLesson()
	{

		// Calculate deltatime of current frame
		GLfloat currentFrame = glfwGetTime();
		deltaTime = currentFrame - lastFrame;
		lastFrame = currentFrame;

		// Check if any events have been activiated (key pressed, mouse moved etc.) and call corresponding response functions
		glfwPollEvents();
		processInput(sceneWindow);

		// Clear the colorbuffer
		glClearColor(0.1f, 0.1f, 0.1f, 1.0f);
		glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

		// Use cooresponding shader when setting uniforms/drawing objects
		lightingShader->Use();

		lightingShader->SetVec3(camera->Position.x, camera->Position.y, camera->Position.z, "viewPosition");


		lightingShader->SetVec3(1.0f, 0.5f, 0.31f, "objectColor");
		lightingShader->SetVec3(1.0f, 1.0f, 1.0f, "lightColor");
		lightingShader->SetVec3(lightPos.x, lightPos.y, lightPos.z, "lightPosition");

		// Create camera transformations
		glm::mat4 view;
		view = camera->GetViewMatrix();
		glm::mat4 projection = glm::perspective(camera->Zoom, (GLfloat)WIDTH / (GLfloat)HEIGHT, 0.1f, 100.0f);
		// Get the uniform locations
		lightingShader->setMat4("view", view);
		lightingShader->setMat4("projection", projection);

		// Draw the container (using container's vertex attributes)
		glBindVertexArray(containerVAO);
		glm::mat4 model;
		/*glUniformMatrix4fv(modelLoc, 1, GL_FALSE, glm::value_ptr(model));*/
		lightingShader->setMat4("model", model);


		glDrawArrays(GL_TRIANGLES, 0, 36);
		glBindVertexArray(0);

		// Also draw the lamp object, again binding the appropriate shader
		lampShader->Use();
		lampShader->setMat4("view", view);
		lampShader->setMat4("projection", projection);

		model = glm::mat4();
		model = glm::translate(model, lightPos);
		model = glm::scale(model, glm::vec3(0.05f)); // Make it a smaller cube
		lampShader->setMat4("model", model);

		// Draw the light object (using light's vertex attributes)
		glBindVertexArray(lightVAO);
		glDrawArrays(GL_TRIANGLES, 0, 36);
		glBindVertexArray(0);

		// Swap the screen buffers
		glfwSwapBuffers(sceneWindow);

		return 0;
	}

	__declspec(dllexport) int EntireDiffuseLesson()
	{

		glfwInit();
		// Set all the required options for GLFW
		glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
		glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 3);
		glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);
		glfwWindowHint(GLFW_RESIZABLE, GL_FALSE);

		// Create a GLFWwindow object that we can use for GLFW's functions
		sceneWindow = glfwCreateWindow(screenWidth, screenHeight, "Diffuse Lighting", nullptr, nullptr);
		glfwMakeContextCurrent(sceneWindow);

		// Set the required callback functions
		glfwSetKeyCallback(sceneWindow, key_callback);
		glfwSetCursorPosCallback(sceneWindow, mouse_callback);
		glfwSetScrollCallback(sceneWindow, scroll_callback);

		// GLFW Options
		glfwSetInputMode(sceneWindow, GLFW_CURSOR, GLFW_CURSOR_DISABLED);

		// Set this to true so GLEW knows to use a modern approach to retrieving function pointers and extensions
		glewExperimental = GL_TRUE;
		// Initialize GLEW to setup the OpenGL Function pointers
		glewInit();

		// Define the viewport dimensions
		glViewport(0, 0, screenWidth, screenHeight);

		// OpenGL options
		glEnable(GL_DEPTH_TEST);


		string d = exeDirectory();
		string vShaderDir = d + "Shaders\\openglexamples\\vertexDiffuse.txt";
		const char * vSource = vShaderDir.c_str();
		cout << "Getting vert shader from dir: " << vShaderDir << endl;
		string fragDir = d + "Shaders\\openglexamples\\fragmentDiffuse.txt";
		const char *fragSource = fragDir.c_str();
		cout << "Getting frag shader from " << fragSource << endl;
		lightingShader = new Shader(vSource, fragSource);

		//lamp debugging shader
		vShaderDir = d + "Shaders\\openglexamples\\vertexSimpleLighting.txt";
		vSource = vShaderDir.c_str();
		fragDir = d + "Shaders\\openglexamples\\fragmentLightShader.txt";
		fragSource = fragDir.c_str();
		lampShader = new Shader(vSource, fragSource);

		// Set up vertex data (and buffer(s)) and attribute pointers
		GLfloat vertices[] = {
			-0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
			0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
			0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
			0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
			-0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
			-0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,

			-0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
			0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
			0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
			0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
			-0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
			-0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,

			-0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,
			-0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
			-0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
			-0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
			-0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,
			-0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,

			0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,
			0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
			0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
			0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
			0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,
			0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,

			-0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,
			0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,
			0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
			0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
			-0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
			-0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,

			-0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,
			0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,
			0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
			0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
			-0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
			-0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f
		};
		// First, set the container's VAO (and VBO)
		//GLuint VBO, containerVAO;
		glGenVertexArrays(1, &containerVAO);
		glGenBuffers(1, &VBO);

		glBindBuffer(GL_ARRAY_BUFFER, VBO);
		glBufferData(GL_ARRAY_BUFFER, sizeof(vertices), vertices, GL_STATIC_DRAW);

		glBindVertexArray(containerVAO);
		// Position attribute
		glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 6 * sizeof(GLfloat), (GLvoid*)0);
		glEnableVertexAttribArray(0);
		// Normal attribute
		glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 6 * sizeof(GLfloat), (GLvoid*)(3 * sizeof(GLfloat)));
		glEnableVertexAttribArray(1);
		glBindVertexArray(0);

		// Then, we set the light's VAO (VBO stays the same. After all, the vertices are the same for the light object (also a 3D cube))
		//	GLuint lightVAO;
		glGenVertexArrays(1, &lightVAO);
		glBindVertexArray(lightVAO);
		// We only need to bind to the VBO (to link it with glVertexAttribPointer), no need to fill it; the VBO's data already contains all we need.
		glBindBuffer(GL_ARRAY_BUFFER, VBO);
		// Set the vertex attributes (only position data for the lamp))
		glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 6 * sizeof(GLfloat), (GLvoid*)0); // Note that we skip over the normal vectors
		glEnableVertexAttribArray(0);
		glBindVertexArray(0);


		// Game loop
		while (!glfwWindowShouldClose(sceneWindow))
		{
			// Calculate deltatime of current frame
			GLfloat currentFrame = glfwGetTime();
			deltaTime = currentFrame - lastFrame;
			lastFrame = currentFrame;

			// Check if any events have been activiated (key pressed, mouse moved etc.) and call corresponding response functions
			glfwPollEvents();
			processInput(sceneWindow);

			// Clear the colorbuffer
			glClearColor(0.1f, 0.1f, 0.1f, 1.0f);
			glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

			// Use cooresponding shader when setting uniforms/drawing objects
			lightingShader->Use();

			lightingShader->SetVec3(1.0f, 0.5f, 0.31f, "objectColor");
			lightingShader->SetVec3(1.0f, 1.0f, 1.0f, "lightColor");
			lightingShader->SetVec3(lightPos.x, lightPos.y, lightPos.z, "lightPosition");

			// Create camera transformations
			glm::mat4 view;
			view = camera->GetViewMatrix();
			glm::mat4 projection = glm::perspective(camera->Zoom, (GLfloat)WIDTH / (GLfloat)HEIGHT, 0.1f, 100.0f);
			// Get the uniform locations
			lightingShader->setMat4("view", view);
			lightingShader->setMat4("projection", projection);

			// Draw the container (using container's vertex attributes)
			glBindVertexArray(containerVAO);
			glm::mat4 model;
			/*glUniformMatrix4fv(modelLoc, 1, GL_FALSE, glm::value_ptr(model));*/
			lightingShader->setMat4("model", model);


			glDrawArrays(GL_TRIANGLES, 0, 36);
			glBindVertexArray(0);

			// Also draw the lamp object, again binding the appropriate shader
			lampShader->Use();
			lampShader->setMat4("view", view);
			lampShader->setMat4("projection", projection);

			model = glm::mat4();
			model = glm::translate(model, lightPos);
			model = glm::scale(model, glm::vec3(0.05f)); // Make it a smaller cube
			lampShader->setMat4("model", model);

			// Draw the light object (using light's vertex attributes)
			glBindVertexArray(lightVAO);
			glDrawArrays(GL_TRIANGLES, 0, 36);
			glBindVertexArray(0);

			// Swap the screen buffers
			glfwSwapBuffers(sceneWindow);
		}

		// Terminate GLFW, clearing any resources allocated by GLFW.
		glfwTerminate();
		return 0;
	}

	__declspec(dllexport) int InitiateLightingLesson()
	{
		// Init GLFW
		glfwInit();
		// Set all the required options for GLFW
		glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
		glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 3);
		glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);
		glfwWindowHint(GLFW_RESIZABLE, GL_FALSE);

		// Create a GLFWwindow object that we can use for GLFW's functions
		sceneWindow = glfwCreateWindow(WIDTH, HEIGHT, "Basic Lighting Lesson", nullptr, nullptr);
		glfwMakeContextCurrent(sceneWindow);

		// Set the required callback functions
		glfwSetKeyCallback(sceneWindow, key_callback);
		glfwSetCursorPosCallback(sceneWindow, mouse_callback);
		glfwSetScrollCallback(sceneWindow, scroll_callback);

		// GLFW Options
		glfwSetInputMode(sceneWindow, GLFW_CURSOR, GLFW_CURSOR_DISABLED);

		// Set this to true so GLEW knows to use a modern approach to retrieving function pointers and extensions
		glewExperimental = GL_TRUE;
		// Initialize GLEW to setup the OpenGL Function pointers
		glewInit();

		// Define the viewport dimensions
		glViewport(0, 0, WIDTH, HEIGHT);

		// OpenGL options
		glEnable(GL_DEPTH_TEST);


		//this will obviously have to change
		// Build and compile our shader program
		/*	Shader lightingShader("path/to/shaders/lighting.vs", "path/to/shaders/lighting.frag");
		Shader lampShader("path/to/shaders/lamp.vs", "path/to/shaders/lamp.frag");*/

		string d = exeDirectory();
		string vShaderDir = d + "Shaders\\openglexamples\\vertexSimpleLighting.txt";
		const char * vSource = vShaderDir.c_str();
		cout << "Getting vert shader from dir: " << vShaderDir << endl;
		string fragDir = d + "Shaders\\openglexamples\\fragmentSimpleLighting.txt";
		const char *fragSource = fragDir.c_str();
		cout << "Getting frag shader from " << fragSource << endl;
		lightingShader = new Shader(vSource, fragSource);
		fragDir = d + "Shaders\\openglexamples\\fragmentLightShader.txt";
		fragSource = fragDir.c_str();
		lampShader = new Shader(vSource, fragSource);




		// Set up vertex data (and buffer(s)) and attribute pointers
		GLfloat vertices[] = {
			-0.5f, -0.5f, -0.5f,
			0.5f, -0.5f, -0.5f,
			0.5f,  0.5f, -0.5f,
			0.5f,  0.5f, -0.5f,
			-0.5f,  0.5f, -0.5f,
			-0.5f, -0.5f, -0.5f,

			-0.5f, -0.5f,  0.5f,
			0.5f, -0.5f,  0.5f,
			0.5f,  0.5f,  0.5f,
			0.5f,  0.5f,  0.5f,
			-0.5f,  0.5f,  0.5f,
			-0.5f, -0.5f,  0.5f,

			-0.5f,  0.5f,  0.5f,
			-0.5f,  0.5f, -0.5f,
			-0.5f, -0.5f, -0.5f,
			-0.5f, -0.5f, -0.5f,
			-0.5f, -0.5f,  0.5f,
			-0.5f,  0.5f,  0.5f,

			0.5f,  0.5f,  0.5f,
			0.5f,  0.5f, -0.5f,
			0.5f, -0.5f, -0.5f,
			0.5f, -0.5f, -0.5f,
			0.5f, -0.5f,  0.5f,
			0.5f,  0.5f,  0.5f,

			-0.5f, -0.5f, -0.5f,
			0.5f, -0.5f, -0.5f,
			0.5f, -0.5f,  0.5f,
			0.5f, -0.5f,  0.5f,
			-0.5f, -0.5f,  0.5f,
			-0.5f, -0.5f, -0.5f,

			-0.5f,  0.5f, -0.5f,
			0.5f,  0.5f, -0.5f,
			0.5f,  0.5f,  0.5f,
			0.5f,  0.5f,  0.5f,
			-0.5f,  0.5f,  0.5f,
			-0.5f,  0.5f, -0.5f
		};
		// First, set the container's VAO (and VBO)
		//GLuint  containerVAO;
		glGenVertexArrays(1, &containerVAO);
		glGenBuffers(1, &VBO);

		glBindBuffer(GL_ARRAY_BUFFER, VBO);
		glBufferData(GL_ARRAY_BUFFER, sizeof(vertices), vertices, GL_STATIC_DRAW);

		glBindVertexArray(containerVAO);
		// Position attribute
		glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 3 * sizeof(GLfloat), (GLvoid*)0);
		glEnableVertexAttribArray(0);
		glBindVertexArray(0);

		// Then, we set the light's VAO (VBO stays the same. After all, the vertices are the same for the light object (also a 3D cube))
		//	GLuint lightVAO;
		glGenVertexArrays(1, &lightVAO);
		glBindVertexArray(lightVAO);
		// We only need to bind to the VBO (to link it with glVertexAttribPointer), no need to fill it; the VBO's data already contains all we need.
		glBindBuffer(GL_ARRAY_BUFFER, VBO);
		// Set the vertex attributes (only position data for the lamp))
		glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 3 * sizeof(GLfloat), (GLvoid*)0);
		glEnableVertexAttribArray(0);
		glBindVertexArray(0);

		return 0;
	}

	__declspec(dllexport) int RenderLightingLesson()
	{
		// Calculate deltatime of current frame
		GLfloat currentFrame = glfwGetTime();
		deltaTime = currentFrame - lastFrame;
		lastFrame = currentFrame;

		// Check if any events have been activiated (key pressed, mouse moved etc.) and call corresponding response functions
		glfwPollEvents();
		//do_movement();
		processInput(sceneWindow);

		// Clear the colorbuffer
		glClearColor(0.1f, 0.1f, 0.1f, 1.0f);
		glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

		// Use cooresponding shader when setting uniforms/drawing objects
		lightingShader->Use();
		lightingShader->SetVec3(1.0f, 0.5f, 0.31f, "objectColor");
		lightingShader->SetVec3(1.0f, 0.5f, 1.0f, "lightColor");

		// Create camera transformations
		glm::mat4 view;
		view = camera->GetViewMatrix();
		glm::mat4 projection = glm::perspective(camera->Zoom, (GLfloat)WIDTH / (GLfloat)HEIGHT, 0.1f, 100.0f);


		lightingShader->setMat4("view", view);
		lightingShader->setMat4("projection", projection);

		// Draw the container (using container's vertex attributes)
		glBindVertexArray(containerVAO);
		glm::mat4 model;
		lightingShader->setMat4("model", model);
		glDrawArrays(GL_TRIANGLES, 0, 36);
		glBindVertexArray(0);

		// Also draw the lamp object, again binding the appropriate shader
		lampShader->Use();

		lampShader->setMat4("view", view);
		lampShader->setMat4("projection", projection);

		model = glm::mat4();
		model = glm::translate(model, lightPos);
		model = glm::scale(model, glm::vec3(0.2f)); // Make it a smaller cube
		lampShader->setMat4("model", model);
		// Draw the light object (using light's vertex attributes)
		glBindVertexArray(lightVAO);
		glDrawArrays(GL_TRIANGLES, 0, 36);
		glBindVertexArray(0);

		// Swap the screen buffers
		glfwSwapBuffers(sceneWindow);

		return 0;
	}

	__declspec(dllexport) int InitiateCameraLesson()
	{

		// Init GLFW
		glfwInit();
		glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
		glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 3);
		glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);
		glfwWindowHint(GLFW_RESIZABLE, GL_FALSE);
		//glfwWindowHint(GLFW_SAMPLES, 4);

		sceneWindow = glfwCreateWindow(screenWidth, screenHeight, "Camera Lesson", nullptr, nullptr); // Windowed
		glfwMakeContextCurrent(sceneWindow);

		// Set the required callback functions
		glfwSetKeyCallback(sceneWindow, key_callback);
		glfwSetCursorPosCallback(sceneWindow, mouse_callback);
		//glfwSetScrollCallback(sceneWindow, scroll_callback);

		// Options
		glfwSetInputMode(sceneWindow, GLFW_CURSOR, GLFW_CURSOR_DISABLED);

		// Initialize GLEW to setup the OpenGL Function pointers
		glewExperimental = GL_TRUE;
		glewInit();

		// Define the viewport dimensions
		glViewport(0, 0, screenWidth, screenHeight);

		// Setup some OpenGL options
		glEnable(GL_DEPTH_TEST);

		string d = exeDirectory();
		string vShaderDir = d + "Shaders\\openglexamples\\vertexPerspective.txt";
		const char * vSource = vShaderDir.c_str();
		cout << "Getting vert shader from dir: " << vShaderDir << endl;
		string fragDir = d + "Shaders\\openglexamples\\fragmentTextured.txt";
		const char *fragSource = fragDir.c_str();
		cout << "Getting frag shader from " << fragSource << endl;
		myShader = new Shader(vSource, fragSource);

		// Set up our vertex data (and buffer(s)) and attribute pointers
		GLfloat vertices[] = {
			-0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
			0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
			0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
			0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
			-0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
			-0.5f, -0.5f, -0.5f,  0.0f, 0.0f,

			-0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
			0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
			0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
			0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
			-0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
			-0.5f, -0.5f,  0.5f,  0.0f, 0.0f,

			-0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
			-0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
			-0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
			-0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
			-0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
			-0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

			0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
			0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
			0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
			0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
			0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
			0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

			-0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
			0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
			0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
			0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
			-0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
			-0.5f, -0.5f, -0.5f,  0.0f, 1.0f,

			-0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
			0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
			0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
			0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
			-0.5f,  0.5f,  0.5f,  0.0f, 0.0f,
			-0.5f,  0.5f, -0.5f,  0.0f, 1.0f
		};
		//GLuint VBO, VAO;
		glGenVertexArrays(1, &VAO);
		glGenBuffers(1, &VBO);
		// Bind our Vertex Array Object first, then bind and set our buffers and pointers.
		glBindVertexArray(VAO);

		glBindBuffer(GL_ARRAY_BUFFER, VBO);
		glBufferData(GL_ARRAY_BUFFER, sizeof(vertices), vertices, GL_STATIC_DRAW);

		// Position attribute
		glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 5 * sizeof(GLfloat), (GLvoid*)0);
		glEnableVertexAttribArray(0);
		// TexCoord attribute
		glVertexAttribPointer(1, 2, GL_FLOAT, GL_FALSE, 5 * sizeof(GLfloat), (GLvoid*)(3 * sizeof(GLfloat)));
		glEnableVertexAttribArray(1);//shouldn't this be 1?! Yes, it should.

		glBindBuffer(GL_ARRAY_BUFFER, 0);
		glBindVertexArray(0); // Unbind VAO
							  //glBindBuffer(GL_ARRAY_BUFFER, 0);



		string imaged = d + "textures\\container.jpg";
		const char *iDir = imaged.c_str();

		// Load and create a texture 

		texture = CreateTexture(iDir, GL_REPEAT, GL_LINEAR);

		string hap = d + "textures\\awesomeface.png";
		iDir = hap.c_str();

		texture2 = CreateTexture(iDir, GL_REPEAT, GL_LINEAR);

		//enable blending for text display
		glEnable(GL_BLEND);
		glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);

		return 0;
	}

	__declspec(dllexport) int RenderCameraLesson()
	{
		// Set frame time
		GLfloat currentFrame = glfwGetTime();
		deltaTime = currentFrame - lastFrame;
		lastFrame = currentFrame;


		// input
		// -----
		processInput(sceneWindow);


		// Clear the colorbuffer
		glClearColor(0.2f, 0.3f, 0.3f, 1.0f);
		glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

		// Draw our first triangle
		myShader->Use();

		// Bind Textures using texture units
		glActiveTexture(GL_TEXTURE0);
		glBindTexture(GL_TEXTURE_2D, texture);
		myShader->SetInt("ourTexture1", 0);
		glActiveTexture(GL_TEXTURE1);
		glBindTexture(GL_TEXTURE_2D, texture2);
		myShader->SetInt("ourTexture2", 1);

		// Create camera transformation
		glm::mat4 view;
		view = camera->GetViewMatrix();
		glm::mat4 projection;
		projection = glm::perspective(camera->Zoom, (float)screenWidth / (float)screenHeight, 0.1f, 1000.0f);
		myShader->setMat4("view", view);
		myShader->setMat4("projection", projection);



		glBindVertexArray(VAO);
		for (GLuint i = 0; i < 10; i++)
		{
			// Calculate the model matrix for each object and pass it to shader before drawing
			glm::mat4 model;
			model = glm::translate(model, cubePositions[i]);
			GLfloat angle = 20.0f * i;
			model = glm::rotate(model, angle, glm::vec3(1.0f, 0.3f, 0.5f));
			myShader->setMat4("model", model);

			glDrawArrays(GL_TRIANGLES, 0, 36);
		}
		glBindVertexArray(0);
		// Swap the buffers
		glfwSwapBuffers(sceneWindow);
		glfwPollEvents();

		return 0;
	}
	//now that you've done this, beef up the shader class to find location of prog and set uniform
	__declspec(dllexport) int InitiateCoordinateSystemLesson()
	{
		glfwInit();
		// Set all the required options for GLFW
		glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
		glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 3);
		glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);
		glfwWindowHint(GLFW_RESIZABLE, GL_FALSE);

		// Create a GLFWwindow object that we can use for GLFW's functions
		sceneWindow = glfwCreateWindow(WIDTH, HEIGHT, "LearnOpenGL", nullptr, nullptr);
		glfwMakeContextCurrent(sceneWindow);

		// Set the required callback functions
		glfwSetKeyCallback(sceneWindow, key_callback);

		// Set this to true so GLEW knows to use a modern approach to retrieving function pointers and extensions
		glewExperimental = GL_TRUE;
		// Initialize GLEW to setup the OpenGL Function pointers
		glewInit();

		// Define the viewport dimensions
		glViewport(0, 0, WIDTH, HEIGHT);

		// Setup OpenGL options
		glEnable(GL_DEPTH_TEST);



		//the shader for this box only accepts two attributes: location and texCoord
		string d = exeDirectory();
		string vShaderDir = d + "Shaders\\openglexamples\\vertexPerspective.txt";
		const char * vSource = vShaderDir.c_str();
		cout << "Getting vert shader from dir: " << vShaderDir << endl;
		string fragDir = d + "Shaders\\openglexamples\\fragmentTextured.txt";
		const char *fragSource = fragDir.c_str();
		cout << "Getting frag shader from " << fragSource << endl;
		myShader = new Shader(vSource, fragSource);

		// Build and compile our shader program
		//Shader ourShader("path/to/shaders/shader.vs", "path/to/shaders/shader.frag");


		// Set up vertex data (and buffer(s)) and attribute pointers
		GLfloat vertices[] = {
			-0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
			0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
			0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
			0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
			-0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
			-0.5f, -0.5f, -0.5f,  0.0f, 0.0f,

			-0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
			0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
			0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
			0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
			-0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
			-0.5f, -0.5f,  0.5f,  0.0f, 0.0f,

			-0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
			-0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
			-0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
			-0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
			-0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
			-0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

			0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
			0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
			0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
			0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
			0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
			0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

			-0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
			0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
			0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
			0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
			-0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
			-0.5f, -0.5f, -0.5f,  0.0f, 1.0f,

			-0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
			0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
			0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
			0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
			-0.5f,  0.5f,  0.5f,  0.0f, 0.0f,
			-0.5f,  0.5f, -0.5f,  0.0f, 1.0f
		};
		//	GLuint VBO, VAO;
		glGenVertexArrays(1, &VAO);
		glGenBuffers(1, &VBO);

		glBindVertexArray(VAO);

		glBindBuffer(GL_ARRAY_BUFFER, VBO);
		glBufferData(GL_ARRAY_BUFFER, sizeof(vertices), vertices, GL_STATIC_DRAW);

		// Position attribute
		glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 5 * sizeof(GLfloat), (GLvoid*)0);
		glEnableVertexAttribArray(0);
		// TexCoord attribute
		glVertexAttribPointer(1, 2, GL_FLOAT, GL_FALSE, 5 * sizeof(GLfloat), (GLvoid*)(3 * sizeof(GLfloat)));
		glEnableVertexAttribArray(1);

		//it turns out that it's CRUCIAL to unbind both the buffer and vertex array before configuring textures/other buffers
		glBindBuffer(GL_ARRAY_BUFFER, 0);
		glBindVertexArray(0); // Unbind VAO


		string imaged = d + "textures\\container.jpg";
		const char *iDir = imaged.c_str();

		// Load and create a texture 

		texture = CreateTexture(iDir, GL_REPEAT, GL_LINEAR);

		string hap = d + "textures\\awesomeface.png";
		iDir = hap.c_str();

		texture2 = CreateTexture(iDir, GL_REPEAT, GL_LINEAR);

		//enable blending for text display
		glEnable(GL_BLEND);
		glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
		return 0;
	}
	__declspec(dllexport) int RenderCoordinateSystemLesson()
	{
		// Check if any events have been activiated (key pressed, mouse moved etc.) and call corresponding response functions
		glfwPollEvents();

		// Render
		// Clear the colorbuffer
		glClearColor(0.2f, 0.3f, 0.3f, 1.0f);
		glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);


		// Activate shader
		myShader->Use();
		// Bind Textures using texture units
		glActiveTexture(GL_TEXTURE0);
		glBindTexture(GL_TEXTURE_2D, texture);
		glUniform1i(glGetUniformLocation(myShader->Program, "ourTexture1"), 0);
		glActiveTexture(GL_TEXTURE1);
		glBindTexture(GL_TEXTURE_2D, texture2);
		glUniform1i(glGetUniformLocation(myShader->Program, "ourTexture2"), 1);


		// Create transformations
		glm::mat4 model;
		glm::mat4 view;
		glm::mat4 projection;
		model = glm::rotate(model, (GLfloat)glfwGetTime() * 50.0f, glm::vec3(0.5f, 1.0f, 0.0f));
		view = glm::translate(view, glm::vec3(0.0f, 0.0f, -3.0f));
		// Note: currently we set the projection matrix each frame, but since the projection matrix rarely changes it's often best practice to set it outside the main loop only once.
		projection = glm::perspective(45.0f, (GLfloat)WIDTH / (GLfloat)HEIGHT, 0.1f, 100.0f);

		myShader->setMat4("model", model);
		myShader->setMat4("view", view);
		myShader->setMat4("projection", projection);

		// Draw container
		glBindVertexArray(VAO);
		glDrawArrays(GL_TRIANGLES, 0, 36);
		glBindVertexArray(0);

		// Swap the screen buffers
		glfwSwapBuffers(sceneWindow);
		return 0;
	}

	__declspec(dllexport) int EntireCoordinateSystemLesson()
	{
		glfwInit();
		// Set all the required options for GLFW
		glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
		glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 3);
		glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);
		glfwWindowHint(GLFW_RESIZABLE, GL_FALSE);

		// Create a GLFWwindow object that we can use for GLFW's functions
		GLFWwindow* window = glfwCreateWindow(WIDTH, HEIGHT, "LearnOpenGL", nullptr, nullptr);
		glfwMakeContextCurrent(window);

		// Set the required callback functions
		glfwSetKeyCallback(window, key_callback);

		// Set this to true so GLEW knows to use a modern approach to retrieving function pointers and extensions
		glewExperimental = GL_TRUE;
		// Initialize GLEW to setup the OpenGL Function pointers
		glewInit();

		// Define the viewport dimensions
		glViewport(0, 0, WIDTH, HEIGHT);

		// Setup OpenGL options
		glEnable(GL_DEPTH_TEST);



		//the shader for this box only accepts two attributes: location and texCoord
		string d = exeDirectory();
		string vShaderDir = d + "Shaders\\openglexamples\\vertexPerspective.txt";
		const char * vSource = vShaderDir.c_str();
		cout << "Getting vert shader from dir: " << vShaderDir << endl;
		string fragDir = d + "Shaders\\openglexamples\\fragmentTextured.txt";
		const char *fragSource = fragDir.c_str();
		cout << "Getting frag shader from " << fragSource << endl;
		myShader = new Shader(vSource, fragSource);

		// Build and compile our shader program
		//Shader ourShader("path/to/shaders/shader.vs", "path/to/shaders/shader.frag");


		// Set up vertex data (and buffer(s)) and attribute pointers
		GLfloat vertices[] = {
			-0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
			0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
			0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
			0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
			-0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
			-0.5f, -0.5f, -0.5f,  0.0f, 0.0f,

			-0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
			0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
			0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
			0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
			-0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
			-0.5f, -0.5f,  0.5f,  0.0f, 0.0f,

			-0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
			-0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
			-0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
			-0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
			-0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
			-0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

			0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
			0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
			0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
			0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
			0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
			0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

			-0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
			0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
			0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
			0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
			-0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
			-0.5f, -0.5f, -0.5f,  0.0f, 1.0f,

			-0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
			0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
			0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
			0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
			-0.5f,  0.5f,  0.5f,  0.0f, 0.0f,
			-0.5f,  0.5f, -0.5f,  0.0f, 1.0f
		};
		//	GLuint VBO, VAO;
		glGenVertexArrays(1, &VAO);
		glGenBuffers(1, &VBO);

		glBindVertexArray(VAO);

		glBindBuffer(GL_ARRAY_BUFFER, VBO);
		glBufferData(GL_ARRAY_BUFFER, sizeof(vertices), vertices, GL_STATIC_DRAW);

		// Position attribute
		glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 5 * sizeof(GLfloat), (GLvoid*)0);
		glEnableVertexAttribArray(0);
		// TexCoord attribute
		glVertexAttribPointer(1, 2, GL_FLOAT, GL_FALSE, 5 * sizeof(GLfloat), (GLvoid*)(3 * sizeof(GLfloat)));
		glEnableVertexAttribArray(1);

		//it turns out that it's CRUCIAL to unbind both the buffer and vertex array before configuring textures/other buffers
		glBindBuffer(GL_ARRAY_BUFFER, 0);
		glBindVertexArray(0); // Unbind VAO


		string imaged = d + "textures\\container.jpg";
		const char *iDir = imaged.c_str();

		// Load and create a texture 

		texture = CreateTexture(iDir, GL_REPEAT, GL_LINEAR);

		string hap = d + "textures\\awesomeface.png";
		iDir = hap.c_str();

		texture2 = CreateTexture(iDir, GL_REPEAT, GL_LINEAR);

		//enable blending for text display
		glEnable(GL_BLEND);
		glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);

		// Game loop
		while (!glfwWindowShouldClose(window))
		{
			// Check if any events have been activiated (key pressed, mouse moved etc.) and call corresponding response functions
			glfwPollEvents();

			// Render
			// Clear the colorbuffer
			glClearColor(0.2f, 0.3f, 0.3f, 1.0f);
			glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);


			// Activate shader
			myShader->Use();


			// Bind Textures using texture units
			glActiveTexture(GL_TEXTURE0);
			glBindTexture(GL_TEXTURE_2D, texture);
			glUniform1i(glGetUniformLocation(myShader->Program, "ourTexture1"), 0);
			glActiveTexture(GL_TEXTURE1);
			glBindTexture(GL_TEXTURE_2D, texture2);
			glUniform1i(glGetUniformLocation(myShader->Program, "ourTexture2"), 1);


			// Create transformations
			glm::mat4 model;
			glm::mat4 view;
			glm::mat4 projection;
			model = glm::rotate(model, (GLfloat)glfwGetTime() * 50.0f, glm::vec3(0.5f, 1.0f, 0.0f));
			view = glm::translate(view, glm::vec3(0.0f, 0.0f, -3.0f));
			// Note: currently we set the projection matrix each frame, but since the projection matrix rarely changes it's often best practice to set it outside the main loop only once.
			projection = glm::perspective(45.0f, (GLfloat)WIDTH / (GLfloat)HEIGHT, 0.1f, 100.0f);
			// Get their uniform location
			GLint modelLoc = glGetUniformLocation(myShader->Program, "model");
			GLint viewLoc = glGetUniformLocation(myShader->Program, "view");
			GLint projLoc = glGetUniformLocation(myShader->Program, "projection");
			// Pass them to the shaders
			glUniformMatrix4fv(modelLoc, 1, GL_FALSE, glm::value_ptr(model));
			glUniformMatrix4fv(viewLoc, 1, GL_FALSE, glm::value_ptr(view));
			glUniformMatrix4fv(projLoc, 1, GL_FALSE, glm::value_ptr(projection));

			// Draw container
			glBindVertexArray(VAO);
			glDrawArrays(GL_TRIANGLES, 0, 36);
			glBindVertexArray(0);

			// Swap the screen buffers
			glfwSwapBuffers(window);
		}
		// Properly de-allocate all resources once they've outlived their purpose
		glDeleteVertexArrays(1, &VAO);
		glDeleteBuffers(1, &VBO);
		// Terminate GLFW, clearing any resources allocated by GLFW.
		glfwTerminate();
		return 0;
	}

	__declspec(dllexport) int Initiate()
	{

		std::cout << "Initiating GLFW context, OpenGL 3.3 from stand alone method" << std::endl;
		// Init GLFW
		glfwInit();
		// Set all the required options for GLFW
		glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
		glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 3);
		glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);
		glfwWindowHint(GLFW_RESIZABLE, GL_FALSE);

		// Create a GLFWwindow object that we can use for GLFW's functions
		sceneWindow = glfwCreateWindow(screenWidth, screenHeight, "Graphics Pipeline", nullptr, nullptr);
		if (sceneWindow == nullptr)
		{
			std::cout << "Failed to create GLFW window" << std::endl;
			glfwTerminate();
			return -1;
		}
		glfwMakeContextCurrent(sceneWindow);
		// Set the required callback functions
		glfwSetKeyCallback(sceneWindow, key_callback);


		// Set this to true so GLEW knows to use a modern approach to retrieving function pointers and extensions
		glewExperimental = GL_TRUE;
		// Initialize GLEW to setup the OpenGL Function pointers
		if (glewInit() != GLEW_OK)
		{
			std::cout << "Failed to initialize GLEW" << std::endl;
			return -1;
		}

		GLint nrAttributes;
		glGetIntegerv(GL_MAX_VERTEX_ATTRIBS, &nrAttributes);
		std::cout << "Maximum nr of vertex attributes supported: " << nrAttributes
			<< std::endl;


		// Define the viewport dimensions
		int width, height;
		glfwGetFramebufferSize(sceneWindow, &width, &height);
		glViewport(0, 0, width, height);

		//make some vertices
		GLfloat triangleVerts[] = {
			-1.0f,0.0f, 0.0f,
			1.0f, 0.0f, 0.0f,
			0.0f, 1.0f, 0.0f
		};

		GLfloat vertices[] = {
			0.5f, 0.5f, 0.0f, // Top Right
			0.5f, -0.5f, 0.0f, // Bottom Right
			-0.5f, -0.5f, 0.0f, // Bottom Left
			-0.5f, 0.5f, 0.0f // Top Left
		};
		GLuint indices[] = { // Note that we start from 0!
			0, 1, 3, // First Triangle
			1, 2, 3 // Second Triangle
		};
		GLfloat vertices2[] = {
			// Positions          // Colors           // Texture Coords
			0.5f,  0.5f, 0.0f,   1.0f, 0.0f, 0.0f,   1.0f, 1.0f, // Top Right
			0.5f, -0.5f, 0.0f,   0.0f, 1.0f, 0.0f,   1.0f, 0.0f, // Bottom Right
			-0.5f, -0.5f, 0.0f,   0.0f, 0.0f, 1.0f,   0.0f, 0.0f, // Bottom Left
			-0.5f,  0.5f, 0.0f,   1.0f, 1.0f, 0.0f,   0.0f, 1.0f  // Top Left 
		};


		glGenVertexArrays(1, &VAO);

		glGenBuffers(1, &VBO);
		glGenBuffers(1, &EBO);

		glBindVertexArray(VAO);

		glBindBuffer(GL_ARRAY_BUFFER, VBO);
		glBufferData(GL_ARRAY_BUFFER, sizeof(vertices2), vertices2, GL_STATIC_DRAW);

		glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, EBO);
		glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(indices), indices, GL_STATIC_DRAW);
		//position
		glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 8 * sizeof(float), (GLvoid*)0);
		glEnableVertexAttribArray(0);
		//color
		glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 8 * sizeof(float), (GLvoid*)(3 * sizeof(float)));
		glEnableVertexAttribArray(1);
		//tex coords
		glVertexAttribPointer(2, 2, GL_FLOAT, GL_FALSE, 8 * sizeof(float), (GLvoid*)(6 * sizeof(float)));
		glEnableVertexAttribArray(2);

		//A simplified explanation of vertexAttribPointer's:
		//position number, how many things, type of things, (bool) normalize things, total size of all the things passed, offset



		glBindBuffer(GL_ARRAY_BUFFER, 0);
		glBindVertexArray(0);


		//glBindBuffer(GL_ARRAY_BUFFER, VBO);
		//configure the buffer
		//glBufferData(GL_ARRAY_BUFFER, sizeof(triangleVerts), triangleVerts, GL_STATIC_DRAW);
		//• GL_STATIC_DRAW : the data will most likely not change at all or very rarely.
		//• GL_DYNAMIC_DRAW : the data is likely to change a lot.
		//• GL_STREAM_DRAW : the data will change every time it is drawn.

		//configure the vertex attributes
		//glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 3 * sizeof(GLfloat), (GLvoid*)0);
		//params of this arg
		//1. the location to configure (which corresponds to the location in the shader)
		//2. how many "things" this location will consist of
		//3. what type of "thing" will be passed
		//4. should the data be normalized?
		//5. stride length - the length of data over which to stride as it gets processed
		//6. this is the offset of the data (where it begins) in the buffer
		//   if there was another position, there would be an offset of 3*sizeof(GLfloat)



		//Now we compile and link our two very basic shaders, vert and frag
		string d = exeDirectory();
		//string vShaderDir = d + "Shaders\\vertexBasic.txt";
		string vShaderDir = d + "Shaders\\vertexPerspective.txt";
		const char * vSource = vShaderDir.c_str();
		cout << "Getting vert shader from dir: " << vShaderDir << endl;


		//string fragDir = d + "Shaders\\fragmentBasic.txt";
		string fragDir = d + "Shaders\\fragmentTextured.txt";
		const char *fragSource = fragDir.c_str();
		cout << "Getting frag shader from " << fragSource << endl;

		myShader = new Shader(vSource, fragSource);


		string imaged = d + "textures\\container.jpg";
		const char *iDir = imaged.c_str();

		// Load and create a texture 

		texture = CreateTexture(iDir, GL_REPEAT, GL_LINEAR);

		string hap = d + "textures\\awesomeface.png";
		iDir = hap.c_str();

		texture2 = CreateTexture(iDir, GL_REPEAT, GL_LINEAR);

		//enable blending for text display
		glEnable(GL_BLEND);
		glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);

		//draw in wireframe
		// Uncommenting this call will result in wireframe polygons.
		//glPolygonMode(GL_FRONT_AND_BACK, GL_LINE);


		//by the way, you need to free() all the arrays, now that you're done
		//using them

		//Continue on page 106 (transform matrices)
		return 0;
	}

	__declspec(dllexport) int OnlyRender()
	{
		// Check if any events have been activiated (key pressed, mouse moved etc.) and call corresponding response functions
		glfwPollEvents();
		if (glfwWindowShouldClose(sceneWindow))
		{
			//this is the point at which we should start returning meaningful values to c#
			return 2;
		}
		// Render	
		// Clear the colorbuffer
		glClearColor(0.0f, 0.0f, 0.3f, 1.0f);
		glClear(GL_COLOR_BUFFER_BIT);

		myShader->Use();

		glm::mat4 model;
		model = glm::rotate(model, -55.0f, glm::vec3(1.0f, 0.0f, 0.0f));


		float t = glfwGetTime();
		model = glm::rotate(model, t * 20, glm::vec3(0.0, 1.0, 0.0f));
		myShader->setMat4("model", model);

		glm::mat4 view;
		// Note that we’re translating the scene in the reverse direction of where
		//we want to move
		view = glm::translate(view, glm::vec3(0.0f, 0.0f, -3.0f));
		myShader->setMat4("view", view);

		glm::mat4 projection;
		projection = glm::perspective(45.0f, screenWidth / screenHeight, 0.1f, 100.0f); //the last 2 params are near/far clipping planes
		myShader->setMat4("projection", projection);


		glActiveTexture(GL_TEXTURE0);
		glBindTexture(GL_TEXTURE_2D, texture);
		glUniform1i(glGetUniformLocation(myShader->Program, "myTexture"), 0);
		glActiveTexture(GL_TEXTURE1);
		glBindTexture(GL_TEXTURE_2D, texture2);
		glUniform1i(glGetUniformLocation(myShader->Program, "myTexture2"), 1);


		glBindVertexArray(VAO);
		//	glDrawArrays(GL_TRIANGLES, 0, 3);
		glDrawElements(GL_TRIANGLES, 6, GL_UNSIGNED_INT, 0);


		glBindVertexArray(0);



		// Swap the screen buffers
		glfwSwapBuffers(sceneWindow);
		return 0;
	}

	__declspec(dllexport) int CloseAllWindows()
	{
		// Properly de-allocate all resources once they've outlived their purpose
		glDeleteVertexArrays(1, &VAO);
		glDeleteBuffers(1, &VBO);
		glDeleteBuffers(1, &EBO);

		// Terminate GLFW, clearing any resources allocated by GLFW.
		glfwTerminate();
		return 0;
	}

#pragma endregion
#pragma endregion

}

#pragma region Interior Methods

void key_callback(GLFWwindow* window, int key, int scancode, int action, int mode) 
{
	if (key == GLFW_KEY_ESCAPE && action == GLFW_PRESS)
		glfwSetWindowShouldClose(window, GL_TRUE);

}

//Windows dependant
string exeDirectory() 
{

	char ownPth[MAX_PATH];
	string wut;

	// When NULL is passed to GetModuleHandle, the handle of the exe itself is returned
	HMODULE hModule = GetModuleHandle(NULL);
	if (hModule != NULL)
	{
		// Use GetModuleFileName() with module handle to get the path
		GetModuleFileName(hModule, ownPth, sizeof(ownPth));
		wut = ownPth;
	}
	else
	{
		cout << "Module handle is NULL" << endl;
		system("PAUSE");
		return 0;
	}

	//this returns the full path of the .exe
	//so we need to truncate it
	string end = "graphicsPipeline.exe";
	unsigned int last = (wut.length() - end.length());
	string dir = wut.substr(0, last);
	return dir;
}

//Texture wrap params
//GL_REPEAT: The default behavior for textures.Repeats the texture image.
//• GL_MIRRORED_REPEAT : Same as GL_REPEAT but mirrors the image with each repeat.
//• GL_CLAMP_TO_EDGE : Clamps the coordinates between 0 and 1. The result is that higher
//	coordinates become clamped to the edge, resulting in a stretched edge pattern.
//• GL_CLAMP_TO_BORDER : Coordinates outside the range are now given a user - specified
//	border color.

//texture filtering params
//• GL_NEAREST_MIPMAP_NEAREST : takes the nearest mipmap to match the pixel size and
//uses nearest neighbor interpolation for texture sampling.
//• GL_LINEAR_MIPMAP_NEAREST : takes the nearest mipmap level and samples using linear
//interpolation.
//• GL_NEAREST_MIPMAP_LINEAR : linearly interpolates between the two mipmaps that
//most closely match the size of a pixel and samples via nearest neighbor interpolation.
//• GL_LINEAR_MIPMAP_LINEAR : linearly interpolates between the two closest mipmaps
//and samples the texture via linear interpolation
//
GLuint CreateTexture(const char * iDir, GLuint wrap, GLuint filter) 
{
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
	unsigned char* image = SOIL_load_image(iDir, &w, &h, 0, SOIL_LOAD_RGB);
	glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB, w, h, 0, GL_RGB, GL_UNSIGNED_BYTE, image);
	glGenerateMipmap(GL_TEXTURE_2D);
	SOIL_free_image_data(image);
	glBindTexture(GL_TEXTURE_2D, 0); // Unbind texture when done, so we won't accidentily mess up our texture.


	return texture;
}
GLuint CreateTextureX(const char * path, const string &directory, GLuint wrap, GLuint filter)
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

void mouse_callback(GLFWwindow* window, double xpos, double ypos)
{
	if (firstMouse)
	{
		lastX = xpos;
		lastY = ypos;
		firstMouse = false;
	}

	float xoffset = xpos - lastX;
	float yoffset = lastY - ypos; // reversed since y-coordinates go from bottom to top

	lastX = xpos;
	lastY = ypos;

	camera->ProcessMouseMovement(xoffset, yoffset);
}

void processInput(GLFWwindow *window)
{
	if (glfwGetKey(window, GLFW_KEY_ESCAPE) == GLFW_PRESS)
		glfwSetWindowShouldClose(window, true);

	if (glfwGetKey(window, GLFW_KEY_W) == GLFW_PRESS)
		camera->ProcessKeyboard(FORWARD, deltaTime);
	if (glfwGetKey(window, GLFW_KEY_S) == GLFW_PRESS)
		camera->ProcessKeyboard(BACKWARD, deltaTime);
	if (glfwGetKey(window, GLFW_KEY_A) == GLFW_PRESS)
		camera->ProcessKeyboard(LEFT, deltaTime);
	if (glfwGetKey(window, GLFW_KEY_D) == GLFW_PRESS)
		camera->ProcessKeyboard(RIGHT, deltaTime);
}

void framebuffer_size_callback(GLFWwindow* window, int width, int height)
{
	// make sure the viewport matches the new window dimensions; note that width and 
	// height will be significantly larger than specified on retina displays.
	glViewport(0, 0, width, height);
}

// glfw: whenever the mouse scroll wheel scrolls, this callback is called
// ----------------------------------------------------------------------
void scroll_callback(GLFWwindow* window, double xoffset, double yoffset)
{
	camera->ProcessMouseScroll(yoffset);
}


static wchar_t * strTowchar(string str) 
{
	wstring predata;
	for (unsigned int i = 0; i < str.length(); i++)
	{
		predata += wchar_t(str[i]);
	}

	//convert wstring to locally allocated  static wchar_t *
	static wchar_t * result = const_cast<wchar_t*>(predata.c_str()); //This line is lopping off all chars after 4!


																	 //convert locally allocated wchar_t* to heap allocated wchar_t*
	size_t wize = wcslen(predata.c_str());
	cout << "C++ size_t: " << wize << endl;
	//now convert the size_t to a const int... easy, right?
	static const int isize = static_cast<int>(wize);
	const int finSize = isize;
	cout << "C++ cast result: " << isize << endl;

	const int s = sizeof(result);
	static wchar_t * array = new wchar_t[isize];


	for (unsigned int i = 0; i < isize; i++)
	{
		array[i] = result[i];
	}

	return array;
}

void DrawDebuggingLights() 
{
	lampShader->Use();
	lampShader->setMat4("view", ViewMatrix);
	lampShader->setMat4("projection", ProjectionMatrix);
	glm::mat4 model;
	for (size_t i = 0; i < ScenePointLights.size(); i++)
	{
		model = glm::mat4();
		model = glm::translate(model, ScenePointLights[i].position);
		model = glm::scale(model, glm::vec3(0.05f)); // Make it a smaller cube
		lampShader->setMat4("model", model);

		lampShader->SetVec3(1.0f, 1.0f, 1.0f, "debugColor");

		// Draw the light object (using light's vertex attributes)
		glBindVertexArray(lightVAO);
		glDrawArrays(GL_TRIANGLES, 0, 36);
		glBindVertexArray(0);
	}

	for (size_t i = 0; i < SceneDirectionalLights.size(); i++)
	{
		glm::vec3 position = glm::vec3(0.0, i, 0.0);

		model = glm::mat4();
		model = glm::translate(model, position);
		model = glm::scale(model, glm::vec3(0.05f)); // Make it a smaller cube
		lampShader->setMat4("model", model);

		lampShader->SetVec3(1.0f, 0.0, 0.0, "debugColor");

		// Draw the light object (using light's vertex attributes)
		glBindVertexArray(lightVAO);
		glDrawArrays(GL_TRIANGLES, 0, 36);
		glBindVertexArray(0);
	}

	for (size_t i = 0; i < SceneSpotLights.size(); i++)
	{
		model = glm::mat4();
		model = glm::translate(model, SceneSpotLights[i].position);
		model = glm::scale(model, glm::vec3(0.05f)); // Make it a smaller cube
		lampShader->setMat4("model", model);

		lampShader->SetVec3(1.0f, 1.0f, 0.0, "debugColor");

		// Draw the light object (using light's vertex attributes)
		glBindVertexArray(lightVAO);
		glDrawArrays(GL_TRIANGLES, 0, 36);
		glBindVertexArray(0);
	}


}
#pragma endregion