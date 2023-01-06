using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class TerrainInfo : MonoBehaviour
{
    [field: SerializeField, ReadOnly] public float Min { get; private set; } = float.MaxValue;
    [field: SerializeField, ReadOnly] public float Max { get; private set; } = float.MinValue;
    
    public void ProcessInfoComputation(RenderTexture rt)
    {
        // mesure execution time
        float startTime = Time.realtimeSinceStartup;

        float[,] heights = ImageLib.ConvertRenderTextureToFloatArray(rt);
        
        Min = float.MaxValue;
        Max = float.MinValue;
        
        for (int x = 0; x < heights.GetLength(0); x++)
        {
            for (int y = 0; y < heights.GetLength(1); y++)
            {
                float height = heights[x, y];
                if (height < Min)
                    Min = height;
                if (height > Max)
                    Max = height;
            }
        }

        // mesure execution time
        float endTime = Time.realtimeSinceStartup;
        Debug.Log("ProcessInfoComputation took " + (endTime - startTime) + " seconds");
    }

    public void ProcessInfoComputation(float[,] heights)
    {
        Min = float.MaxValue;
        Max = float.MinValue;

        for (int x = 0; x < heights.GetLength(0); x++)
        {
            for (int y = 0; y < heights.GetLength(1); y++)
            {
                float height = heights[x, y];
                if (height < Min)
                    Min = height;
                if (height > Max)
                    Max = height;
            }
        }
    }
}
