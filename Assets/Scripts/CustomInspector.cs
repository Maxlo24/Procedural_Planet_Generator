using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GenerationManager))]
public class CustomInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate"))
        {
            ((GenerationManager)target).Generate();
        }
        if (GUILayout.Button("Clear"))
        {
            ((GenerationManager)target).Clear();
        }
    }

}
