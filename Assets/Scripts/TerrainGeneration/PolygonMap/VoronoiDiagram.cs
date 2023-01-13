using System;
using System.Collections.Generic;
using UnityEngine;
using VoronoiLib;
using VoronoiLib.Structures;

[ExecuteInEditMode]
public class VoronoiDiagram : MonoBehaviour
{
    [field: SerializeField] public int Seed { get; private set; } = 0;
    [field: SerializeField] public Terrain Terrain { get; private set; }
    [field: SerializeField, Range(4, 100)] public int PointCount { get; private set; } = 5;
    [field: SerializeField, Range(1, 10)] public int LlyodIterations { get; private set; } = 4;
    [field: SerializeField] public Texture2D Texture { get; private set; }
    [field: SerializeField] public RenderTexture PolygonMapNoise { get; private set; }
    [field: SerializeField] public NoiseType NoiseType { get; private set; }
    [field: SerializeField, Range(-2, 2)] public float IslandRatio { get; private set; } = 0f;
    [field: SerializeField] public Vector2 Offset { get; private set; }
    [field: SerializeField, Range(0, 20)] public float Frequency { get; private set; } = 1f;
    [field: SerializeField, Range(-1, 1)] public float Threshold { get; private set; } = 0.2f;
    [field: SerializeField] public DistanceType DistanceType { get; private set; }
    [field: SerializeField] public bool LiveUpdate { get; private set; }

    private List<FortuneSite> _sites;
    private Dictionary<FortuneSite, HashSet<Vector2>> _voronoiDiagram;
    private Dictionary<FortuneSite, List<VEdge>> _voronoiDiagramE;

    public float size = 10;

    private void Update()
    {
        if (LiveUpdate)
        {
            GenerateNoiseLand();
        }
    }

    private void GeneratePoints()
    {
        System.Random prng = new System.Random(Seed);

        for (int i = 0; i < PointCount; i++)
        {
            float x = prng.Next(0, Terrain.terrainData.heightmapResolution);
            float y = prng.Next(0, Terrain.terrainData.heightmapResolution);

            _sites.Add(new FortuneSite(x, y));
        }
    }

    void DrawLine(int x1, int y1, int x2, int y2, Color a_Color)
    {
        float xPix = x1;
        float yPix = y1;

        float width = x2 - x1;
        float height = y2 - y1;
        float length = Mathf.Abs(width);
        if (Mathf.Abs(height) > length) length = Mathf.Abs(height);
        int intLength = (int)length;
        float dx = width / (float)length;
        float dy = height / (float)length;
        for (int i = 0; i <= intLength; i++)
        {
            Texture.SetPixel((int)xPix, (int)yPix, a_Color);

            xPix += dx;
            yPix += dy;
        }
    }

