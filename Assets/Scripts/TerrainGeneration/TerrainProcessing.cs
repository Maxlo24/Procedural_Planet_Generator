using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TerrainProcessing : MonoBehaviour
{
    [field: SerializeField, Header("Island Process")] public bool IslandProcess { get; private set; }
    [field: SerializeField] public float IslandRatio { get; private set; }
    [field: SerializeField] public AnimationCurve DistanceCurve { get; private set; } = AnimationCurve.Linear(0, 1, 1, 1);

    [field: SerializeField, Header("Height Map Addition")] public float ElevationOffset { get; private set; }
    
    public void ApplyIslandProcessing(ref RenderTexture heights)
    {
        float[] curve = DistanceLib.BakedCurveToFloatArray(DistanceCurve);

        ComputeShader islandProcessShader = Resources.Load<ComputeShader>(ShaderLib.IslandProcessShader);
        int kernel = islandProcessShader.FindKernel("CSMain");

        // Define buffer for curve
        ComputeBuffer curveBuffer = new ComputeBuffer(curve.Length, sizeof(float));
        curveBuffer.SetData(curve);
        islandProcessShader.SetBuffer(kernel, "curve", curveBuffer);

        islandProcessShader.SetTexture(kernel, "heights", heights);
        islandProcessShader.SetInt("width", heights.width);
        islandProcessShader.SetInt("height", heights.height);
        islandProcessShader.SetInt("curveLength", curve.Length);
        islandProcessShader.SetFloat("firstTime", DistanceCurve.keys.First().time);
        islandProcessShader.SetFloat("lastTime", DistanceCurve.keys.Last().time);
        islandProcessShader.SetBool("islandProcess", IslandProcess);
        islandProcessShader.SetFloat("elevationOffset", ElevationOffset);
        islandProcessShader.SetFloat("islandRatio", IslandRatio);
        islandProcessShader.Dispatch(kernel, heights.width/ 32 + 1, heights.height/ 32 + 1, 1);

        curveBuffer.Release();
    }
}
