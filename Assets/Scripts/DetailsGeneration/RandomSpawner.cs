using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.DetailsGeneration;
public class RandomSpawner : MonoBehaviour
{

    [Header("Details options")]
    [SerializeField] private DetailsType[] _detailsType;
    [SerializeField] private DetailsSize _detailsSize;


    [Header("Randomness")]
    [SerializeField] private bool _randomize = true;

    [Header("Scale")]
    [SerializeField] private Vector3 _scaleRandomness = Vector3.one;


    [Header("Rotation")]
    [Range(0, 1)]
    [SerializeField] private float _randomRotation = 1;
    [Header("Rotation direction")]
    [SerializeField] private Vector3 _rotationRandomness = new Vector3(0, 1, 0);



    private GameObject[] selectionList;
    private GenerationManager generationManager;

    public void Spawn()
    {
        
        generationManager = GameObject.FindGameObjectWithTag("GenerationManager").GetComponent<GenerationManager>();

        selectionList = generationManager.GetDetailsList(_detailsType, _detailsSize);



        int randomIndex = Random.Range(0, selectionList.Length);
        GameObject randomObject = selectionList[randomIndex];
        GameObject spawnedObject = Instantiate(randomObject, transform.position, transform.rotation);

        if (_randomize)
        {
            spawnedObject.transform.localScale = new Vector3(
                spawnedObject.transform.localScale.x * Random.Range(1 / (1 + _scaleRandomness.x), (1 + _scaleRandomness.x)),
                spawnedObject.transform.localScale.y * Random.Range(1 / (1 + _scaleRandomness.y), (1 + _scaleRandomness.y)),
                spawnedObject.transform.localScale.z * Random.Range(1 / (1 + _scaleRandomness.z), (1 + _scaleRandomness.z))
            );

            spawnedObject.transform.Rotate(new Vector3(
                Random.Range(-_randomRotation, _randomRotation) * 180 * _rotationRandomness.x,
                Random.Range(-_randomRotation, _randomRotation) * 180 * _rotationRandomness.y,
                Random.Range(-_randomRotation, _randomRotation) * 180 * _rotationRandomness.z
            ));
        }

        spawnedObject.transform.parent = transform;
        spawnedObject.GetComponent<DetailEntity>().ActivateSpawner();
    }
    private void OnDrawGizmos()
    {

        float radius = 1f;

        if (_detailsSize == DetailsSize.Small)
        {
            radius = 0.5f;
        }
        else if (_detailsSize == DetailsSize.Medium)
        {
            radius = 1f;
        }
        else if (_detailsSize == DetailsSize.Large)
        {
            radius = 2f;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, radius);
    }

}