    private void LlyodAlgorithm()
    {
        for (int i = 0; i < LlyodIterations; i++)
        {
            _sites.Sort((x, y) => x.X.CompareTo(y.X));

            LinkedList<VEdge> voronoiResult = FortunesAlgorithm.Run(_sites, 0, 0, Terrain.terrainData.heightmapResolution, Terrain.terrainData.heightmapResolution);
            _voronoiDiagram = new Dictionary<FortuneSite, HashSet<Vector2>>();
            _voronoiDiagramE = new Dictionary<FortuneSite, List<VEdge>>();
            Dictionary<FortuneSite, List<VEdge>> _voronoiDiagramECopy = new Dictionary<FortuneSite, List<VEdge>>();
            for (LinkedListNode<VEdge> node = voronoiResult.First; node != null; node = node.Next)
            {
                VEdge edge = node.Value;

                if (i == LlyodIterations - 1)
                    DrawLine((int)edge.Start.X, (int)edge.Start.Y, (int)edge.End.X, (int)edge.End.Y, Color.red);

                if (!_voronoiDiagram.ContainsKey(edge.Left))
                    _voronoiDiagram.Add(edge.Left, new HashSet<Vector2>());
                if (!_voronoiDiagram.ContainsKey(edge.Right))
                    _voronoiDiagram.Add(edge.Right, new HashSet<Vector2>());

                if (!_voronoiDiagramE.ContainsKey(edge.Left))
                    _voronoiDiagramE.Add(edge.Left, new List<VEdge>());
                if (!_voronoiDiagramE.ContainsKey(edge.Right))
                    _voronoiDiagramE.Add(edge.Right, new List<VEdge>());

                Vector2 start = new Vector2((float)edge.Start.X, (float)edge.Start.Y);
                Vector2 end = new Vector2((float)edge.End.X, (float)edge.End.Y);

                _voronoiDiagram[edge.Left].Add(start);
                _voronoiDiagram[edge.Left].Add(end);
                _voronoiDiagram[edge.Right].Add(start);
                _voronoiDiagram[edge.Right].Add(end);

                if (!_voronoiDiagramE[edge.Left].Contains(edge))
                    _voronoiDiagramE[edge.Left].Add(edge);
                if (!_voronoiDiagramE[edge.Right].Contains(edge))
                    _voronoiDiagramE[edge.Right].Add(edge);
            }
            foreach (var site in _voronoiDiagramE)
            {
                List<VEdge> edges = site.Value;
                FortuneSite fortuneSite = site.Key;

                for (int j = 0; j < edges.Count; j++)
                {
                    VEdge edge = edges[j];

                    if (edge.Left != fortuneSite)
                    {
                        edge.Reverse();
                        edges[j] = edge;
                    }
                    Debug.Log("Start: " + edge.Start.X + " " + edge.Start.Y + " End: " + edge.End.X + " " + edge.End.Y);
                }
                Debug.Log(" ");

                //List<VEdge> edgesList = new List<VEdge>();
                //VEdge currentLastEdge = edges[0];
                //VEdge currentFirstEdge = edges[0];
                //edgesList.Add(currentLastEdge);
                //while (edgesList.Count < edges.Count)
                //{
                //    VEdge nextEdge = FindNextEdge(currentLastEdge, edges);
                //    if (nextEdge != null && !edgesList.Contains(nextEdge))
                //    {
                //        edgesList.Add(nextEdge);
                //        currentLastEdge = nextEdge;
                //    }
                //    VEdge prevEdge = FindPrevEdge(currentFirstEdge, edges);
                //    if (prevEdge != null && !edgesList.Contains(prevEdge))
                //    {
                //        edgesList.Insert(0, prevEdge);
                //        currentFirstEdge = prevEdge;
                //    }
                //}

                //foreach (var edge in edgesList)
                //{
                //    Debug.Log("Start: " + edge.Start.X + " " + edge.Start.Y + " End: " + edge.End.X + " " + edge.End.Y);
                //}
                //Debug.Log(" ");

            }
        }
    }


    //            for (int n = 0; n < edges.Count; n++)
    //            {
    //                VEdge edge = edges[n];
    //                if (edgesList.Count == 0)
    //                {
    //                    edgesList.Add(edge);
    //                }
    //                else
    //                {
    //                    int index = 0;
    //                    bool added = false;
    //                    for (int l = 0; l < edgesList.Count; l++)
    //                    {
    //                        VEdge Ledge = edgesList[l];
    //                        if ((Ledge.Start.X == edge.Start.X && Ledge.Start.Y == edge.Start.Y) || (Ledge.End.X == edge.End.X && Ledge.End.Y == edge.End.Y))
    //                        {
    //                            edge.Reverse();
    //                        }
    //                        if (Ledge.Start.X == edge.End.X && Ledge.Start.Y == edge.End.Y)
    //                        {
    //                            edgesList.Insert(index, edge);
    //                            added = true;
    //                            break;
    //                        }
    //                        else if (Ledge.End.X == edge.Start.X && Ledge.End.Y == edge.Start.Y)
    //                        {
    //                            edgesList.Insert(index + 1, edge);
    //                            added = true;
    //                            break;
    //                        }
    //                        index++;
    //                    }
    //                    //if (!added)
    //                    //{
    //                    //    //VEdge borderEdge = new VEdge(edgesList[0].Start, edgesList[edgesList.Count - 1].End);
    //                    //    edgesList.Add(edge);
    //                    //    Debug.Log("Error 1111111111111111111");
    //                    //}
    //                }
    //            }
    //            _voronoiDiagramECopy.Add(site.Key, edgesList);

