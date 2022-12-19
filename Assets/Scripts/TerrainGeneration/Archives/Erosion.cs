using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using UnityEngine;


public class Erosion : MonoBehaviour
{

    //[field: SerializeField, Range(0, 1)] public float Inertia { get; private set; } = 0.5f;
    //[field: SerializeField, Range(0, 500)] public int LifeTime { get; private set; } = 30;
    //[field: SerializeField, Range(0, 20)] public float InitialVel { get; private set; } = 1f;
    //[field: SerializeField, Range(0, 20)] public float InitialWaterVolume { get; private set; } = 1f;
    //[field: SerializeField, Range(0, 20)] public float SedimentCapacityFactor { get; private set; } = 4f;
    //[field: SerializeField, Range(0, 20)] public float SedimentMinSlope { get; private set; } = 0.01f;
    //[field: SerializeField, Range(0, 1)] public float DebositPrct { get; private set; } = 0.05f;
    //[field: SerializeField, Range(0, 1)] public float ErodePrct { get; private set; } = 0.05f;
    //[field: SerializeField, Range(0, 50)] public float ErosionRadius { get; private set; } = 3f;
    //[field: SerializeField, Range(0, 20)] public float Gravity { get; private set; } = 4f;
    //[field: SerializeField, Range(0, 1)] public float EvaporationPrct { get; private set; } = 0.01f;
    //[field: SerializeField, ] public Vector2 Pos { get; private set; }

    //private float dirThreshold = 0.02f;

    //private Vector2 grad;
    //private float _currentH = 0f;
    //private float _currentSediment = 0f;

    //private struct GradientHeight
    //{
    //    public float height;
    //    public Vector2 gradient;
    //}

    //private GradientHeight GetGradientHeight(float[,] height, Vector2 pos)
    //{
    //    Vector2Int Pxy = new Vector2Int((int)pos.x, (int)pos.y);
    //    // Get 4 neighbours
    //    Vector2Int Px1y = new Vector2Int(Pxy.x + 1, Pxy.y);
    //    Vector2Int Pxy1 = new Vector2Int(Pxy.x, Pxy.y + 1);
    //    Vector2Int Px1y1 = new Vector2Int(Pxy.x + 1, Pxy.y + 1);

    //    // Get height for each neighbour
    //    float hPxy = height[Pxy.x, Pxy.y];
    //    float hPx1y = height[Px1y.x, Px1y.y];
    //    float hPxy1 = height[Pxy1.x, Pxy1.y];
    //    float hPx1y1 = height[Px1y1.x, Px1y1.y];

    //    // Get weight for each neighbour
    //    float u = pos.x - Pxy.x;
    //    float v = pos.y - Pxy.y;


    //    // Calculate pos height with the bilinear interpolation
    //    float hPos = (1 - u) * (1 - v) * hPxy + u * (1 - v) * hPx1y + (1 - u) * v * hPxy1 + u * v * hPx1y1;

    //    // Calculate gradient with bilinear inteprolation
    //    float dx = (hPx1y - hPxy) * (1 - v) + (hPx1y1 - hPxy1) * v;
    //    float dy = (hPxy1 - hPxy) * (1 - u) + (hPx1y1 - hPx1y) * u;

    //    return new GradientHeight
    //    {
    //        height = hPos,
    //        gradient = new Vector2(dx, dy)
    //    };
    //}

    //GradientHeight CalculateHeightAndGradient(float[,] heights, Vector2 pos)
    //{
    //    int coordX = (int)pos.x;
    //    int coordY = (int)pos.y;

    //    // Calculate droplet's offset inside the cell (0,0) = at NW node, (1,1) = at SE node
    //    float x = pos.x - coordX;
    //    float y = pos.y - coordY;

    //    // Calculate heights of the four nodes of the droplet's cell
    //    float heightNW = heights[coordX, coordY];
    //    float heightNE = heights[coordX + 1, coordY];
    //    float heightSW = heights[coordX, coordY + 1];
    //    float heightSE = heights[coordX + 1, coordY + 1];

