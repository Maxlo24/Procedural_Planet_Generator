using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]

public class ColorStyle : MonoBehaviour
{

    [Range(0, 3)]
    [SerializeField] public int activeCrustPalette = 0;
    [Range(0, 3)]
    [SerializeField] public int activeVegetationPalette = 0;
    [Range(0, 3)]
    [SerializeField] public int activeSkybox = 0;
    [Range(0, 3)]
    [SerializeField] public int activeAtmosphere = 0;

    [HideInInspector] public Material activePlanetMaterial = null;


    [SerializeField] private CrustColorPalette[] crustColorPalettes = new CrustColorPalette[0];
    [SerializeField] private VegetalColorPalette[] vegetalColorPalettes = new VegetalColorPalette[0];
    [SerializeField] private Material[] skyboxes;


    [SerializeField] private bool liveUpdate = false;


    public GrassGenerator grassGenerator;
    public float fog_ratio;


    private void Update()
    {
        if (liveUpdate) UpdateStyle();
    }

    private void OnValidate()
    {
        UpdateStyle();
    }


    public void SetRandomPalette()
    {
        activeCrustPalette = Random.Range(0, crustColorPalettes.Length);
        vegetalColorPalettes = crustColorPalettes[activeCrustPalette].CompatibleVegetation;

        activeVegetationPalette = Random.Range(0, vegetalColorPalettes.Length);

        activeSkybox = Random.Range(0, skyboxes.Length);
        activeAtmosphere = Random.Range(0, crustColorPalettes[activeCrustPalette].AtmospherColors.Length);

        RenderSettings.skybox.SetFloat("_Rotation", Random.Range(0, 360));



        UpdateStyle();

    }


    public void UpdateStyle()
    {
        if (activeCrustPalette >= crustColorPalettes.Length) activeCrustPalette = crustColorPalettes.Length - 1;

        CrustColorPalette activeCrust = crustColorPalettes[this.activeCrustPalette];


        activePlanetMaterial = activeCrust.materialPreset;

        activePlanetMaterial.SetColor("_Rock_color", activeCrust.main);
        activePlanetMaterial.SetColor("_Sediment_color", activeCrust.sediment);
        activePlanetMaterial.SetColor("_Dirt_color", activeCrust.dirt);
        activePlanetMaterial.SetColor("_Beach_color", activeCrust.sand);

        vegetalColorPalettes = activeCrust.CompatibleVegetation;
        //activeVegetationPalette = 0;

        if (activeVegetationPalette >= vegetalColorPalettes.Length) activeVegetationPalette = 0;


        VegetalColorPalette activeVegetation = vegetalColorPalettes[this.activeVegetationPalette];

        activePlanetMaterial.SetColor("_Healthy_grass", activeVegetation.healthy);
        activePlanetMaterial.SetColor("_Dry_grass", activeVegetation.dry);


        Material skybox = skyboxes[activeSkybox];
        
        float fogIntensity = GetComponent<PlanetGlobalGeneration>().fogIntensity;
        skybox.SetFloat("_FogIntensity", fogIntensity* fog_ratio);

        RenderSettings.skybox = skybox;

        if (activeAtmosphere >= activeCrust.AtmospherColors.Length) activeAtmosphere = activeCrust.AtmospherColors.Length - 1;

        RenderSettings.fogColor = activeCrust.AtmospherColors[activeAtmosphere];

        grassGenerator.UpdateGrassPrototypes();

    }

    public Vector3 GetGrassColor()
    {
        VegetalColorPalette activeVegetation = vegetalColorPalettes[this.activeVegetationPalette];

        Vector3 color = new Vector3(activeVegetation.healthy.r, activeVegetation.healthy.g, activeVegetation.healthy.b);
        //color /= 255f;
        //Debug.Log("Color: " + color);

        return color;
    }

}
