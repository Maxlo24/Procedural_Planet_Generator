using UnityEngine;

public class RandomLib : MonoBehaviour
{
    public static float NextFloat(System.Random prng)
    {
        return (float) prng.NextDouble();
    }
    
    public static float NextFloat(System.Random prng, float min, float max)
    {
        return NextFloat(prng) * (max - min) + min;
    }
}