    //    // Calculate droplet's direction of flow with bilinear interpolation of height difference along the edges
    //    float gradientX = (heightNE - heightNW) * (1 - y) + (heightSE - heightSW) * y;
    //    float gradientY = (heightSW - heightNW) * (1 - x) + (heightSE - heightNE) * x;

    //    // Calculate height with bilinear interpolation of the heights of the nodes of the cell
    //    float height = heightNW * (1 - x) * (1 - y) + heightNE * x * (1 - y) + heightSW * (1 - x) * y + heightSE * x * y;

    //    return new GradientHeight { height = height, gradient = new Vector2(gradientX, gradientY) };
    //}



    //public void Erode(float[,] height, int iterations)
    //{
    //    //Vector2 pos = Pos;
    //    Debug.Log("Begin Erosion");
    //    float[,] heightCopy = new float[height.GetLength(0), height.GetLength(1)];
    //    Vector2Int previouspos = new Vector2Int(0, 0);
    //    float dh = 0.001f;
    //    for (int i = 0; i < height.GetLength(0); i++)
    //    {
    //        for (int j = 0; j < height.GetLength(1); j++)
    //        {
    //            heightCopy[i, j] = height[i, j];
    //        }
    //    }
    //    Vector2 pos = new Vector2(Random.Range(0f, (float)(height.GetLength(0) - 1)), Random.Range(0, (float)(height.GetLength(1) - 1)));
    //    for (int i = 0; i < iterations; i++)
    //    {
    //        GradientHeight gh = CalculateHeightAndGradient(heightCopy, pos);
    //        Vector2 grad = gh.gradient;
    //        Debug.Log("Magnitude " + grad.magnitude);
    //        grad.Normalize();

    //        Vector2Int Pxy1 = new Vector2Int((int)pos.x, (int)pos.y);

    //        //if (Pxy1 != previouspos)
    //        //{
    //            height[Pxy1.x, Pxy1.y] += dh;
    //            //dh += 0.001f;
    //            Debug.Log(Pxy1 + " " + gh.height);
    //        //}
    //        pos -= grad;

    //        previouspos = Pxy1;


    //        if (pos.x < 0 || pos.x >= height.GetLength(0) - 1 || pos.y < 0 || pos.y >= height.GetLength(1) - 1)
    //        {
    //            break;
    //        }
    //    }
    //}


    //public void Erode(float[,] height, int iterations)
    //{
    //    float[,] brushWeight = new float[(int)ErosionRadius * 2 + 2, (int)ErosionRadius * 2 + 2];
    //    Vector2Int[,] brushIndex = new Vector2Int[(int)ErosionRadius * 2 + 2, (int)ErosionRadius * 2 + 2];

    //    //float[,] heightCopy = new float[height.GetLength(0), height.GetLength(1)];
    //    //for (int i = 0; i < height.GetLength(0); i++)
    //    //{
    //    //    for (int j = 0; j < height.GetLength(1); j++)
    //    //    {
    //    //        heightCopy[i, j] = height[i, j];
    //    //    }
    //    //}

    //    for (int i = 0; i < iterations; i++)
    //    {
    //        float vel = InitialVel;
    //        float waterVolume = InitialWaterVolume;
    //        Vector2 pos = new Vector2(Random.Range(0f, (float)(height.GetLength(0) - 1)), Random.Range(0, (float)(height.GetLength(1) - 1)));
    //        //Vector2 pos = Pos;
    //        float dh = 0.001f;
    //        Vector2 _currentDir = Vector2.zero;

    //        for (int lt = 0; lt < LifeTime; lt++)
    //        {
    //            GradientHeight gh = CalculateHeightAndGradient(height, pos);

    //            Vector2 grad = gh.gradient;
    //            grad.Normalize();
    //            float h = gh.height;

    //            _currentDir = _currentDir * Inertia - grad * (1 - Inertia);

    //            //if (_currentDir.magnitude < dirThreshold)
    //            //{
    //            //    _currentDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
    //            //}

    //            _currentDir.Normalize();
    //            pos += _currentDir;

    //            // Stop simulating droplet if it's not moving or has flowed over edge of map
    //            if ((_currentDir.x == 0 && _currentDir.y == 0) || pos.x < 0 || pos.x >= height.GetLength(0) - 1 || pos.y < 0 || pos.y >= height.GetLength(1) - 1)
    //            {
    //                break;
    //            }

