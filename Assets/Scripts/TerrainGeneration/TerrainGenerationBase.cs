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
    [field: SerializeField] public List<TerrainErosion> Erosions { get; private set; }
    [field: SerializeField] public GameObject ErosionsGameObject { get; private set; }
    [field: SerializeField] public TerrainProcessing TerrainProcessing { get; private set; }
    [field: SerializeField] public TerrainErosion TerrainErosion { get; private set; }
    [field: SerializeField] public ThermalErosion ThermalErosion { get; private set; }
    [field: SerializeField] public CraterGeneration CraterGeneration { get; private set; }
    [field: SerializeField] public TerrainPostProcessing TerrainPostProcessing { get; private set; }
    [field: SerializeField] public HeightMapsAddition HeightMapsAddition { get; private set; }
    [SerializeField] public RenderTexture RenderTexture;
    public RenderTexture ErosionTexture;
    public RenderTexture DepositTexture;

    [field: SerializeField] public String TerrainNameToSave { get; private set; } = "Terrain";
    [field: SerializeField] public String TerrainNameToAdd { get; private set; } = "Terrain";

    [field: SerializeField] public bool LiveUpdate { get; private set; }

    private RenderTexture RenderTextureCopy;
    private RenderTexture ErosionTextureCopy;
    private RenderTexture DepositTextureCopy;

    private void Start()
    {
        RenderTexture = ImageLib.CreateRenderTexture(Terrain.terrainData.heightmapResolution, Terrain.terrainData.heightmapResolution, RenderTextureFormat.RFloat);
    }

    private void Update()
    {
        if (RenderTexture == null)
        {
            RenderTexture = ImageLib.CreateRenderTexture(Terrain.terrainData.heightmapResolution, Terrain.terrainData.heightmapResolution, RenderTextureFormat.RFloat);
        }

        if (LiveUpdate)
        {
            GenerateTerrain();
        }
    }

    private void ApplyNoises()
    {
        foreach (Noise noise in Noises)
        {
            if (noise.Enabled)
            {
                noise.ApplyNoise(ref RenderTexture, Terrain);
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

        RenderTexture nulltexture = ImageLib.CreateRenderTexture(ErosionTexture.width, ErosionTexture.height, RenderTextureFormat.RFloat);
        Graphics.Blit(nulltexture, ErosionTexture);
        Graphics.Blit(nulltexture, DepositTexture);

        if (ErosionTextureCopy != null) ErosionTextureCopy.Release();
        if (DepositTextureCopy != null) DepositTextureCopy.Release();
        ErosionTextureCopy = ImageLib.CopyRenderTexture(ErosionTexture);
        DepositTextureCopy = ImageLib.CopyRenderTexture(DepositTexture);

        nulltexture.Release();
    }

    public void GenerateCraters()
    {
        CraterGeneration.GenerateCraters(ref RenderTexture, ref RenderTextureCopy, Terrain.terrainData.heightmapResolution);
        RedrawTerrain();
        Graphics.Blit(RenderTexture, RenderTextureCopy);
    }

    public void RunThermalErosion()
    {
        ThermalErosion.Erode(ref RenderTexture, Terrain);
        RedrawTerrain();
    }

    public void ErodeTerrain()
    {
        FillErosions();

        foreach (TerrainErosion erosion in Erosions)
        {
            if (erosion.Enabled)
            {
                for (int i = 0; i < erosion.RepetitionCount; i++)
                {

                    ErodeResult erosionResult = erosion.Erode(ref RenderTexture, ErosionTextureCopy, DepositTextureCopy);

                    if (TerrainErosion.GenerateErosionTexture)
                    {
                        Graphics.Blit(erosionResult.ErosionTexture, ErosionTextureCopy);
                        Graphics.Blit(erosionResult.DepositTexture, DepositTextureCopy);
                        RenderTexture temporaryErosion = ImageLib.CopyRenderTexture(ErosionTextureCopy);
                        RenderTexture temporaryDeposit = ImageLib.CopyRenderTexture(DepositTextureCopy);

                        ImageLib.NormalizeRenderTexture(ref temporaryErosion);
                        ImageLib.NormalizeRenderTexture(ref temporaryDeposit);
                        Graphics.Blit(temporaryErosion, ErosionTexture);
                        Graphics.Blit(temporaryDeposit, DepositTexture);
                        temporaryErosion.Release();
                        temporaryDeposit.Release();
                    }

                    RedrawTerrain();
                }
            }
        }

    }

    public void Smooth()
    {
        TerrainPostProcessing.SmoothTerrain(ref RenderTexture, RenderTextureCopy);
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

    public void FillErosions()
    {
        // Fill Noises with all enabled components of type Noise in NoisesGameObject
        Erosions = new List<TerrainErosion>();
        foreach (TerrainErosion erosion in ErosionsGameObject.GetComponents<TerrainErosion>())
        {
            if (erosion.enabled)
            {
                Erosions.Add(erosion);
            }
        }
    }
}
