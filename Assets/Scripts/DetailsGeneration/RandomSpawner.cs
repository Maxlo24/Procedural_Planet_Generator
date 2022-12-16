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


    private void Update()
    {
        Debug.DrawRay(transform.position + new Vector3(0, 5, 0), Vector3.down * 10, Color.red);
    }


    public void Spawn()
    {
        
        generationManager = GameObject.FindGameObjectWithTag("GenerationManager").GetComponent<GenerationManager>();

        if (_detailsType.Length == 0)
        {
            Debug.LogError("No details type");
            return;
        }
        else if (_detailsType[0] == DetailsType.None)
        {
            Debug.LogError("No details type selected");
            return;
        }

        if (_detailsSize == DetailsSize.None)
        {
            Debug.LogError("No details size selected");
            return;
        }


        selectionList = generationManager.GetDetailsList(_detailsType, _detailsSize);

        if (selectionList.Length == 0)
        {
            Debug.LogError("No corresponding details in collection");
            return;
        }


        Vector3 spawnPosition = transform.position;


        RaycastHit hit;

        if (Physics.Raycast(spawnPosition + new Vector3(0, 5, 0), Vector3.down, out hit, 10))
        {
            spawnPosition = hit.point;
        }



        int randomIndex = Random.Range(0, selectionList.Length);
        GameObject randomObject = selectionList[randomIndex];
        GameObject spawnedObject = Instantiate(randomObject, spawnPosition, transform.rotation);

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
        else if (_detailsSize == DetailsSize.Large)
        {
            radius = 2f;
        }


        switch (_detailsType[0])
        {
            case DetailsType.Vegetation:
                Gizmos.color = Color.green;
                break;
            case DetailsType.Rock:
                Gizmos.color = Color.black;
                break;
            case DetailsType.Stone:
                Gizmos.color = Color.gray;
                radius /= 2;
                break;
            case DetailsType.Rampe:
                Gizmos.color = Color.red;
                break;
            case DetailsType.Special:
                Gizmos.color = Color.blue;
                break;


        }
        
        Gizmos.DrawSphere(transform.position, radius);

    }

}
