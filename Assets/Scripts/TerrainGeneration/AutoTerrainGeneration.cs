using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public enum TerrainResolution
{
    _129 = 129,
    _257 = 257,
    _513 = 513,
    _1025 = 1025,
    _2049 = 2049,
    _4097 = 4097,
}

public class AutoTerrainGeneration : MonoBehaviour
{
    [field: SerializeField] public int Seed { get; set; } = -1;

    [field: SerializeField] public TerrainResolution TerrainResolution { get; private set; } = TerrainResolution._1025;
    [field: SerializeField] public int TerrainWidth { get; private set; } = 250;
    [field: SerializeField] public int TerrainHeight { get; private set; } = 500;
    [field: SerializeField] public int TerrainCountSquareRoot { get; private set; } = 9;
    [field: SerializeField] public TerrainGen TerrainGeneration { get; set; }
    [field: SerializeField] public GameObject TerrainParent { get; set; }
    [field: SerializeField] public Material TerrainMaterial { get; set; }
    [field: SerializeField] public float GPUPauseTime { get; private set; } = 0.5f;

    private Terrain[,] Terrains;
    public RenderTexture[,] RenderTextures;
    private RenderTexture[,] RenderTexturesCopy;


    public void RedrawTerrain(Terrain terrain, RenderTexture rt)
    {
        RenderTexture.active = rt;
        terrain.terrainData.CopyActiveRenderTextureToHeightmap(new RectInt(0, 0, terrain.terrainData.heightmapResolution, terrain.terrainData.heightmapResolution),
            Vector2Int.zero, TerrainHeightmapSyncControl.HeightAndLod);
        RenderTexture.active = null;
    }

    public void RedrawTerrains()
    {
        for (int i = 0; i < TerrainCountSquareRoot; i++)
        {
            for (int j = 0; j < TerrainCountSquareRoot; j++)
            {
                RedrawTerrain(Terrains[i, j], RenderTextures[i, j]);
            }
        }
    }


    public void GenerateTerrain()
    {
        ClearRenderTextures();
        RemoveTerrainParentChilds();

        int seed = (Seed < 0) ? UnityEngine.Random.Range(0, int.MaxValue) : Seed;
        
        Terrains = new Terrain[TerrainCountSquareRoot, TerrainCountSquareRoot];
        RenderTextures = new RenderTexture[TerrainCountSquareRoot, TerrainCountSquareRoot];
        RenderTexturesCopy = new RenderTexture[TerrainCountSquareRoot, TerrainCountSquareRoot];

        Noise[] noises = new Noise[TerrainGeneration.Noises.Count];
        for (int n = 0; n < TerrainGeneration.Noises.Count; n++)
        {
            SingleNoise noisePreset = TerrainGeneration.Noises[n];
            if (noisePreset == null) continue;
            Noise noise = new Noise(noisePreset, seed);
            noises[n] = noise;
        }

        for (int i = 0; i < TerrainCountSquareRoot; i++)
        {
            for (int j = 0; j < TerrainCountSquareRoot; j++)
            {
                TerrainData terrainData = new TerrainData();
                terrainData.heightmapResolution = (int)TerrainResolution;
                terrainData.size = new Vector3(TerrainWidth, TerrainHeight, TerrainWidth);
                Terrain terrain = Terrain.CreateTerrainGameObject(terrainData).GetComponent<Terrain>();
                terrain.transform.SetParent(TerrainParent.transform);
                terrain.transform.position = new Vector3(i * TerrainWidth, 0, j * TerrainWidth);

                RenderTexture renderTexture = ImageLib.CreateRenderTexture(terrain.terrainData.heightmapResolution, terrain.terrainData.heightmapResolution, RenderTextureFormat.RFloat);
                
                foreach (Noise noise in noises)
                {
                    noise.ApplyNoise(ref renderTexture, terrain, new Vector2(i, j));
                }

                if (TerrainMaterial != null)
                {
                    terrain.materialTemplate = TerrainMaterial;
                }

                Terrains[i, j] = terrain;
                RenderTextures[i, j] = renderTexture;
                RenderTexturesCopy[i, j] = ImageLib.CopyRenderTexture(renderTexture);
                RedrawTerrain(terrain, renderTexture);
            }
        }
        SetTerrainsNeighbors();
    }
    
    private void SetTerrainsNeighbors()
    {
        for (int i = 0; i < TerrainCountSquareRoot; i++)
        {
            for (int j = 0; j < TerrainCountSquareRoot; j++)
            {
                Terrain terrain = Terrains[i, j];
                Terrain terrainX = i > 0 ? Terrains[i - 1, j] : null;
                Terrain terrainZ = j > 0 ? Terrains[i, j - 1] : null;
                Terrain terrain_X = i < TerrainCountSquareRoot - 1 ? Terrains[i + 1, j] : null;
                Terrain terrain_Z = j < TerrainCountSquareRoot - 1 ? Terrains[i, j + 1] : null;

                terrain.SetNeighbors(terrainX, terrain_Z, terrain_X, terrainZ);
            }
        }
    }

