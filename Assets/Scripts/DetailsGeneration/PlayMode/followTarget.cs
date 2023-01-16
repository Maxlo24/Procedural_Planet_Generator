using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followTarget : MonoBehaviour
{
    public Transform target;
    public float speed = 10;
    public Vector3 offset;


    public bool smoothDamp = false;
    public bool stickToFloor = false;
    public float heightAdjustSpeed = 5f;
    public float rayLength = 10;
    public Vector3 rayOffset;
    public float delay = 0.1f;
    private Vector3 velocity;

    private int layerMask;

    private void Start() {
        layerMask = LayerMask.GetMask("Surface");
    }

    void Update()
    {
        Vector3 newPos;

        if (smoothDamp)
        {
            newPos = Vector3.SmoothDamp(transform.position, target.position + offset , ref velocity, delay, speed , Time.deltaTime);
        }
        else
        {
            newPos = Vector3.Lerp(transform.position, target.position + offset,Time.deltaTime* speed);
        }
        if (stickToFloor)
        {
            RaycastHit hit;
            if (Physics.Raycast(newPos + rayOffset, Vector3.down, out hit, rayLength, layerMask))
            {
                newPos.y = Mathf.Lerp(newPos.y, hit.point.y + offset.y, Time.deltaTime * heightAdjustSpeed);
            }
        }
        // newPos += offset;

        transform.position = newPos;

    
    }
}

