using System;
using System.IO;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class TerrainGenerationBaseCPU : MonoBehaviour
{
    [field: SerializeField] public Terrain Terrain { get; private set; }
    [field: SerializeField] public Perlin Perlin { get; private set; }
    [field: SerializeField] public RenderTexture RenderTexture { get; private set; }
    [field: SerializeField] public RawImage RawImage { get; private set; }
    [field: SerializeField] public Button Button { get; private set; }

    private float[,] mesh;
    
    private void Start()
    {
        Button.onClick.AddListener(RedrawTerrainCPU);
        mesh = new float[Terrain.terrainData.heightmapResolution, Terrain.terrainData.heightmapResolution];
        RedrawTerrainCPU();
    }

    private void Update()
    {
        // if E is down save the texture
        if (Input.GetKeyDown(KeyCode.E))
        {
            ImageLib.SavePNG(mesh, "Assets/", "Terrain");
        }
       
        if (Input.GetKeyDown(KeyCode.A))
        {
            RedrawTerrainCPU();
        }
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
        // render tex on raw image
        RenderTexture rt = new RenderTexture(tex.width / 2, tex.height / 2, 0);
        RenderTexture.active = rt;
        // Copy your texture ref to the render texture
        Graphics.Blit(tex, rt);
        RawImage.texture = rt;
        RenderTexture.active = null;

        this.Terrain.terrainData.SetHeights(0, 0, mesh);
    }
}
