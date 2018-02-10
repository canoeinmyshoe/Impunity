#ifndef SCENEOBJECT_H
#define SCENEOBJECT_H

#include "Mesh.h"

////openGL math lib
//#include <glm/glm.hpp>
//#include <glm/gtc/matrix_transform.hpp>
//#include <glm/gtc/type_ptr.hpp>
//vector<SceneObject> AllSceneObjects;

//extern glm::mat4 ViewMatrix;
extern vector<Shader*> LoadedShaders;
extern vector<PointLight> ScenePointLights;
extern vector<DirectionalLight> SceneDirectionalLights;
extern vector<SpotLight> SceneSpotLights;

struct Transform {
	glm::vec3 position;
	glm::vec3 rotation;
	glm::vec3 scale;
	glm::mat4 matrix;//for use with c# code that sets pos/rot/scale
	//when you want a "LookAt" algo, just use camera->front
};
//Eventually, we'll use declare the Texture struct here instead of Mesh
//And the textures will be stuck vectors in the material
struct Material {
	glm::vec3 ambient;
	glm::vec3 diffuse;
	//sampler2D texture_diffuse1;
	glm::vec3 specular;
	//sampler2D texture_specular1;
	float shininess;

	//we should be tracking and configuring textures from here
	float xTiling;
	float yTiling;

	float xOffset;
	float yOffset;

	bool useExtTexture;
	int extDiffuseIndex;

};

class SceneObject
{
public:
	//All the variables involved. Everything has an ID!
	GLuint ID;
	string Name;
	const char * Tag;
	GLboolean Enabled = true;
	vector<Mesh> meshes;
	int MeshID;//to get from a global vector<Mesh> 


	//These two variables are a wash. None of them work for their intended purpose
	//And this is the reason c# has been brought in
	vector<SceneObject> Children;
	SceneObject* Parent;


	Transform transform;
	Shader * shader;
	Material material;

	GLuint ShaderNumber = 0;//Index in LoadedShaders of shader to use
	

	//set material values in the ctor
	SceneObject() 
	{
		material.ambient = glm::vec3(0.1f);//perhaps this should be set to something relative to globalAmbient...
		material.diffuse = glm::vec3(0.8f);
		material.specular = glm::vec3(1.0f);
		material.shininess = 32.0f;

		transform.position = glm::vec3(0.0);
		transform.scale = glm::vec3(1.0f);
		Name = "SceneObject";
		Enabled = true;
	};
	SceneObject(GLuint id)
	{
		this->transform.position = glm::vec3(0.0);
		this->transform.scale = glm::vec3(1.0f);
		this->Name = "SceneObject";
		this->Enabled = true;
		this->ID = id;

		material.ambient = glm::vec3(0.1f);//perhaps this should be set to something relative to globalAmbient...
		material.diffuse = glm::vec3(0.8f);
		material.specular = glm::vec3(1.0f);
		material.shininess = 32.0f;
	};

	SceneObject(const char * name, Mesh mesh)
	{
		transform.position = glm::vec3(0.0);
		transform.scale = glm::vec3(1.0f);
		Name = name;
		Enabled = true;
		meshes.push_back(mesh);

		material.ambient = glm::vec3(0.1f);//perhaps this should be set to something relative to globalAmbient...
		material.diffuse = glm::vec3(0.8f);
		material.specular = glm::vec3(1.0f);
		material.shininess = 32.0f;
	};

	~SceneObject();

	

	//at some point in time, abstract the setting of uniforms away from main
	//in fact, the sceneObject should have its own shader
	//also, there should be a vector of shaders to be ref'd by ID by c#, just like scene objects

	//The sceneObject shouldn't do any of this if it's empty!

