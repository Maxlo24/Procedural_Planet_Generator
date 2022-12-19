using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainGenerationBaseCPU))]
public class CPUTerrainInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var terrain = (TerrainGenerationBaseCPU)target;
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