    //            for (int k = 1; k < edgesList.Count; k++)
    //            {
    //                VEdge edge = edgesList[k];
    //                VEdge prevEdge = edgesList[k - 1];
    //                if (edge.Start.X != prevEdge.End.X && edge.Start.Y != prevEdge.End.Y)
    //                {
    //                    Debug.Log("Error");
    //                    VPoint corner = new VPoint(-1,-1);
    //                    if (prevEdge.End.X == 0 || prevEdge.End.X == Terrain.terrainData.heightmapResolution)
    //                    {
    //                        corner.X = prevEdge.End.X;
    //                        corner.Y = edge.Start.Y;
    //                    }
    //                    else if (prevEdge.End.Y == 0 || prevEdge.End.Y == Terrain.terrainData.heightmapResolution)
    //                    {
    //                        corner.X = edge.Start.X;
    //                        corner.Y = prevEdge.End.Y;
    //                    }
    //                    VEdge cornerEdge = new VEdge(prevEdge.End, corner);
    //                    cornerEdge.border = true;
    //                    edgesList.Insert(k, cornerEdge);
    //                    k++;
    //                    VEdge cornerEdge1 = new VEdge(corner, edge.Start);
    //                    cornerEdge1.border = true;
    //                    edgesList.Insert(k, cornerEdge1);
    //                    k++;
    //                    site.Key.border = true;

    //                }
    //                else if (edge.Start.X != prevEdge.End.X || edge.Start.Y != prevEdge.End.Y)
    //                {

    //                    VEdge borderEdge = new VEdge(prevEdge.End, edge.Start);
    //                    borderEdge.border = true;
    //                    edgesList.Insert(k, borderEdge);
    //                    k++;
    //                    site.Key.border = true;
    //                }
    //            }

    //            foreach (var edge in edgesList)
    //            {
    //                Debug.Log("Start: " + edge.Start.X + " " + edge.Start.Y + " End: " + edge.End.X + " " + edge.End.Y);
    //            }
    //            Debug.Log(" ");

    //        }
    //        _voronoiDiagramE = _voronoiDiagramECopy;


    //        foreach (var site in _voronoiDiagram)
    //        {
    //            HashSet<Vector2> edges = site.Value;
    //            List<Vector2> borders = new List<Vector2>();
    //            int borderX0 = 0;
    //            int borderY0 = 0;
    //            int borderX1 = 0;
    //            int borderY1 = 0;
    //            foreach (var point in edges)
    //            {
    //                if (point.x == 0)
    //                {
    //                    borders.Add(new Vector2(point.x, -1));
    //                    borderX0++;
    //                }
    //                else if (point.x == Terrain.terrainData.heightmapResolution)
    //                {
    //                    borders.Add(new Vector2(point.x, -1));
    //                    borderX1++;
    //                }
    //                else if (point.y == 0)
    //                {
    //                    borders.Add(new Vector2(-1, point.y));
    //                    borderY0++;
    //                }
    //                else if (point.y == Terrain.terrainData.heightmapResolution)
    //                {
    //                    borders.Add(new Vector2(-1, point.y));
    //                    borderY1++;
    //                }
    //            }

    //            if (borders.Count > 0)
    //                site.Key.border = true;

    //            for (int j = 0; j < borders.Count; j++)
    //            {
    //                Vector2 border = borders[j];
    //                if ((border.y == 0 && borderY0 > 1) || (border.y == Terrain.terrainData.heightmapResolution && borderY1 > 1)
    //                || (border.x == 0 && borderX0 > 1) || (border.x == Terrain.terrainData.heightmapResolution && borderX1 > 1))
    //                {
    //                    borders.Remove(border);
    //                    j--;
    //                }
    //            }

