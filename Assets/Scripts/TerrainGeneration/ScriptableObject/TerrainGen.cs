using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTerrain", menuName = "Scriptable objects/Terrain/TerrainPreset", order = 0)]
public class TerrainGen : ScriptableObject
{
    [SerializeField] public string Name;
    [SerializeField] public List<SingleNoise> Noises;
    [SerializeField] public List<ErosionPreset> Erosions;
    [SerializeField] public ThermalErosionPreset ThermalErosion;
    [SerializeField] public List<CraterPreset> Craters;
}
