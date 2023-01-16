using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainPostProcessing : MonoBehaviour
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField, Range(0, 5)] public float Sigma { get; private set; } = 1f;
    [field: SerializeField, Range(0, 8)] public int KernelRadius { get; private set; } = 1;
    [field: SerializeField] public bool ApplyWeight { get; private set; } = true;
    [field: SerializeField, Range(0, 1)] public float Weight { get; private set; } = 1f;

    public TerrainPostProcessing(PostProcessing postProcessing)
    {
        Name = postProcessing.Name;
        Sigma = postProcessing.Sigma;
        KernelRadius = postProcessing.KernelRadius;
        ApplyWeight = postProcessing.ApplyWeight;
        Weight = postProcessing.Weight;
    }


    public void SmoothTerrain(ref RenderTexture heights, RenderTexture basicHeights = null)
    {
        ComputeShader smoothShader = Resources.Load<ComputeShader>(ShaderLib.GaussianBlurShader);
        int kernel = smoothShader.FindKernel("CSMain");

        // Create Gaussian kernel with Kernel Radius and Sigma and convert it to a compute buffer
        float[] kernelArray = ImageLib.Create2DGaussianKernel(KernelRadius, Sigma);
        ComputeBuffer kernelBuffer = new ComputeBuffer(kernelArray.Length, sizeof(float));
        kernelBuffer.SetData(kernelArray);
        smoothShader.SetBuffer(kernel, "kernel", kernelBuffer);

        //smoothShader.SetTexture(kernel, "heights", basicHeights);
        smoothShader.SetTexture(kernel, "normalHeights", heights);

        smoothShader.SetInt("kernelRadius", KernelRadius);
        smoothShader.SetInt("sizeX", heights.width);
        smoothShader.SetInt("sizeY", heights.height);
        smoothShader.Dispatch(kernel, heights.width/ 32 + 1, heights.height/ 32 + 1, 1);

        // release buffer
        kernelBuffer.Release();

        if (!ApplyWeight || basicHeights == null) return;

        ComputeShader weightedShader = Resources.Load<ComputeShader>(ShaderLib.WeightedShader);
        int kernel1 = weightedShader.FindKernel("CSMain");

        weightedShader.SetTexture(kernel1, "result", heights);
        weightedShader.SetTexture(kernel1, "toWeight", basicHeights);
        weightedShader.SetFloat("weight", Weight);
        weightedShader.Dispatch(kernel1, basicHeights.width / 32 + 1, basicHeights.height / 32 + 1, 1);

    }
}
