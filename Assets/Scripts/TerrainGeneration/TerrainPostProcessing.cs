using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainPostProcessing : MonoBehaviour
{
    [field: SerializeField] public ComputeShader SmoothShader { get; private set; }

    [field: SerializeField, Range(0, 5)] public float Sigma { get; private set; } = 1f;
    [field: SerializeField, Range(0, 1)] public float Weight { get; private set; } = 1f;
    [field: SerializeField, Range(0, 8)] public int KernelRadius { get; private set; } = 1;


    public RenderTexture SmoothTerrain(RenderTexture basicHeights, RenderTexture heights)
    {
        int kernel = SmoothShader.FindKernel("CSMain");
        
        RenderTexture RT = new RenderTexture(basicHeights.width, basicHeights.height, 32, RenderTextureFormat.RFloat);
        RT.enableRandomWrite = true;
        RT.Create();

        // Create Gaussian kernel with Kernel Radius and Sigma and convert it to a compute buffer
        float[] kernelArray = ImageLib.Create2DGaussianKernel(KernelRadius, Sigma);
        ComputeBuffer kernelBuffer = new ComputeBuffer(kernelArray.Length, sizeof(float));
        kernelBuffer.SetData(kernelArray);
        SmoothShader.SetBuffer(kernel, "kernel", kernelBuffer);

        SmoothShader.SetTexture(kernel, "heights", basicHeights);
        SmoothShader.SetTexture(kernel, "outputHeights", RT);
        SmoothShader.SetTexture(kernel, "normalHeights", heights);

        SmoothShader.SetFloat("weight", Weight);
        SmoothShader.SetInt("kernelRadius", KernelRadius);
        SmoothShader.SetInt("sizeX", basicHeights.width);
        SmoothShader.SetInt("sizeY", basicHeights.height);
        SmoothShader.Dispatch(kernel, basicHeights.width / 32, basicHeights.height / 32, 1);

        // release buffer
        kernelBuffer.Release();

        return RT;
    }
}
