#ifndef _INCLUDE_FRACTAL_BROWNIAN_NOISE_ 
#define _INCLUDE_FRACTAL_BROWNIAN_NOISE_

#include "Assets/Scripts/TerrainGeneration/Resources/ComputeShaders/Noises/NoiseUtils.hlsl"
#include "Assets/Scripts/TerrainGeneration/Resources/ComputeShaders/Noises/SimplexNoise2D.hlsl"

#define NUM_OCTAVES 5

float fbnoise(float2 x) {
	float v = 0.0;
	float a = 0.5;
	float2 shift = float2(100,100);
	// Rotate to reduce axial bias
	float2x2 rot = float2x2(cos(0.5), sin(0.5), -sin(0.5), cos(0.50));
	for (int i = 0; i < NUM_OCTAVES; ++i) {
		v += a * snoise(x);
		x = mul(rot, x) * 2.0 + shift;
		a *= 0.5;
	}
	return v;
}

#endif 