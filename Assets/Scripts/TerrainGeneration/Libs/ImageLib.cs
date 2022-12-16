using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ImageLib : MonoBehaviour
{
    public static void SavePNG(RenderTexture renderTexture, string path = "Assets/", string filename = "test")
    {
        var tex = new Texture2D(renderTexture.width, renderTexture.height);
        RenderTexture.active = renderTexture;
        tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        tex.Apply();

        var completePath = path + filename + ".png";
        File.WriteAllBytes(completePath, tex.EncodeToPNG());
        Debug.Log("Saved file to: " + completePath);
    }
    public static void SavePNG(float[,] array, string path = "Assets/", string filename = "test")
    {

        var tex = new Texture2D(array.GetLength(0), array.GetLength(1));
        for (int x = 0; x < array.GetLength(0); x++)
        {
            for (int y = 0; y < array.GetLength(1); y++)
            {
                tex.SetPixel(x, y, new Color(array[x, y], array[x, y], array[x, y]));
            }
        }
        tex.Apply();
        var completePath = path + filename + ".png";
        File.WriteAllBytes(completePath, tex.EncodeToPNG());
        Debug.Log("Saved file to: " + completePath);
    }

}
