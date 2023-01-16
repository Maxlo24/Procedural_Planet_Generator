using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlanetGlobalGeneration))]

public class PlanetAttributesCI : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10);
        if (GUILayout.Button("Random attributes"))
        {
            ((PlanetGlobalGeneration)target).GenerateRandomAttributes();
        }

        //GUILayout.Space(10);
        //GUILayout.BeginHorizontal();


        //if (GUILayout.Button("Spawn"))
        //{
        //    ((GenerationManager)target).Spwan();
        //}
        //if (GUILayout.Button("Clip to floor"))
        //{
        //    ((GenerationManager)target).ClipToFloor();
        //}
        //if (GUILayout.Button("Clear"))
        //{
        //    ((GenerationManager)target).Clear();
        //}

        //GUILayout.EndHorizontal();

        //if (GUILayout.Button("Generate elements"))
        //{
        //    ((GenerationManager)target).GenerateElements();
        //}

    }
}
