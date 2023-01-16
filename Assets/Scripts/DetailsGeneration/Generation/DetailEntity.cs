using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetailEntity : MonoBehaviour
{


    [SerializeField] private float visibleRadius = 100f;

    void Start()
    {
        Optimizer optimizer = GameObject.FindGameObjectWithTag("GenerationManager").GetComponent<Optimizer>();
        optimizer.objectsToOptimize.Add(new ItemActivator { gObject = gameObject, position = transform.position, activeRadius = visibleRadius });
    }

    
    public void ActivateSpawner()
    {
        GameObject[] children = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            children[i] = transform.GetChild(i).gameObject;
        }

        foreach (var child in children)
        {
            if (child.tag == "DetailSpawner")
            {
                child.GetComponent<RandomSpawner>().Spawn();
            }
        }

        //gameObject.GetComponent<MeshCollider>().enabled = true;
    }
}
