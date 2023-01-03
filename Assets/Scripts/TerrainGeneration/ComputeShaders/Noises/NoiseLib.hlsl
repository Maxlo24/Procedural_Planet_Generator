#ifndef _INCLUDE_NOISE_LIB_
#define _INCLUDE_NOISE_LIB_

#include "Assets/Scripts/TerrainGeneration/ComputeShaders/Noises/LibUtils.hlsl"

#include "Assets/Scripts/TerrainGeneration/ComputeShaders/Noises/ClassicNoise2D.hlsl"
#include "Assets/Scripts/TerrainGeneration/ComputeShaders/Noises/SimplexNoise2D.hlsl"
#include "Assets/Scripts/TerrainGeneration/ComputeShaders/Noises/BillowNoise.hlsl"
#include "Assets/Scripts/TerrainGeneration/ComputeShaders/Noises/RidgeNoise.hlsl"
#include "Assets/Scripts/TerrainGeneration/ComputeShaders/Noises/VoronoiNoise.hlsl"
#include "Assets/Scripts/TerrainGeneration/ComputeShaders/Noises/ValueNoise.hlsl"
#include "Assets/Scripts/TerrainGeneration/ComputeShaders/Noises/WorleyNoise.hlsl"
#include "Assets/Scripts/TerrainGeneration/ComputeShaders/Noises/FractalBrownianNoise.hlsl"



float noise(float2 p, int noiseType)
{
    switch (noiseType)
    {
    case 0:
        return cnoise(p);
    case 1:
        return snoise(p);
    case 2:
        return bnoise(p);
    case 3:
        return rnoise(p);
    case 4:
        return vnoise(p);
	case 5:
		return vrnoise(p);
    case 6:
        return wnoise(p,1.0,false);
	case 7:
		return fbnoise(p);
    default:
        return 0;
    }

    // floa noise, fractal noise
}


#endif