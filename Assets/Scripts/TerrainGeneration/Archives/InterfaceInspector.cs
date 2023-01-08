using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(InterfaceTerrainShader))]
public class InterfaceInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var inter = (InterfaceTerrainShader)target;
        if (GUILayout.Button("Generate Erosion Texture"))
        {
            inter.ErosionTexture();
        }
    }
}