    //            _currentH = h;

    //            h = GetGradientHeight(height, pos).height;

    //            float hDiff = h - _currentH;

    //            float sedimentCapacity = Mathf.Max(-hDiff, SedimentMinSlope) * vel * waterVolume;
    //            Vector2Int Pxy1 = new Vector2Int((int)pos.x, (int)pos.y);

    //            //height[Pxy1.x, Pxy1.y] += dh;

    //            if (_currentSediment > sedimentCapacity || hDiff > 0f)
    //            {
    //                float sedimentDeposit = (hDiff > 0f) ? Mathf.Min((sedimentCapacity - _currentSediment) * DebositPrct, -hDiff)
    //                                                     : (_currentSediment - sedimentCapacity) * DebositPrct;

    //                _currentSediment -= sedimentDeposit;

    //                Vector2Int Pxy = new Vector2Int((int)pos.x, (int)pos.y);

    //                float u = pos.x - Pxy.x;
    //                float v = pos.y - Pxy.y;
    //                //Debug.Log("test deposit");

    //                //height[Pxy1.x, Pxy1.y] += dh;

    //                //sedimentDeposit = dh;
    //                // Get height for each neighbour
    //                height[Pxy.x, Pxy.y] += sedimentDeposit * (1 - u) * (1 - v);
    //                height[Pxy.x + 1, Pxy.y] += sedimentDeposit * u * (1 - v);
    //                height[Pxy.x, Pxy.y + 1] += sedimentDeposit * (1 - u) * v;
    //                height[Pxy.x + 1, Pxy.y + 1] += sedimentDeposit * u * v;
    //                //Debug.Log("Deposit");
    //                //Debug.Log(sedimentDeposit * u * v);
    //            }
    //            else
    //            {
    //                float sedimentEroded = Mathf.Min((sedimentCapacity - _currentSediment) * ErodePrct, -hDiff);
    //                //Debug.Log("test erode");
    //                Vector2Int Pxy = new Vector2Int((int)pos.x, (int)pos.y);
    //                height[Pxy.x, Pxy.y] -= 0.001f;
    //                Vector2 Puv = new Vector2(pos.x - Pxy.x, pos.y - Pxy.y);
    //                float sumWeights = 0f;
    //                for (int k = Pxy.x - (int)ErosionRadius; k < Pxy.x + (int)ErosionRadius + 2; k++)
    //                {
    //                    for (int l = Pxy.y - (int)ErosionRadius; l < Pxy.y + (int)ErosionRadius + 2; l++)
    //                    {
    //                        Vector2Int vecKL = new Vector2Int(k, l);

    //                        float dist = Vector2.Distance(vecKL, pos);

    //                        brushIndex[k - Pxy.x + (int)ErosionRadius, l - Pxy.y + (int)ErosionRadius] = vecKL;

    //                        if (dist < ErosionRadius)
    //                        {
    //                            float weight = Mathf.Max(0, ErosionRadius - dist);
    //                            sumWeights += weight;
    //                            brushWeight[k - Pxy.x + (int)ErosionRadius, l - Pxy.y + (int)ErosionRadius] = weight;
    //                            //if (IsIndexesInBounds(height, vecKL))
    //                            //{
    //                            //    height[k, l] -= dh;
    //                            //}
    //                        }
    //                        else
    //                        {
    //                            brushWeight[k - Pxy.x + (int)ErosionRadius, l - Pxy.y + (int)ErosionRadius] = 0f;
    //                        }
    //                    }
    //                }

