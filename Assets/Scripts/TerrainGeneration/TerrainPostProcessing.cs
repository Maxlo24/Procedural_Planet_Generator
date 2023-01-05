using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainPostProcessing : MonoBehaviour
{
    [field: SerializeField] public ComputeShader SmoothShader { get; private set; }

    [field: SerializeField, Range(0, 5)] public float Sigma { get; private set; } = 1f;
    [field: SerializeField, Range(0, 1)] public float Weight { get; private set; } = 1f;
    [field: SerializeField, Range(0, 8)] public int KernelRadius { get; private set; } = 1;

    [field: SerializeField] public RenderTexture RT { get; private set; }


    public RenderTexture SmoothTerrain(float[,] basicHeights, float[,] heights)
    {
        int kernel = SmoothShader.FindKernel("CSMain");

        // Copy the heightmap to a compute buffer
        ComputeBuffer buffer = new ComputeBuffer(basicHeights.Length, sizeof(float));
        buffer.SetData(basicHeights);

        // Convert basicHeights float[,] to RenderTexture
        RT = ImageLib.ConvertFloatArrayToRenderTexture(basicHeights);
        RenderTexture RTBasic = ImageLib.ConvertFloatArrayToRenderTexture(heights);
        
        RenderTexture RT2 = new RenderTexture(RT.width, RT.height, 32, RenderTextureFormat.RFloat);
        RT2.enableRandomWrite = true;
        RT2.Create();

        // Create Gaussian kernel with Kernel Radius and Sigma and convert it to a compute buffer
        float[] kernelArray = ImageLib.Create2DGaussianKernel(KernelRadius, Sigma);
        ComputeBuffer kernelBuffer = new ComputeBuffer(kernelArray.Length, sizeof(float));
        kernelBuffer.SetData(kernelArray);
        SmoothShader.SetBuffer(kernel, "kernel", kernelBuffer);

        SmoothShader.SetTexture(kernel, "heights", RT);
        SmoothShader.SetTexture(kernel, "outputHeights", RT2);
        SmoothShader.SetTexture(kernel, "normalHeights", RTBasic);

        //SmoothShader.SetTexture(kernel, "basicHeights", RT2);
        SmoothShader.SetFloat("weight", Weight);
        SmoothShader.SetInt("kernelRadius", KernelRadius);
        SmoothShader.SetInt("sizeX", basicHeights.GetLength(0));
        SmoothShader.SetInt("sizeY", basicHeights.GetLength(1));
        SmoothShader.Dispatch(kernel, basicHeights.GetLength(0) / 32, basicHeights.GetLength(1) / 32, 1);

        return RT2;
    }
}
