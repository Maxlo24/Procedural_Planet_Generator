using UnityEngine;

public static class ShaderLib
{
    public static string NoiseShader = "ComputeShaders/NoiseShader";
    public static string MinMaxShader = "ComputeShaders/Lib/MinMax";
    public static string TruncateShader = "ComputeShaders/Lib/Truncate";
    public static string TerraceShader = "ComputeShaders/Lib/Terrace";
    public static string NormalizeShader = "ComputeShaders/Lib/Normalize";
    public static string MultiplyShader = "ComputeShaders/Lib/Multiply";
    public static string DiffShader = "ComputeShaders/Lib/Diff";
    public static string ErosionShader = "ComputeShaders/Erosion/Erosion";
    public static string UpscaleErosionShader = "ComputeShaders/Erosion/UpscaleErosion";
    public static string GaussianBlurShader = "ComputeShaders/PostProcessing/GaussianBlur";
    public static string WeightedShader = "ComputeShaders/PostProcessing/WeightedImages";
    public static string IslandProcessShader = "ComputeShaders/Lib/IslandProcess";
    public static string CraterShader = "ComputeShaders/Details/Craters";
}
