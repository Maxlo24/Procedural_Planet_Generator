using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ApplicationMode
{
    ADDITION,
    MULTIPLICATION
};

public enum NoiseType
{
    PERLIN,
    SIMPLEX,
    BILLOW,
    RIDGE,
    VALUE,
    VORONOI,
    WORLEY,
    FRACTALBROWNIAN
}

public class NoiseLib
{
    public static float Noise(float x, float y, NoiseType noiseType)
    {
        switch (noiseType)
        {
            case NoiseType.PERLIN:
                return Perlin(x, y);
            case NoiseType.RIDGE:
                return Ridge(x, y);
            default:
                return 0;
        }
    }

    private static float Perlin(float x, float y)
    {
        return Mathf.PerlinNoise(x, y);
    }

    private static float Ridge(float x, float y)
    {
        return 2f * (0.5f - Mathf.Abs(0.5f - Mathf.PerlinNoise(x, y)));
    }
}
