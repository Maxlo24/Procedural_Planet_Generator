using UnityEngine;

public class CraterGeneration : MonoBehaviour
{
    [field: SerializeField] public int Seed { get; private set; } = 0;
    [field: SerializeField] public int Number { get; private set; } = 0;
    [field: SerializeField] public Vector2 RadiusRange { get; private set; } = new Vector2(2, 5);
    [field: SerializeField] public Vector2 SecondaryRadiusRange { get; private set; } = new Vector2(2, 5);
    [field: SerializeField] public Vector2 DepthRange { get; private set; } = new Vector2(0.5f, 0.5f);
    [field: SerializeField] public Vector2 ElevationRange { get; private set; } = new Vector2(0.5f, 0.5f);
    [field: SerializeField] public int TerrainBorder { get; private set; } = 0;

    struct Crater
    {
        public Vector2[] Position;
        public float[] Radius;
        public float[] SecondaryRadiusOffset;
        public float[] Depth;
        public float[] ElevationRange;

        public Crater(Vector2[] position, float[] radius, float[] secondaryRadiusOffset, float[] depth, float[] elevationRange)
        {
            Position = position;
            Radius = radius;
            SecondaryRadiusOffset = secondaryRadiusOffset;
            Depth = depth;
            ElevationRange = elevationRange;
        }
    }
    
    public void GenerateCraters(ref RenderTexture rt, int terrainRes)
    {
        RenderTexture rtCopy = ImageLib.CopyRenderTexture(rt);
        
        System.Random prng = new System.Random(Seed);

        ComputeShader craterShader = Resources.Load<ComputeShader>(ShaderLib.CraterShader);
        int kernel = craterShader.FindKernel("CSMain");
        
        Crater craters = GetCratersInformation(rt.width, rt.height, prng);

        for (int i = 0; i < Number; i++)
        {
            float radius = craters.Radius[i];
            Vector2 vecMin = craters.Position[i] - new Vector2(radius, radius) / 1f;
            Vector2 vecMax = craters.Position[i] + new Vector2(radius, radius) / 1f;
            
            ImageLib.GetMinMaxFromRenderTexture(terrainRes, rt, (int)vecMin.x, (int)vecMax.x, (int)vecMin.y, (int)vecMax.y, out float minCenter, out float maxCenter);
            craterShader.SetFloat("minCenter", minCenter);
            craterShader.SetVector("position", new Vector2(0, terrainRes) + new Vector2(craters.Position[i].x, -craters.Position[i].y));
            craterShader.SetFloat("radius", craters.Radius[i]);
            craterShader.SetFloat("secondaryRadiusOffset", craters.SecondaryRadiusOffset[i]);
            craterShader.SetFloat("depth", craters.Depth[i] / 100f);
            craterShader.SetFloat("elevationRange", craters.ElevationRange[i] / 100f);
            craterShader.SetTexture(kernel, "result", rt);
            craterShader.SetTexture(kernel, "copy", rtCopy);
            craterShader.Dispatch(kernel, ((int)(craters.Radius[i] + craters.SecondaryRadiusOffset[i]) * 2 + 1) / 8 + 1, ((int)(craters.Radius[i] + craters.SecondaryRadiusOffset[i]) * 2 + 1) / 8 + 1, 1);
        }

        rtCopy.Release();
    }

    private Vector2 GetRandomPosition(int minX, int minY, int sizeX, int sizeY, System.Random prng)
    {
        return new Vector2(prng.Next(minX, sizeX - 1), prng.Next(minY, sizeY - 1));
    }

    private Crater GetCratersInformation(int sizeX, int sizeY, System.Random prng)
    {
        Vector2[] positions = new Vector2[Number];
        float[] radius = new float[Number];
        float[] secondaryRadiusOffset = new float[Number];
        float[] depth = new float[Number];
        float[] elevationRange = new float[Number];


        for (int i = 0; i < Number; i++)
        {
            radius[i] = RandomLib.NextFloat(prng, RadiusRange.x, RadiusRange.y);
            secondaryRadiusOffset[i] = RandomLib.NextFloat(prng, SecondaryRadiusRange.x, SecondaryRadiusRange.y);
            int radiusInt = (int)radius[i] + TerrainBorder;
            positions[i] = GetRandomPosition(radiusInt, radiusInt, sizeX - radiusInt, sizeY - radiusInt, prng); /*new Vector2(512, 512)*/;
            depth[i] = RandomLib.NextFloat(prng, DepthRange.x, DepthRange.y);
            elevationRange[i] = RandomLib.NextFloat(prng, ElevationRange.x, ElevationRange.y);
        }

        return new Crater(positions, radius, secondaryRadiusOffset, depth, elevationRange);
    }

}
