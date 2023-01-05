using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class TerrainErosion : MonoBehaviour
{
    [field: SerializeField] public ComputeShader ErosionShader { get; private set; }
    [field: SerializeField] public ComputeShader ErosionShader2 { get; private set; }
    [field: SerializeField] public float Scale { get; private set; } = 10f;
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

    [field: SerializeField] public RenderTexture RenderTexture { get; private set; }

    
    public void Erode(float[,] heights)
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
        ErosionShader.SetBuffer(0, "brushWeights", brushWeightBuffer);

        ComputeBuffer heightMapBuffer = new ComputeBuffer(heightsArray.Length, sizeof(float));
        heightMapBuffer.SetData(heightsArray);
        ErosionShader.SetBuffer(0, "heights", heightMapBuffer);

        ComputeBuffer startPosBuffer = new ComputeBuffer(IterationNumber, 2 * sizeof(float));
        startPosBuffer.SetData(StartPos);
        ErosionShader.SetBuffer(0, "initialPositions", startPosBuffer);

        float[] erosionMap = new float[1];
        if (ErosionMapUsed)
        {
            erosionMap = ErosionMapLib.ErodeMapFromHeightMap(heights);
        }
        ComputeBuffer erosionMapBuffer = new ComputeBuffer(erosionMap.Length, sizeof(float));
        erosionMapBuffer.SetData(erosionMap);
        ErosionShader.SetBuffer(0, "erosionMap", erosionMapBuffer);

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

        ErosionShader.Dispatch(0, IterationNumber / 512, 1, 1);

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

    
    public RenderTexture Erode(RenderTexture heights)
    {
        RenderTexture heightsCopy = new RenderTexture(heights.width, heights.height, 0, RenderTextureFormat.RFloat);
        heightsCopy.enableRandomWrite = true;
        Graphics.Blit(heights, heightsCopy);

        int kernel = ErosionShader2.FindKernel("CSMain");
        
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

        ErosionShader2.SetTexture(kernel, "heights", heightsCopy);

        // Send brush data to compute shader
        ComputeBuffer brushWeightBuffer = new ComputeBuffer(brushWeightsArray.Length, sizeof(int));
        brushWeightBuffer.SetData(brushWeightsArray);
        ErosionShader2.SetBuffer(kernel, "brushWeights", brushWeightBuffer);

        ComputeBuffer startPosBuffer = new ComputeBuffer(IterationNumber, 2 * sizeof(float));
        startPosBuffer.SetData(StartPos);
        ErosionShader2.SetBuffer(kernel, "initialPositions", startPosBuffer);

        float[] erosionMap = new float[1];
        //if (ErosionMapUsed)
        //{
        //    erosionMap = ErosionMapLib.ErodeMapFromHeightMap(heights);
        //}
        ComputeBuffer erosionMapBuffer = new ComputeBuffer(erosionMap.Length, sizeof(float));
        erosionMapBuffer.SetData(erosionMap);
        ErosionShader2.SetBuffer(0, "erosionMap", erosionMapBuffer);

        ErosionShader2.SetInt("brushSize", BrushSize);
        ErosionShader2.SetInt("borderSize", BorderSize);
        ErosionShader2.SetInt("sizeX", sizeX);
        ErosionShader2.SetInt("sizeY", sizeY);
        ErosionShader2.SetInt("lifetime", DropletLifeTime);
        ErosionShader2.SetInt("brushSize", BrushSize);
        ErosionShader2.SetFloat("initialVelocity", InitialVelocity);
        ErosionShader2.SetFloat("acceleration", Acceleration);
        ErosionShader2.SetFloat("drag", Drag);
        ErosionShader2.SetFloat("initialWater", InitialWater);
        ErosionShader2.SetFloat("sedimentCapacityFactor", SedimentCapacityFactor);
        ErosionShader2.SetFloat("depositRatio", DepositRatio);
        ErosionShader2.SetFloat("erosionRatio", ErosionRatio);
        ErosionShader2.SetFloat("evaporationRatio", EvaporationRatio);
        ErosionShader2.SetFloat("gravity", Gravity);
        ErosionShader2.SetBool("erodeEnabled", ErodeEnabled);
        ErosionShader2.SetBool("erosionMapUsed", ErosionMapUsed);

        ErosionShader2.Dispatch(kernel, IterationNumber / 512, 1, 1);
        RenderTexture = heightsCopy;

        startPosBuffer.Release();
        brushWeightBuffer.Release();
        erosionMapBuffer.Release();

        return heightsCopy;
    }

}