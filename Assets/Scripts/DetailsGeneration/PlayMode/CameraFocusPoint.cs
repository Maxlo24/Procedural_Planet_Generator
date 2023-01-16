using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFocusPoint : MonoBehaviour
{

    [SerializeField] private Transform horizontalPivot;
    [SerializeField] private Transform mainCamera;
    [SerializeField] private Transform raySource;
    [SerializeField] private Transform rayTarget;

    [SerializeField] private float minAngle;
    [SerializeField] private float maxAngle;

    [SerializeField] private Vector3 cameraOffset;


    // public float ray_length = 100f;

    [SerializeField] private float sensitivity = 1f;

    [SerializeField] private float rotationSmooth;
    [SerializeField] private float targetSmooth;

    [SerializeField] private float cameraSmooth;



    private float targetRotation;
    private float targetPitch;


    private float cameraDist;
    private Vector3 cameraTargetPos;

    private float rayLength;
    private float i_rayLength;

    private int layerMask;

    private float distRatio;

    private void Start() {
        
        // layerMask = LayerMask.GetMask("Surface", "Enemy");

        cameraDist = cameraOffset.magnitude;

        rayLength = cameraDist*1.5f;
        i_rayLength = 1/rayLength;

    }



    // // Update is called once per frame
    void Update()
    {


        Vector3 mouse_delta = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);

        targetRotation += sensitivity * mouse_delta.x ;

        targetPitch -= sensitivity * mouse_delta.y * 1.78f;
        targetPitch = Mathf.Clamp(targetPitch, minAngle, maxAngle);

        horizontalPivot.localRotation = Quaternion.Lerp(horizontalPivot.localRotation, Quaternion.Euler(targetPitch, 0,0), Time.deltaTime * rotationSmooth);
        // horizontalPivot.localRotation = Quaternion.Euler(targetPitch, 0, 0);

        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0, targetRotation,0), Time.deltaTime * rotationSmooth);
        // transform.localRotation = Quaternion.Euler(0, targetRotation,0);



        RaycastHit hit;

        cameraTargetPos = -horizontalPivot.forward * cameraDist;

        Vector3 rayOrigin = horizontalPivot.position;

        float ratio = 1f;

        if (Physics.Raycast (rayOrigin, - horizontalPivot.forward, out hit, rayLength)) {
            // Debug.Log (hit.transform.name);
            // Debug.Log ("hit");
            // rayTarget.position = Vector3.Lerp(rayTarget.position, hit.point, Time.deltaTime * targetSmooth);

            ratio = (hit.point - rayOrigin).magnitude * i_rayLength;
        }

        distRatio = Mathf.Lerp(distRatio, ratio, Time.deltaTime * cameraSmooth);

        cameraTargetPos *= distRatio;
        cameraTargetPos += rayOrigin;

        mainCamera.position = cameraTargetPos;
        mainCamera.LookAt(rayOrigin);
        // camera.position = Vector3.Lerp(camera.position,cameraTargetPos, Time.deltaTime * cameraSmooth);

        if (Physics.Raycast (raySource.position, raySource.forward , out hit, Mathf.Infinity)) {
            // Debug.Log (hit.transform.name);
            // Debug.Log ("hit");
            // rayTarget.position = Vector3.Lerp(rayTarget.position, hit.point, Time.deltaTime * targetSmooth);
            rayTarget.position = hit.point;
        }

    }
}
