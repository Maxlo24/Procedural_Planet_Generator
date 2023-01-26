using UnityEngine;
using System.IO;
using System;
using Unity.VisualScripting;

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

    public static void SavePNG_Red_Channel(RenderTexture renderTexture, string path = "Assets/", string filename = "test")
    {
        
        var tex = new Texture2D(renderTexture.width, renderTexture.height);
        RenderTexture.active = renderTexture;
        tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        tex.Apply();

        var completePath = path + filename + ".png";
        // save only the red channel of the render texture
        var pixels = tex.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i].g = 0;
            pixels[i].b = 0;
        }
        tex.SetPixels(pixels);
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

    public static void SaveRaw_Red_Channel(RenderTexture heights, string path = "Assets/", string filename = "test")
    {
        var completePath = path + filename + ".raw";
        using (BinaryWriter writer = new BinaryWriter(File.Open(completePath, FileMode.Create)))
        {
            var tex = new Texture2D(heights.width, heights.height);
            RenderTexture.active = heights;
            tex.ReadPixels(new Rect(0, 0, heights.width, heights.height), 0, 0);
            tex.Apply();
            var pixels = tex.GetPixels();
            for (int i = 0; i < pixels.Length; i++)
            {
                short value = (short)(pixels[i].r * 65535);
                writer.Write(value);
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

        float[,] heightMap = new float[tex.width, tex.height];
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
        Graphics.Blit(rt, rtCopy);
        return rtCopy;
    }

    public static Vector2 GetMinMaxFromRenderTexture(RenderTexture rt, int xmin, int xmax, int ymin, int ymax)
    {
        Texture2D tex = new Texture2D(xmax - xmin, ymax - ymin, (TextureFormat)rt.format, false);
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(xmin, ymin, xmax - xmin, ymax - ymin), 0, 0);
        tex.Apply();
        float min = float.MaxValue;
        float max = float.MinValue;

        for (int x = 0; x < tex.width; x++)
        {
            for (int y = 0; y < tex.height; y++)
            {
                float value = tex.GetPixel(x, y).r;
                if (value < min)
                {
                    min = value;
                }
                if (value > max)
                {
                    max = value;
                }
            }
        }

        return new Vector2(min, max);
    }

    public static Vector2 GetMinMaxFromRenderTexture(RenderTexture rt)
    {
        return GetMinMaxFromRenderTexture(rt, 0, rt.width, 0, rt.height);
    }

    public static void NormalizeRenderTexture(ref RenderTexture rt)
    {
        GetMinMaxFromRenderTexture(rt, out float min, out float max);

        ComputeShader normalizeShader = Resources.Load<ComputeShader>(ShaderLib.NormalizeShader);
        int kernel = normalizeShader.FindKernel("CSMain");
        normalizeShader.SetTexture(kernel, "result", rt);
        normalizeShader.SetFloat("min", min);
        normalizeShader.SetFloat("max", max);
        normalizeShader.Dispatch(kernel, rt.width / 32 + 1, rt.height / 32 + 1, 1);
    }

    public static void MultiplyRenderTexture(ref RenderTexture rt, RenderTexture rt2)
    {
        ComputeShader multiplyShader = Resources.Load<ComputeShader>(ShaderLib.MultiplyShader);
        int kernel = multiplyShader.FindKernel("CSMain");
        multiplyShader.SetTexture(kernel, "result", rt);
        multiplyShader.SetTexture(kernel, "imageToMultiply", rt2);
        multiplyShader.Dispatch(kernel, rt.width / 32 + 1, rt.height / 32 + 1, 1);
    }

    public static void GetMinMaxFromRenderTexture(int terrainRes, RenderTexture rt, int xmin, int xmax, int ymin, int ymax, out float min, out float max)
    {
        ComputeShader minMaxShader = Resources.Load<ComputeShader>(ShaderLib.MinMaxShader);
        int kernel = minMaxShader.FindKernel("CSMain");

        RenderTexture rtCopyMin;
        if (xmax - xmin == rt.width && ymax - ymin == rt.height)
        {
            rtCopyMin = CopyRenderTexture(rt);
        }
        else
        {
            rtCopyMin = Truncate(rt, xmin, xmax, terrainRes - ymax, terrainRes - ymin);
        }
        RenderTexture rtCopyMax = CopyRenderTexture(rtCopyMin);

        RenderTexture rtMin = CreateRenderTexture(rtCopyMin.width / 3 + 1, rtCopyMax.width / 3 + 1, rt.format);
        RenderTexture rtMax = CreateRenderTexture(rtCopyMax.width / 3 + 1, rtCopyMax.width / 3 + 1, rt.format);

        int divParam = 3; // !!!! > 2

        while (rtCopyMin.width >= divParam || rtCopyMin.height >= divParam)
        {
            minMaxShader.SetTexture(kernel, "rtMin", rtMin);
            minMaxShader.SetTexture(kernel, "rtMax", rtMax);
            minMaxShader.SetTexture(kernel, "rtCopyMin", rtCopyMin);
            minMaxShader.SetTexture(kernel, "rtCopyMax", rtCopyMax);
            minMaxShader.SetInt("width", rtCopyMin.width);
            minMaxShader.SetInt("height", rtCopyMin.height);
            minMaxShader.SetInt("divParam", divParam);
            minMaxShader.Dispatch(kernel, rtMin.width / 32 + 1, rtMin.height / 32 + 1, 1);

            rtCopyMin = rtMin;
            rtCopyMax = rtMax;

            rtMin = CreateRenderTexture(rtMin.width / divParam + 1, rtMin.width / divParam + 1, rt.format);
            rtMax = CreateRenderTexture(rtMax.width / divParam + 1, rtMax.width / divParam + 1, rt.format);
        }

        min = GetMinMaxFromRenderTexture(rtCopyMin).x;
        max = GetMinMaxFromRenderTexture(rtCopyMax).y;
    }

    public static void GetMinMaxFromRenderTexture(RenderTexture rt, out float min, out float max)
    {
        GetMinMaxFromRenderTexture(-1, rt, 0, rt.width, 0, rt.height, out min, out max);
    }

    public static RenderTexture Truncate(RenderTexture rt, int xmin, int xmax, int ymin, int ymax)
    {
        ComputeShader truncateShader = Resources.Load<ComputeShader>(ShaderLib.TruncateShader);

        RenderTexture rtResized = CreateRenderTexture(xmax - xmin, ymax - ymin, rt.format);

        int kernel = truncateShader.FindKernel("CSMain");
        truncateShader.SetTexture(kernel, "result", rtResized);
        truncateShader.SetTexture(kernel, "original", rt);
        truncateShader.SetInt("xmin", xmin);
        truncateShader.SetInt("ymin", ymin);
        truncateShader.SetInt("xmax", xmax);
        truncateShader.SetInt("ymax", ymax);
        truncateShader.Dispatch(kernel, (xmax - xmin) / 32 + 1, (ymax - ymin) / 32 + 1, 1);

        return rtResized;
    }
}
