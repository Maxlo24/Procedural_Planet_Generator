using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.DetailsGeneration;
public class RandomSpawner : MonoBehaviour
{
    [Header("Details options")]
    [SerializeField] private DetailsType _detailsType;
    [SerializeField] private DetailsSize _detailsSize;


    public GameObject[] selectionList;



    public void Spawn()
    {
        int randomIndex = Random.Range(0, selectionList.Length);
        GameObject randomObject = selectionList[randomIndex];
        GameObject spawnedObject = Instantiate(randomObject, transform.position, transform.rotation);
        spawnedObject.transform.parent = transform;
        spawnedObject.transform.localScale = transform.localScale;
    }

}
