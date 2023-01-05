using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainGenerationBase))]
public class GPUTerrainInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var terrain = (TerrainGenerationBase)target;
        GUILayout.Label("Warning : Live Update need to be unchecked to erode the terrain or add a new terrain");
        if (GUILayout.Button("Generate Terrain"))
        {
            terrain.GenerateTerrain();
        }
        if (GUILayout.Button("Erode Terrain"))
        {
            terrain.ErodeTerrain();
        }
        if (GUILayout.Button("Smooth Terrain"))
        {
            terrain.Smooth();
        }
        if (GUILayout.Button("Save Heightmap"))
        {
            terrain.Save();
        }
        if (GUILayout.Button("Add Heightmap Test"))
        {
            terrain.Add();
        }
    }
}
