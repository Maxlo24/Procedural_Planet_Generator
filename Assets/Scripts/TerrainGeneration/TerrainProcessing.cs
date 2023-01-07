using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TerrainProcessing : MonoBehaviour
{
    [field: SerializeField] public ComputeShader IslandShader { get; private set; }

    [field: SerializeField, Header("Island Process")] public bool IslandProcess { get; private set; }
    [field: SerializeField] public float IslandRatio { get; private set; }
    [field: SerializeField] public AnimationCurve DistanceCurve { get; private set; } = AnimationCurve.Linear(0, 1, 1, 1);

    [field: SerializeField, Header("Height Map Addition")] public float ElevationOffset { get; private set; }
    
    public RenderTexture ApplyIslandProcessing(RenderTexture heights)
    {

        RenderTexture RT = new RenderTexture(heights.width, heights.height, 0, RenderTextureFormat.RFloat);
        RT.enableRandomWrite = true;
        Graphics.Blit(heights, RT);
        
        float[] curve = DistanceLib.BakedCurveToFloatArray(DistanceCurve);
        
        int kernel = IslandShader.FindKernel("CSMain");

        // Define buffer for curve
        ComputeBuffer curveBuffer = new ComputeBuffer(curve.Length, sizeof(float));
        curveBuffer.SetData(curve);
        IslandShader.SetBuffer(kernel, "curve", curveBuffer);

        IslandShader.SetTexture(kernel, "heights", RT);
        IslandShader.SetInt("width", heights.width);
        IslandShader.SetInt("height", heights.height);
        IslandShader.SetInt("curveLength", curve.Length);
        IslandShader.SetFloat("firstTime", DistanceCurve.keys.First().time);
        IslandShader.SetFloat("lastTime", DistanceCurve.keys.Last().time);
        IslandShader.SetBool("islandProcess", IslandProcess);

        IslandShader.SetFloat("elevationOffset", ElevationOffset);

        IslandShader.SetFloat("islandRatio", IslandRatio);
        
        IslandShader.Dispatch(kernel, heights.width / 32, heights.height / 32, 1);

        curveBuffer.Release();

        return RT;
    }
}
