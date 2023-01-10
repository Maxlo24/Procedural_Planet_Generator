#ifndef _INCLUDE_RIDGE_NOISE_ 
#define _INCLUDE_RIDGE_NOISE_

#include "Assets/Scripts/TerrainGeneration/Resources/ComputeShaders/Noises/NoiseUtils.hlsl"

float rnoise(float2 p)
{
    float2 i = floor(p);
    float2 f = frac(p);

    float2 u = quintic(f);

    float2 a = hash(i + float2(0.0, 0.0));
    float2 b = hash(i + float2(1.0, 0.0));
    float2 c = hash(i + float2(0.0, 1.0));
    float2 d = hash(i + float2(1.0, 1.0));

    float ga = dot(a, f - float2(0.0, 0.0));
    float gb = dot(b, f - float2(1.0, 0.0));
    float gc = dot(c, f - float2(0.0, 1.0));
    float gd = dot(d, f - float2(1.0, 1.0));

    float ret = lerp(lerp(ga, gb, u.x), lerp(gc, gd, u.x), u.y);                        

    return 1 - abs(ret);
}

#endif