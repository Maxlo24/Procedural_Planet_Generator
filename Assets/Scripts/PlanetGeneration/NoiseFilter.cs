using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseFilter
{
    Noise noise = new Noise();
    NoiseSettings settings;


    //constructor
    public NoiseFilter(NoiseSettings settings)
    {
        this.settings = settings;
    }
    public float Evaluate(Vector3 point)
    {
        float noiseValue = 0;
        float frequency = settings.baseRoughness;
        float amplitude = 1;
        for (int i = 0; i < settings.numLayers; i++)
        {
            float v = noise.Evaluate(point * frequency);
            noiseValue += (v + 1) * 0.5f * amplitude;
            frequency *= settings.roughness;
            amplitude *= settings.persistance;
        }
        noiseValue = Mathf.Max(0, noiseValue - settings.seaHeight);
        return noiseValue * settings.scale;
    }
}
