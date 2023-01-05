#ifndef _INCLUDE_VALUE_NOISE_ 
#define _INCLUDE_VALUE_NOISE_

#include "Assets/Scripts/TerrainGeneration/ComputeShaders/Noises/NoiseUtils.hlsl"

float vnoise(float2 p)
{
    float2 i = floor(p);
    float2 f = frac(p);

    float2 u = quintic(f);

    /*=====================================================================

        c(0,1)      d(1,1)
            _____________
           |            |
           |            |
           |            |
           |            |
           |____________|

        a(0,0)      b(1,0)

    =====================================================================*/

    float2 a = hash(i + float2(0.0, 0.0));
    float2 b = hash(i + float2(1.0, 0.0));
    float2 c = hash(i + float2(0.0, 1.0));
    float2 d = hash(i + float2(1.0, 1.0));

    float t1 = lerp(a, b, u.x);   // lerp along bottom edge of cell
    float t2 = lerp(c, d, u.x);   // lerp along top edge of cell

    return remap(lerp(t1, t2, u.y).xxxx, -1, 1, 0, 1).x;
}


#endif 