using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class SimpleNoiseFilter : INoiseFilter
{
    PlanetNoise noise = new PlanetNoise();
    NoiseSettings settings;


    //constructor
    public SimpleNoiseFilter(NoiseSettings settings)
    {
        this.settings = settings;
    }
    public float Evaluate(Vector3 point, Vector3 center)
    {
        float noiseValue = 0;
        float frequency = settings.baseRoughness;
        float amplitude = 1;
        for (int i = 0; i < settings.numLayers; i++)
        {
            float v = noise.Evaluate(point * frequency + center);
            noiseValue += (v + 1) * 0.5f * amplitude;
            frequency *= settings.roughness;
            amplitude *= settings.persistance;
        }
        noiseValue = Mathf.Max(0, noiseValue - settings.seaThreshold);
        if (settings.useMaxThreshold)
        {
            noiseValue = Mathf.Min(settings.maxThreshold, noiseValue);
        }
        return noiseValue * settings.scale;
    }
}
