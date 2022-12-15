using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.DetailsGeneration;


[CreateAssetMenu(fileName = "NewDetailsCollectionGroup", menuName = "Scriptable objects/DetailsCollectionGroup", order = 0)]
public class DetailsCollectionGroup : ScriptableObject
{
    [Header("Grass")]
    public DetailsCollection grass;

    [Header("Flower")]
    public DetailsCollection flower;

    [Header("Stone")]
    public DetailsCollection stone;

    [Header("Bush")]
    public DetailsCollection bush;

    [Header("Tree")]
    public DetailsCollection tree;

    [Header("Rock")]
    public DetailsCollection rock;

    [Header("Rampe")]
    public DetailsCollection rampe;

    [Header("Special")]
    public DetailsCollection special;


    public DetailsCollection GetCollectionOfType(DetailsType dt)
    {
        switch (dt)
        {
            case DetailsType.Grass:
                return grass;
            case DetailsType.Flower:
                return flower;
            case DetailsType.Stone:
                return stone;
            case DetailsType.Bush:
                return bush;
            case DetailsType.Tree:
                return tree;
            case DetailsType.Rock:
                return rock;
            case DetailsType.Rampe:
                return rampe;
            case DetailsType.Special:
                return special;
            default:
                return null;
        }

    }

}
