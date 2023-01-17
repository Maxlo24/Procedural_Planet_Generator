using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.DetailsGeneration;


[CreateAssetMenu(fileName = "NewDetailGeneration", menuName = "Scriptable objects/Details/Generation", order = 0)]
public class DetailGeneration : ScriptableObject
{

    public DetailsType type;
    public DetailsSize size;

    [Range(0, 100)]
    public int density = 0;

    [Range(1, 50)]
    public int numberOfIterations = 1;

    public Vector2 sizeRange = new Vector2(0.5f, 2);


}
