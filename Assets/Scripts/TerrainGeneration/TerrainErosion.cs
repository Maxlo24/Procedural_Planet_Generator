using System;
using UnityEngine;
using static UnityEngine.UI.Image;

public struct ErodeResult
{
    public RenderTexture ErosionTexture;
    public RenderTexture DepositTexture;

    public ErodeResult(RenderTexture erosionTexture, RenderTexture depositTexture) : this()
    {
        ErosionTexture = erosionTexture;
        DepositTexture = depositTexture;
    }
}

public class TerrainErosion : MonoBehaviour
{
    [field: SerializeField] public bool Enabled { get; private set; }
    [field: SerializeField, Range(1, 10)] public int RepetitionCount { get; private set; } = 1;
    [field: SerializeField, Range(0, 5)] public int Power2ResolutionDivisor { get; private set; } = 0;
    [field: SerializeField] public int Seed { get; private set; } = 0;
    [field: SerializeField, Range(0, 10)] public int BorderSize { get; private set; } = 5;
    [field: SerializeField, Range(0, 1000000)] public int IterationNumber { get; private set; } = 70000;
    [field: SerializeField, Range(0, 500)] public int DropletLifeTime { get; private set; } = 300;
    [field: SerializeField] public float Acceleration { get; private set; } = 100f;
    [field: SerializeField] public float Drag { get; private set; } = 0.2f;
    [field: SerializeField, Range(0, 10)] public int BrushSize { get; private set; } = 3;
    [field: SerializeField] public float InitialVelocity { get; private set; } = 3f;
    [field: SerializeField] public float InitialWater { get; private set; } = 0.01f;
    [field: SerializeField, Range(0, 100)] public float SedimentCapacityFactor { get; private set; } = 24f;
    [field: SerializeField, Range(0, 1)] public float DepositRatio { get; private set; } = 0.1f;
    [field: SerializeField, Range(0, 1)] public float ErosionRatio { get; private set; } = 0.3f;
    [field: SerializeField, Range(0, 1)] public float EvaporationRatio { get; private set; } = 0.02f;
    [field: SerializeField, Range(0, 500)] public float Gravity { get; private set; } = 100f;
    [field: SerializeField] public bool ErodeEnabled { get; private set; } = true;
    [field: SerializeField] public bool ErosionMapUsed { get; private set; } = false;
    [field: SerializeField] public bool GenerateErosionTexture { get; private set; } = false;

