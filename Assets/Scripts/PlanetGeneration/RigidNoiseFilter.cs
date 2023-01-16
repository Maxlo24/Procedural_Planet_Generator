using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidNoiseFilter : INoiseFilter
{
    Noise noise = new Noise();
    NoiseSettings settings;


    //constructor
    public RigidNoiseFilter(NoiseSettings settings)
    {
        this.settings = settings;
    }
    public float Evaluate(Vector3 point)
    {
        float noiseValue = 0;
        float frequency = settings.baseRoughness;
        float amplitude = 1;
        float weight = 1;
        for (int i = 0; i < settings.numLayers; i++)
        {
            float v = 1 - Mathf.Abs(noise.Evaluate(point * frequency + settings.center));
            v *= weight * v;
            weight = v;
            
            noiseValue += v * amplitude;
            frequency *= settings.roughness;
            amplitude *= settings.persistance;
        }
        noiseValue = Mathf.Max(0, noiseValue - settings.seaThreshold);
        return noiseValue * settings.scale;
    }
}
