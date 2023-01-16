using UnityEngine;

public class ThermalErosion : MonoBehaviour
{
    [field: SerializeField] public bool Enabled { get; private set; }
    [field: SerializeField, Range(0, 10)] public int RepetitionCount { get; private set; } = 1;
    [field: SerializeField, Range(0, 800)] public int ErosionLifeTime { get; private set; } = 200;
    [field: SerializeField, Range(0, 0.5f)] public float Strength { get; private set; } = 0.1f;
    [field: SerializeField, Range(0, 89.9f)] public float MinTanAngle { get; private set; } = 5f;
    [field: SerializeField, Range(0, 89.9f)] public float MaxTanAngle { get; private set; } = 80f;

    public ThermalErosion(ThermalErosionPreset preset, int seed)
    {
        Enabled = true;
        ErosionLifeTime = preset.LifeTime;
        Strength = preset.Strength;
        MinTanAngle = preset.MinTanAngle;
        MaxTanAngle = preset.MaxTanAngle;
    }

    public void Erode(ref RenderTexture heights, Terrain terrain)
    {
        ComputeShader erosionShader = Resources.Load<ComputeShader>(ShaderLib.ThermalErosionShader);
        int kernel = erosionShader.FindKernel("CSMain");

        erosionShader.SetTexture(kernel, "heights", heights);
        erosionShader.SetInt("width", heights.width);
        erosionShader.SetInt("height", heights.height);
        erosionShader.SetFloat("cellSize", (float) terrain.terrainData.size.x / (float) terrain.terrainData.heightmapResolution);
        erosionShader.SetFloat("terrainHeight", (float)terrain.terrainData.size.y);
        erosionShader.SetInt("erosionLifeTime", ErosionLifeTime);
        erosionShader.SetFloat("strength", Strength);
        erosionShader.SetFloat("minTanAngle", Mathf.Tan(MinTanAngle * Mathf.Deg2Rad));
        erosionShader.SetFloat("maxTanAngle", Mathf.Tan(MaxTanAngle * Mathf.Deg2Rad));
        erosionShader.Dispatch(kernel, heights.width / 32, heights.height / 32, 1);
    }
}
