using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using StarterAssets;

public class GlobalManager : MonoBehaviour
{


    [Header("Solar system")]
    public GameObject solarSystem;
    public PlanetGenerator planetGenerator;

    [Header("Map")]
    public GameObject map;
    public GameObject mapCamera;


    [Header("Exploration")]
    public GameObject player;
    public StarterAssetsInputs starterAssetsInputs;

    //public GameObject Loading;


    [Header("Generation")]
    public PlanetGlobalGeneration planetGlobalGeneration;
    public ColorStyle colorStyle;
    public GenerationManager generationManager;



    private int menuState = 0;


    public int seed;

    private int previousSeed;


    [Header("UI")]
    public GameObject mainMenu;
    public GameObject goBackMenu;
    public GameObject loadingMenu;
    public TextMeshProUGUI atmosphereText;
    public TextMeshProUGUI temperatureText;
    public TextMeshProUGUI humidityText;
    public TextMeshProUGUI vegetationText;
    public TextMeshProUGUI seedText;



    void Start()
    {
        menuState = 0;
        SetView();
        Next();
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            GoBack();
        }
    }

    public void Explore()
    {
        menuState++;

        SetView();
        Generate();

    }


    public void GoBack()
    {
        menuState--;
        if (menuState < 0) menuState = 0;

        SetView();

    }


    public void Next()
    {
        previousSeed = seed;

        seed = Random.Range(0, 1000000);

        Random.InitState(seed);
        Generate();
            
    }


    public void Previous()
    {
        seed = previousSeed;

        Random.InitState(seed);
        Generate();
    }

    public void Generate()
    {

        seedText.text = "Seed : " + seed.ToString();


        if (menuState == 0)
        {
            planetGlobalGeneration.GenerateRandomAttributes();

            atmosphereText.text = "Atmosphere : " + (planetGlobalGeneration.atmosphere*100/3).ToString() + "%";
            temperatureText.text = "Temperature : " + planetGlobalGeneration.temperature.ToString() + "°C";
            humidityText.text = "Humidity : " + planetGlobalGeneration.humidity.ToString() + "%";
            vegetationText.text = "Vegetation : " + planetGlobalGeneration.vegetation.ToString() + "%";
            planetGenerator.GeneratePlanet();

            




        }

        if (menuState == 1)
        {
            StartCoroutine(Generation());

        }


    }



    private void SetView()
    {
        switch (menuState)
        {
            case 0:
                solarSystem.SetActive(true);
                mainMenu.SetActive(true);
                goBackMenu.SetActive(false);
                map.SetActive(false);
                colorStyle.fog_ratio = 0;
                colorStyle.grass = false;
                // colorStyle.UpdateStyle();

                starterAssetsInputs.cursorLocked = false;


                break;
            case 1:
                goBackMenu.SetActive(true);
                colorStyle.fog_ratio = 1.5f;


                solarSystem.SetActive(false);
                map.SetActive(true);
                player.SetActive(false);
                mapCamera.SetActive(true);

                
                mainMenu.SetActive(true);

                starterAssetsInputs.cursorLocked = false;


                break;
            case 2:
                mapCamera.SetActive(false);
                
                mainMenu.SetActive(false);
                player.SetActive(true);

                starterAssetsInputs.cursorLocked = true;


                RaycastHit hit;
                if (Physics.Raycast(new Vector3(0,800,0), Vector3.down, out hit, 1000, 1 << 6))
                {
                    player.transform.position = hit.point;
                }



                break;
        }

        Debug.Log(menuState);




    }

    IEnumerator Generation()
    {
        loadingMenu.SetActive(true);
        mainMenu.SetActive(false);
        goBackMenu.SetActive(false);


        yield return (0.1f);

        //planetGlobalGeneration.GeneratePlanet();
        colorStyle.grass = true;
        // colorStyle.UpdateStyle();

        generationManager.Generate();


        goBackMenu.SetActive(true);
        mainMenu.SetActive(true);
        loadingMenu.SetActive(false);
    }
    


}
