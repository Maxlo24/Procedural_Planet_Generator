using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Icosahedron : MonoBehaviour
{
    [Header("Resolution")]
    [Range(0, 8)][SerializeField] private int subdivisions = 1;
    // scale of the icosahedron (default 1)
    [Header("Scale")]
    [Range(1, 10)][SerializeField] private float scale = 1;

    [Header("Noise")]
    public NoiseLayer[] noiseLayers;

    public Mesh sphereMesh;
    public MeshFilter meshFilter;

    public IcosahedronGenerator icosahedron;


    private void Start()
    {

        UpdateMesh();

    }

    public void genMesh()
    {
        int vertexCount = icosahedron.Polygons.Count * 3;
        int[] indices = new int[vertexCount];

        Vector3[] vertices = new Vector3[vertexCount];
        // print size of vertices
        Debug.Log("vertices size: " + vertices.Length);
        Vector3[] normals = new Vector3[vertexCount];

        for (int i = 0; i < icosahedron.Polygons.Count; i++)
        {
            var poly = icosahedron.Polygons[i];

            indices[i * 3 + 0] = i * 3 + 0;
            indices[i * 3 + 1] = i * 3 + 1;
            indices[i * 3 + 2] = i * 3 + 2;

            vertices[i * 3 + 0] = icosahedron.Vertices[poly.vertices[0]];
            vertices[i * 3 + 1] = icosahedron.Vertices[poly.vertices[1]];
            vertices[i * 3 + 2] = icosahedron.Vertices[poly.vertices[2]];

            normals[i * 3 + 0] = icosahedron.Vertices[poly.vertices[0]];
            normals[i * 3 + 1] = icosahedron.Vertices[poly.vertices[1]];
            normals[i * 3 + 2] = icosahedron.Vertices[poly.vertices[2]];
        }
        sphereMesh.vertices = vertices;
        sphereMesh.normals = normals;
        sphereMesh.SetTriangles(indices, 0);
    }
    
    public void UpdateMesh()
    {


        icosahedron = new IcosahedronGenerator(noiseLayers);
        icosahedron.Initialize();
        // créer sphere 


        sphereMesh = new Mesh();
        // change mesh index format
        sphereMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        sphereMesh.name = "IcoSphere";

        //Assigning the mesh to the object
        // if (meshFilter != null) meshFilter = new MeshFilter();
        MeshRenderer meshRenderer = this.gameObject.GetComponent<MeshRenderer>();
        MeshFilter meshFilter = this.gameObject.GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            this.gameObject.AddComponent<MeshFilter>();
        }
        if (meshRenderer == null)
        {
            this.gameObject.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
        }


        icosahedron.Subdivide(subdivisions);


        // for each point in the icosahedron : call the noise filter
        for (int i = 0; i < icosahedron.Vertices.Count; i++)
        {
            icosahedron.Vertices[i] = icosahedron.Vertices[i]*(1+ icosahedron.ComputeNoise(icosahedron.Vertices[i]));
        }

        Shader shader = Shader.Find("Shader Graphs/PlanetShader");
        Material material = new Material(shader);
        material.SetFloat("_Ratio", scale);
        this.gameObject.GetComponent<MeshRenderer>().sharedMaterial = material;
        genMesh();

        icosahedron.Rescale(scale);

        genMesh();
        sphereMesh.RecalculateNormals();
        this.gameObject.GetComponent<MeshFilter>().mesh = sphereMesh;

    }




    IEnumerator Destroy(GameObject go)
    {
        yield return new WaitForEndOfFrame();
        DestroyImmediate(go);
    }

    private void OnValidate()
    {
        /*        if (subdivisions != lastSubdivision || scale != lastScale)
                {
                    lastSubdivision = subdivisions;
                    lastScale = scale;
                    UpdateMesh();
                }*/
        UpdateMesh();

    }

    private void Update()
    {
        /*        if (subdivisions != lastSubdivision || scale != lastScale)
                {
                    lastSubdivision = subdivisions;
                    lastScale = scale;
                    UpdateMesh();
                }*/
        // UpdateMesh();
    }
}