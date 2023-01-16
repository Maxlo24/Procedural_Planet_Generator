using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Liquid : MonoBehaviour
{



    [SerializeField] private Material SolidForm;
    [SerializeField] private Material LiquidForm;
    [SerializeField] private Material FusiForm;


    
    [SerializeField] private float SolidFormThreshold = 0f;
    [SerializeField] private float LiquidFormThreshold = 80f;

    private float temperature = 25f;


    public void UpdateTemperature(float temp)
    {
        temperature = temp;

        gameObject.SetActive(true);

        if (temperature < SolidFormThreshold)
        {
            GetComponent<MeshRenderer>().material = SolidForm;
            GetComponent<Collider>().enabled = true;
        }
        else if (temperature < LiquidFormThreshold)
        {
            GetComponent<MeshRenderer>().material = LiquidForm;
            GetComponent<Collider>().enabled = false;
        }
        else
            //GetComponent<MeshRenderer>().material = FusiForm;
            gameObject.SetActive(false);




    }



}
