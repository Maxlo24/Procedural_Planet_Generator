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

    public static RenderTexture BakedCurveToRenderTexture(AnimationCurve curve)
    {
        // Calculate the length of the curve in pixels
        int curveLength = curve.keys.Length;

        // Create a new render texture with the specified width and height
        RenderTexture rt = new RenderTexture(1, curveLength, 24);
        rt.enableRandomWrite = true;
        rt.Create();

        // Create a material that will be used to draw the curve to the render texture
        Material material = new Material(Shader.Find("Unlit/CurveShader"));

        // Set the curve as a property of the material
        material.SetFloatArray("_Curve", curve.keys.Select(x => x.value).ToArray());

        // Set the render texture as the target for the material's draw call
        RenderTexture.active = rt;
        material.SetPass(0);

        // Draw a fullscreen quad to the render texture using the material
        GL.PushMatrix();
        GL.LoadOrtho();
        GL.Color(Color.white);
        GL.Begin(GL.QUADS);
        GL.Vertex3(0f, 0f, 0f);
        GL.Vertex3(0f, curveLength, 0f);
        GL.Vertex3(1f, curveLength, 0f);
        GL.Vertex3(1f, 0f, 0f);
        GL.End();
        GL.PopMatrix();

        // Reset the render target and return the render texture
        RenderTexture.active = null;
        return rt;
    }



}
