using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

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

    public static void SaveRaw(float[,] heights, string path = "Assets/", string filename = "test")
    {
        var completePath = path + filename + ".raw";
        using (BinaryWriter writer = new BinaryWriter(File.Open(completePath, FileMode.Create)))
        {
            for (int y = 0; y < heights.GetLength(0); y++)
            {
                for (int x = 0; x < heights.GetLength(1); x++)
                {
                    short value = (short)(heights[x, y] * 65535);
                    writer.Write(value);
                }
            }
        }
    }

    public float[,] LoadPNGAsHeightMap(string path)
    {
        var tex = new Texture2D(2, 2);
        tex.LoadImage(File.ReadAllBytes(path));
        var heightMap = new float[tex.width, tex.height];
        for (int x = 0; x < tex.width; x++)
        {
            for (int y = 0; y < tex.height; y++)
            {
                heightMap[x, y] = tex.GetPixel(x, y).grayscale;
            }
        }
        return heightMap;
    }

    public static float[,] LoadRawAsHeightmap(string path)
    {
        //reading file
        System.IO.FileInfo fileInfo = new System.IO.FileInfo(path);
        System.IO.FileStream stream = fileInfo.Open(System.IO.FileMode.Open, System.IO.FileAccess.Read);

        int size = (int)Mathf.Sqrt(stream.Length / 2);
        byte[] vals = new byte[size * size * 2];
        float[,] rawHeights = new float[size, size];

        stream.Read(vals, 0, vals.Length);
        stream.Close();

        //setting matrix
        Rect rect = new Rect(0, 0, size, size);
        int i = 0;
        for (int z = size - 1; z >= 0; z--)
            for (int x = 0; x < size; x++)
            {
                rawHeights[x, z] = (vals[i + 1] * 256f + vals[i]) / 65535f;
                i += 2;
            }
        return rawHeights;
    }

    public static float[,] LoadRawAsErosionMap(string path)
    {
        return LoadRawAsHeightmap(path);
    }

    public static float[,] ConvertRenderTextureToFloatArray(RenderTexture rt)
    {
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RFloat, false);
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();

        float [,] heightMap = new float[tex.width, tex.height];
        for (int x = 0; x < tex.width; x++)
        {
            for (int y = 0; y < tex.height; y++)
            {
                heightMap[x, y] = tex.GetPixel(x, y).grayscale;
            }
        }
        return heightMap;
    }
    
    public static RenderTexture ConvertFloatArrayToRenderTexture(float[,] heights)
    {
        Texture2D tex = new Texture2D(heights.GetLength(0), heights.GetLength(1), TextureFormat.RFloat, false);
        
        int width = heights.GetLength(0);
        int height = heights.GetLength(1);
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tex.SetPixel(y, x, new Color(heights[x, y], heights[x, y], heights[x, y]));
            }
        }

        tex.Apply();
        RenderTexture rt = new RenderTexture(heights.GetLength(0), heights.GetLength(1), 0, RenderTextureFormat.RFloat);
        rt.enableRandomWrite = true;
        Graphics.Blit(tex, rt);
        return rt;
    }

    public static float[] Create2DGaussianKernel(int KernelRadius, float sigma)
    {
        // Calculate the size of the kernel
        int KernelSize = KernelRadius * 2 + 1;
        // Create the kernel array
        float[] Kernel = new float[KernelSize * KernelSize];

        // Calculate the sum of the kernel values
        float sum = 0;
        for (int i = -KernelRadius; i <= KernelRadius; i++)
        {
            for (int j = -KernelRadius; j <= KernelRadius; j++)
            {
                // Calculate the Gaussian value for the current position
                float value = (float)((1.0 / (2 * Math.PI * sigma * sigma)) * Math.Exp(-(i * i + j * j) / (2 * sigma * sigma)));
                // Set the value in the kernel array
                Kernel[(i + KernelRadius) * KernelSize + (j + KernelRadius)] = value;
                // Add the value to the sum
                sum += value;
            }
        }

        // Normalize the kernel values by dividing by the sum
        for (int i = 0; i < KernelSize * KernelSize; i++)
        {
            Kernel[i] /= sum;
        }
        
        return Kernel;
    }

    public static RenderTexture CreateRenderTexture(int width, int height, RenderTextureFormat format)
    {
        RenderTexture rt = new RenderTexture(width, height, 0, format);
        rt.enableRandomWrite = true;
        rt.Create();
        return rt;
    }
    public static RenderTexture CopyRenderTexture(RenderTexture rt)
    {
        RenderTexture rtCopy = new RenderTexture(rt.width, rt.height, 0, rt.format);
        rtCopy.enableRandomWrite = true;
        rtCopy.Create();
        Graphics.Blit(rt, rtCopy);
        return rtCopy;
    }

    public static float GetMinFromRenderTexture(RenderTexture rt, int xmin, int xmax, int ymin, int ymax)
    {
        Texture2D tex = new Texture2D(xmax - xmin, ymax - ymin, (TextureFormat)rt.format, false);
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, xmax, ymax), 0, 0);
        tex.Apply();
        float min = float.MaxValue;
        for (int x = 0; x < tex.width; x++)
        {
            for (int y = 0; y < tex.height; y++)
            {
                float value = tex.GetPixel(x, y).r;
                if (value < min)
                {
                    min = value;
                }
            }
        }
        return min;
    }

    public static float GetMinFromRenderTexture(RenderTexture rt)
    {
        return GetMinFromRenderTexture(rt, 0, rt.width, 0, rt.height);
    }
}
