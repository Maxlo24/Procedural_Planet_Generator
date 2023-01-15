using UnityEngine;

public class ThermalErosion : MonoBehaviour
{
    [field: SerializeField] public bool Enabled { get; private set; }
    [field: SerializeField] public int Seed { get; private set; } = 0;
    [field: SerializeField, Range(0, 5000000)] public int IterationNumber { get; private set; } = 1000000;
    [field: SerializeField, Range(0, 500)] public int ErosionLifeTime { get; private set; } = 100;
    [field: SerializeField, Range(0, 0.5f)] public float Strength { get; private set; } = 0.1f;
    [field: SerializeField, Range(0, 89.9f)] public float MinTanAngle { get; private set; } = 5f;
    [field: SerializeField, Range(0, 89.9f)] public float MaxTanAngle { get; private set; } = 80f;

    public ThermalErosion(ThermalErosionPreset preset, int seed)
    {
        Enabled = true;
        Seed = seed;
        IterationNumber = preset.IterationCount;
        ErosionLifeTime = preset.LifeTime;
        Strength = preset.Strength;
        MinTanAngle = preset.MinTanAngle;
        MaxTanAngle = preset.MaxTanAngle;
    }

    public void Erode(ref RenderTexture heights, Terrain terrain)
    {
        ComputeShader erosionShader = Resources.Load<ComputeShader>(ShaderLib.ThermalErosionShader);
        int kernel = erosionShader.FindKernel("CSMain");
        Vector2[] StartPos = new Vector2[IterationNumber];

        System.Random prng = new System.Random(Seed);
        for (int i = 0; i < IterationNumber; i++)
        {
            StartPos[i].x = prng.Next(0, heights.width - 1);
            StartPos[i].y = prng.Next(0, heights.height - 1);
        }

        ComputeBuffer startPosBuffer = new ComputeBuffer(IterationNumber, 2 * sizeof(float));
        startPosBuffer.SetData(StartPos);
        erosionShader.SetBuffer(kernel, "initialPositions", startPosBuffer);
        erosionShader.SetTexture(kernel, "heights", heights);
        erosionShader.SetInt("width", heights.width);
        erosionShader.SetInt("height", heights.height);
        erosionShader.SetFloat("cellSize", (float) terrain.terrainData.size.x / (float) terrain.terrainData.heightmapResolution);
        erosionShader.SetFloat("terrainHeight", (float)terrain.terrainData.size.y);
        erosionShader.SetInt("erosionLifeTime", ErosionLifeTime);
        erosionShader.SetFloat("strength", Strength);
        erosionShader.SetFloat("minTanAngle", Mathf.Tan(MinTanAngle * Mathf.Deg2Rad));
        erosionShader.SetFloat("maxTanAngle", Mathf.Tan(MaxTanAngle * Mathf.Deg2Rad));
        erosionShader.Dispatch(kernel, IterationNumber / 256, 1, 1);

        startPosBuffer.Release();
    }
}