    //            if (borders.Count == 2)
    //            {
    //                Vector2 corner = (borders[0].x == -1) ? new Vector2(borders[1].x, borders[0].y) : new Vector2(borders[0].x, borders[1].y);
    //                _voronoiDiagram[site.Key].Add(corner);
    //            }
    //        }

    //        _sites = new List<FortuneSite>();

    //        foreach (var site in _voronoiDiagram)
    //        {
    //            HashSet<Vector2> edges = site.Value;
    //            Vector2 sum = new Vector2(0, 0);
    //            foreach (var point in edges)
    //            {
    //                sum += point;
    //            }
    //            Vector2 newCenter = sum / edges.Count;
    //            _sites.Add(new FortuneSite(newCenter.x, newCenter.y));
    //        }
    //    }
    //}

    private VEdge FindNextEdge(VEdge currentEdge, List<VEdge> edges)
    {
        foreach (VEdge edge in edges)
        {
            if (edge == currentEdge) continue;
            if (edge.Start == currentEdge.End)
            {
                return edge;
            }
        }
        return null;
    }

    private VEdge FindPrevEdge(VEdge currentEdge, List<VEdge> edges)
    {
        foreach (VEdge edge in edges)
        {
            if (edge == currentEdge) continue;
            if (edge.End == currentEdge.Start)
            {
                return edge;
            }
        }
        return null;
    }

    private void DrawPoints()
    {
        foreach (var point in _sites)
            Texture.SetPixel((int)point.X, (int)point.Y, Color.black);
    }

    public void GenerateNoiseLand()
    {
        PolygonMapNoise = ImageLib.CreateRenderTexture(Terrain.terrainData.heightmapResolution, Terrain.terrainData.heightmapResolution);
        ComputeShader noiseShader = Resources.Load<ComputeShader>(ShaderLib.PolygonMapNoiseShader);

        int kernel = noiseShader.FindKernel("CSMain");
        noiseShader.SetTexture(kernel, "Result", PolygonMapNoise);
        noiseShader.SetInt("resolution", Terrain.terrainData.heightmapResolution);
        noiseShader.SetInt("noiseType", (int)NoiseType);
        noiseShader.SetFloat("xOffset", Offset.x);
        noiseShader.SetFloat("yOffset", Offset.y);
        noiseShader.SetFloat("frequency", Frequency);
        noiseShader.SetFloat("islandRatio", IslandRatio);
        noiseShader.SetFloat("threshold", Threshold);
        noiseShader.SetInt("distanceType", (int)DistanceType);
        noiseShader.Dispatch(kernel, Terrain.terrainData.heightmapResolution / 8, Terrain.terrainData.heightmapResolution / 8, 1);

        RedrawTerrain();
    }

    private void RedrawTerrain()
    {
        RenderTexture.active = PolygonMapNoise;
        Terrain.terrainData.CopyActiveRenderTextureToHeightmap(new RectInt(0, 0, Terrain.terrainData.heightmapResolution, Terrain.terrainData.heightmapResolution),
            Vector2Int.zero, TerrainHeightmapSyncControl.HeightAndLod);
        RenderTexture.active = null;
    }


    public void ComputeVoronoiDiagram()
    {
        _sites = new List<FortuneSite>();

        GeneratePoints();
        Texture = new Texture2D(Terrain.terrainData.heightmapResolution, Terrain.terrainData.heightmapResolution);
        LlyodAlgorithm();
        DrawPoints();
        Texture.Apply();
        GenerateNoiseLand();
    }

}

//public class BySlope : IComparer<VEdge>
//{
//    public int Compare(VEdge x, VEdge y)
//    {
//        float xslope;
//        if ((float)x.SlopeRun == 0)
//            xslope = float.MaxValue;
//        else
//            xslope = (float)x.SlopeRise / (float)x.SlopeRun;
//        float yslope;
//        if ((float)y.SlopeRun == 0)
//            yslope = float.MaxValue;
//        else
//            yslope = (float)y.SlopeRise / (float)y.SlopeRun;

//        if (xslope > yslope)
//            return 1;
//        else if (xslope < yslope)
//            return -1;
//        else
//            return 0;
//    }
//}
