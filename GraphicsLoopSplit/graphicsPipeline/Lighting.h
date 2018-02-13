#pragma once



struct PointLight {
	glm::vec3 position;
	glm::vec3 ambient;
	glm::vec3 diffuse;
	glm::vec3 specular;

	float constant;
	float linear;
	float quadratic;
	float maxDistance;

	bool enabled = true;
};

struct DirectionalLight {
	glm::vec3 direction;//This should really be called eulerAngles
	//TODO: Change to eulerAngles
	glm::vec3 ambient;
	glm::vec3 diffuse;
	glm::vec3 specular;
	bool enabled = true;
};

struct SpotLight {
	glm::vec3 position;
	glm::vec3 ambient;
	glm::vec3 diffuse;
	glm::vec3 specular;
	glm::vec3 direction;
	float cutOff;
	float outerCutOff;
	float maxDistance;
	bool enabled = true;
};