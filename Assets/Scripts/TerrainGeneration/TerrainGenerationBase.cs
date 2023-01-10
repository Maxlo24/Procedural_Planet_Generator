using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class TerrainGenerationBase : MonoBehaviour
{
    [field: SerializeField] public Terrain Terrain { get; private set; }
    [field: SerializeField] public List<Noise> Noises { get; private set; }
    [field: SerializeField] public GameObject NoisesGameObject { get; private set; }
    [field: SerializeField] public TerrainProcessing TerrainProcessing { get; private set; }
    [field: SerializeField] public TerrainErosion TerrainErosion { get; private set; }
    [field: SerializeField] public CraterGeneration CraterGeneration { get; private set; }
    [field: SerializeField] public TerrainPostProcessing TerrainPostProcessing { get; private set; }
    [field: SerializeField] public HeightMapsAddition HeightMapsAddition { get; private set; }
    [SerializeField] public RenderTexture RenderTexture;
    [SerializeField] public RenderTexture ErosionTexture;
    [SerializeField] public RenderTexture DepositTexture;

    [field: SerializeField] public String TerrainNameToSave { get; private set; } = "Terrain";
    [field: SerializeField] public String TerrainNameToAdd { get; private set; } = "Terrain";

    [field: SerializeField] public bool LiveUpdate { get; private set; }

    private RenderTexture RenderTextureCopy;

    int indexOfKernel;

    private void Start()
    {
        RenderTexture = ImageLib.CreateRenderTexture(Terrain.terrainData.heightmapResolution, Terrain.terrainData.heightmapResolution, RenderTextureFormat.RFloat);
        ErosionTexture = ImageLib.CreateRenderTexture(Terrain.terrainData.heightmapResolution, Terrain.terrainData.heightmapResolution, RenderTextureFormat.RFloat);
    }

    private void Update()
    {
        if (RenderTexture == null)
        {
            RenderTexture = ImageLib.CreateRenderTexture(Terrain.terrainData.heightmapResolution, Terrain.terrainData.heightmapResolution, RenderTextureFormat.RFloat);
        }
        if (ErosionTexture == null)
        {
            ErosionTexture = ImageLib.CreateRenderTexture(Terrain.terrainData.heightmapResolution, Terrain.terrainData.heightmapResolution, RenderTextureFormat.RFloat);
        }

        if (LiveUpdate)
        {
            GenerateTerrain();
        }
    }

    private void ApplyNoise(Noise noise)
    {
        ComputeShader noiseShader = Resources.Load<ComputeShader>(ShaderLib.NoiseShader);
        ComputeShader terraceShader = Resources.Load<ComputeShader>(ShaderLib.TerraceShader);

        indexOfKernel = noiseShader.FindKernel("CSMain");

        RenderTexture rt;

        if (noise.Mode == ApplicationMode.ADDITION)
        {
            rt = ImageLib.CopyRenderTexture(RenderTexture);
        }
        else
        {
            rt = ImageLib.CreateRenderTexture(Terrain.terrainData.heightmapResolution, Terrain.terrainData.heightmapResolution, RenderTextureFormat.RFloat);
        }

        noiseShader.SetTexture(indexOfKernel, "height", rt);

        noiseShader.SetInt("resolution", Terrain.terrainData.heightmapResolution);
        noiseShader.SetInt("octaveCount", noise.OctaveNumber);
        noiseShader.SetFloat("xOffset", noise.Offset.x);
        noiseShader.SetFloat("yOffset", noise.Offset.z);
        noiseShader.SetFloat("elevationOffset", noise.Offset.y);
        noiseShader.SetFloat("scale", noise.Scale);
        noiseShader.SetFloat("scaleElevation", noise.ScaleElevation);
        noiseShader.SetFloat("redistribution", noise.Redistribution);
        noiseShader.SetFloat("islandRatio", noise.IslandRatio);
        noiseShader.SetBool("ridge", noise.Ridge);
        noiseShader.SetBool("octaveDependentAmplitude", noise.OctaveDependentAmplitude);

        noiseShader.SetBool("absolute", noise.Absolute);
        noiseShader.SetBool("invert", noise.Invert);

        noiseShader.SetInt("distanceType", (int)noise.DistanceType);
        noiseShader.SetInt("noiseType", (int)noise.NoiseType);

        if (noise.Octaves.Count == 0)
        {
            noise.FillOctaves();
        }

        ComputeBuffer octaveBuffer = new ComputeBuffer(noise.Octaves.Count, 8);
        octaveBuffer.SetData(noise.Octaves);
        noiseShader.SetBuffer(indexOfKernel, "octaves", octaveBuffer);

        noiseShader.Dispatch(indexOfKernel, Terrain.terrainData.heightmapResolution / 32 + 1, Terrain.terrainData.heightmapResolution / 32 + 1, 1);

        octaveBuffer.Release();


        indexOfKernel = terraceShader.FindKernel("CSMain");
        terraceShader.SetTexture(indexOfKernel, "heights", rt);

        terraceShader.SetBool("elevationLimit", noise.ElevationLimit);
        terraceShader.SetVector("elevationLimitHeights", noise.ElevationLimitHeights);

        terraceShader.SetBool("basicTerraces", noise.BasicTerraces);
        noiseShader.SetFloat("basicTerracesHeight", noise.BasicTerracesHeight);

        terraceShader.SetBool("customTerraces", noise.CustomTerraces);

        if (noise.CustomTerraces)
        {
            noise.SortTerraces();
        }
        List<Vector2> terracesDescription = noise.GetCustomTerraces();
        terraceShader.SetInt("customTerracesLength", terracesDescription.Count);
        ComputeBuffer terracesBuffer = new ComputeBuffer(terracesDescription.Count, 2 * sizeof(float));
        terracesBuffer.SetData(terracesDescription);
        terraceShader.SetBuffer(indexOfKernel, "customTerracesDescription", terracesBuffer);

        terraceShader.Dispatch(indexOfKernel, Terrain.terrainData.heightmapResolution / 32 + 1, Terrain.terrainData.heightmapResolution / 32 + 1, 1);

        terracesBuffer.Release();

        if (noise.Mode == ApplicationMode.MULTIPLICATION)
        {
            ImageLib.NormalizeRenderTexture(ref rt);
            ImageLib.MultiplyRenderTexture(ref rt, RenderTexture);
        }

        RenderTexture = rt;
    }

    private void ApplyNoises()
    {
        foreach (Noise noise in Noises)
        {
            if (noise.Enabled)
            {
                ApplyNoise(noise);
            }
        }
    }

    private void RedrawTerrain()
    {
        RenderTexture.active = RenderTexture;
        Terrain.terrainData.CopyActiveRenderTextureToHeightmap(new RectInt(0, 0, Terrain.terrainData.heightmapResolution, Terrain.terrainData.heightmapResolution),
            Vector2Int.zero, TerrainHeightmapSyncControl.HeightAndLod);
        RenderTexture.active = null;
    }

    public void GenerateTerrain()
    {
        FillNoises();
        RenderTexture?.Release();
        RenderTexture = ImageLib.CreateRenderTexture(Terrain.terrainData.heightmapResolution, Terrain.terrainData.heightmapResolution, RenderTextureFormat.RFloat);

        ApplyNoises();

        TerrainProcessing.ApplyIslandProcessing(ref RenderTexture);

        RedrawTerrain();
        if (RenderTextureCopy != null) RenderTextureCopy.Release();
        RenderTextureCopy = ImageLib.CopyRenderTexture(RenderTexture);

        //ImageLib.GetMinMaxFromRenderTexture(RenderTexture, out float min, out float max);

        RenderTexture nulltexture = ImageLib.CreateRenderTexture(Terrain.terrainData.heightmapResolution, Terrain.terrainData.heightmapResolution, RenderTextureFormat.RFloat);

        Graphics.Blit(nulltexture, ErosionTexture);
        Graphics.Blit(nulltexture, DepositTexture);

        nulltexture.Release();
    }

    public void GenerateCraters()
    {
        CraterGeneration.GenerateCraters(ref RenderTexture, Terrain.terrainData.heightmapResolution);
        RedrawTerrain();
    }

    public void ErodeTerrain()
    {
        TerrainErosion.Erode(ref RenderTexture, ref ErosionTexture, ref DepositTexture);
        RedrawTerrain();
    }

    public void Smooth()
    {
        TerrainPostProcessing.SmoothTerrain(RenderTextureCopy, ref RenderTexture);
        RedrawTerrain();
    }

    public void Save()
    {
        float[,] mesh = new float[Terrain.terrainData.heightmapResolution, Terrain.terrainData.heightmapResolution];
        mesh = this.Terrain.terrainData.GetHeights(0, 0, mesh.GetLength(0), mesh.GetLength(1));
        ImageLib.SaveRaw(mesh, "Assets/Scripts/TerrainGeneration/HeightMaps/", TerrainNameToSave);
    }

    public void Add()
    {
        float[,] mesh = new float[Terrain.terrainData.heightmapResolution, Terrain.terrainData.heightmapResolution];
        mesh = this.Terrain.terrainData.GetHeights(0, 0, mesh.GetLength(0), mesh.GetLength(1));

        float[,] terrainToAdd = ImageLib.LoadRawAsHeightmap("Assets/Scripts/TerrainGeneration/HeightMaps/" + TerrainNameToAdd + ".raw");

        mesh = HeightMapsAddition.AddHeightMaps(mesh, terrainToAdd);
        Terrain.terrainData.SetHeights(0, 0, mesh);
    }

    public void FillNoises()
    {
        // Fill Noises with all enabled components of type Noise in NoisesGameObject
        Noises = new List<Noise>();
        foreach (Noise noise in NoisesGameObject.GetComponents<Noise>())
        {
            if (noise.enabled)
            {
                Noises.Add(noise);
            }
        }
    }
}
