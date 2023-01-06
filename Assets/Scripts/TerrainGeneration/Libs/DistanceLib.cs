using System.Linq;
using UnityEngine;

public enum DistanceType
{
    SQUAREBUMP,
    EUCLIDEANSQUARE,
    EUCLIDEAN,
    MANHATTAN
}

public class DistanceLib
{
    public static float Distance(float x, float y, DistanceType distanceType)
    {
        switch (distanceType)
        {
            case DistanceType.SQUAREBUMP:
                return SquareBump(x, y);
            case DistanceType.EUCLIDEANSQUARE:
                return EuclideanSquare(x, y);
            case DistanceType.EUCLIDEAN:
                return Euclidean(x, y);
            case DistanceType.MANHATTAN:
                return Manhattan(x, y);
            default:
                return 0;
        }
    }

    private static float SquareBump(float x, float y)
    {
        return 1 - (1-Mathf.Pow(x, 2)) * (1-Mathf.Pow(y, 2));
    }

    private static float EuclideanSquare(float x, float y)
    {
        return Mathf.Min(1, (Mathf.Pow(x, 2) + Mathf.Pow(y, 2)) / Mathf.Sqrt(2f));
    }

    private static float Euclidean(float x, float y)
    {
        return Mathf.Min(1, Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2)));
    }

    private static float Manhattan(float x, float y)
    {
        return Mathf.Min(1, Mathf.Abs(x) + Mathf.Abs(y));
    }

    public static float[] BakedCurveToFloatArray(AnimationCurve curve, int size = 100)
    {
        // Calculate the length of the curve in pixels
        float first = curve.keys.First().time;
        
        float last = curve.keys.Last().time;

        // Create a new array of floats
        float[] curveArray = new float[size];

        // Loop through the curve and add the values to the array
        for (int i = 0; i < size; i++)
        {
            curveArray[i] = curve.Evaluate(Mathf.Lerp(first, last, (float)i / (float) (size - 1)));
            Debug.Log(curveArray[i]);
        }
        return curveArray;
    }



}