    //                for (int k = 0; k < brushWeight.GetLength(0); k++)
    //                {
    //                    for (int l = 0; l < brushWeight.GetLength(1); l++)
    //                    {
    //                        brushWeight[k, l] /= sumWeights;
    //                        Vector2Int indexes = brushIndex[k, l];
    //                        if (IsIndexesInBounds(height, indexes))
    //                        {
    //                            float toErode = sedimentEroded * brushWeight[k, l];
    //                            float deltaSediment = (height[indexes.x, indexes.y] < toErode) ? height[indexes.x, indexes.y] : toErode;
    //                            //deltaSediment = dh;
    //                            height[indexes.x, indexes.y] -= deltaSediment;
    //                            _currentSediment += deltaSediment;
    //                            if (deltaSediment > 0.0001)
    //                            {
    //                                //Debug.Log("Erode");
    //                                Debug.Log(deltaSediment);
    //                            }
    //                        }
    //                    }
    //                }
    //                //height[Pxy1.x, Pxy1.y] -= dh;

    //            }
    //            vel = Mathf.Sqrt(vel * vel + hDiff * Gravity);
    //            waterVolume *= (1 - EvaporationPrct);
    //        }

    //    }
    //}


    ////private GradientHeight GetGradientHeight(float[,] height, Vector2 pos)
    ////{
    ////    Vector2Int Pxy = new Vector2Int((int) pos.x, (int) pos.y);
    ////    // Get 4 neighbours
    ////    Vector2Int Px1y = new Vector2Int(Pxy.x + 1, Pxy.y);
    ////    Vector2Int Pxy1 = new Vector2Int(Pxy.x, Pxy.y + 1);
    ////    Vector2Int Px1y1 = new Vector2Int(Pxy.x + 1, Pxy.y + 1);

    ////    // Get height for each neighbour
    ////    float hPxy = height[Pxy.x, Pxy.y];
    ////    float hPx1y = height[Px1y.x, Px1y.y];
    ////    float hPxy1 = height[Pxy1.x, Pxy1.y];
    ////    float hPx1y1 = height[Px1y1.x, Px1y1.y];

    ////    // Get weight for each neighbour
    ////    float u = pos.x - Pxy.x;
    ////    float v = pos.y - Pxy.y;


    ////    // Calculate pos height with the bilinear interpolation
    ////    float hPos = (1 - u) * (1 - v) * hPxy + u * (1 - v) * hPx1y + (1 - u) * v * hPxy1 + u * v * hPx1y1;

    ////    // Calculate gradient with bilinear inteprolation
    ////    float dx = (hPx1y - hPxy) * (1 - v) + (hPx1y1 - hPxy1) * v;
    ////    float dy = (hPxy1 - hPxy) * (1 - u) + (hPx1y1 - hPx1y) * u;

    ////    return new GradientHeight
    ////    {
    ////        height = hPos,
    ////        gradient = new Vector2(dx, dy)
    ////    };
    ////}

    //private bool IsIndexesInBounds(float[,] height, Vector2Int indexes)
    //{
    //    if (0 <= indexes.x && indexes.x <= height.GetLength(0) - 1 && 0 <= indexes.y && indexes.y <= height.GetLength(1) - 1)
    //    {
    //        return true;
    //    }
    //    return false;
    //}









































    public int seed;
    [Range(2, 8)]
    public int erosionRadius = 3;
    [Range(0, 1)]
    public float inertia = .05f; // At zero, water will instantly change direction to flow downhill. At 1, water will never change direction. 
    public float sedimentCapacityFactor = 4; // Multiplier for how much sediment a droplet can carry
    public float minSedimentCapacity = .01f; // Used to prevent carry capacity getting too close to zero on flatter terrain
    [Range(0, 1)]
    public float erodeSpeed = .3f;
    [Range(0, 1)]
    public float depositSpeed = .3f;
    [Range(0, 1)]
    public float evaporateSpeed = .01f;
    public float gravity = 4;
    public int maxDropletLifetime = 30;

    public float initialWaterVolume = 1;
    public float initialSpeed = 1;

    // Indices and weights of erosion brush precomputed for every node
    int[,][] erosionBrushIndices;
    float[,][] erosionBrushWeights;
    System.Random prng;

    int currentSeed;
    int currentErosionRadius;
    int currentMapSizeX;
    int currentMapSizeY;

