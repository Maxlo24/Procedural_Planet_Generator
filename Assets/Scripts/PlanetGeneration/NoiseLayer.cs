using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoiseLayer
{
    public bool enabled = true;
    public bool useFirstLayerAsMask;
    public NoiseSettings noiseSettings;
}