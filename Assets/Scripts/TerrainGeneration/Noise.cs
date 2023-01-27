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

    [field: SerializeField] public ApplicationMode Mode { get; private set; } = ApplicationMode.ADDITION;
    [field: SerializeField] public DistanceType DistanceType { get; private set; }
    [field: SerializeField] public NoiseType NoiseType { get; private set; }

    [field: SerializeField, Range(1, 20)] public int OctaveNumber { get; private set; } = 8;                  // Number of octaves
    [field: SerializeField, Range(0, 20)] public float Redistribution { get; private set; } = 1f;
    [field: SerializeField, Range(-2, 2)] public float IslandRatio { get; private set; } = 0f;
    [field: SerializeField, Range(0, 100)] public float Scale { get; private set; } = 1f;
    [field: SerializeField, Range(0, 4)] public float ScaleElevation { get; private set; } = 1f;
    [field: SerializeField] public Vector3 Offset { get; private set; }

    [field: SerializeField] public bool Ridge { get; private set; }
    [field: SerializeField] public bool OctaveDependentAmplitude { get; private set; }

    [field: SerializeField] public bool ElevationLimit { get; private set; }
    [field: SerializeField] public Vector2 ElevationLimitHeights { get; private set; }

    [field: SerializeField] public bool BasicTerraces { get; private set; }
    [field: SerializeField] public float BasicTerracesHeight { get; private set; }

    [field: SerializeField] public bool CustomTerraces { get; private set; }
    [field: SerializeField] public List<Vector2> CustomTerracesDescription { get; private set; }
    [field: SerializeField] public bool AddNoise { get; private set; }
    [field: SerializeField] public float NoiseFrequency { get; private set; } = 0.5f;
    [field: SerializeField] public float NoiseAmplitude { get; private set; } = 0.2f;

    [field: SerializeField] public bool Absolute { get; private set; }
    [field: SerializeField] public bool Invert { get; private set; }


    [field: SerializeField] public List<Octave> Octaves { get; private set; }

    public Noise(SingleNoise single, int seed)
    {
        System.Random prng = new System.Random(seed);
        Enabled = true;
        Name = single.Name;
        Mode = single.Mode;
        DistanceType = single.DistanceType;
        NoiseType = single.NoiseType;
        OctaveNumber = RandomLib.NextInt(prng, single.OctaveNumberLimits.x, single.OctaveNumberLimits.y);
        Redistribution = RandomLib.NextFloat(prng, single.RedistributionLimits.x, single.RedistributionLimits.y);
        IslandRatio = RandomLib.NextFloat(prng, single.IslandRatioLimits.x, single.IslandRatioLimits.y);
        Scale = RandomLib.NextFloat(prng, single.ScaleLimits.x, single.ScaleLimits.y);
        ScaleElevation = RandomLib.NextFloat(prng, single.ScaleElevationLimits.x, single.ScaleElevationLimits.y);
        Offset = new Vector3(RandomLib.NextFloat(prng, single.XZOffsetLimits.x, single.XZOffsetLimits.y),
        RandomLib.NextFloat(prng, single.YOffsetLimits.x, single.YOffsetLimits.y),
        RandomLib.NextFloat(prng, single.XZOffsetLimits.x, single.XZOffsetLimits.y));
        Ridge = single.Ridge;
        OctaveDependentAmplitude = single.OctaveDependentAmplitude;
        ElevationLimit = single.ElevationLimit;
        ElevationLimitHeights = new Vector2(RandomLib.NextFloat(prng, single.MinElevationLimits.x, single.MinElevationLimits.y),
            RandomLib.NextFloat(prng, single.MaxElevationLimits.x, single.MaxElevationLimits.y));

        BasicTerraces = false;
        CustomTerraces = single.CustomTerraces;

        if (CustomTerraces)
        {
            if (single.FixedTerraces)
            {
                CustomTerracesDescription = single.FixedTerracesDescription;
            }
            else
            {
                CustomTerracesDescription = new List<Vector2>();
                int CustomTerracesCount = RandomLib.NextInt(prng, single.TerracesCountLimits.x, single.TerracesCountLimits.y);
                float height = 0;
                for (int i = 0; i < CustomTerracesCount; i++)
                {
                    height += RandomLib.NextFloat(prng, single.TerracesOffsetLimits.x, single.TerracesOffsetLimits.y);
                    float size = RandomLib.NextFloat(prng, single.TerracesSizeLimits.x, single.TerracesSizeLimits.y);
                    CustomTerracesDescription.Add(new Vector2(height, size));
                    height += size;
                }
            }
            AddNoise = single.AddNoise;
            if (AddNoise)
            {
                NoiseFrequency = RandomLib.NextFloat(prng, single.NoiseFrequencyLimits.x, single.NoiseFrequencyLimits.y);
                NoiseAmplitude = RandomLib.NextFloat(prng, single.NoiseAmplitudeyLimits.x, single.NoiseAmplitudeyLimits.y);
            }
            else { NoiseFrequency = 0; NoiseAmplitude = 0; }
        }

        Octaves = new List<Octave>();
    }

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
            float sumElevation = !OctaveDependentAmplitude || elevation == 0f ? 1f : elevation;
            elevation += NoiseLib.Noise((x + Offset.x) * Octaves[i].frequency * Scale, (y + Offset.z) * Octaves[i].frequency * Scale, NoiseType) * Octaves[i].amplitude * sumElevation;

        }

        elevation = elevation / sumPersistency;
        elevation = Mathf.Pow(elevation, Redistribution);
        float nx = x * 2f - 1f;
        float ny = y * 2f - 1f;
        elevation = elevation * (1f - IslandRatio) + IslandRatio * (elevation + (1f - DistanceLib.Distance(nx, ny, DistanceType))) / 2f;

        if (BasicTerraces)
        {
            elevation = Mathf.Floor(elevation * BasicTerracesHeight) / BasicTerracesHeight;
        }
        return ScaleElevation * (elevation + Offset.y);
    }

    public void SortTerraces()
    {
        CustomTerracesDescription.Sort((x, y) => x.x.CompareTo(y.x));
    }

    public List<Vector2> GetCustomTerraces()
    {
        if (CustomTerraces && CustomTerracesDescription.Count > 0)
        {
            return CustomTerracesDescription;
        }

        return new List<Vector2>() { Vector2.zero };
    }

    public void ApplyNoise(ref RenderTexture renderTexture, Terrain Terrain)
    {
        ComputeShader noiseShader = Resources.Load<ComputeShader>(ShaderLib.NoiseShader);
        ComputeShader terraceShader = Resources.Load<ComputeShader>(ShaderLib.TerraceShader);

        int indexOfKernel = noiseShader.FindKernel("CSMain");

        RenderTexture rt;

        if (this.Mode == ApplicationMode.ADDITION)
        {
            rt = ImageLib.CopyRenderTexture(renderTexture);
        }
        else
        {
            rt = ImageLib.CreateRenderTexture(Terrain.terrainData.heightmapResolution, Terrain.terrainData.heightmapResolution, RenderTextureFormat.RFloat);
        }

        noiseShader.SetTexture(indexOfKernel, "height", rt);

        noiseShader.SetInt("resolution", Terrain.terrainData.heightmapResolution);
        noiseShader.SetInt("octaveCount", this.OctaveNumber);
        noiseShader.SetFloat("xOffset", this.Offset.x);
        noiseShader.SetFloat("yOffset", this.Offset.z);
        noiseShader.SetFloat("elevationOffset", this.Offset.y);
        noiseShader.SetFloat("scale", this.Scale);
        noiseShader.SetFloat("scaleElevation", this.ScaleElevation);
        noiseShader.SetFloat("redistribution", this.Redistribution);
        noiseShader.SetFloat("islandRatio", this.IslandRatio);
        noiseShader.SetBool("ridge", this.Ridge);
        noiseShader.SetBool("octaveDependentAmplitude", this.OctaveDependentAmplitude);

        noiseShader.SetBool("absolute", this.Absolute);
        noiseShader.SetBool("invert", this.Invert);

        noiseShader.SetInt("distanceType", (int)this.DistanceType);
        noiseShader.SetInt("noiseType", (int)this.NoiseType);

        if (this.Octaves.Count == 0)
        {
            this.FillOctaves();
        }

        ComputeBuffer octaveBuffer = new ComputeBuffer(this.Octaves.Count, 8);
        octaveBuffer.SetData(this.Octaves);
        noiseShader.SetBuffer(indexOfKernel, "octaves", octaveBuffer);

        noiseShader.Dispatch(indexOfKernel, Terrain.terrainData.heightmapResolution / 32 + 1, Terrain.terrainData.heightmapResolution / 32 + 1, 1);

        octaveBuffer.Release();


        indexOfKernel = terraceShader.FindKernel("CSMain");
        terraceShader.SetTexture(indexOfKernel, "heights", rt);

        terraceShader.SetBool("elevationLimit", this.ElevationLimit);
        terraceShader.SetVector("elevationLimitHeights", this.ElevationLimitHeights);

        terraceShader.SetBool("basicTerraces", this.BasicTerraces);
        noiseShader.SetFloat("basicTerracesHeight", this.BasicTerracesHeight);

        terraceShader.SetBool("customTerraces", this.CustomTerraces);

        if (this.CustomTerraces)
        {
            this.SortTerraces();
        }
        List<Vector2> terracesDescription = this.GetCustomTerraces();
        terraceShader.SetInt("customTerracesLength", terracesDescription.Count);
        ComputeBuffer terracesBuffer = new ComputeBuffer(terracesDescription.Count, 2 * sizeof(float));
        terracesBuffer.SetData(terracesDescription);
        terraceShader.SetBuffer(indexOfKernel, "customTerracesDescription", terracesBuffer);

        terraceShader.SetBool("addNoise", this.AddNoise);
        terraceShader.SetFloat("addNoiseFrequency", this.NoiseFrequency);
        terraceShader.SetFloat("addNoiseAmplitude", this.NoiseAmplitude);

        terraceShader.Dispatch(indexOfKernel, Terrain.terrainData.heightmapResolution / 32 + 1, Terrain.terrainData.heightmapResolution / 32 + 1, 1);

        terracesBuffer.Release();

        if (this.Mode == ApplicationMode.MULTIPLICATION)
        {
            ImageLib.NormalizeRenderTexture(ref rt);
            ImageLib.MultiplyRenderTexture(ref rt, renderTexture);
        }

        renderTexture = rt;
    }

    public void SummarizeAttributes()
    {
        Debug.Log("OctaveNumber: " + OctaveNumber);
        Debug.Log("Offset: " + Offset);
        Debug.Log("Scale: " + Scale);
        Debug.Log("ScaleElevation: " + ScaleElevation);
        Debug.Log("Redistribution: " + Redistribution);
        Debug.Log("IslandRatio: " + IslandRatio);
        Debug.Log("Ridge: " + Ridge);
        Debug.Log("OctaveDependentAmplitude: " + OctaveDependentAmplitude);
        Debug.Log("Absolute: " + Absolute);
        Debug.Log("Invert: " + Invert);
        Debug.Log("DistanceType: " + DistanceType);
        Debug.Log("NoiseType: " + NoiseType);
        Debug.Log("Mode: " + Mode);
        Debug.Log("ElevationLimit: " + ElevationLimit);
        Debug.Log("ElevationLimitHeights: " + ElevationLimitHeights);
        Debug.Log("BasicTerraces: " + BasicTerraces);
        Debug.Log("BasicTerracesHeight: " + BasicTerracesHeight);

    }
}
