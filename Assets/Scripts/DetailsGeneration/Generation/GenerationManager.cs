using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.DetailsGeneration;


public class GenerationManager : MonoBehaviour
{

    [SerializeField] private DetailEntity spawnRoot;

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

            if (collider != null)
            {
                collider.enabled = false;
            }


            if (Physics.Raycast(detail.transform.position + new Vector3(0, 1, 0), Vector3.down, out hit, 5, 1<<6))
            {
                detail.transform.position = hit.point;
            }
            
            if (collider != null)
            {
                collider.enabled = true;
            }
        }


    }




}
