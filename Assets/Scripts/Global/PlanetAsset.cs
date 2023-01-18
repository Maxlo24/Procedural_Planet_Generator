using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewPlanet", menuName = "Scriptable objects/Planet/Planet asset", order = 0)]

public class PlanetAsset : ScriptableObject
{

    [Header("Global attributes")]
    [SerializeField] public Vector2 atmosphereRange = new Vector2(0, 3);
    [SerializeField] public Vector2 humidityRange = new Vector2(0, 100);
    [SerializeField] public Vector2 temperatureRange = new Vector2(-25, 100);
    [SerializeField] public Vector2 vegetationRange = new Vector2(0, 100);

    [SerializeField] public bool forceSnow = false;


    [SerializeField] public CrustColorPalette[] crustColorPalettes = new CrustColorPalette[0];

    [Header("Planet Generation")]
    [SerializeField] public NoiseLayer[] noiseLayers = new NoiseLayer[0];


    [Header("Terrain Generation")]
    [SerializeField] public TerrainGen[] terrainGen;


    [Header("Details generation")]
    [SerializeField] public DetailsGenerationSequence[] detailsGenerationSequences = new DetailsGenerationSequence[0];

}
