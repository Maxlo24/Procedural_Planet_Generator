using System.Collections.Generic;
using UnityEngine;


public class Noise : MonoBehaviour
{
    [System.Serializable]
    public struct Octave
    {
        public float frequency;
        public float amplitude;
    };
    
    [field: SerializeField] public bool Enabled { get; private set; }
    [field: SerializeField] public string Name { get; private set; } = "Noise";

    [field: SerializeField] public DistanceType DistanceType { get; private set; }
    [field: SerializeField] public NoiseType NoiseType { get; private set; }
    
    [field: SerializeField, Range(1,20)] public int OctaveNumber { get; private set; } = 8;                  // Number of octaves
    [field: SerializeField, Range(0, 20)] public float Redistribution { get; private set; } = 1f;
    [field: SerializeField, Range(-2, 2)] public float IslandRatio { get; private set; } = 0f;
    [field: SerializeField, Range(0, 20)] public float Scale { get; private set; } = 1f;
    [field: SerializeField, Range(0, 4)] public float ScaleElevation { get; private set; } = 1f;
    [field: SerializeField] public Vector3 Offset { get; private set; }
    [field: SerializeField] public bool Ridge { get; private set; }
    [field: SerializeField] public bool OctaveDependentAmplitude { get; private set; }
    
    [field: SerializeField] public bool ElevationLimit { get; private set; }
    [field: SerializeField] public Vector2 ElevationLimitHeights { get; private set; }

    [field: SerializeField] public bool Terraces { get; private set; }
    [field: SerializeField] public float TerracesHeight { get; private set; }

    [field: SerializeField] public List<Octave> Octaves { get; private set; }

    public void FillOctaves(int octaveNumber = 10)
    {
        Octaves = new List<Octave>();

        float frequency = 1f;
        float amplitude = 1f;
        for (int i = 0; i < octaveNumber; i++)
        {
            Octaves.Add(new Octave { frequency = frequency, amplitude = amplitude });
            frequency *= 2f;
            amplitude *= 0.5f;
        }
    }


    public float PerlinNoise(float x, float y)
    {
        float elevation = 0f;
        float sumPersistency = 0f;
        
        for (int i = 0; i < OctaveNumber; i++)
        {
            sumPersistency += Octaves[i].amplitude;
            float sumElevation = !OctaveDependentAmplitude || elevation == 0f? 1f : elevation;
            elevation += NoiseLib.Noise((x + Offset.x) * Octaves[i].frequency * Scale, (y + Offset.z) * Octaves[i].frequency * Scale, NoiseType) * Octaves[i].amplitude * sumElevation;
            
        }

        elevation = elevation / sumPersistency;
        elevation = Mathf.Pow(elevation, Redistribution);
        float nx = x * 2f - 1f;
        float ny = y * 2f - 1f;
        elevation = elevation * (1f - IslandRatio) + IslandRatio * (elevation + (1f - DistanceLib.Distance(nx, ny, DistanceType))) / 2f;

        if (Terraces)
        {
            elevation = Mathf.Floor(elevation * TerracesHeight) / TerracesHeight;
        }
        return ScaleElevation * (elevation + Offset.y);
    }
}
