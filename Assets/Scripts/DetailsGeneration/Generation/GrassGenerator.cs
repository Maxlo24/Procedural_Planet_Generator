using sc.terrain.vegetationspawner;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class GrassGenerator : MonoBehaviour
{
    [SerializeField] private Terrain terrain;


    [HideInInspector] public Vector2 slopeAngleRange = new Vector2(0f, 60f);
    [HideInInspector] public Vector2 altitudeRange = new Vector2(0f, 100f);


    [Range(0,100)]
    [SerializeField] private int grassDensity = 50;
    [Range(1, 5)]
    [SerializeField] private int nbrPerPatch = 1;

    [SerializeField] private int count = 0;

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
        AddGrassDetail();

        int[,] map = new int[terrain.terrainData.detailWidth, terrain.terrainData.detailHeight];


        for (int u = 0; u < terrain.terrainData.detailWidth; u++)
        {
            for (int v = 0; v < terrain.terrainData.detailHeight; v++)
            {

                Vector3 wpos = terrain.DetailToWorld(v, u);

                Vector2 UVpos = terrain.GetNormalizedPosition(wpos);

               
                if (Random.Range(0, 100) > grassDensity) continue;

                float altitude;
                terrain.SampleHeight(UVpos, out _, out altitude, out _);
                if (altitude < altitudeRange.x || altitude > altitudeRange.y) continue;

                float slopeAngle = terrain.GetSlope(UVpos);
                if (slopeAngle < slopeAngleRange.x || slopeAngle > slopeAngleRange.y) continue;

                count += nbrPerPatch;
                map[u, v] = nbrPerPatch;

            }
        }

        terrain.terrainData.SetDetailLayer(0, 0, 0, map);

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



}
