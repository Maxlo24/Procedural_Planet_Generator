using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewCrustCollorPalette", menuName = "Scriptable objects/ColorPalette/Crust", order = 0)]
public class CrustColorPalette : ScriptableObject
{

    [SerializeField] public Color main = new Color(0.5f, 0.5f, 0.5f);
    [SerializeField] public Color sediment = new Color(0.5f, 0.5f, 0.5f);
    [SerializeField] public Color dirt = new Color(0.5f, 0.5f, 0.5f);
    [SerializeField] public Color sand = new Color(0.5f, 0.5f, 0.5f);

    [SerializeField] public VegetalColorPalette[] CompatibleVegetation = new VegetalColorPalette[0];

    [SerializeField] public Material materialPreset = null;

    [SerializeField] public Color[] AtmospherColors = new Color[0];

    //public Color GetRandomColor()
    //{
    //    return colors[Random.Range(0, colors.Length)];
    //}

    //public Color GetColor(int index)
    //{
    //    return colors[index];
    //}


}
