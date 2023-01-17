using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

public class PlanetGenerator : MonoBehaviour
{
    public GameObject planetPrefab;
    public GameObject planetAtmosphere;
    public GameObject planetSea;
    public PlanetGlobalGeneration planetGlobalGeneration;
    public bool sea = true;
    

    // Start is called before the first frame update
    void Start()
    {

        GeneratePlanet();
    }


    private void OnValidate()
    {
        GeneratePlanet();
    }

    public void GeneratePlanet()
    {


        Material prefab_material = planetPrefab.GetComponent<MeshRenderer>().material;
        Material atmosphere_material = planetAtmosphere.GetComponent<MeshRenderer>().material;
        Material water_material = planetSea.GetComponent<MeshRenderer>().material;

        float snow_level = planetGlobalGeneration.snowLevel;



        float atmosphere = planetGlobalGeneration.atmosphere / 3.0f;

        int humidity = planetGlobalGeneration.humidity;
        float temperature = planetGlobalGeneration.temperature;
        int vegetation = planetGlobalGeneration.vegetation;

        // if atmosphere is null : print error
        if (planetAtmosphere == null)
        {
            Debug.LogError("atmosphere is null");
        }

        Icosahedron icosahedron_planet = planetPrefab.GetComponent<Icosahedron>();



        // get float "Opacity" of atmosphere_material 
        float opacity = atmosphere_material.GetFloat("_Opacity");


        // change "opacity" value of atmosphere material
        atmosphere_material.SetFloat("_Opacity", atmosphere);
        if (sea)
            water_material.SetFloat("_Transparency", 0);
        else
            water_material.SetFloat("_Transparency", .95f);



        // icosahedron_planet.GetComponent<MeshRenderer>().sharedMaterial = prefab_material;
        prefab_material.SetFloat("_Humidity", humidity / 100.0f);
        prefab_material.SetFloat("_Temperature", temperature / 100.0f);
        prefab_material.SetFloat("_Vegetation", vegetation / 100.0f);
        prefab_material.SetFloat("_Snow_level", snow_level/100);
    }

    public void UpdateColors(CrustColorPalette activeCrust, VegetalColorPalette activeVegetation)
    {
        Material prefab_material = planetPrefab.GetComponent<MeshRenderer>().material;
        prefab_material.SetColor("_Rock_color", activeCrust.main);
        prefab_material.SetColor("_Sediment_color", activeCrust.sediment);
        prefab_material.SetColor("_Dirt_color", activeCrust.dirt);
        prefab_material.SetColor("_Beach_color", activeCrust.sand);

        prefab_material.SetColor("_Healthy_grass", activeVegetation.healthy);
        prefab_material.SetColor("_Dry_grass", activeVegetation.dry);
    }


}


