using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSingleNoise", menuName = "Scriptable objects/Terrain/SingleNoise", order = 0)]
public class SingleNoise : ScriptableObject
{
    [SerializeField] public string Name;
    [Header("Noise")]
    [SerializeField] public ApplicationMode Mode = ApplicationMode.ADDITION;
    [SerializeField] public DistanceType DistanceType = DistanceType.SQUAREBUMP;
    [SerializeField] public NoiseType NoiseType;
    [SerializeField] public Vector2Int OctaveNumberLimits = new Vector2Int(6,9);
    [SerializeField] public Vector2 RedistributionLimits = new Vector2(0.8f, 2.5f);
    [SerializeField] public Vector2 IslandRatioLimits = new Vector2(0.0f, 0.2f);
    [SerializeField] public Vector2 ScaleLimits = new Vector2(2f, 4f);
    [SerializeField] public Vector2 ScaleElevationLimits = new Vector2(0.2f, 0.7f);
    [SerializeField] public Vector2 XZOffsetLimits = new Vector2(-9999f, 9999f);
    [SerializeField] public Vector2 YOffsetLimits = new Vector2(-0.4f, 0.2f);
    
    [SerializeField] public bool Ridge = false;
    [SerializeField] public bool OctaveDependentAmplitude = false;

    [Header("Elevation Limits")]
    [SerializeField] public bool ElevationLimit = false;
    [SerializeField] public Vector2 MinElevationLimits;
    [SerializeField] public Vector2 MaxElevationLimits;

    [Header("Custom Terraces")]
    [SerializeField] public bool CustomTerraces = false;
    [SerializeField] public Vector2Int TerracesCountLimits = new Vector2Int(0, 2);
    [SerializeField] public Vector2 TerracesOffsetLimits = new Vector2(0.2f, 0.7f);
    [SerializeField] public Vector2 TerracesSizeLimits = new Vector2(0.05f, 0.2f);
    [SerializeField] public bool FixedTerraces = false;
    [SerializeField] public List<Vector2> FixedTerracesDescription = new List<Vector2>();

    [SerializeField] public bool AddNoise;
    [SerializeField] public Vector2 NoiseFrequencyLimits;
    [SerializeField] public Vector2 NoiseAmplitudeyLimits;
}
