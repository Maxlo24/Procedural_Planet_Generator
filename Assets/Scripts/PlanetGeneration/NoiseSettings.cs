using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewNoise", menuName = "Scriptable objects/Planet/Noise", order = 0)]
public class NoiseSettings : ScriptableObject
{

    public bool useFirstLayerAsMask = true;
    public enum FilterType { Simple, Rigid };
    public FilterType filterType;
    [Range(0, 1)]
    public float scale = 1;
    [Range(1, 8)]
    public int numLayers = 4;


    [Range(0, 5)]
    public float baseRoughness = 1;
    [Range(0, 5)]
    public float roughness = 2;


    [Range(0, 2)]
    public float persistance = 0.5f;
    public Vector3 center;

    [Range(0, 2)]
    public float seaThreshold = 0.5f;
}
 