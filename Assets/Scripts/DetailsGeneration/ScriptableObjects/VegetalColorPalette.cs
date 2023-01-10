using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewVegetalCollorPalette", menuName = "Scriptable objects/ColorPalette/Vegetal", order = 1)]
public class VegetalColorPalette : ScriptableObject
{

    [SerializeField] public Color healthy = new Color(0.5f, 0.5f, 0.5f);
    [SerializeField] public Color dry = new Color(0.5f, 0.5f, 0.5f);

    [SerializeField] public Color wood = new Color(0.5f, 0.5f, 0.5f);

    [SerializeField] public Color[] flowers = new Color[0];

    //public Color GetRandomColor()
    //{
    //    return colors[Random.Range(0, colors.Length)];
    //}

    //public Color GetColor(int index)
    //{
    //    return colors[index];
    //}

}
