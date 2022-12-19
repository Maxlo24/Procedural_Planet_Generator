using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainErosion : MonoBehaviour
{
    [field: SerializeField] public ComputeShader ErosionShader { get; private set; }


    [field: SerializeField] public float Scale { get; private set; } = 10f;

    [field: SerializeField] public int Seed { get; private set; } = 0;

    [field: SerializeField, Range(0, 10)] public int BorderSize { get; private set; } = 5;
    [field: SerializeField, Range(0, 1000000)] public int IterationNumber { get; private set; } = 70000;
    [field: SerializeField, Range(0, 500)] public int DropletLifeTime { get; private set; } = 300;
    [field: SerializeField] public float Acceleration { get; private set; } = 100f;
    [field: SerializeField] public float Drag { get; private set; } = 0.2f;
    [field: SerializeField, Range(1, 10)] public int BrushSize { get; private set; } = 3;
    [field: SerializeField] public float InitialVelocity { get; private set; } = 3f;
    [field: SerializeField] public float InitialWater { get; private set; } = 0.01f;
    [field: SerializeField, Range(0, 100)] public float SedimentCapacityFactor { get; private set; } = 24f;
    [field: SerializeField, Range(0, 1)] public float DepositRatio { get; private set; } = 0.1f;
    [field: SerializeField, Range(0, 1)] public float ErosionRatio { get; private set; } = 0.3f;
    [field: SerializeField, Range(0, 1)] public float EvaporationRatio { get; private set; } = 0.02f;
    [field: SerializeField, Range(0, 500)] public float Gravity { get; private set; } = 100f;

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

        //Vector3[] brush = new Vector3[(2 * BrushSize + 1) * (2 * BrushSize + 1)];
        //float normalizeC = 0;
        //for (int i = 0; i < BrushSize; i++)
        //{
        //    for (int j = 0; j < BrushSize; j++)
        //    {
        //        normalizeC += 1.1f * (i + j + 1);
        //        brush[i * BrushSize * 2 + j].z = (i + j + 1);
        //        brush[(2 * BrushSize - 1 - i) * BrushSize * 2 + j].z = (i + j + 1);
        //        brush[(2 * BrushSize - 1 - i) * BrushSize * 2 + (2 * BrushSize - 1 - j)].z = (i + j + 1);
        //        brush[i * BrushSize * 2 + (2 * BrushSize - 1 - j)].z = (i + j + 1);
        //    }
        //}
        //for (int i = 0; i < BrushSize * 2; i++)
        //{
        //    for (int j = 0; j < BrushSize * 2; j++)
        //    {
        //        brush[i * BrushSize * 2 + j].z /= normalizeC;
        //        brush[i * BrushSize * 2 + j].x = i - (BrushSize - 1);
        //        brush[i * BrushSize * 2 + j].y = j - (BrushSize - 1);
        //    }
        //}

        // Create brush
        
        List<int> brushIndexOffsets = new List<int>();
        List<float> brushWeights = new List<float>();

        float weightSum = 0;
        for (int brushY = -BrushSize; brushY <= BrushSize; brushY++)
        {
            for (int brushX = -BrushSize; brushX <= BrushSize; brushX++)
            {
                float sqrDst = brushX * brushX + brushY * brushY;
                if (sqrDst < BrushSize * BrushSize)
                {
                    brushIndexOffsets.Add(brushY * sizeX + brushX);
                    float brushWeight = 1 - Mathf.Sqrt(sqrDst) / BrushSize;
                    weightSum += brushWeight;
                    brushWeights.Add(brushWeight);
                }
            }
        }
        for (int i = 0; i < brushWeights.Count; i++)
        {
            brushWeights[i] /= weightSum;
        }
        
        // Send brush data to compute shader
        ComputeBuffer brushIndexBuffer = new ComputeBuffer(brushIndexOffsets.Count, sizeof(int));
        ComputeBuffer brushWeightBuffer = new ComputeBuffer(brushWeights.Count, sizeof(int));
        brushIndexBuffer.SetData(brushIndexOffsets);
        brushWeightBuffer.SetData(brushWeights);
        ErosionShader.SetBuffer(0, "brushIndices", brushIndexBuffer);
        ErosionShader.SetBuffer(0, "brushWeights", brushWeightBuffer);

        ComputeBuffer heightMapBuffer = new ComputeBuffer(heightsArray.Length, sizeof(float));
        heightMapBuffer.SetData(heightsArray);
        ErosionShader.SetBuffer(0, "heights", heightMapBuffer);

        ComputeBuffer startPosBuffer = new ComputeBuffer(IterationNumber, 2 * sizeof(float));
        startPosBuffer.SetData(StartPos);
        ErosionShader.SetBuffer(0, "initialPositions", startPosBuffer);

        //ComputeBuffer brushBufferIndex = new ComputeBuffer(brush.Length, 3 * sizeof(float));
        //brushBufferIndex.SetData(brush);
        //ErosionShader.SetBuffer(0, "brush", brushBufferIndex);
        //ComputeBuffer brushBufferWeights = new ComputeBuffer(brush.Length, 3 * sizeof(float));
        //brushBufferWeights.SetData(brush);
        //ErosionShader.SetBuffer(0, "brush", brushBufferWeights);
        ErosionShader.SetInt("brushLength", brushIndexOffsets.Count);
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

        ErosionShader.Dispatch(0, IterationNumber / 512, 1, 1);

        heightMapBuffer.GetData(heightsArray);
        heightMapBuffer.Release();
        startPosBuffer.Release();
        brushIndexBuffer.Release();
        brushWeightBuffer.Release();

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                heights[i, j] = heightsArray[i * sizeY + j];
            }
        }
    }

}