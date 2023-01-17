using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudManager : MonoBehaviour
{


    [SerializeField] private ParticleSystem[] clouds;
    [SerializeField] Material CloudMaterial;


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < clouds.Length; i++)
        {
            clouds[i].Simulate(400f);
            clouds[i].Play();
        }

    }

    // Update is called once per frame
    public void UpdateClouds()
    {


        bool active = GameObject.FindGameObjectWithTag("PlanetAttributes").GetComponent<PlanetGlobalGeneration>().clouds;

        gameObject.SetActive(active);

        if (active)
        {

            for (int i = 0; i < clouds.Length; i++)
            {
                clouds[i].Simulate(400f);
                clouds[i].Play();
            }

        }
    }
}
