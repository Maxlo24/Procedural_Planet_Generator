using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainGenerationBase))]
public class GPUTerrainInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var terrain = (TerrainGenerationBase)target;
        GUILayout.Label("Warning : Live Update need to be unchecked to erode the terrain");
        if (GUILayout.Button("Generate Terrain"))
        {
            terrain.GenerateTerrain();
        }
        if (GUILayout.Button("Erode Terrain"))
        {
            terrain.ErodeTerrain();
        }
    }
}
