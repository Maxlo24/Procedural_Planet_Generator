using UnityEngine;
using Assets.Scripts.DetailsGeneration;


[CreateAssetMenu(fileName = "NewDetailsCollection", menuName = "Scriptable objects/DetailsCollection", order = 0)]
public class DetailsCollection : ScriptableObject
{
    [Header("Small")]
    public GameObject[] small = new GameObject[0];

    [Header("Medium")]
    public GameObject[] medium = new GameObject[0];

    [Header("Large")]
    public GameObject[] large = new GameObject[0];


    public GameObject[] GetCollectionOfSize(DetailsSize ds)
    {
        switch (ds)
        {
            case DetailsSize.Small:
                return small;
            case DetailsSize.Medium:
                return medium;
            case DetailsSize.Large:
                return large;
            default:
                return null;
        }

    }


}
