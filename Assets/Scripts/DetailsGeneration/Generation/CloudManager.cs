using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudManager : MonoBehaviour
{


    [SerializeField] private ParticleSystem[] clouds;


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
    void Update()
    {
        
    }
}
