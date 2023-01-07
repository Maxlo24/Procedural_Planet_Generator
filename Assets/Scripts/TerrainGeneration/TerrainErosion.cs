using System;
using UnityEngine;

public struct ErodeResult
{
    public RenderTexture Heights;
    public RenderTexture ErosionTexture;

    public ErodeResult(RenderTexture heightsCopy, RenderTexture erosionTextureCopy) : this()
    {
        Heights = heightsCopy;
        ErosionTexture = erosionTextureCopy;
    }
}

public class TerrainErosion : MonoBehaviour
{
    [field: SerializeField] public ComputeShader ErosionShader { get; private set; }
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
    
    public void Erode(float[,] heights, ComputeShader erosionShader)
    {
        int sizeX = heights.GetLength(0);
        int sizeY = heights.GetLength(1);

        float[] heightsArray = new float[sizeX * sizeY];

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                heightsArray[i * sizeY + j] = heights[i, j];
            }
        }

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

        // Send brush data to compute shader
        ComputeBuffer brushWeightBuffer = new ComputeBuffer(brushWeightsArray.Length, sizeof(int));
        brushWeightBuffer.SetData(brushWeightsArray);
        erosionShader.SetBuffer(0, "brushWeights", brushWeightBuffer);

        ComputeBuffer heightMapBuffer = new ComputeBuffer(heightsArray.Length, sizeof(float));
        heightMapBuffer.SetData(heightsArray);
        erosionShader.SetBuffer(0, "heights", heightMapBuffer);

        ComputeBuffer startPosBuffer = new ComputeBuffer(IterationNumber, 2 * sizeof(float));
        startPosBuffer.SetData(StartPos);
        erosionShader.SetBuffer(0, "initialPositions", startPosBuffer);

        float[] erosionMap = new float[1];
        if (ErosionMapUsed)
        {
            erosionMap = ErosionMapLib.ErodeMapFromHeightMap(heights);
        }
        ComputeBuffer erosionMapBuffer = new ComputeBuffer(erosionMap.Length, sizeof(float));
        erosionMapBuffer.SetData(erosionMap);
        erosionShader.SetBuffer(0, "erosionMap", erosionMapBuffer);

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

        erosionShader.Dispatch(0, IterationNumber / 512, 1, 1);

        heightMapBuffer.GetData(heightsArray);
        heightMapBuffer.Release();
        startPosBuffer.Release();
        brushWeightBuffer.Release();

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                heights[i, j] = heightsArray[i * sizeY + j];
            }
        }
    }

    public ErodeResult Erode(RenderTexture heights, RenderTexture heightsBeforeErosion, RenderTexture erosionTexture)
    {
        RenderTexture heightsCopy = ImageLib.CopyRenderTexture(heights);

        RenderTexture erosionTextureCopy = ImageLib.CopyRenderTexture(erosionTexture);

        int kernel = ErosionShader.FindKernel("CSMain");

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

        ErosionShader.SetTexture(kernel, "heights", heightsCopy);

        // Send brush data to compute shader
        ComputeBuffer brushWeightBuffer = new ComputeBuffer(brushWeightsArray.Length, sizeof(int));
        brushWeightBuffer.SetData(brushWeightsArray);
        ErosionShader.SetBuffer(kernel, "brushWeights", brushWeightBuffer);

        ComputeBuffer startPosBuffer = new ComputeBuffer(IterationNumber, 2 * sizeof(float));
        startPosBuffer.SetData(StartPos);
        ErosionShader.SetBuffer(kernel, "initialPositions", startPosBuffer);

        //float[] erosionMap = new float[1];
        //if (ErosionMapUsed)
        //{
        //    erosionMap = ErosionMapLib.ErodeMapFromHeightMap(heights);
        //}
        //ComputeBuffer erosionMapBuffer = new ComputeBuffer(erosionMap.Length, sizeof(float));
        //erosionMapBuffer.SetData(erosionMap);
        //ErosionShader.SetBuffer(0, "erosionMap", erosionMapBuffer);

        ErosionShader.SetInt("brushSize", BrushSize);
        ErosionShader.SetInt("borderSize", BorderSize);
        ErosionShader.SetInt("sizeX", sizeX);
        ErosionShader.SetInt("sizeY", sizeY);
        ErosionShader.SetInt("lifetime", DropletLifeTime);
        ErosionShader.SetInt("brushSize", BrushSize);
        ErosionShader.SetFloat("initialVelocity", InitialVelocity);
        ErosionShader.SetFloat("acceleration", Acceleration);
        ErosionShader.SetFloat("drag", Drag);
        ErosionShader.SetFloat("initialWater", InitialWater);
        ErosionShader.SetFloat("sedimentCapacityFactor", SedimentCapacityFactor);
        ErosionShader.SetFloat("depositRatio", DepositRatio);
        ErosionShader.SetFloat("erosionRatio", ErosionRatio);
        ErosionShader.SetFloat("evaporationRatio", EvaporationRatio);
        ErosionShader.SetFloat("gravity", Gravity);
        ErosionShader.SetBool("erodeEnabled", ErodeEnabled);
        ErosionShader.SetBool("erosionMapUsed", ErosionMapUsed);
        
        ErosionShader.Dispatch(kernel, IterationNumber / 256, 1, 1);

        /** Generate Erosion texture **/
        startPosBuffer.Release();
        brushWeightBuffer.Release();

        return new ErodeResult(heightsCopy, erosionTextureCopy);
    }

}