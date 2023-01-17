using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class AutoTerrainGeneration : MonoBehaviour
{
    [field: SerializeField] public int Seed { get; private set; } = -1;
    [field: SerializeField] public Terrain Terrain { get; private set; }
    [field: SerializeField] public TerrainGen TerrainGeneration { get; private set; }

    public RenderTexture RenderTexture;
    private RenderTexture RenderTextureCopy;
    public RenderTexture ErosionTexture;
    public RenderTexture DepositTexture;
    private RenderTexture ErosionTextureCopy;
    private RenderTexture DepositTextureCopy;

    public void RedrawTerrain()
    {
        RenderTexture.active = RenderTexture;
        Terrain.terrainData.CopyActiveRenderTextureToHeightmap(new RectInt(0, 0, Terrain.terrainData.heightmapResolution, Terrain.terrainData.heightmapResolution),
            Vector2Int.zero, TerrainHeightmapSyncControl.HeightAndLod);
        RenderTexture.active = null;
    }

    public void GenerateTerrain()
    {
        int seed = (Seed < 0) ? UnityEngine.Random.Range(0, int.MaxValue) : Seed;

        if (RenderTexture != null)
            RenderTexture.Release();
        RenderTexture = ImageLib.CreateRenderTexture(Terrain.terrainData.heightmapResolution, Terrain.terrainData.heightmapResolution, RenderTextureFormat.RFloat);
        
        foreach (SingleNoise noisePreset in TerrainGeneration.Noises)
        {
            if (noisePreset == null) continue;
            Noise noise = new Noise(noisePreset, seed);
            noise.ApplyNoise(ref RenderTexture, Terrain);
            seed++;
        }

        if (RenderTextureCopy != null) RenderTextureCopy.Release();
        RenderTextureCopy = ImageLib.CopyRenderTexture(RenderTexture);

        RenderTexture nulltexture = ImageLib.CreateRenderTexture(ErosionTexture.width, ErosionTexture.height, RenderTextureFormat.RFloat);
        Graphics.Blit(nulltexture, ErosionTexture);
        Graphics.Blit(nulltexture, DepositTexture);

        if (ErosionTextureCopy != null) ErosionTextureCopy.Release();
        if (DepositTextureCopy != null) DepositTextureCopy.Release();
        ErosionTextureCopy = ImageLib.CopyRenderTexture(ErosionTexture);
        DepositTextureCopy = ImageLib.CopyRenderTexture(DepositTexture);

        nulltexture.Release();
    }

    public void ThermalErosion()
    {
        int seed = (Seed < 0) ? UnityEngine.Random.Range(0, int.MaxValue) : Seed;

        foreach (ThermalErosionPreset erosionPreset in TerrainGeneration.ThermalErosions)
        {
            if (erosionPreset == null) continue;
            ThermalErosion erosion = new ThermalErosion(erosionPreset, seed);
            for (int i = 0; i < erosion.RepetitionCount; i++)
            {
                erosion.Erode(ref RenderTexture, Terrain);
            }
        }
    }

    public void ErodeTerrain()
    {
        int seed = (Seed < 0) ? UnityEngine.Random.Range(0, int.MaxValue) : Seed;

        foreach (ErosionPreset erosionPreset in TerrainGeneration.Erosions)
        {
            if (erosionPreset == null) continue;
            TerrainErosion erosion = new TerrainErosion(erosionPreset, seed);
            for (int i = 0; i < erosion.RepetitionCount; i++)
            {
                erosion.Erode(ref RenderTexture, ErosionTextureCopy, DepositTextureCopy);
            }
        }
    }

    public void GenerateCraters()
    {
        int seed = (Seed < 0) ? UnityEngine.Random.Range(0, int.MaxValue) : Seed;
        foreach (CraterPreset craterPreset in TerrainGeneration.Craters)
        {
            if (craterPreset == null) continue;
            Debug.Log("Generating crater: " + seed);
            CraterGeneration craterGeneration = new CraterGeneration(craterPreset, seed);
            craterGeneration.GenerateCraters(ref RenderTexture, ref RenderTextureCopy, Terrain.terrainData.heightmapResolution);
            if (RenderTextureCopy != null) RenderTextureCopy.Release();
            RenderTextureCopy = ImageLib.CopyRenderTexture(RenderTexture);
            seed /= 2;
        }
    }

    public void PostProcessing()
    {
        if (TerrainGeneration.PostProcessing == null)
            return;
        TerrainPostProcessing postProcessing = new TerrainPostProcessing(TerrainGeneration.PostProcessing);
        postProcessing.SmoothTerrain(ref RenderTexture, null);
    }

    public void GenerateEntireMap()
    {
        GenerateTerrain();
        GenerateCraters();
        ErodeTerrain();
        ThermalErosion();
        PostProcessing();
        RedrawTerrain();
    }
}
