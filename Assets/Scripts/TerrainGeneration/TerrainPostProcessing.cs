using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainPostProcessing : MonoBehaviour
{
    [field: SerializeField, Range(0, 5)] public float Sigma { get; private set; } = 1f;
    [field: SerializeField, Range(0, 1)] public float Weight { get; private set; } = 1f;
    [field: SerializeField, Range(0, 8)] public int KernelRadius { get; private set; } = 1;


    public void SmoothTerrain(RenderTexture basicHeights, ref RenderTexture heights)
    {
        ComputeShader smoothShader = Resources.Load<ComputeShader>(ShaderLib.GaussianBlurShader);
        int kernel = smoothShader.FindKernel("CSMain");

        // Create Gaussian kernel with Kernel Radius and Sigma and convert it to a compute buffer
        float[] kernelArray = ImageLib.Create2DGaussianKernel(KernelRadius, Sigma);
        ComputeBuffer kernelBuffer = new ComputeBuffer(kernelArray.Length, sizeof(float));
        kernelBuffer.SetData(kernelArray);
        smoothShader.SetBuffer(kernel, "kernel", kernelBuffer);

        smoothShader.SetTexture(kernel, "heights", basicHeights);
        smoothShader.SetTexture(kernel, "normalHeights", heights);

        smoothShader.SetFloat("weight", Weight);
        smoothShader.SetInt("kernelRadius", KernelRadius);
        smoothShader.SetInt("sizeX", basicHeights.width);
        smoothShader.SetInt("sizeY", basicHeights.height);
        smoothShader.Dispatch(kernel, basicHeights.width/ 32 + 1, basicHeights.height/ 32 + 1, 1);

        // release buffer
        kernelBuffer.Release();
    }
}
