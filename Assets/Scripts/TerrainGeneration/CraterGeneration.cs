using UnityEngine;

public class CraterGeneration : MonoBehaviour
{
    [field: SerializeField] public ComputeShader CraterShader { get; private set; }
    [field: SerializeField] public int Seed { get; private set; } = 0;
    [field: SerializeField] public int Number { get; private set; } = 0;
    [field: SerializeField] public Vector2 RadiusRange { get; private set; } = new Vector2(2, 5);
    [field: SerializeField] public Vector2 DepthRange { get; private set; } = new Vector2(0.5f, 0.5f);
    [field: SerializeField] public int TerrainBorder { get; private set; } = 0;

    struct Crater
    {
        public Vector2[] Position;
        public float[] Radius;
        public float[] Depth;

        public Crater(Vector2[] position, float[] radius, float[] depth)
        {
            Position = position;
            Radius = radius;
            Depth = depth;
        }
    }

    public RenderTexture GenerateCraters(RenderTexture rt, Terrain terrain)
    {
        TerrainData td = terrain.terrainData;
        RenderTexture rtOutput = ImageLib.CopyRenderTexture(rt);

        System.Random prng = new System.Random(Seed);
        int kernel = CraterShader.FindKernel("CSMain");

        Crater craters = GetCratersInformation(rt.width, rt.height, prng);

        for (int i = 0; i < Number; i++)
        {
            float realRadius = craters.Radius[i] * (float)td.heightmapResolution / (float)td.size.x;
            Vector2 vecMin = craters.Position[i] - new Vector2(realRadius, realRadius) / 2.0f;
            Vector2 vecMax = craters.Position[i] + new Vector2(realRadius, realRadius) / 2.0f; ;

            CraterShader.SetFloat("minCenter", ImageLib.GetMinFromRenderTexture(rt, (int)vecMin.x, (int)vecMax.x, (int)vecMin.y, (int)vecMax.y));
            CraterShader.SetVector("position", craters.Position[i]);
            CraterShader.SetFloat("radius", realRadius);
            CraterShader.SetFloat("depth", craters.Depth[i] / 100f);
            CraterShader.SetTexture(kernel, "result", rtOutput);
            CraterShader.Dispatch(kernel, ((int)(craters.Radius[i]) * 2 + 1) / 8 + 1, ((int)(craters.Radius[i]) * 2 + 1) / 8 + 1, 1);

        }

        return rtOutput;
    }
    
    private Vector2 GetRandomPosition(int minX, int minY, int sizeX, int sizeY, System.Random prng)
    {
        return new Vector2(prng.Next(minX, sizeX - 1), prng.Next(minY, sizeY - 1));
    }

    private Crater GetCratersInformation(int sizeX, int sizeY, System.Random prng)
    {        
        Vector2[] positions = new Vector2[Number];
        float[] radius = new float[Number];
        float[] depth = new float[Number];
        
        for (int i = 0; i < Number; i++)
        {
            radius[i] = RandomLib.NextFloat(prng, RadiusRange.x, RadiusRange.y);
            int radiusInt = (int) radius[i] + TerrainBorder;
            positions[i] = GetRandomPosition(radiusInt, radiusInt, sizeX - radiusInt, sizeY - radiusInt, prng);
            depth[i] = RandomLib.NextFloat(prng, DepthRange.x, DepthRange.y);
        }

        return new Crater(positions, radius, depth);
    }

}
