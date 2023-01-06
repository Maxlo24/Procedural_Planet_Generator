using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class TerrainGenerationBase : MonoBehaviour
{
    [field: SerializeField] public Terrain Terrain { get; private set; }
    [field: SerializeField] public TerrainInfo TerrainInfo { get; private set; }
    [field: SerializeField] public List<Noise> Noises { get; private set; }
    [field: SerializeField] public GameObject NoisesGameObject { get; private set; }
    [field: SerializeField] public ComputeShader ComputeShader { get; private set; }
    [field: SerializeField] public TerrainErosion TerrainErosion { get; private set; }
    [field: SerializeField] public TerrainPostProcessing TerrainPostProcessing { get; private set; }
    [field: SerializeField] public HeightMapsAddition HeightMapsAddition { get; private set; }
    [field: SerializeField] public RenderTexture RenderTexture { get; private set; }
    [field: SerializeField] public RenderTexture ErosionTexture { get; private set; }


    [field: SerializeField] public String TerrainNameToSave { get; private set; } = "Terrain";
    [field: SerializeField] public String TerrainNameToAdd { get; private set; } = "Terrain";

    [field: SerializeField] public bool LiveUpdate { get; private set; }

    private RenderTexture RenderTextureCopy;

    int indexOfKernel;
    
    private void Start()
    {
        indexOfKernel = ComputeShader.FindKernel("CSMain");

        RenderTexture = new RenderTexture(Terrain.terrainData.heightmapResolution, Terrain.terrainData.heightmapResolution, 32, RenderTextureFormat.RHalf);
        RenderTexture.enableRandomWrite = true;
        RenderTexture.Create();

        ErosionTexture = new RenderTexture(Terrain.terrainData.heightmapResolution, Terrain.terrainData.heightmapResolution, 32, RenderTextureFormat.RHalf);
        ErosionTexture.enableRandomWrite = true;
        ErosionTexture.Create();
    }

    private void Update()
    {
        
            
        if (RenderTexture == null)
        {
            RenderTexture = new RenderTexture(Terrain.terrainData.heightmapResolution, Terrain.terrainData.heightmapResolution, 32, RenderTextureFormat.RHalf);
            RenderTexture.enableRandomWrite = true;
            RenderTexture.Create();
        }
        
        if (ErosionTexture == null)
        {
            ErosionTexture = new RenderTexture(Terrain.terrainData.heightmapResolution, Terrain.terrainData.heightmapResolution, 32, RenderTextureFormat.RHalf);
            ErosionTexture.enableRandomWrite = true;
            ErosionTexture.Create();
        }

        if (LiveUpdate)
        {
            GenerateTerrain();
        }
    }

    private void ApplyNoise(Noise noise)
    {
        indexOfKernel = ComputeShader.FindKernel("CSMain");
        
        ComputeShader.SetTexture(indexOfKernel, "height", RenderTexture);

        ComputeShader.SetInt("resolution", Terrain.terrainData.heightmapResolution);
        ComputeShader.SetInt("octaveCount", noise.OctaveNumber);
        ComputeShader.SetFloat("xOffset", noise.Offset.x);
        ComputeShader.SetFloat("yOffset", noise.Offset.z);
        ComputeShader.SetFloat("elevationOffset", noise.Offset.y);
        ComputeShader.SetFloat("scale", noise.Scale);
        ComputeShader.SetFloat("scaleElevation", noise.ScaleElevation);
        ComputeShader.SetFloat("redistribution", noise.Redistribution);
        ComputeShader.SetFloat("islandRatio", noise.IslandRatio);
        ComputeShader.SetBool("ridge", noise.Ridge);
        ComputeShader.SetBool("octaveDependentAmplitude", noise.OctaveDependentAmplitude);
        ComputeShader.SetBool("terraces", noise.Terraces);
        ComputeShader.SetFloat("terracesHeight", noise.TerracesHeight);
        ComputeShader.SetInt("distanceType", (int)noise.DistanceType);
        ComputeShader.SetInt("noiseType", (int)noise.NoiseType);

        if (noise.Octaves.Count == 0)
        {
            noise.FillOctaves();
        }

        ComputeBuffer octaveBuffer = new ComputeBuffer(noise.Octaves.Count, 8);
        octaveBuffer.SetData(noise.Octaves);
        ComputeShader.SetBuffer(indexOfKernel, "octaves", octaveBuffer);

        ComputeShader.Dispatch(indexOfKernel, Terrain.terrainData.heightmapResolution / 32, Terrain.terrainData.heightmapResolution / 32, 1);

        octaveBuffer.Release();
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

        RenderTextureCopy = new RenderTexture(RenderTexture.width, RenderTexture.height, 0, RenderTextureFormat.RFloat);
        RenderTextureCopy.enableRandomWrite = true;
        Graphics.Blit(RenderTexture, RenderTextureCopy);
    }

    public void GenerateTerrain()
    {
        FillNoises();
        
        RenderTexture = new RenderTexture(Terrain.terrainData.heightmapResolution, Terrain.terrainData.heightmapResolution, 32, RenderTextureFormat.RHalf);
        RenderTexture.enableRandomWrite = true;
        RenderTexture.Create();

        ErosionTexture = new RenderTexture(Terrain.terrainData.heightmapResolution, Terrain.terrainData.heightmapResolution, 32, RenderTextureFormat.RHalf);
        ErosionTexture.enableRandomWrite = true;
        ErosionTexture.Create();

        ApplyNoises();
        RedrawTerrain();
        float startTime = Time.realtimeSinceStartup;
        TerrainInfo.ProcessInfoComputation(RenderTexture);
        //TerrainInfo.ProcessInfoComputation(Terrain.terrainData.GetHeights(0, 0, Terrain.terrainData.heightmapResolution, Terrain.terrainData.heightmapResolution));
        // mesure execution time
        float endTime = Time.realtimeSinceStartup;
        Debug.Log("ProcessInfoComputation took " + (endTime - startTime) + " seconds");
    }
    
    public void ErodeTerrain()
    {
        ErodeResult erodeResult = TerrainErosion.Erode(RenderTexture, RenderTextureCopy, ErosionTexture);
        RenderTexture = erodeResult.Heights;
        ErosionTexture = erodeResult.ErosionTexture;

        RenderTexture.active = RenderTexture;
        Terrain.terrainData.CopyActiveRenderTextureToHeightmap(new RectInt(0, 0, Terrain.terrainData.heightmapResolution, Terrain.terrainData.heightmapResolution),
            Vector2Int.zero, TerrainHeightmapSyncControl.HeightAndLod);
        RenderTexture.active = null;
    }

    public void Smooth()
    {
        RenderTexture = TerrainPostProcessing.SmoothTerrain(RenderTextureCopy, RenderTexture);
        RenderTexture.active = RenderTexture;
        Terrain.terrainData.CopyActiveRenderTextureToHeightmap(new RectInt(0, 0, Terrain.terrainData.heightmapResolution, Terrain.terrainData.heightmapResolution),
            Vector2Int.zero, TerrainHeightmapSyncControl.HeightAndLod);
        RenderTexture.active = null;
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