    // Initialization creates a System.Random object and precomputes indices and weights of erosion brush
    void Initialize(int sizeX, int sizeY, bool resetSeed)
    {
        if (resetSeed || prng == null || currentSeed != seed)
        {
            prng = new System.Random(seed);
            currentSeed = seed;
        }

        if (erosionBrushIndices == null || currentErosionRadius != erosionRadius || currentMapSizeX != sizeX || currentMapSizeY != sizeY)
        {
            InitializeBrushIndices(sizeX, sizeY, erosionRadius);
            currentErosionRadius = erosionRadius;
            currentMapSizeX = sizeX;
            currentMapSizeY = sizeY;
        }
    }

    public void Erode(float[,] heights, int numIterations = 1, bool resetSeed = false)
    {
        int mapSizeX = heights.GetLength(0);
        int mapSizeY = heights.GetLength(1);

        Initialize(mapSizeX, mapSizeY, resetSeed);

        for (int iteration = 0; iteration < numIterations; iteration++)
        {
            // Create water droplet at random point on map
            float posX = prng.Next(0, mapSizeX - 1);
            float posY = prng.Next(0, mapSizeY - 1);
            float dirX = 0;
            float dirY = 0;
            float speed = initialSpeed;
            float water = initialWaterVolume;
            float sediment = 0;

            for (int lifetime = 0; lifetime < maxDropletLifetime; lifetime++)
            {
                int nodeX = (int)posX;
                int nodeY = (int)posY;
                // Calculate droplet's offset inside the cell (0,0) = at NW node, (1,1) = at SE node
                float cellOffsetX = posX - nodeX;
                float cellOffsetY = posY - nodeY;

                // Calculate droplet's height and direction of flow with bilinear interpolation of surrounding heights
                HeightAndGradient heightAndGradient = CalculateHeightAndGradient(heights, posX, posY);

                // Update the droplet's direction and position (move position 1 unit regardless of speed)
                dirX = (dirX * inertia - heightAndGradient.gradientX * (1 - inertia));
                dirY = (dirY * inertia - heightAndGradient.gradientY * (1 - inertia));
                // Normalize direction
                float len = Mathf.Sqrt(dirX * dirX + dirY * dirY);
                if (len != 0)
                {
                    dirX /= len;
                    dirY /= len;
                }
                posX += dirX;
                posY += dirY;

                // Stop simulating droplet if it's not moving or has flowed over edge of map
                if ((dirX == 0 && dirY == 0) || posX < 0 || posX >= mapSizeX - 1 || posY < 0 || posY >= mapSizeY - 1)
                {
                    break;
                }

                // Find the droplet's new height and calculate the deltaHeight
                float newHeight = CalculateHeightAndGradient(heights, posX, posY).height;
                float deltaHeight = newHeight - heightAndGradient.height;

                // Calculate the droplet's sediment capacity (higher when moving fast down a slope and contains lots of water)
                float sedimentCapacity = Mathf.Max(-deltaHeight * speed * water * sedimentCapacityFactor, minSedimentCapacity);

                // If carrying more sediment than capacity, or if flowing uphill:
                if (sediment > sedimentCapacity || deltaHeight > 0)
                {
                    // If moving uphill (deltaHeight > 0) try fill up to the current height, otherwise deposit a fraction of the excess sediment
                    float amountToDeposit = (deltaHeight > 0) ? Mathf.Min(deltaHeight, sediment) : (sediment - sedimentCapacity) * depositSpeed;
                    sediment -= amountToDeposit;

                    // Add the sediment to the four nodes of the current cell using bilinear interpolation
                    // Deposition is not distributed over a radius (like erosion) so that it can fill small pits
                    heights[nodeX, nodeY] += amountToDeposit * (1 - cellOffsetX) * (1 - cellOffsetY);
                    heights[nodeX + 1, nodeY] += amountToDeposit * cellOffsetX * (1 - cellOffsetY);
                    heights[nodeX, nodeY + 1] += amountToDeposit * (1 - cellOffsetX) * cellOffsetY;
                    heights[nodeX + 1, nodeY + 1] += amountToDeposit * cellOffsetX * cellOffsetY;

                }
                else
                {
                    // Erode a fraction of the droplet's current carry capacity.
                    // Clamp the erosion to the change in height so that it doesn't dig a hole in the terrain behind the droplet
                    float amountToErode = Mathf.Min((sedimentCapacity - sediment) * erodeSpeed, -deltaHeight);

                    // Use erosion brush to erode from all nodes inside the droplet's erosion radius
                    for (int brushPointIndex = 0; brushPointIndex < erosionBrushIndices[nodeX, nodeY].Length; brushPointIndex++)
                    {
                        int nodeIndex = erosionBrushIndices[nodeX, nodeY][brushPointIndex];
                        float weighedErodeAmount = amountToErode * erosionBrushWeights[nodeX, nodeY][brushPointIndex];
                        float deltaSediment = (heights[nodeX, nodeY] < weighedErodeAmount) ? heights[nodeX, nodeY] : weighedErodeAmount;
                        heights[nodeX, nodeY] -= deltaSediment;
                        sediment += deltaSediment;
                    }
                }

                // Update droplet's speed and water content
                speed = Mathf.Sqrt(speed * speed + deltaHeight * gravity);
                water *= (1 - evaporateSpeed);
            }
        }
    }

