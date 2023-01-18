using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{

    public float rotSpeed;

    void Update()
    {
        transform.Rotate(Vector3.up * rotSpeed * Time.deltaTime);
    }


}
