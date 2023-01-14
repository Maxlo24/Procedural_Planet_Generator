using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetGlobalGeneration : MonoBehaviour
{
    [Range(0, 3)]
    [SerializeField] public int atmosphere = 3;
    [Range(0, 100)]
    [SerializeField] public int humidity = 50;
    [Range(-25, 100)]
    [SerializeField] public float temperature = 25.0f;
    [Range(0, 100)]
    [SerializeField] public int vegetation = 50;

    [SerializeField] public bool clouds = true;
    [SerializeField] public bool raining = true;

    [SerializeField] private Liquid liquid;

    [SerializeField] private CloudManager cloudGenerator;


    public float fogIntensity;



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
        vegetation = Random.Range(0, 101);
        clouds = Random.Range(0, 4) != 0;
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
            temperature = 100;

            raining = false;
            clouds = false;

            humidity = 0;

            RenderSettings.ambientLight = Color.black;
            RenderSettings.ambientIntensity = 0.0f;



        }
        else
        {
            
            RenderSettings.fog = true;
            //RenderSettings.fogColor = new Color(189, 234, 255, 255.0f)/255f;
            RenderSettings.ambientLight = Color.white;

            RenderSettings.ambientIntensity = atmosphere/ 3.0f + 1;


        }


        Material planetMaterial = GetComponent<ColorStyle>().activePlanetMaterial;
        planetMaterial.SetFloat("_Humidity", humidity/100f);
        planetMaterial.SetFloat("_Vegetation", vegetation / 100f);

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

        fogIntensity = humidity * atmosphere / 400f;
        //RenderSettings.fogDensity = humidity / 65000f;

        //Sky.material.SetColor("_Color", new Color(156, 243, 253, 255 / (4 - atmosphere)));

        cloudGenerator.UpdateClouds();
        GetComponent<ColorStyle>().UpdateStyle();



    }
}
