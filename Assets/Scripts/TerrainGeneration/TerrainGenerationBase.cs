using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class TerrainGenerationBase : MonoBehaviour
{
    [field: SerializeField] public Terrain Terrain { get; private set; }
    [field: SerializeField] public Perlin Perlin { get; private set; }
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

    }

    private void Update()
    {
        if (RenderTexture == null)
        {
            RenderTexture = new RenderTexture(Terrain.terrainData.heightmapResolution, Terrain.terrainData.heightmapResolution, 32, RenderTextureFormat.RHalf);
            RenderTexture.enableRandomWrite = true;
            RenderTexture.Create();
        }
        
        //if (ErosionTexture == null)
        //{
        //    ErosionTexture = new RenderTexture(Terrain.terrainData.heightmapResolution, Terrain.terrainData.heightmapResolution, 32, RenderTextureFormat.RHalf);
        //    ErosionTexture.enableRandomWrite = true;
        //    ErosionTexture.Create();
        //}

        if (LiveUpdate)
        {
            SendDatas();
            RedrawTerrain();
        }
    }

    private void SendDatas()
    {
        ComputeShader.SetTexture(0, "height", RenderTexture);

        ComputeShader.SetInt("resolution", Terrain.terrainData.heightmapResolution);
        ComputeShader.SetInt("octaveCount", Perlin.OctaveNumber);
        ComputeShader.SetFloat("xOffset", Perlin.XOffset);
        ComputeShader.SetFloat("yOffset", Perlin.YOffset);
        ComputeShader.SetFloat("elevationOffset", Perlin.ElevationOffset);
        ComputeShader.SetFloat("scale", Perlin.Scale);
        ComputeShader.SetFloat("scaleElevation", Perlin.ScaleElevation);
        ComputeShader.SetFloat("redistribution", Perlin.Redistribution);
        ComputeShader.SetFloat("islandRatio", Perlin.IslandRatio);
        ComputeShader.SetBool("ridge", Perlin.Ridge);
        ComputeShader.SetBool("octaveDependentAmplitude", Perlin.OctaveDependentAmplitude);
        ComputeShader.SetBool("terraces", Perlin.Terraces);
        ComputeShader.SetFloat("terracesHeight", Perlin.TerracesHeight);
        ComputeShader.SetInt("distanceType", (int)Perlin.DistanceType);
        ComputeShader.SetInt("noiseType", (int)Perlin.NoiseType);

        // Create buffer to send Perlin.octaves to compute shader in RWStructuredBuffer
        ComputeBuffer octaveBuffer = new ComputeBuffer(Perlin.Octaves.Count, 8);
        octaveBuffer.SetData(Perlin.Octaves);
        ComputeShader.SetBuffer(0, "octaves", octaveBuffer);

        ComputeShader.Dispatch(indexOfKernel, Terrain.terrainData.heightmapResolution / 32, Terrain.terrainData.heightmapResolution / 32, 1);

        octaveBuffer.Release();
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
        indexOfKernel = ComputeShader.FindKernel("CSMain");
        
        RenderTexture = new RenderTexture(Terrain.terrainData.heightmapResolution, Terrain.terrainData.heightmapResolution, 32, RenderTextureFormat.RHalf);
        RenderTexture.enableRandomWrite = true;
        RenderTexture.Create();

        //ErosionTexture = new RenderTexture(Terrain.terrainData.heightmapResolution, Terrain.terrainData.heightmapResolution, 32, RenderTextureFormat.RHalf);
        //ErosionTexture.enableRandomWrite = true;
        //ErosionTexture.Create();

        SendDatas();        
        
        RedrawTerrain();
    }
    
    public void ErodeTerrain()
    {
        ErodeResult erodeResult = TerrainErosion.Erode(RenderTexture, RenderTextureCopy, ErosionTexture);
        RenderTexture = erodeResult.Heights;

        //ErosionTexture.enableRandomWrite = true;
        //ErosionTexture = erodeResult.ErosionTexture;

        Graphics.Blit(erodeResult.ErosionTexture, ErosionTexture);


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
}
