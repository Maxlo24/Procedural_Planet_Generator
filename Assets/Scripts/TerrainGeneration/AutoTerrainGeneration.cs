using UnityEngine;

public enum TerrainResolution
{
    _65 = 65,
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

    [field: SerializeField] public Terrain TerrainRef { get; private set; }
    //[field: SerializeField] public Terrain Terrain { get; private set; }
    [field: SerializeField] public TerrainResolution TerrainResolution { get; private set; } = TerrainResolution._1025;
    [field: SerializeField] public int TerrainWidth { get; private set; } = 250;
    [field: SerializeField] public int TerrainHeight { get; private set; } = 500;
    [field: SerializeField] public int TerrainCountSquareRoot { get; private set; } = 9;
    [field: SerializeField] public TerrainGen TerrainGeneration { get; set; }
    [field: SerializeField] public GameObject TerrainParent { get; set; }
    [field: SerializeField] public Material TerrainMaterial { get; set; }

    private Terrain Terrain;
    private Terrain[,] Terrains;
    public RenderTexture[,] RenderTextures;
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

    public void RedrawTerrain(Terrain terrain, RenderTexture rt)
    {
        RenderTexture.active = rt;
        terrain.terrainData.CopyActiveRenderTextureToHeightmap(new RectInt(0, 0, terrain.terrainData.heightmapResolution, terrain.terrainData.heightmapResolution),
            Vector2Int.zero, TerrainHeightmapSyncControl.HeightAndLod);
        RenderTexture.active = null;
    }


    public void GenerateTerrain()
    {
        ClearRenderTextures();
        RemoveTerrainParentChilds();

        int seed = (Seed < 0) ? UnityEngine.Random.Range(0, int.MaxValue) : Seed;
        
        Terrains = new Terrain[TerrainCountSquareRoot, TerrainCountSquareRoot];
        RenderTextures = new RenderTexture[TerrainCountSquareRoot, TerrainCountSquareRoot];

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
        //RedrawTerrain();
    }
}
