using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Icosahedron : MonoBehaviour
{
    [Header("Resolution")]
    [Range(0, 5)][SerializeField] private int subdivisions = 1;
    // scale of the icosahedron (default 1)
    [Header("Scale")]
    [Range(1,10)][SerializeField] private int scale = 1;

    [Header("Noise")]
    [SerializeField] private float noiseScale = 1;

    [Header("Graphics")]
    [SerializeField] private Shader shader;

    private GameObject sphereMesh;
    protected MeshFilter meshFilter;

    private IcosahedronGenerator icosahedron;

    private int lastSubdivision = int.MinValue;
    private int lastScale = int.MinValue;


    private void Start()
    {
        // créer sphere 
        print("Start");
        lastSubdivision = subdivisions;
        lastScale = scale;
        this.name = "IcoSphere";
        icosahedron = new IcosahedronGenerator();
        icosahedron.Initialize();
        icosahedron.Subdivide(subdivisions);
        this.sphereMesh = new GameObject("Sphere Mesh");
        this.sphereMesh.transform.parent = this.transform;
        Material newMat = Resources.Load("Green", typeof(Material)) as Material;
        // assign material to sphere
        this.sphereMesh.AddComponent<MeshFilter>();
        UpdateMesh();

    }
    public void UpdateMesh()
    {

        if (this.sphereMesh)
        {
            // print "destroying old mesh"
            print("destroying old mesh");
            // destroy current meshs
            

            StartCoroutine(Destroy(this.sphereMesh));
        }


        icosahedron.Rescale(scale); 
        // rescale the icosahedron

        // for each point in the icosahedron : call the noise filter
        for (int i = 0; i < icosahedron.Vertices.Count; i++)
        {
            icosahedron.Vertices[i] = icosahedron.Vertices[i]*(1+ noiseScale * icosahedron.ComputeNoise(icosahedron.Vertices[i]));
        }



        MeshRenderer surfaceRenderer = this.sphereMesh.AddComponent<MeshRenderer>();
        surfaceRenderer.sharedMaterial = new Material(shader);

        Mesh sphereMesh = new Mesh();

        int vertexCount = icosahedron.Polygons.Count * 3;
        int[] indices = new int[vertexCount];

        Vector3[] vertices = new Vector3[vertexCount];
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

        MeshFilter terrainFilter = this.sphereMesh.AddComponent<MeshFilter>();
        terrainFilter.sharedMesh = sphereMesh;
    }




    IEnumerator Destroy(GameObject go)
    {
        yield return new WaitForEndOfFrame();
        DestroyImmediate(go);
    }

    private void OnValidate()
    {
        Debug.Log("OnValidate");
        if (subdivisions != lastSubdivision || scale != lastScale)
        {
            lastSubdivision = subdivisions;
            lastScale = scale;
            UpdateMesh();
        }

    }

    private void Update()
    {
        if (subdivisions != lastSubdivision || scale != lastScale)
        {
            lastSubdivision = subdivisions;
            lastScale = scale;
            UpdateMesh();
        }
        
    }
}