using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewErosionPreset", menuName = "Scriptable objects/Terrain/ErosionPreset", order = 0)]
public class ErosionPreset : ScriptableObject
{
    [SerializeField] public string Name;
    [SerializeField, Range(1, 10)] public int RepetitionCount = 1;
    [SerializeField] public Resolution Resolution = Resolution._1024;
    [SerializeField, Range(0, 1000000)] public int IterationCount = 100000;
    [SerializeField, Range(0, 800)] public int DropletLifeTime = 800;
    [SerializeField] public float Acceleration = 20f;
    [SerializeField] public float Drag = 0.2f;
    [SerializeField] public float InitialVelocity = 3f;
    [SerializeField] public float InitialWater = 0.01f;
    [SerializeField, Range(0, 100)] public float SedimentCapacityFactor = 24f;
    [SerializeField, Range(0, 1)] public float DepositRatio = 0.1f;
    [SerializeField, Range(0, 1)] public float ErosionRatio = 0.3f;
    [SerializeField, Range(0, 1)] public float EvaporationRatio = 0.02f;
    [SerializeField, Range(0, 500)] public float Gravity = 50f;
    [SerializeField] public bool ErodeEnabled = true;
    [SerializeField] public bool GenerateErosionTexture = false;
    [SerializeField] public bool SmoothResult = false;
}
