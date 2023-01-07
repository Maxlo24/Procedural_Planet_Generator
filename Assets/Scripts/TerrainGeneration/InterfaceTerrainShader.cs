using UnityEngine;

[ExecuteInEditMode]

public class InterfaceTerrainShader : MonoBehaviour
{
    [field: SerializeField] public TerrainGenerationBase Terrain { get; private set; }
    [field: SerializeField] public Texture2D ErosionTexture2D { get; private set; }

    public Texture2D ErosionTexture()
    {
        RenderTexture rt = Terrain.ErosionTexture;

        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RFloat, false);
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();

        // Normalize texture

        //float min = float.MaxValue;
        //float max = float.MinValue;

        //for (int i = 0; i < tex.width; i++)
        //{
        //    for (int j = 0; j < tex.height; j++)
        //    {
        //        float value = tex.GetPixel(i, j).r;
        //        if (value < min) min = value;
        //        if (value > max) max = value;
        //    }
        //}

        //max = Mathf.Max(Mathf.Abs(min), Mathf.Abs(max));
        

        //for (int i = 0; i < tex.width; i++)
        //{
        //    for (int j = 0; j < tex.height; j++)
        //    {
        //        float value = tex.GetPixel(i, j).r;
        //        value = value / max;
        //        tex.SetPixel(i, j, new Color(value, value, value));
        //    }
        //}
        

        ErosionTexture2D = tex;

        Shader.SetGlobalTexture("_Erosion_map", ErosionTexture2D);

        return tex;
    }
}
