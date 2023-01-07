/*using System.Collections.Generic;
using System.Linq;
using UnityEngine;




public class PlanetCreator : MonoBehaviour
{

    Vector3[] vertices;
    MeshFilter planetMeshFilter;
    int[] triangles;
    
    float radius = 1f;
    public static int nbPoints = 100;

    public Shader shader;
    
    private float phi = Mathf.PI * (3 - Mathf.Sqrt(5));




    public void Start()
    {
      
        CreatePlanet();

    }



    public void CreatePlanet()
    {
        Mesh mesh = new Mesh();
        vertices = new Vector3[nbPoints];
        for (int i = 0; i < nbPoints; i++)

        {
            float y = 1 - (i / (nbPoints - 1)) * 2;  // y goes from 1 to -1
            float rad_y = Mathf.Sqrt(1 - y * y);  //  at y
            float theta = phi * i;
            float x = Mathf.Cos(theta) * rad_y;
            float z = Mathf.Sin(theta) * rad_y;
            Vector3 vertex = new Vector3(x, y, z);
            // log the vertex
            vertices[i] = vertex;
        }
        mesh.triangles = ComputeTriangles(mesh.vertices);


        if (planetMeshFilter != null) planetMeshFilter = new MeshFilter();
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            this.gameObject.AddComponent<MeshFilter>();
        }
        if (meshRenderer == null)
        {
            this.gameObject.AddComponent<MeshRenderer>().sharedMaterial = new Material(shader);
        }


        meshFilter.mesh = mesh;

    }

    public int[] ComputeTriangles(Vector3[] vertices)
    {

        //Instantiate a list that will be used to store the triangle triplets
        List<int> triangleList = new List<int>();

        //Calculate an "average" distance between points (I used the first two points for this purpose)
        float averageDistance = Vector3.Distance(vertices[0], vertices[1]);

        //Loop through all vertices and for each find two other points that are at least 
        //1.2 * averageDistance away from that specific vertex.

        for (int i = 0; i < vertices.Length; i++)
        {
            int idx2 = 0, idx3 = 0, cnt = 0;

            if (Mathf.Abs(vertices[i].y) < (1 / radius)) cnt++;

            List<int> closeVertices = new List<int>();
            for (int j = 0; j < vertices.Length; j++)
            {
                if (i != j)
                {
                    float dist = Vector3.Distance(vertices[i], vertices[j]);
                    if (cnt > 0)
                    {
                        if (dist <= 1.7F * averageDistance)
                        {
                            closeVertices.Add(j);
                        }
                    }
                    else
                    {
                        if (dist <= 1.2F * averageDistance)
                        {
                            closeVertices.Add(j);
                        }
                    }

                }
            }

            cnt = 0;
            //if(closeVertices.Count % 2 != 0) closeVertices.RemoveAt(closeVertices.Count-1);

            for (int k = 1; k < closeVertices.Count; k++)
            {
                idx2 = closeVertices.ElementAt(k - 1);
                idx3 = closeVertices.ElementAt(k);
                AddTrianglesInClockwiseDirection(triangleList, vertices, i, idx2, idx3);
            }

            //After 3 vertices have been found, add them in the triangle list in clockwise direction
            //AddTrianglesInClockwiseDirection(triangleList, vertices, i, idx2, idx3);
        }

        return triangleList.ToArray();
    }

    public void AddTrianglesInClockwiseDirection(List<int> triangleList, Vector3[] vertices, int index1, int index2, int index3)
    {

        //Checking for all 6 possible edge combinations in a set of 3 vertices.

        //First permutation
        if (IsClockwise(vertices, index1, index2, index3))
        {
            triangleList.Add(index1);
            triangleList.Add(index2);
            triangleList.Add(index3);
            return;
        }

        //Second permutation
        if (IsClockwise(vertices, index1, index3, index2))
        {
            triangleList.Add(index1);
            triangleList.Add(index3);
            triangleList.Add(index2);
            return;
        }

        //Third permutation
        if (IsClockwise(vertices, index3, index1, index2))
        {
            triangleList.Add(index3);
            triangleList.Add(index1);
            triangleList.Add(index2);
            return;
        }

        //Fourth permutation
        if (IsClockwise(vertices, index3, index2, index1))
        {
            triangleList.Add(index3);
            triangleList.Add(index2);
            triangleList.Add(index1);
            return;
        }

        //Fifth permutation
        if (IsClockwise(vertices, index2, index1, index3))
        {
            triangleList.Add(index2);
            triangleList.Add(index1);
            triangleList.Add(index3);
            return;
        }

        //Sixth permutation
        if (IsClockwise(vertices, index2, index3, index1))
        {
            triangleList.Add(index2);
            triangleList.Add(index3);
            triangleList.Add(index1);
            return;
        }
    }

    public bool IsClockwise(Vector3[] vertices, int index1, int index2, int index3)
    {

        int cnt = 0;

        //Use only the x and z coordinates to "project" the vertices in a 2D plane
        Vector2 p1 = new Vector2(vertices[index1].x, vertices[index1].z);
        Vector2 p2 = new Vector2(vertices[index2].x, vertices[index2].z);
        Vector2 p3 = new Vector2(vertices[index3].x, vertices[index3].z);

        float determinant = p1.x * p2.y + p3.x * p1.y + p2.x * p3.y - p1.x * p3.y - p3.x * p2.y - p2.x * p1.y;

        //If any of the vertices is lower than the origin point, increment the counter
        if (vertices[index1].y < 0) cnt++;
        if (vertices[index2].y < 0) cnt++;
        if (vertices[index3].y < 0) cnt++;

        //If at least 2 vertices are lower than the origin point, change the condition for determining
        //clockwise direction
        if (cnt > 1) return determinant > 0F;
        return determinant < 0F;
    }

}
*/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlanetCreator : MonoBehaviour
{

    public int resolution, radius;
    public Shader shader;

    private float goldenRatio = (1 + Mathf.Pow(5, 0.5F)) / 2;
    Vector3[] vertices;
    MeshFilter sphereMeshFilter;
    int[] triangles;

    void OnValidate()
    {
        createCircle();
    }

    public void createCircle()
    {
        Mesh mesh = new Mesh();
        vertices = new Vector3[resolution];

        //Calculate Vertex Positions
        for (int i = 0; i < resolution; i++)
        {
            float theta = 2 * Mathf.PI * i / goldenRatio;
            float phi = Mathf.Acos((float)(1 - 2 * (i + 0.5) / resolution));
            Vector3 pointOnSphere = (new Vector3(Mathf.Cos(theta) * Mathf.Sin(phi),
            Mathf.Sin(theta) * Mathf.Sin(phi), Mathf.Cos(phi)) * radius);
            vertices[i] = pointOnSphere;
        }

        //Setting the vertices and the triangles to the mesh
        mesh.vertices = vertices;
        mesh.triangles = CalculateTriangles(vertices);
        //mesh.RecalculateNormals();
        mesh.normals = vertices;

        //Assigning the mesh to the object
        if (sphereMeshFilter != null) sphereMeshFilter = new MeshFilter();
        MeshRenderer meshRenderer = this.gameObject.GetComponent<MeshRenderer>();
        MeshFilter meshFilter = this.gameObject.GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            this.gameObject.AddComponent<MeshFilter>();
        }
        if (meshRenderer == null)
        {
            this.gameObject.AddComponent<MeshRenderer>().sharedMaterial = new Material(shader);
        }
        meshFilter.mesh = mesh;

    }

    //Calculate the triangles for a set of vertices
    public int[] CalculateTriangles(Vector3[] vertices)
    {

        //Instantiate a list that will be used to store the triangle triplets
        List<int> triangleList = new List<int>();

        //Calculate an "average" distance between points (I used the first two points for this purpose)
        float averageDistance = Vector3.Distance(vertices[0], vertices[1]);

        //Loop through all vertices and for each find two other points that are at least 
        //1.2 * averageDistance away from that specific vertex.

        for (int i = 0; i < vertices.Length; i++)
        {
            int idx2 = 0, idx3 = 0, cnt = 0;

            if (Mathf.Abs(vertices[i].y) < (1 / radius)) cnt++;

            List<int> closeVertices = new List<int>();
            for (int j = 0; j < vertices.Length; j++)
            {
                if (i != j)
                {
                    float dist = Vector3.Distance(vertices[i], vertices[j]);
                    if (cnt > 0)
                    {
                        if (dist <= 1.7F * averageDistance)
                        {
                            closeVertices.Add(j);
                        }
                    }
                    else
                    {
                        if (dist <= 1.2F * averageDistance)
                        {
                            closeVertices.Add(j);
                        }
                    }

                }
            }

            cnt = 0;
            //if(closeVertices.Count % 2 != 0) closeVertices.RemoveAt(closeVertices.Count-1);

            for (int k = 1; k < closeVertices.Count; k++)
            {
                idx2 = closeVertices.ElementAt(k - 1);
                idx3 = closeVertices.ElementAt(k);
                AddTrianglesInClockwiseDirection(triangleList, vertices, i, idx2, idx3);
            }

            //After 3 vertices have been found, add them in the triangle list in clockwise direction
            //AddTrianglesInClockwiseDirection(triangleList, vertices, i, idx2, idx3);
        }

        return triangleList.ToArray();
    }

    public void AddTrianglesInClockwiseDirection(List<int> triangleList, Vector3[] vertices, int index1, int index2, int index3)
    {

        //Checking for all 6 possible edge combinations in a set of 3 vertices.

        //First permutation
        if (IsClockwise(vertices, index1, index2, index3))
        {
            triangleList.Add(index1);
            triangleList.Add(index2);
            triangleList.Add(index3);
            return;
        }

        //Second permutation
        if (IsClockwise(vertices, index1, index3, index2))
        {
            triangleList.Add(index1);
            triangleList.Add(index3);
            triangleList.Add(index2);
            return;
        }

        //Third permutation
        if (IsClockwise(vertices, index3, index1, index2))
        {
            triangleList.Add(index3);
            triangleList.Add(index1);
            triangleList.Add(index2);
            return;
        }

        //Fourth permutation
        if (IsClockwise(vertices, index3, index2, index1))
        {
            triangleList.Add(index3);
            triangleList.Add(index2);
            triangleList.Add(index1);
            return;
        }

        //Fifth permutation
        if (IsClockwise(vertices, index2, index1, index3))
        {
            triangleList.Add(index2);
            triangleList.Add(index1);
            triangleList.Add(index3);
            return;
        }

        //Sixth permutation
        if (IsClockwise(vertices, index2, index3, index1))
        {
            triangleList.Add(index2);
            triangleList.Add(index3);
            triangleList.Add(index1);
            return;
        }
    }

    public bool IsClockwise(Vector3[] vertices, int index1, int index2, int index3)
    {

        int cnt = 0;

        //Use only the x and z coordinates to "project" the vertices in a 2D plane
        Vector2 p1 = new Vector2(vertices[index1].x, vertices[index1].z);
        Vector2 p2 = new Vector2(vertices[index2].x, vertices[index2].z);
        Vector2 p3 = new Vector2(vertices[index3].x, vertices[index3].z);

        float determinant = p1.x * p2.y + p3.x * p1.y + p2.x * p3.y - p1.x * p3.y - p3.x * p2.y - p2.x * p1.y;

        //If any of the vertices is lower than the origin point, increment the counter
        if (vertices[index1].y < 0) cnt++;
        if (vertices[index2].y < 0) cnt++;
        if (vertices[index3].y < 0) cnt++;

        //If at least 2 vertices are lower than the origin point, change the condition for determining
        //clockwise direction
        if (cnt > 1) return determinant > 0F;
        return determinant < 0F;
    }
    private void OnDrawGizmos()
    {
        if (vertices != null)
        {
            for (int i = 0; i < resolution; i++)
            {
                Gizmos.DrawSphere(vertices[i], 0.03F);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        createCircle();
    }

    // Update is called once per frame
    void Update()
    {

    }
}