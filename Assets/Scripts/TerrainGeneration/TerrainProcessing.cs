using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainProcessing : MonoBehaviour
{
    [field: SerializeField] public ComputeShader IslandShader { get; private set; }

    [field: SerializeField] public bool IslandProcess { get; private set; }
    [field: SerializeField] public AnimationCurve DistanceCurve { get; private set; } = AnimationCurve.Linear(0, 1, 1, 1);

    public RenderTexture ApplyIslandProcessing(RenderTexture heights)
    {

        RenderTexture RT = new RenderTexture(heights.width, heights.height, 0, RenderTextureFormat.RFloat);
        RT.enableRandomWrite = true;
        Graphics.Blit(heights, RT);

        
        if (!IslandShader) return heights;

        RenderTexture curve = DistanceLib.BakedCurveToRenderTexture(DistanceCurve);

        int kernel = IslandShader.FindKernel("CSMain");
        IslandShader.SetTexture(kernel, "heights", RT);
        IslandShader.SetTexture(kernel, "curve", curve);
        IslandShader.SetInt("width", heights.width);
        IslandShader.SetInt("height", heights.height);
        IslandShader.Dispatch(kernel, heights.width / 32, heights.height / 32, 1);

        return RT;
    }
}
