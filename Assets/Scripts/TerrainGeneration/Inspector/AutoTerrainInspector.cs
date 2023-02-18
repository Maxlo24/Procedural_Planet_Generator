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
            //terrain.RedrawTerrain();
        }
        if (GUILayout.Button("Generate Craters"))
        {
            terrain.GenerateCraters();
            terrain.RedrawTerrains();
        }
        if (GUILayout.Button("Thermal Erosion"))
        {
            terrain.ThermalErosionCoroutine();
            terrain.RedrawTerrains();
        }
        if (GUILayout.Button("Hydraulic Erosion"))
        {
            terrain.HydraulicErosionCoroutine();
            terrain.RedrawTerrains();
        }
        if (GUILayout.Button("PostProcessing"))
        {
            terrain.PostProcessing();
            terrain.RedrawTerrains();
        }
        if (GUILayout.Button("Generate Entire Map"))
        {
            terrain.GenerateEntireMap();
        }
        if (GUILayout.Button("Clear Terrains"))
        {
            terrain.ClearRenderTextures();
            terrain.RemoveTerrainParentChilds();
        }
    }
}