    public void ClearRenderTextures()
    {
        if (RenderTextures == null) return;
        foreach (RenderTexture rt in RenderTextures)
        {
            rt?.Release();
        }
    }   

    public void RemoveTerrainParentChilds()
    {
        for (int i = TerrainParent.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(TerrainParent.transform.GetChild(i).gameObject);
        }
    }

    public IEnumerator ThermalErosion()
    {
        int seed = (Seed < 0) ? UnityEngine.Random.Range(0, int.MaxValue) : Seed;

        for (int i = 0; i < TerrainCountSquareRoot; i++)
        {
            for (int j = 0; j < TerrainCountSquareRoot; j++)
            {
                Debug.Log("Thermal erosion " + i + " " + j);
                foreach (ThermalErosionPreset erosionPreset in TerrainGeneration.ThermalErosions)
                {
                    if (erosionPreset == null) continue;
                    ThermalErosion erosion = new ThermalErosion(erosionPreset, seed);
                    for (int erodeCount = 0; erodeCount < erosion.RepetitionCount; erodeCount++)
                    {
                        erosion.Erode(ref RenderTextures[i, j], Terrains[i, j]);
                    }
                }
                yield return new WaitForSeconds(GPUPauseTime);
            }
        }
    }

    public IEnumerator HydraulicErosion()
    {
        int seed = (Seed < 0) ? UnityEngine.Random.Range(0, int.MaxValue) : Seed;

        for (int i = 0; i < TerrainCountSquareRoot; i++)
        {
            for (int j = 0; j < TerrainCountSquareRoot; j++)
            {
                Debug.Log("Hydraulic erosion " + i + " " + j);
                foreach (ErosionPreset erosionPreset in TerrainGeneration.Erosions)
                {
                    if (erosionPreset == null) continue;
                    TerrainErosion erosion = new TerrainErosion(erosionPreset, seed);
                    for (int erodeCount = 0; erodeCount < erosion.RepetitionCount; erodeCount++)
                    {
                        erosion.Erode(ref RenderTextures[i, j], null, null);
                    }
                }
                yield return new WaitForSeconds(GPUPauseTime);
            }
        }
    }

    public IEnumerator GenerateCraters()
    {
        int seed = (Seed < 0) ? UnityEngine.Random.Range(0, int.MaxValue) : Seed;

        for (int i = 0; i < TerrainCountSquareRoot; i++)
        {
            for (int j = 0; j < TerrainCountSquareRoot; j++)
            {
                foreach (CraterPreset craterPreset in TerrainGeneration.Craters)
                {
                    if (craterPreset == null) continue;
                    Debug.Log("Generating crater: " + seed);
                    CraterGeneration craterGeneration = new CraterGeneration(craterPreset, seed);
                    craterGeneration.GenerateCraters(ref RenderTextures[i, j], ref RenderTexturesCopy[i, j], Terrains[i, j].terrainData.heightmapResolution);
                    if (RenderTexturesCopy[i,j] != null) RenderTexturesCopy[i, j].Release();
                    RenderTexturesCopy[i, j] = ImageLib.CopyRenderTexture(RenderTextures[i, j]);
                    seed /= 2;
                }
                yield return new WaitForSeconds(GPUPauseTime);
            }
        }
    }

    public void HydraulicErosionCoroutine()
    {
        StartCoroutine(HydraulicErosion());
    }
    
    public void ThermalErosionCoroutine()
    {
        StartCoroutine(ThermalErosion());
    }

    public void GenerateCratersCoroutine()
    {
        StartCoroutine(GenerateCraters());
    }

    public void PostProcessing()
    {
        if (TerrainGeneration.PostProcessing == null)
            return;

        for (int i = 0; i < TerrainCountSquareRoot; i++)
        {
            for (int j = 0; j < TerrainCountSquareRoot; j++)
            {
                TerrainPostProcessing postProcessing = new TerrainPostProcessing(TerrainGeneration.PostProcessing);
                postProcessing.SmoothTerrain(ref RenderTextures[i, j], null);
            }
        }

    }

    public void GenerateEntireMap()
    {
        GenerateTerrain();
        StartCoroutine(GenerateCraters());
        StartCoroutine(HydraulicErosion());
        StartCoroutine(ThermalErosion());
        PostProcessing();
        //RedrawTerrain();
    }
}
