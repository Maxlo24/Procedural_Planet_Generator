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

        if (Input.GetKeyDown(KeyCode.R))
        {
            Clear();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Generate();
        }

    }

    public GameObject[] GetDetailsList(DetailsType[] detailsType, DetailsSize detailsSize)
    {
        List<GameObject> detailsList = new List<GameObject>();

        foreach (var type in detailsType)
        {

            GameObject[] collectionObjects = detailsCollectionGroup.GetCollectionOfType(type).GetCollectionOfSize(detailsSize);

            foreach (var obj in collectionObjects)
            {
                detailsList.Add(obj);
            }
        }

        return detailsList.ToArray();
    }

    public void Generate()
    {
        Clear();
        spawnRoot.ActivateSpawner();
        
    }

    public void Clear()
    {
        GameObject[] details = GameObject.FindGameObjectsWithTag("Details");
        foreach (var detail in details)
        {
            DestroyImmediate(detail);
        }
    }




}
