using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.DetailsGeneration;


public class GenerationManager : MonoBehaviour
{

    [SerializeField] private DetailEntity spawnRoot;
    [SerializeField] private bool manualSpawn = false;

    [Header("Details collection")]
    public DetailsCollectionGroup detailsCollectionGroup;

    void Start()
    {
        //spawnRoot.ActivateSpawner();
    }
    void Update()
    {

        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    Clear();
        //    Generate();
        //}

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //}

        //ClipToFloor();

    }

    public GameObject[] GetDetailsList(DetailsType[] detailsType, DetailsSize detailsSize)
    {
        List<GameObject> detailsList = new List<GameObject>();

        foreach (DetailsType type in detailsType)
        {

            GameObject[] collectionObjects = detailsCollectionGroup.GetCollectionOfType(type).GetCollectionOfSize(detailsSize);

            foreach (GameObject obj in collectionObjects)
            {
                detailsList.Add(obj);
            }
        }

        return detailsList.ToArray();
    }

    public void Generate()
    {
        Clear();
        Spwan();
        ClipToFloor();

    }

    public void Clear()
    {
        GameObject[] details = GameObject.FindGameObjectsWithTag("Details");
        foreach (var detail in details)
        {
            DestroyImmediate(detail);
        }
    }

    public void Spwan()
    {
        spawnRoot.ActivateSpawner();
    }

    public void ClipToFloor()
    {

        GameObject[] details = GameObject.FindGameObjectsWithTag("Details");

        RaycastHit hit;

        foreach (var detail in details)
        {

            MeshCollider collider = detail.GetComponent<MeshCollider>();

            if (collider == null) continue;



            Bounds bbox = collider.bounds;

            Vector3[] boxCorners = GetBoundingBoxBottomCorners(bbox);

            if (collider != null)
            {
                collider.enabled = false;
            }


            //foreach (Vector3 corner in boxCorners)
            //{
            //    Debug.DrawRay(corner, Vector3.down * 2, Color.red);
            //}
            //Debug.Log(boxCorners[0]);

            float minAltitude = bbox.center.y;

            foreach (var corner in boxCorners)
            {

                Vector3 rayOrigin = corner + Vector3.up * 10;

                //Debug.DrawRay(rayOrigin, Vector3.down * 4, Color.red);

                if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 1000, 1 << 6))
                {
                    if (hit.point.y < minAltitude)
                    {
                        minAltitude = hit.point.y;
                    }
                }
            }

            detail.transform.position = new Vector3(detail.transform.position.x, minAltitude, detail.transform.position.z);


            //Debug.Log(bbox.size);


            //if (Physics.Raycast(detail.transform.position + new Vector3(0, 1, 0), Vector3.down, out hit, 5, 1<<6))
            //{
            //    detail.transform.position = hit.point;
            //}

            if (collider != null)
            {
                collider.enabled = true;
            }
        }


    }


    public void GenerateRocks()
    {
       GetComponent<RocksGenerator>().SpawnRocks();

    }




    public Vector3[] GetBoundingBoxBottomCorners(Bounds bbox)
    {

        Vector3[] corners = new Vector3[4];

        corners[0] = bbox.center + new Vector3(bbox.extents.x, 0, bbox.extents.z);

        corners[1] = bbox.center + new Vector3(bbox.extents.x, 0, -bbox.extents.z);

        corners[2] = bbox.center + new Vector3(-bbox.extents.x, 0, bbox.extents.z);

        corners[3] = bbox.center + new Vector3(-bbox.extents.x, 0, -bbox.extents.z);

        return corners;
    }




}
