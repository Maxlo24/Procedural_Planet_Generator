using System;
using System.IO;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

[ExecuteInEditMode]
public class TerrainGenerationBaseCPU : MonoBehaviour
{
    [field: SerializeField] public Terrain Terrain { get; private set; }
    [field: SerializeField] public Perlin Perlin { get; private set; }
    [field: SerializeField] public RenderTexture RenderTexture { get; private set; }
    [field: SerializeField] public TerrainErosion TerrainErosion { get; private set; }
    [field: SerializeField] public HeightMapsAddition HeightMapsAddition { get; private set; }
    [field: SerializeField] public RawImage RawImage { get; private set; }
    [field: SerializeField] public int ErosionIteration { get; private set; }
    [field: SerializeField] public String TerrainNameToSave { get; private set; } = "Terrain";
    [field: SerializeField] public String TerrainNameToAdd { get; private set; } = "Terrain";

    public float[,] mesh;

    public bool isErosion = false;

    private void Start()
    {
        mesh = new float[Terrain.terrainData.heightmapResolution, Terrain.terrainData.heightmapResolution];
        RedrawTerrainCPU();
    }

    private void Update()
    {
    }

    private void RedrawTerrainCPU()
    {
        int resolution = Terrain.terrainData.heightmapResolution;
        mesh = new float[resolution, resolution];
        var tex = new Texture2D(mesh.GetLength(0), mesh.GetLength(1));

        // Generate terrain with perlin noise
        for (int x = 0; x < resolution; x++)
        {
            for (int y = 0; y < resolution; y++)
            {
                float x_n = (float)x / resolution;
                float y_n = (float)y / resolution;
                mesh[x, y] = Perlin.PerlinNoise(x_n, y_n);
                tex.SetPixel(x, y, new Color(mesh[x, y], mesh[x, y], mesh[x, y]));
            }
        }
        tex.Apply();

        if (isErosion)
        {
            TerrainErosion.Erode(mesh);
        }

        // render tex on raw image
        RenderTexture rt = new RenderTexture(tex.width / 2, tex.height / 2, 0);
        RenderTexture.active = rt;
        // Copy your texture ref to the render texture
        Graphics.Blit(tex, rt);
        RawImage.texture = rt;
        RenderTexture.active = null;

        this.Terrain.terrainData.SetHeights(0, 0, mesh);
    }

    public void GenerateTerrain()
    {
        int resolution = Terrain.terrainData.heightmapResolution;
        mesh = new float[resolution, resolution];
        var tex = new Texture2D(mesh.GetLength(0), mesh.GetLength(1));

        // Generate terrain with perlin noise
        for (int x = 0; x < resolution; x++)
        {
            for (int y = 0; y < resolution; y++)
            {
                float x_n = (float)x / resolution;
                float y_n = (float)y / resolution;
                mesh[x, y] = Perlin.PerlinNoise(x_n, y_n);
                tex.SetPixel(x, y, new Color(mesh[x, y], mesh[x, y], mesh[x, y]));
            }
        }
        tex.Apply();

        // render tex on raw image
        RenderTexture rt = new RenderTexture(tex.width / 2, tex.height / 2, 0);
        RenderTexture.active = rt;
        // Copy your texture ref to the render texture
        Graphics.Blit(tex, rt);
        RawImage.texture = rt;
        RenderTexture.active = null;

        this.Terrain.terrainData.SetHeights(0, 0, mesh);
    }

    public void ErodeTerrain()
    {
        if (mesh == null)
        {
            mesh = new float[Terrain.terrainData.heightmapResolution, Terrain.terrainData.heightmapResolution];
            mesh = this.Terrain.terrainData.GetHeights(0, 0, mesh.GetLength(0), mesh.GetLength(1));
        }
        TerrainErosion.Erode(mesh);
        this.Terrain.terrainData.SetHeights(0, 0, mesh);
        this.Terrain.terrainData.GetHeights(0, 0, mesh.GetLength(0), mesh.GetLength(1));
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
        this.mesh = mesh;
        Terrain.terrainData.SetHeights(0, 0, mesh);
    }
}

