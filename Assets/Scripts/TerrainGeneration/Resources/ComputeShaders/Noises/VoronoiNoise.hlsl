#ifndef _INCLUDE_VORONOI_NOISE_ 
#define _INCLUDE_VORONOI_NOISE_

#include "Assets/Scripts/TerrainGeneration/Resources/ComputeShaders/Noises/NoiseUtils.hlsl"

float vrnoise(float2 p)
{
    float2 i = floor(p);
    float2 f = frac(p);

    float n = 1;

    for (float x = -2; x <= 2; ++x)
    {
        for (float y = -2; y <= 2; ++y)
        {
            float2 offset = float2(x, y);

            float2 r = hash(i + offset);
            r = r + offset - f;
            float t = dot(r, r);

            n = min(n, t);
        }
    }

    return n;
}

#endif 