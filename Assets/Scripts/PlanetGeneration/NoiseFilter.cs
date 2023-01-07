using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseFilter
{
    Noise noise = new Noise();
    
    public float Evaluate(Vector3 point)
    {
        float noiseValue = 0.5f * (noise.Evaluate(point) + 1);
        return noiseValue;
    }
}
