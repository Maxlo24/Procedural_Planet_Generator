using Assets.Scripts.DetailsGeneration;
using sc.terrain.vegetationspawner;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocksGenerator : MonoBehaviour
{
    [SerializeField] private Terrain terrain;
    [SerializeField] private Transform spawnRoot;

    [Header("Details options")]
    [SerializeField] private DetailsType[] _detailsType;
    [SerializeField] private DetailsSize _detailsSize;


    [SerializeField] public Vector2 slopeAngleRange = new Vector2(0f, 25f);
    [SerializeField] public Vector2 altitudeRange = new Vector2(-20f, 100f);

    [Range(0, 100)]
    [SerializeField] private int elementDensity = 50;

    [Header("Scale")]
    [SerializeField] private Vector2 _scaleRandomness = new Vector2(1f, 1f);
    [SerializeField] private Vector3 _rotationRandomness = new Vector3(0, 1, 0);

    private float _randomRotation = 1.0f;

    private int MaxCount = 10000;


    public void SpawnElements()
    {
        //LoadTerrain();

        //int[,] map = new int[terrain.terrainData.detailWidth, terrain.terrainData.detailHeight];

        GenerationManager generationManager = GameObject.FindGameObjectWithTag("GenerationManager").GetComponent<GenerationManager>();

        //terrain.terrainData.SetDetailResolution(1000, 2);

        GameObject[] selectionList = generationManager.GetDetailsList(_detailsType, _detailsSize);

        if (selectionList.Length == 0)
        {
            Debug.LogError("No corresponding details in collection");
            return;
        }


        int count = 0;

        for (int u = 0; u < terrain.terrainData.detailWidth; u += 1)
        {
            for (int v = 0; v < terrain.terrainData.detailHeight; v += 1)
            {
                if (Random.Range(0, 100000) > elementDensity) continue;

                
                Vector3 wpos = terrain.DetailToWorld(v, u);

                Vector2 UVpos = terrain.GetNormalizedPosition(wpos);



                float altitude;
                terrain.SampleHeight(UVpos, out _, out altitude, out _);
                if (altitude < altitudeRange.x || altitude > altitudeRange.y) continue;

                float slopeAngle = terrain.GetSlope(UVpos);
                if (slopeAngle < slopeAngleRange.x || slopeAngle > slopeAngleRange.y) continue;

                //count += nbrPerPatch;
                //map[u, v] = nbrPerPatch;

                Vector3 spawnPosition = wpos;


                RaycastHit hit;

                if (Physics.Raycast(wpos + Vector3.up * 100, Vector3.down, out hit, 1000, 1 << 6))
                {
                    spawnPosition = hit.point;
                }

                int randomIndex = Random.Range(0, selectionList.Length);
                GameObject randomObject = selectionList[randomIndex];
                GameObject spawnedObject = Instantiate(randomObject, spawnPosition, transform.rotation);


                float newScale = Random.Range(_scaleRandomness.x, _scaleRandomness.y);


                spawnedObject.transform.localScale = spawnedObject.transform.localScale * newScale;

                //spawnedObject.transform.localScale = new Vector3(
                //    spawnedObject.transform.localScale.x * newMainScale),
                //    spawnedObject.transform.localScale.y * newMainScale),
                //    spawnedObject.transform.localScale.z * newMainScale)
                //);

                spawnedObject.transform.Rotate(new Vector3(
                    Random.Range(-_randomRotation, _randomRotation) * 180 * _rotationRandomness.x,
                    Random.Range(-_randomRotation, _randomRotation) * 180 * _rotationRandomness.y,
                    Random.Range(-_randomRotation, _randomRotation) * 180 * _rotationRandomness.z
                ));
                
                spawnedObject.transform.parent = spawnRoot;
                //spawnedObject.GetComponent<DetailEntity>().ActivateSpawner();

                count++;
                if (count > MaxCount) break;

            }
        }

        //terrain.terrainData.SetDetailLayer(0, 0, 0, map);

    }
}


