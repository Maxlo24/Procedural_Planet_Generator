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
    [field: SerializeField] public HeightMapsAddition HeightMapsAddition { get; private set; }
    [field: SerializeField] public RenderTexture RenderTexture { get; private set; }
    [field: SerializeField] public RawImage RawImage { get; private set; }

    [field: SerializeField] public String TerrainNameToSave { get; private set; } = "Terrain";
    [field: SerializeField] public String TerrainNameToAdd { get; private set; } = "Terrain";

    [field: SerializeField] public bool LiveUpdate { get; private set; }

    int indexOfKernel;
    
    private void Start()
    {
        indexOfKernel = ComputeShader.FindKernel("CSMain");

        RenderTexture = new RenderTexture(Terrain.terrainData.heightmapResolution, Terrain.terrainData.heightmapResolution, 32, RenderTextureFormat.RHalf);
        RenderTexture.enableRandomWrite = true;
        RenderTexture.Create();

        SendDatas();

        ComputeShader.Dispatch(indexOfKernel, Terrain.terrainData.heightmapResolution / 32, Terrain.terrainData.heightmapResolution / 32, 1);

        RenderTexture.active = RenderTexture;
        RawImage.texture = RenderTexture;
        RenderTexture.active = null;
    }

    private void Update()
    {
        if (RenderTexture == null)
        {
            RenderTexture = new RenderTexture(Terrain.terrainData.heightmapResolution, Terrain.terrainData.heightmapResolution, 32, RenderTextureFormat.RHalf);
            RenderTexture.enableRandomWrite = true;
            RenderTexture.Create();
        }

        SendDatas();
        ComputeShader.Dispatch(indexOfKernel, Terrain.terrainData.heightmapResolution / 32, Terrain.terrainData.heightmapResolution / 32, 1);
        if (LiveUpdate)
        {
            RedrawTerrain();
        }
        RenderTexture.active = RenderTexture;
        RawImage.texture = RenderTexture;
        RenderTexture.active = null;
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
        indexOfKernel = ComputeShader.FindKernel("CSMain");
        
        RenderTexture = new RenderTexture(Terrain.terrainData.heightmapResolution, Terrain.terrainData.heightmapResolution, 32, RenderTextureFormat.RHalf);
        RenderTexture.enableRandomWrite = true;
        RenderTexture.Create();

        SendDatas();
        ComputeShader.Dispatch(indexOfKernel, Terrain.terrainData.heightmapResolution / 32, Terrain.terrainData.heightmapResolution / 32, 1);
        RenderTexture.active = RenderTexture;
        RawImage.texture = RenderTexture;
        RenderTexture.active = null;
        RedrawTerrain();
    }

    public void ErodeTerrain()
    {
        
        float[,] mesh = new float[Terrain.terrainData.heightmapResolution, Terrain.terrainData.heightmapResolution];
        mesh = this.Terrain.terrainData.GetHeights(0, 0, mesh.GetLength(0), mesh.GetLength(1));
        TerrainErosion.Erode(mesh);
        this.Terrain.terrainData.SetHeights(0, 0, mesh);
        this.Terrain.terrainData.SyncHeightmap();
        
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
