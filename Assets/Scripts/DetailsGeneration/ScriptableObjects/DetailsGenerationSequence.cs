using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDetailGenerationSequence", menuName = "Scriptable objects/Details/Generation sequence", order = 0)]
public class DetailsGenerationSequence : ScriptableObject
{

    public DetailGeneration[] detailsGenerations = new DetailGeneration[0];


}
