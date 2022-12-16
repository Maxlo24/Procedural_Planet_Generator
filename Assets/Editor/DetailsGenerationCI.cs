using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GenerationManager))]
public class CustomInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        GUILayout.Space(10);
        if (GUILayout.Button("Generate"))
        {
            ((GenerationManager)target).Generate();
        }

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();


        if (GUILayout.Button("Spawn"))
        {
            ((GenerationManager)target).Spwan();
        }
        if (GUILayout.Button("Clip to floor"))
        {
            ((GenerationManager)target).ClipToFloor();
        }
        if (GUILayout.Button("Clear"))
        {
            ((GenerationManager)target).Clear();
        }

        GUILayout.EndHorizontal();
    }

}
