using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.DetailsGeneration;


[CreateAssetMenu(fileName = "NewDetailsCollectionGroup", menuName = "Scriptable objects/Details/DetailsCollectionGroup", order = 0)]
public class DetailsCollectionGroup : ScriptableObject
{
    [Header("Vegetation")]
    public DetailsCollection vegetation;


    [Header("Stone")]
    public DetailsCollection stone;


    [Header("Rock")]
    public DetailsCollection rock;


    
    [Header("Special")]
    public DetailsCollection special;



    public DetailsCollection GetCollectionOfType(DetailsType dt)
    {
        switch (dt)
        {
            case DetailsType.Vegetation:
                return vegetation;
            case DetailsType.Stone:
                return stone;
            case DetailsType.Rock:
                return rock;
            case DetailsType.Special:
                return special;
            default:
                return null;
        }

    }

}
