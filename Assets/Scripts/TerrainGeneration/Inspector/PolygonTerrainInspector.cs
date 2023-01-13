using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VoronoiDiagram))]
public class PolygonTerrainInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var terrain = (VoronoiDiagram)target;
        if (GUILayout.Button("Generate Terrain"))
        {
            terrain.ComputeVoronoiDiagram();
        }
        if (GUILayout.Button("Generate Water Map"))
        {
            terrain.GenerateNoiseLand();
        }
    }
}
