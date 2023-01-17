using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewNoiseLayers", menuName = "Scriptable objects/Planet/NoiseLayers", order = 0)]
public class NoiseLayer : ScriptableObject
{
    public NoiseSettings[] noiseSettings;
    public bool setMaxThreshold = false;

    [Range(0.5f, .80f)]
    [SerializeField] public float maxThreshold = .6f;



}