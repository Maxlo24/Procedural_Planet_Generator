using UnityEngine;

public class RandomLib : MonoBehaviour
{
    public static float NextFloat(System.Random prng)
    {
        return (float) prng.NextDouble();
    }
    
    public static float NextFloat(System.Random prng, float min, float max)
    {
        if (min > max)
        {
            float temp = min;
            min = max;
            max = temp;
        }
        else if (min == max)
        {
            return min;
        }
        return NextFloat(prng) * (max - min) + min;
    }

    public static int NextInt(System.Random prng, int min, int max)
    {
        if (min > max)
        {
            int temp = min;
            min = max;
            max = temp;
        }
        else if (min == max)
        {
            return min;
        }
        return prng.Next(min, max);
    }

    public static string GenerateRandomString(int lenght)
    {
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        System.Random random = new System.Random();
        char[] stringChars = new char[lenght];
        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length)];
        }
        string finalString = new string(stringChars);
        return finalString;
    }
}
