using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AutoTerrainGeneration))]
public class AutoTerrainInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var terrain = (AutoTerrainGeneration)target;
        if (GUILayout.Button("Generate Terrain"))
        {
            terrain.GenerateTerrain();
            terrain.RedrawTerrain();
        }
        if (GUILayout.Button("Generate Craters"))
        {
            terrain.GenerateCraters();
            terrain.RedrawTerrain();
        }
        if (GUILayout.Button("Erode Terrain"))
        {
            terrain.ErodeTerrain();
            terrain.RedrawTerrain();
        }
        if (GUILayout.Button("Generate Entire Map"))
        {
            terrain.GenerateEntireMap();
        }
    }
}
