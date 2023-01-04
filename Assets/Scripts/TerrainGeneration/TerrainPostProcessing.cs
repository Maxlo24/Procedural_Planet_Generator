using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainPostProcessing : MonoBehaviour
{
    [field: SerializeField] public ComputeShader SmoothShader { get; private set; }

    [field: SerializeField, Range(0, 1)] public float BlurWeight { get; private set; } = 1f;
    [field: SerializeField, Range(0, 1)] public float Weight { get; private set; } = 1f;
    [field: SerializeField, Range(1, 5)] public int KernelRadius { get; private set; } = 1;


    public float[,] SmoothTerrain(float[,] basicHeights, float[,] heights)
    {
        int kernel = SmoothShader.FindKernel("CSMain");

        // Copy the heightmap to a compute buffer
        ComputeBuffer buffer = new ComputeBuffer(basicHeights.Length, sizeof(float));
        buffer.SetData(basicHeights);

        //RenderTexture RT2 = new RenderTexture(RT.width, RT.height, 0, RenderTextureFormat.RFloat);
        //RT2.enableRandomWrite = true;
        //RT2.Create();

        ComputeBuffer heightMapBuffer = new ComputeBuffer(basicHeights.Length, sizeof(float));
        heightMapBuffer.SetData(basicHeights);
        SmoothShader.SetBuffer(0, "heights", heightMapBuffer);

        //SmoothShader.SetTexture(kernel, "basicHeights", RT2);
        SmoothShader.SetFloat("blurWeight", BlurWeight);
        SmoothShader.SetFloat("kernelRadius", KernelRadius);
        SmoothShader.SetFloat("sizeX", basicHeights.GetLength(0));
        SmoothShader.SetFloat("sizeY", basicHeights.GetLength(1));
        SmoothShader.Dispatch(kernel, basicHeights.GetLength(0) / 512, 1, 1);

        // Copy the result back to the heightmap
        float[] result = new float[basicHeights.Length];
        buffer.GetData(result);

        // Release the buffer
        buffer.Release();

        // Copy the result to a new heightmap
        float[,] newbasicHeights = new float[basicHeights.GetLength(0), basicHeights.GetLength(1)];

        for (int i = 0; i < basicHeights.GetLength(0); i++)
        {
            for (int j = 0; j < basicHeights.GetLength(1); j++)
            {
                newbasicHeights[i, j] = result[i * basicHeights.GetLength(0) + j] * (1 - Weight) + heights[i, j] * Weight;
            }
        }

        return newbasicHeights;
    }
}
