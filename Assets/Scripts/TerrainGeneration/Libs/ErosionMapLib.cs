using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErosionMapLib : MonoBehaviour
{
    public static float[] FirstBasicErodeMap(int size)
    {
        float[] map = new float[size * size];

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                map[x + y * size] = (x < size / 2) ? 0.1f : 1f;
            }
        }

        return map;
    }

    public static float[] ErodeMapFromHeightMap(float[,] heights)
    {
        int size = heights.GetLength(0);
        float[] map = new float[size * size];

        float min = float.MaxValue;
        float max = float.MinValue;
        
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                map[x + y * size] = heights[x, y];
                if (heights[x, y] < min)
                {
                    min = heights[x, y];
                }
                if (heights[x, y] > max)
                {
                    max = heights[x, y];
                }
            }
        }

        // Normalize map
        for (int i = 0; i < map.Length; i++)
        {
            map[i] = (1 - (map[i] - min) / (max - min));
        }

        return map;
    }
}