    HeightAndGradient CalculateHeightAndGradient(float[,] heights, float posX, float posY)
    {
        int coordX = (int)posX;
        int coordY = (int)posY;

        // Calculate droplet's offset inside the cell (0,0) = at NW node, (1,1) = at SE node
        float x = posX - coordX;
        float y = posY - coordY;

        // Calculate heights of the four nodes of the droplet's cell
        float heightNW = heights[coordX, coordY];
        float heightNE = heights[coordX + 1, coordY];
        float heightSW = heights[coordX, coordY + 1];
        float heightSE = heights[coordX + 1, coordY + 1];

        // Calculate droplet's direction of flow with bilinear interpolation of height difference along the edges
        float gradientX = (heightNE - heightNW) * (1 - y) + (heightSE - heightSW) * y;
        float gradientY = (heightSW - heightNW) * (1 - x) + (heightSE - heightNE) * x;

        // Calculate height with bilinear interpolation of the heights of the nodes of the cell
        float height = heightNW * (1 - x) * (1 - y) + heightNE * x * (1 - y) + heightSW * (1 - x) * y + heightSE * x * y;

        return new HeightAndGradient() { height = height, gradientX = gradientX, gradientY = gradientY };
    }

    void InitializeBrushIndices(int sizeX, int sizeY, int radius)
    {
        erosionBrushIndices = new int[sizeX, sizeY][];
        erosionBrushWeights = new float[sizeX, sizeY][];

        int[] xOffsets = new int[radius * radius * 4];
        int[] yOffsets = new int[radius * radius * 4];
        float[] weights = new float[radius * radius * 4];
        float weightSum = 0;
        int addIndex = 0;

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                int centreX = i;
                int centreY = j;

                if (centreY <= radius || centreY >= sizeY - radius || centreX <= radius + 1 || centreX >= sizeX - radius)
                {
                    weightSum = 0;
                    addIndex = 0;
                    for (int y = -radius; y <= radius; y++)
                    {
                        for (int x = -radius; x <= radius; x++)
                        {
                            float sqrDst = x * x + y * y;
                            if (sqrDst < radius * radius)
                            {
                                int coordX = centreX + x;
                                int coordY = centreY + y;

                                if (coordX >= 0 && coordX < sizeX && coordY >= 0 && coordY < sizeY)
                                {
                                    float weight = 1 - Mathf.Sqrt(sqrDst) / radius;
                                    weightSum += weight;
                                    weights[addIndex] = weight;
                                    xOffsets[addIndex] = x;
                                    yOffsets[addIndex] = y;
                                    addIndex++;
                                }

                            }
                        }
                    }
                }

                int numEntries = addIndex;
                erosionBrushIndices[i, j] = new int[numEntries];
                erosionBrushWeights[i, j] = new float[numEntries];

                for (int k = 0; k < numEntries; k++)
                {
                    erosionBrushIndices[i, j][k] = (yOffsets[k] + centreY) * sizeX + xOffsets[k] + centreX;
                    erosionBrushWeights[i, j][k] = weights[k] / weightSum;
                }
            }
        }
    }

    struct HeightAndGradient
    {
        public float height;
        public float gradientX;
        public float gradientY;
    }

}