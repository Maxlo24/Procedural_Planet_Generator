using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCraterPreset", menuName = "Scriptable objects/Terrain/CraterPreset", order = 0)]
public class CraterPreset : ScriptableObject
{
    [SerializeField] public string Name;
    [SerializeField] public Vector2Int CraterCount = new Vector2Int(0, 1);
    [SerializeField] public bool InBounds = false;
    [SerializeField] public Vector2 RadiusRange = new Vector2(2, 5);
    [SerializeField] public Vector2 SecondaryRadiusOffsetLimits = new Vector2(2, 5);
    [SerializeField] public Vector2 ThirdRadiusOffsetLimits = new Vector2(2, 5);
    [SerializeField] public Vector2 DepthRange = new Vector2(0.5f, 1);
    [SerializeField] public Vector2 ElevationRange = new Vector2(0.5f, 1);
}
