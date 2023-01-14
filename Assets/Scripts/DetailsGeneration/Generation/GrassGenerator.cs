using sc.terrain.vegetationspawner;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static sc.terrain.vegetationspawner.SpawnerBase;
using static UnityEditor.Progress;


public class GrassGenerator : MonoBehaviour
{
    [SerializeField] private Terrain terrain;

    //[SerializeField] private Grass[] grassAssets;

    //public DetailPrototype grass_prototype;


    [HideInInspector] public Vector2 slopeAngleRange = new Vector2(0f, 60f);
    [HideInInspector] public Vector2 altitudeRange = new Vector2(0f, 100f);
    [HideInInspector] public Vector2 heightRange = new Vector2(0f, 100f);
    [HideInInspector] public Vector2 widthRange = new Vector2(0f, 100f);
    

    [Range(0,100)]
    [SerializeField] private int grassDensity = 50;
    [Range(1, 10)]
    [SerializeField] private int nbrPerPatch = 1;
    [SerializeField] private float noiseScale = 1f;

    public GrassAsset[] grassAssets;


    private int count = 0;

    private void LoadTerrain()
    {

        Terrain t = GameObject.FindGameObjectWithTag("Terrain").GetComponent<Terrain>();

        //terrain = GetComponent<Terrain>();

        if (terrain == null)
        {
            Debug.LogError("Terrain not found");
            return;
        }

        terrain = t;
    }

    public void SpawnGrass()
    {
        //LoadTerrain();
        //AddGrassDetail();

        int index = 0;
        foreach (GrassAsset grass in grassAssets)
        {

            int[,] map = new int[terrain.terrainData.detailWidth, terrain.terrainData.detailHeight];

            for (int u = 0; u < terrain.terrainData.detailWidth; u++)
            {
                for (int v = 0; v < terrain.terrainData.detailHeight; v++)
                {

                    Vector3 wpos = terrain.DetailToWorld(v, u);

                    Vector2 UVpos = terrain.GetNormalizedPosition(wpos);


                    if (Random.Range(0, 100) > grass.density) continue;

                    float altitude;
                    terrain.SampleHeight(UVpos, out _, out altitude, out _);
                    if (altitude < altitudeRange.x || altitude > altitudeRange.y) continue;

                    float distanceToLimit = Mathf.Min(altitude - altitudeRange.x, 5f);

                    if (Random.Range(0, 5) > distanceToLimit) continue;


                    float slopeAngle = terrain.GetSlope(UVpos);
                    if (slopeAngle < slopeAngleRange.x || slopeAngle > slopeAngleRange.y) continue;

                    count += nbrPerPatch;
                    map[u, v] = nbrPerPatch;

                }
            }

            terrain.terrainData.SetDetailLayer(0, 0, index, map);
            index++;
        }
    }

    public void ClearGrass()
    {
        int[,] map = new int[terrain.terrainData.detailWidth, terrain.terrainData.detailHeight];

        for (int i = 0; i < terrain.terrainData.detailPrototypes.Length; i++)
        {
            terrain.terrainData.SetDetailLayer(0, 0, i, map);
        }
    }

    public void AddGrassDetail()
    {
        DetailPrototype[] detailPrototypes = terrain.terrainData.detailPrototypes;

        Debug.Log("Detail Prototypes: " + detailPrototypes.Length);
        //DetailPrototype detailPrototype = GetGrassPrototype(item, terrain);

        ////Could have been removed
        //if (detailPrototype == null) continue;

        //UpdateGrassItem(item, detailPrototype);

        //detailPrototypes[0] = detailPrototype;
        //terrain.terrainData.detailPrototypes = detailPrototypes;
    }

    public void UpdateGrassAssets(GrassAsset[] newGrassAssets)
    {
        grassAssets = newGrassAssets;
        UpdateGrassPrototypes();
    }
    

    public void UpdateGrassPrototypes()
    {

        //Grass item = new Grass();

        List<DetailPrototype> grassPrototypeCollection = new List<DetailPrototype>();
        int index = 0;
        foreach (GrassAsset item in grassAssets)
        {
            //item.index = index;

            DetailPrototype detailPrototype = new DetailPrototype();

            GenerateGrassPrototype(item, detailPrototype);

            grassPrototypeCollection.Add(detailPrototype);

            index++;
        }
        if (grassPrototypeCollection.Count > 0) terrain.terrainData.detailPrototypes = grassPrototypeCollection.ToArray();
    }

    private void GenerateGrassPrototype(GrassAsset item , DetailPrototype detail)
    {
        

        if (item.useOriginalColor)
        {
            detail.healthyColor = Color.white;
            detail.dryColor = Color.white;
        }
        else
        {
            detail.healthyColor = item.healthyColor;
            detail.dryColor = item.dryColor;
        }

        //= item.useOriginalColor ? Color.white : item.healthyColor;
        //detail.dryColor = item.useOriginalColor ? Color.white : item.dryColor;
        detail.minHeight = heightRange.x * item.sizeMultiplyer.y;
        detail.maxHeight = heightRange.y * item.sizeMultiplyer.y;

        detail.minWidth = widthRange.x * item.sizeMultiplyer.x;
        detail.maxWidth = widthRange.y * item.sizeMultiplyer.x;

#if UNITY_2021_2_OR_NEWER
        detail.noiseSeed = Random.Range(0, 100000);
#endif
        detail.noiseSpread = noiseScale;

        //if (item.type == GrassType.Texture && item.billboard)
        //{
#if UNITY_2021_2_OR_NEWER
        detail.useInstancing = false;
#endif
        detail.renderMode = DetailRenderMode.Grass;
        detail.usePrototypeMesh = false;
        detail.prototypeTexture = item.texture;
        detail.prototype = null;
        //}

    }




}
