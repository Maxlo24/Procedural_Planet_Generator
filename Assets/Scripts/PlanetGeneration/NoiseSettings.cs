using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoiseSettings
{
    public enum FilterType { Simple, Rigid };
    public FilterType filterType;
    [Range(0, 10)]
    public float scale = 1;
    [Range(1, 8)]
    public int numLayers = 4;
    
    public float baseRoughness = 1;
    public float roughness = 2;

    public float persistance = 0.5f;
    public Vector3 center;

    public float seaThreshold = 0.5f;
}
 