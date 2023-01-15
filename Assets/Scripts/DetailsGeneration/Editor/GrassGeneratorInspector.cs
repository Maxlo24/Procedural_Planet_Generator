using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GrassGenerator))]
public class GrassGeneratorInspector : Editor
{
    public override void OnInspectorGUI()
    {

        GrassGenerator grassGenerator = (GrassGenerator)target;


        base.OnInspectorGUI();

        
        GUILayout.Space(10);

        DrawRangeSlider(new GUIContent("Slope angle"), ref grassGenerator.slopeAngleRange, 0, 90);
        DrawRangeSlider(new GUIContent("Altitude"), ref grassGenerator.altitudeRange, 0, 100);
        DrawRangeSlider(new GUIContent("Height"), ref grassGenerator.heightRange, 0, 2);
        DrawRangeSlider(new GUIContent("Width"), ref grassGenerator.widthRange, 0, 2);


        GUILayout.Space(10);
        if (GUILayout.Button("Generate"))
        {
            ((GrassGenerator)target).SpawnGrass();
        }

        if (GUILayout.Button("Update prototypes"))
        {
            ((GrassGenerator)target).UpdateGrassPrototypes();
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
        if (GUILayout.Button("Clear"))
        {
            ((GrassGenerator)target).ClearGrass();
        }

        //GUILayout.EndHorizontal();
    }
    public static void DrawRangeSlider(GUIContent label, ref Vector2 input, int min, int max)
    {
        float minBrightness = input.x;
        float maxBrightness = input.y;

        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField(label, GUILayout.MaxWidth(EditorGUIUtility.labelWidth));

            minBrightness = EditorGUILayout.FloatField(minBrightness, GUILayout.MaxWidth(40f));
            EditorGUILayout.MinMaxSlider(ref minBrightness, ref maxBrightness, min, max);
            maxBrightness = EditorGUILayout.FloatField(maxBrightness, GUILayout.MaxWidth(40f));
        }

        input.x = minBrightness;
        input.y = maxBrightness;
    }

}