    public ErodeResult Erode(ref RenderTexture heights, RenderTexture erosionText, RenderTexture depositText)
    {
        RenderTexture erosionTexture = ImageLib.CopyRenderTexture(erosionText);
        RenderTexture depositTexture = ImageLib.CopyRenderTexture(depositText);

        RenderTexture lowRes;
        RenderTexture lowResCopy;
        if (Power2ResolutionDivisor == 0)
        {
            lowRes = heights;
            lowResCopy = lowRes;
        }
        else
        {
            lowRes = ImageLib.CreateRenderTexture((heights.width >> Power2ResolutionDivisor) + 1, (heights.height >> Power2ResolutionDivisor) + 1, RenderTextureFormat.RFloat);
            lowResCopy = ImageLib.CreateRenderTexture(lowRes.width, lowRes.height, RenderTextureFormat.RFloat);
            Graphics.Blit(heights, lowRes);
            Graphics.Blit(lowRes, lowResCopy);
        }

        ComputeShader erosionShader = Resources.Load<ComputeShader>(ShaderLib.ErosionShader);
        int kernel = erosionShader.FindKernel("CSMain");

        /** Hydraulic Erosion Simulation **/
        int sizeX = lowRes.width;
        int sizeY = lowRes.height;

        Vector2[] StartPos = new Vector2[IterationNumber];

        System.Random prng = new System.Random(Seed);
        for (int i = 0; i < IterationNumber; i++)
        {
            StartPos[i].x = prng.Next(0, sizeX - 1);
            StartPos[i].y = prng.Next(0, sizeY - 1);
        }

        float[] brushWeightsArray = new float[(2 * BrushSize + 3) * (2 * BrushSize + 3)];

        float weightSum = 0;
        for (int brushY = -BrushSize; brushY <= BrushSize; brushY++)
        {
            for (int brushX = -BrushSize; brushX <= BrushSize; brushX++)
            {
                float sqrDst = brushX * brushX + brushY * brushY;
                if (sqrDst < 2 * (BrushSize + 1) * 2 * (BrushSize + 1))
                {
                    float value = Mathf.Abs(BrushSize - Mathf.Sqrt(sqrDst));
                    brushWeightsArray[(brushY + BrushSize + 1) * (2 * BrushSize + 3) + (brushX + BrushSize + 1)] = value;
                    weightSum += value;
                }
            }
        }
        for (int i = 0; i < brushWeightsArray.Length; i++)
        {
            brushWeightsArray[i] /= weightSum;
        }

        erosionShader.SetTexture(kernel, "heights", lowRes);
        erosionShader.SetTexture(kernel, "erosion", erosionTexture);
        erosionShader.SetTexture(kernel, "deposit", depositTexture);

        // Send brush data to compute shader
        ComputeBuffer brushWeightBuffer = new ComputeBuffer(brushWeightsArray.Length, sizeof(int));
        brushWeightBuffer.SetData(brushWeightsArray);
        erosionShader.SetBuffer(kernel, "brushWeights", brushWeightBuffer);

        ComputeBuffer startPosBuffer = new ComputeBuffer(IterationNumber, 2 * sizeof(float));
        startPosBuffer.SetData(StartPos);
        erosionShader.SetBuffer(kernel, "initialPositions", startPosBuffer);

        //float[] erosionMap = new float[1];
        //if (ErosionMapUsed)
        //{
        //    erosionMap = ErosionMapLib.ErodeMapFromHeightMap(heights);
        //}
        //ComputeBuffer erosionMapBuffer = new ComputeBuffer(erosionMap.Length, sizeof(float));
        //erosionMapBuffer.SetData(erosionMap);
        //erosionShader.SetBuffer(0, "erosionMap", erosionMapBuffer);

        erosionShader.SetInt("brushSize", BrushSize);
        erosionShader.SetInt("borderSize", BorderSize);
        erosionShader.SetInt("sizeX", sizeX);
        erosionShader.SetInt("sizeY", sizeY);
        erosionShader.SetInt("textureRes", erosionText.width);
        erosionShader.SetInt("lifetime", DropletLifeTime);
        erosionShader.SetInt("brushSize", BrushSize);
        erosionShader.SetFloat("initialVelocity", InitialVelocity);
        erosionShader.SetFloat("acceleration", Acceleration);
        erosionShader.SetFloat("drag", Drag);
        erosionShader.SetFloat("initialWater", InitialWater);
        erosionShader.SetFloat("sedimentCapacityFactor", SedimentCapacityFactor);
        erosionShader.SetFloat("depositRatio", DepositRatio);
        erosionShader.SetFloat("erosionRatio", ErosionRatio);
        erosionShader.SetFloat("evaporationRatio", EvaporationRatio);
        erosionShader.SetFloat("gravity", Gravity);
        erosionShader.SetBool("erodeEnabled", ErodeEnabled);
        erosionShader.SetBool("erosionMapUsed", ErosionMapUsed);
        erosionShader.SetBool("generateErosionTexture", GenerateErosionTexture);

        erosionShader.Dispatch(kernel, IterationNumber / 256, 1, 1);

        /** Generate Erosion texture **/
        startPosBuffer.Release();
        brushWeightBuffer.Release();

        //if (Power2ResolutionDivisor != 0)
        //{
        //    DifferenceErosion(ref lowRes, lowResCopy);
        //    RenderTexture highResDiff = ImageLib.CreateRenderTexture(heights.width, heights.height, RenderTextureFormat.RFloat);
        //    Graphics.Blit(lowRes, highResDiff);
        //    UpscaleErosion(ref heights, highResDiff);
        //}
        //else
        {
            Graphics.Blit(lowRes, heights);
            SmoothErosion(ref heights, Power2ResolutionDivisor);

        }

        return new ErodeResult(erosionTexture, depositTexture);
    }

    private void DifferenceErosion(ref RenderTexture result, RenderTexture original)
    {
        ComputeShader diffShader = Resources.Load<ComputeShader>(ShaderLib.DiffShader);
        int kernel = diffShader.FindKernel("CSMain");

        diffShader.SetTexture(kernel, "result", result);
        diffShader.SetTexture(kernel, "imageToDiff", original);

        diffShader.Dispatch(kernel, result.width / 32 + 1, result.height / 32 + 1, 1);
    }

    private void UpscaleErosion(ref RenderTexture result, RenderTexture diff)
    {
        ComputeShader upscaleShader = Resources.Load<ComputeShader>(ShaderLib.UpscaleErosionShader);
        int kernel = upscaleShader.FindKernel("CSMain");

        upscaleShader.SetTexture(kernel, "result", result);
        upscaleShader.SetTexture(kernel, "diff", diff);

        upscaleShader.SetInt("resDivider", (int)Mathf.Pow(2, Power2ResolutionDivisor));

        upscaleShader.Dispatch(kernel, result.width / 32, result.height / 32, 1);
    }

    private void SmoothErosion(ref RenderTexture result, int KernelRadius = 3)
    {
        ComputeShader smoothShader = Resources.Load<ComputeShader>(ShaderLib.GaussianBlurShader);
        int kernel = smoothShader.FindKernel("CSMain");

        // Create Gaussian kernel with Kernel Radius and Sigma and convert it to a compute buffer
        float[] kernelArray = ImageLib.Create2DGaussianKernel(Power2ResolutionDivisor, 1f);
        ComputeBuffer kernelBuffer = new ComputeBuffer(kernelArray.Length, sizeof(float));
        kernelBuffer.SetData(kernelArray);
        smoothShader.SetBuffer(kernel, "kernel", kernelBuffer);

        smoothShader.SetTexture(kernel, "heights", result);
        smoothShader.SetTexture(kernel, "normalHeights", result);

        smoothShader.SetInt("kernelRadius", Power2ResolutionDivisor);
        smoothShader.SetInt("sizeX", result.width);
        smoothShader.SetInt("sizeY", result.height);
        smoothShader.Dispatch(kernel, result.width / 32 + 1, result.height / 32 + 1, 1);

        // release buffer
        kernelBuffer.Release();
    }

}