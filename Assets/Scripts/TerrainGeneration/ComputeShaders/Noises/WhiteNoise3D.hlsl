#include "Assets/Scripts/TerrainGeneration/ComputeShaders/Noises/NoiseUtils.hlsl" 


void WhiteNoise3D_float(float3 input, out float Out)
{
    Out = rand3dTo1d(input);
}