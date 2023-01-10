using System;
using UnityEngine;

public struct ErodeResult
{
    public RenderTexture Heights;
    public RenderTexture ErosionTexture;
    public RenderTexture DepositTexture;

    public ErodeResult(RenderTexture heightsCopy, RenderTexture erosionTextureCopy, RenderTexture depositTexture) : this()
    {
        Heights = heightsCopy;
        ErosionTexture = erosionTextureCopy;
        DepositTexture = depositTexture;
    }
}

public class TerrainErosion : MonoBehaviour
{
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
    
    public void Erode(ref RenderTexture heights, ref RenderTexture erosionText, ref RenderTexture depositText)
    {
        erosionText = ImageLib.CopyRenderTexture(erosionText);
        depositText = ImageLib.CopyRenderTexture(depositText);

        ComputeShader erosionShader = Resources.Load<ComputeShader>(ShaderLib.ErosionShader);
        int kernel = erosionShader.FindKernel("CSMain");

        /** Hydraulic Erosion Simulation **/
        int sizeX = heights.width;
        int sizeY = heights.height;

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

        erosionShader.SetTexture(kernel, "heights", heights);
        erosionShader.SetTexture(kernel, "erosion", erosionText);
        erosionShader.SetTexture(kernel, "deposit", depositText);

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
    }

}