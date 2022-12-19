using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Optimizer : MonoBehaviour
{


    [SerializeField] private bool optimize = true;
    [Range(0,100)]
    [SerializeField] private float optimisationFactor = 50;


    public List<ItemActivator> objectsToOptimize;


    private GameObject player;

    public float optimisationIntensity;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        objectsToOptimize = new List<ItemActivator>();

        if (optimize) StartCoroutine(OptimizeVisibility());
    }


    //private void Update()
    //{
    //    optimisationIntensity = 1 / optimisationFactor;
    //}

    //void Update()
    //{
    //    foreach (ItemActivator item in objectsToOptimize)
    //    {
    //        if (item == null)
    //        {
    //        }
    //        else
    //        {

    //            if (Vector3.Distance(player.transform.position, item.position) > item.activeRadius)
    //                Debug.DrawLine(player.transform.position, item.position, Color.red);
    //            else if (Vector3.Distance(player.transform.position, item.position) > item.activeRadius / 2)
    //            {
    //                Debug.DrawLine(player.transform.position, item.position, Color.yellow);
    //                item.gObject.SetActive(false);
    //            }
    //            else
    //            {
    //                Debug.DrawLine(player.transform.position, item.position, Color.green);
    //                item.gObject.SetActive(true);
    //            }


    //        }
    //    }

    //}

    IEnumerator OptimizeVisibility()
    {
        List<ItemActivator> objectsToRemove = new List<ItemActivator>();

        optimisationIntensity = ((100-optimisationFactor)-50)/50 + 1.5f;


        if (objectsToOptimize.Count > 0)
        {
            foreach (ItemActivator item in objectsToOptimize)
            {
                if (item == null)
                {
                    objectsToRemove.Add(item);
                }
                else
                {
                    if (Vector3.Distance(player.transform.position, item.position) < item.activeRadius * optimisationIntensity) item.gObject.SetActive(true);
                    else item.gObject.SetActive(false);

                }
            }
        }

        yield return new WaitForSeconds(0.05f);

        if (objectsToRemove.Count > 0)
        {
            foreach (ItemActivator item in objectsToRemove) objectsToOptimize.Remove(item);
        }
        
        yield return new WaitForSeconds(0.05f);

        StartCoroutine(OptimizeVisibility());

    }

}


public class ItemActivator
{
    public GameObject gObject;
    public Vector3 position;
    public float activeRadius;
    

}