using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetGlobalGeneration : MonoBehaviour
{
    [Range(0, 3)]
    [SerializeField] private int atmosphere = 3;
    [Range(0, 100)]
    [SerializeField] private int humidity = 50;
    [Range(-25, 100)]
    [SerializeField] private float temperature = 25.0f;

    [SerializeField] private bool clouds = true;
    [SerializeField] private bool raining = true;

    [SerializeField] private Liquid liquid;
    [SerializeField] private MeshRenderer Sky;



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnValidate()
    {
        UpdateAttributes();
    }


    public void GenerateRandomAttributes()
    {
        atmosphere = Random.Range(0, 4);
        humidity = Random.Range(0, 101);
        temperature = Random.Range(-25, 101);
        //clouds = Random.Range(0, 2) == 1;
        raining = Random.Range(0, 2) == 1;
        

        
        UpdateAttributes();

        GetComponent< ColorStyle > ().SetRandomPalette();

    }

    public void UpdateAttributes()
    {
        //Debug.Log("UpdateAttributes");
        //Debug.Log("atmosphere: " + atmosphere);
        //Debug.Log("humidity: " + humidity);
        //Debug.Log("temperature: " + temperature);
        //Debug.Log("clouds: " + clouds);


        if (atmosphere == 0)
        {
            RenderSettings.fog = false;

            int r = Random.Range(0, 2);
            //if (r == 0) temperature = -25;
            //else temperature = 100;
            //temperature = 100;

            //raining = false;

            humidity = 0;
            Sky.material.SetFloat("_Alpha", 0);

        }
        else
        {
            RenderSettings.fog = true;
            Sky.material.SetFloat("_Alpha", atmosphere / 3f);

        }


        Material planetMaterial = GetComponent<ColorStyle>().activePlanetMaterial;
        planetMaterial.SetFloat("_Humidity", humidity/100f);

        planetMaterial.SetFloat("_Temperature", temperature);

        if (raining)
        {
            planetMaterial.SetFloat("_Snow_level", temperature * 5f);
        }
        else
        {
            planetMaterial.SetFloat("_Snow_level", 400);
        }

        if (liquid != null)
        {
            liquid.UpdateTemperature(temperature);
        }

        RenderSettings.fogDensity = humidity * atmosphere/ 150000f;
        //RenderSettings.fogDensity = humidity / 65000f;

        //Sky.material.SetColor("_Color", new Color(156, 243, 253, 255 / (4 - atmosphere)));

   

    }
}
