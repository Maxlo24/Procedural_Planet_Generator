using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoiseSettings
{
    [Range(0, 10)]
    public float scale = 1;
    public float roughness = 2;
    public Vector3 center;
    [Range(1, 8)]
    public int numLayers = 4;
    public float persistance = 0.5f;
    public float baseRoughness = 1;
    public float seaHeight = 0.5f;
}
