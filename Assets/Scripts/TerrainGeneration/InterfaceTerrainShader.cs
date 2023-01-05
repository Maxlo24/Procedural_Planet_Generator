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

        ErosionTexture2D = tex;
        return tex;
    }
}