	void Draw()
	{
		if (Enabled != true)
			return; 

		LoadedShaders[ShaderNumber]->Use();
		LoadedShaders[ShaderNumber]->setMat4("view", ViewMatrix);
		LoadedShaders[ShaderNumber]->setMat4("projection", ProjectionMatrix);
		glm::mat4 model;
		model = glm::translate(model, transform.position);
		model = glm::rotate(model, transform.rotation.x, glm::vec3(1.0f, 0, 0));
		model = glm::rotate(model, transform.rotation.y, glm::vec3(0, 1.0f, 0));
		model = glm::rotate(model, transform.rotation.z, glm::vec3(0, 0, 1.0f));
		 
		LoadedShaders[ShaderNumber]->setMat4("model", model);

		//set all the material details
		LoadedShaders[ShaderNumber]->SetVec3(material.ambient.x, material.ambient.y, material.ambient.z, "material.ambient");
		LoadedShaders[ShaderNumber]->SetVec3(material.diffuse.x, material.diffuse.y, material.diffuse.z, "material.diffuse");
		LoadedShaders[ShaderNumber]->SetVec3(material.specular.x, material.specular.y, material.specular.z, "material.specular");
		LoadedShaders[ShaderNumber]->SetFloat(material.shininess, "material.shininess");

		//texture tiling
		LoadedShaders[ShaderNumber]->SetFloat(material.xTiling, "material.xTiling");
		LoadedShaders[ShaderNumber]->SetFloat(material.yTiling, "material.yTiling");

		//texture offset
		LoadedShaders[ShaderNumber]->SetFloat(material.xOffset, "material.xOffset");
		LoadedShaders[ShaderNumber]->SetFloat(material.yOffset, "material.yOffset");


		//Set lighting
		//IMPORTANT: Uniforms must be set whether the light is out of range or not
		//So, instead of determining distance here, we'll simply set a uniform of maxDist in the shader
		//and let the shader work out the distance, as well as whether or not it should use light
		for (size_t i = 0; i < ScenePointLights.size(); i++)
		{
		/*	if (ScenePointLights[i].enabled != true) 
			{
				continue;
			}*/
		
			string eye = std::to_string(i);
			std::string s = "pointLights[" + eye + "].position";
			GLchar * pos = &s[0u];
			LoadedShaders[ShaderNumber]->SetVec3(ScenePointLights[i].position.x, ScenePointLights[i].position.y, ScenePointLights[i].position.z, pos);
			s = "pointLights[" + eye + "].ambient";
			pos = &s[0u];
			LoadedShaders[ShaderNumber]->SetVec3(ScenePointLights[i].ambient.x, ScenePointLights[i].ambient.y, ScenePointLights[i].ambient.z, pos);
			s = "pointLights[" + eye + "].diffuse";
			pos = &s[0u];
			LoadedShaders[ShaderNumber]->SetVec3(ScenePointLights[i].diffuse.x, ScenePointLights[i].diffuse.y, ScenePointLights[i].diffuse.z, pos);
			s = "pointLights[" + eye + "].specular";
			pos = &s[0u];
			LoadedShaders[ShaderNumber]->SetVec3(ScenePointLights[i].specular.x, ScenePointLights[i].specular.y, ScenePointLights[i].specular.z, pos);
			s = "pointLights[" + eye + "].constant";
			pos = &s[0u];
			LoadedShaders[ShaderNumber]->SetFloat(ScenePointLights[i].constant, pos);
			s = "pointLights[" + eye + "].linear";
			pos = &s[0u];
			LoadedShaders[ShaderNumber]->SetFloat(ScenePointLights[i].linear, pos);
			s = "pointLights[" + eye + "].quadratic";
			pos = &s[0u];
			LoadedShaders[ShaderNumber]->SetFloat(ScenePointLights[i].quadratic, pos);

			s = "pointLights[" + eye + "].maxRange";
			pos = &s[0u];
			LoadedShaders[ShaderNumber]->SetFloat(ScenePointLights[i].maxDistance, pos);

			if (ScenePointLights[i].enabled != true)
			{
				s = "pointLights[" + eye + "].maxRange";
				pos = &s[0u];
				LoadedShaders[ShaderNumber]->SetFloat(0, pos);
			}

		}

		for (size_t i = 0; i < SceneDirectionalLights.size(); i++)
		{
			if (SceneDirectionalLights[i].enabled != true) 
			{
				string eye = std::to_string(i);
				std::string s = "directionalLights[" + eye + "].direction";
				GLchar * pos = &s[0u];
				LoadedShaders[ShaderNumber]->SetVec3(SceneDirectionalLights[i].direction.x, SceneDirectionalLights[i].direction.y, SceneDirectionalLights[i].direction.z, pos);
				s = "directionalLights[" + eye + "].ambient";
				pos = &s[0u];
				LoadedShaders[ShaderNumber]->SetVec3(0, 0, 0, pos);
				s = "directionalLights[" + eye + "].diffuse";
				pos = &s[0u];
				LoadedShaders[ShaderNumber]->SetVec3(0, 0, 0, pos);
				s = "directionalLights[" + eye + "].specular";
				pos = &s[0u];
				LoadedShaders[ShaderNumber]->SetVec3(0, 0, 0, pos);

				continue;
			}

			string eye = std::to_string(i);
			std::string s = "directionalLights[" + eye + "].direction";
			GLchar * pos = &s[0u];
			LoadedShaders[ShaderNumber]->SetVec3(SceneDirectionalLights[i].direction.x, SceneDirectionalLights[i].direction.y, SceneDirectionalLights[i].direction.z, pos);
			s = "directionalLights[" + eye + "].ambient";
			pos = &s[0u];
			LoadedShaders[ShaderNumber]->SetVec3(SceneDirectionalLights[i].ambient.x, SceneDirectionalLights[i].ambient.y, SceneDirectionalLights[i].ambient.z, pos);
			s = "directionalLights[" + eye + "].diffuse";
			pos = &s[0u];
			LoadedShaders[ShaderNumber]->SetVec3(SceneDirectionalLights[i].diffuse.x, SceneDirectionalLights[i].diffuse.y, SceneDirectionalLights[i].diffuse.z, pos);
			s = "directionalLights[" + eye + "].specular";
			pos = &s[0u];
			LoadedShaders[ShaderNumber]->SetVec3(SceneDirectionalLights[i].specular.x, SceneDirectionalLights[i].specular.y, SceneDirectionalLights[i].specular.z, pos);
		
		}

		for (size_t i = 0; i < SceneSpotLights.size(); i++)
		{
		/*	if (SceneSpotLights[i].enabled != true) 
			{
				continue;
			}*/
		
			string eye = std::to_string(i);
			std::string s = "spotLights[" + eye + "].position";
			GLchar * pos = &s[0u];
			LoadedShaders[ShaderNumber]->SetVec3(SceneSpotLights[i].position.x, SceneSpotLights[i].position.y, SceneSpotLights[i].position.z, pos);
			s = "spotLights[" + eye + "].ambient";
			pos = &s[0u];
			LoadedShaders[ShaderNumber]->SetVec3(SceneSpotLights[i].ambient.x, SceneSpotLights[i].ambient.y, SceneSpotLights[i].ambient.z, pos);
			s = "spotLights[" + eye + "].diffuse";
			pos = &s[0u];
			LoadedShaders[ShaderNumber]->SetVec3(SceneSpotLights[i].diffuse.x, SceneSpotLights[i].diffuse.y, SceneSpotLights[i].diffuse.z, pos);
			s = "spotLights[" + eye + "].specular";
			pos = &s[0u];
			LoadedShaders[ShaderNumber]->SetVec3(SceneSpotLights[i].specular.x, SceneSpotLights[i].specular.y, SceneSpotLights[i].specular.z, pos);
			s = "spotLights[" + eye + "].direction";
			pos = &s[0u];
			LoadedShaders[ShaderNumber]->SetVec3(SceneSpotLights[i].direction, pos);
			s = "spotLights[" + eye + "].cutOff";
			pos = &s[0u];
			LoadedShaders[ShaderNumber]->SetFloat(glm::cos(glm::radians(SceneSpotLights[i].cutOff)), pos);
			s = "spotLights[" + eye + "].outerCutOff";
			pos = &s[0u];
			LoadedShaders[ShaderNumber]->SetFloat(glm::cos(glm::radians(SceneSpotLights[i].outerCutOff)), pos);

			s = "spotLights[" + eye + "].maxRange";
			pos = &s[0u];
			LoadedShaders[ShaderNumber]->SetFloat(SceneSpotLights[i].maxDistance, pos);

		if (SceneSpotLights[i].enabled != true)
			{
			s = "spotLights[" + eye + "].maxRange";
			pos = &s[0u];
			LoadedShaders[ShaderNumber]->SetFloat(0, pos);
			}


		}

		//other uniforms
		LoadedShaders[ShaderNumber]->SetVec3(camera->Position.x, camera->Position.y, camera->Position.z, "viewPosition");
		LoadedShaders[ShaderNumber]->SetInt("pointLightCount", ScenePointLights.size());
		LoadedShaders[ShaderNumber]->SetInt("directionalLightCount", SceneDirectionalLights.size());
		LoadedShaders[ShaderNumber]->SetInt("spotLightCount", SceneSpotLights.size());


		for (unsigned int i = 0; i < meshes.size(); i++)
		{
			if (material.useExtTexture == true) 
			{
				meshes[i].Draw(LoadedShaders[ShaderNumber], material.extDiffuseIndex);
			}
			else 
			{
				meshes[i].Draw(LoadedShaders[ShaderNumber]);
			}
		}
	}

	void Draw(Shader * shader)
	{
		//shader->Use();
		//set all the uniforms for transform
		shader->setMat4("view", ViewMatrix);
		shader->setMat4("projection", ProjectionMatrix);
		glm::mat4 model;
		model = glm::translate(model, transform.position);
			
		shader->setMat4("model", model);

		//If a shader does not have a uniform, it will simply ignore
		shader->SetInt("moop", 2);

		for (unsigned int i = 0; i < meshes.size(); i++) 
		{
			meshes[i].Draw(shader);
		}

	}


private:

};


SceneObject::~SceneObject()
{
}


#endif