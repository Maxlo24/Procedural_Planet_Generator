using System.Collections;
using System.Collections.Generic;

public class Polygon
{
    public readonly List<int> vertices;

    public Polygon(int a, int b, int c)
    {
        vertices = new List<int>() { a, b, c };
    }
}
