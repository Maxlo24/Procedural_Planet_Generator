using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class HeightMapsAddition : MonoBehaviour
{
    [field: SerializeField, Range(0, 2048)] public int Width { get; private set; } = 512;
    [field: SerializeField] public Vector2Int Offset { get; private set; }
    [field: SerializeField] public float ElevationOffset { get; private set; }
    [field: SerializeField, Range(-5, 5)] public float ElevationRatio { get; private set; } = 1f;
    [field: SerializeField] public float MinRadiusWeight { get; private set; } = 0f;

    [field: SerializeField] public AnimationCurve RadiusWeight { get; private set; } = AnimationCurve.Linear(0, 1, 1, 1);

    public float[,] AddHeightMaps(float[,] heightMapReference, float[,] heightMapToAdd)
    {
        return AddHeightMaps(heightMapReference, heightMapToAdd, Width, Width, Offset);
    }

    public float[,] AddHeightMaps(float[,] heightMapReference, float[,] heightMapToAdd, Vector2Int offset = default(Vector2Int))
    {
        int width = heightMapToAdd.GetLength(0);
        int height = heightMapToAdd.GetLength(1);

        return AddHeightMaps(heightMapReference, heightMapToAdd, width, height, offset);
    }

    public float[,] AddHeightMaps(float[,] heightMapReference, float[,] heightMapToAdd, int width, int height, Vector2Int offset = default(Vector2Int))
    {
        if (width < 0 || width > heightMapToAdd.GetLength(0) || height < 0 || height > heightMapToAdd.GetLength(1)) { return heightMapReference; }
        if (offset.x < 0 || offset.y < 0) return heightMapReference;
        if (offset.x + width > heightMapReference.GetLength(0) || offset.y + height > heightMapReference.GetLength(1)) return heightMapReference;

        float ratioX = (float)heightMapToAdd.GetLength(0) / (float)width;
        float ratioY = (float)heightMapToAdd.GetLength(1) / (float)height;

        for (int x = offset.x; x < offset.x + width; x++)
        {
            for (int y = offset.y; y < offset.y + height; y++)
            {
                float w = Weight(width, height, (x - offset.x), (y - offset.y));
                heightMapReference[x, y] = heightMapReference[x, y] * (1 - w) + (heightMapToAdd[(x - offset.x) * (int)ratioX, (y - offset.y) * (int)ratioY] * ElevationRatio + ElevationOffset) * w;
            }
        }
        return heightMapReference;
    }

    private float Weight(int sizeX, int sizeY, int x, int y)
    {
        float a = (float)sizeX / 2f;
        float b = (float)sizeY / 2f;

        float maxR4 = Mathf.Pow(a, 4);
        float minR4 = Mathf.Pow(MinRadiusWeight, 4);

        if (minR4 > maxR4) minR4 = 0f;

        float r = Mathf.Pow(x - a, 4) + Mathf.Pow(y - b, 4);

        if (r > maxR4) return 0;
        if (r < minR4) return 1;
        return RadiusWeight.Evaluate((maxR4 - r) / (maxR4 - minR4));
    }
}
