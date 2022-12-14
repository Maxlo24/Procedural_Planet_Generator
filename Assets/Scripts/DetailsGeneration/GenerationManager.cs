using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationManager : MonoBehaviour
{

    [SerializeField] private DetailEntity spawnRoot;


    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.R))
        {
            GameObject[] details = GameObject.FindGameObjectsWithTag("Details");
            foreach (var detail in details)
            {
                Destroy(detail);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            spawnRoot.ActivateSpawner();
        }

    }
}
