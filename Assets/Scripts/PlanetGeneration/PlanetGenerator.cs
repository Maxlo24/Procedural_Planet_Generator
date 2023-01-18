using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class PlanetGenerator : MonoBehaviour
{
    public GameObject planetPrefab;
    public GameObject planetAtmosphere;
    public GameObject planetSea;

    public Material prefab_material;
    public Material atmosphere_material;
    public Material water_material;

    public PlanetGlobalGeneration planetGlobalGeneration;
    public ColorStyle colorStyle;


    // Start is called before the first frame update
    void Start()
    {

        GeneratePlanet();
    }


    private void OnValidate()
    {
        // GeneratePlanet();
    }

    public void GeneratePlanet()
    {
        Icosahedron icosahedron_planet = planetPrefab.GetComponent<Icosahedron>();
        icosahedron_planet.center = new Vector3(Random.Range(-1000, 1000), Random.Range(-1000, 1000), Random.Range(-1000, 1000));
        icosahedron_planet.UpdateMesh();
        planetPrefab.transform.localScale = Vector3.one * (Random.Range(0,100)/100f + 0.5f);



    }

    public void UpdateColors(CrustColorPalette activeCrust, VegetalColorPalette activeVegetation)
    {

        

        Icosahedron icosahedron_planet = planetPrefab.GetComponent<Icosahedron>();

        //prefab_material = planetPrefab.GetComponent<MeshRenderer>().material;
        //Material atmosphere_material = planetAtmosphere.GetComponent<MeshRenderer>().material;
        //Material water_material = planetSea.GetComponent<MeshRenderer>().material;


        float snow_level = planetGlobalGeneration.snowLevel;

        float atmosphere = planetGlobalGeneration.atmosphere / 3.0f;

        int humidity = planetGlobalGeneration.humidity;
        float temperature = planetGlobalGeneration.temperature;
        int vegetation = planetGlobalGeneration.vegetation;



        if (temperature > 90)
        {
            water_material.SetFloat("_Transparency", 1.5f);
            prefab_material.SetFloat("_Ratio", 1f);
        }
        else
        {
            water_material.SetFloat("_Transparency", 0.5f);
            prefab_material.SetFloat("_Ratio", icosahedron_planet.scale);
        }

        // change "opacity" value of atmosphere material
        atmosphere_material.SetFloat("_Opacity", atmosphere);
        atmosphere_material.SetColor("_Secondary_color", colorStyle.AtmosphereColor);

        // icosahedron_planet.GetComponent<MeshRenderer>().sharedMaterial = prefab_material;
        prefab_material.SetFloat("_Humidity", humidity / 100.0f);
        prefab_material.SetFloat("_Temperature", temperature / 100.0f);
        prefab_material.SetFloat("_Vegetation", vegetation / 100.0f);
        prefab_material.SetFloat("_Snow_level", snow_level / 100);
        prefab_material.SetColor("_Rock_color", activeCrust.main);
        prefab_material.SetColor("_Sediment_color", activeCrust.sediment);
        prefab_material.SetColor("_Dirt_color", activeCrust.dirt);
        prefab_material.SetColor("_Beach_color", activeCrust.sand);

        prefab_material.SetColor("_Healthy_grass", activeVegetation.healthy);
        prefab_material.SetColor("_Dry_grass", activeVegetation.dry);
    }


}


