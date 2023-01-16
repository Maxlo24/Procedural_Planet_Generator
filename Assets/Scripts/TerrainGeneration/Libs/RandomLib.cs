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
}
