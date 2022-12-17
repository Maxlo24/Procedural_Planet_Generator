using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Tooltip("Maximum slope the character can jump on")]
    [Range(5f, 60f)]
    public float slopeLimit = 45f;
    [Tooltip("Move speed in meters/second")]
    public float moveSpeed = 5f;
    [Tooltip("Turn speed in degrees/second, left (+) or right (-)")]
    public float turnSpeed = 300;
    [Tooltip("Whether the character can jump")]
    public bool allowJump = false;
    [Tooltip("Upward speed to apply when jumping in meters/second")]
    public float jumpSpeed = 4f;


    public bool IsGrounded { get; private set; }
    public float ForwardInput { get; set; }
    public float TurnInput { get; set; }
    public bool JumpInput { get; set; }


    new private Rigidbody rigidbody;
    private CapsuleCollider capsuleCollider;


    new Vector3 targetVellocity;


    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    private void FixedUpdate()
    {
        CheckGrounded();
        ProcessActions();


        rigidbody.velocity = Vector3.Lerp(rigidbody.velocity, targetVellocity, Time.deltaTime * 5f);
    }



    /// <summary>
    /// Checks whether the character is on the ground and updates <see cref="IsGrounded"/>
    /// </summary>
    private void CheckGrounded()
    {
        IsGrounded = false;
        float capsuleHeight = Mathf.Max(capsuleCollider.radius * 2f, capsuleCollider.height);
        Vector3 capsuleBottom = transform.TransformPoint(capsuleCollider.center - Vector3.up * capsuleHeight / 2f);
        float radius = transform.TransformVector(capsuleCollider.radius, 0f, 0f).magnitude;
        Ray ray = new Ray(capsuleBottom + transform.up * .01f, -transform.up);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, radius * 5f))
        {
            float normalAngle = Vector3.Angle(hit.normal, transform.up);
            if (normalAngle < slopeLimit)
            {
                float maxDist = radius / Mathf.Cos(Mathf.Deg2Rad * normalAngle) - radius + .02f;
                if (hit.distance < maxDist)
                    IsGrounded = true;
            }
        }
    }


    /// <summary>
    /// Processes input actions and converts them into movement
    /// </summary>
    private void ProcessActions()
    {
        // Process Turning
        if (TurnInput != 0f)
        {
            float angle = Mathf.Clamp(TurnInput, -1f, 1f) * turnSpeed;
            transform.Rotate(Vector3.up, Time.fixedDeltaTime * angle);
        }
        // Process Movement/Jumping
        if (IsGrounded)
        {
            // Reset the velocity
            targetVellocity = Vector3.zero;
            // Check if trying to jump
            if (JumpInput && allowJump)
            {
                // Apply an upward velocity to jump
                targetVellocity += Vector3.up * jumpSpeed;
            }

            // Apply a forward or backward velocity based on player input
            targetVellocity += transform.forward * Mathf.Clamp(ForwardInput, -1f, 1f) * moveSpeed;
        }
        else
        {
            // Check if player is trying to change forward/backward movement while jumping/falling
            if (!Mathf.Approximately(ForwardInput, 0f))
            {
                // Override just the forward velocity with player input at half speed
                Vector3 verticalVelocity = Vector3.Project(rigidbody.velocity, Vector3.up);
                targetVellocity = verticalVelocity + transform.forward * Mathf.Clamp(ForwardInput, -1f, 1f) * moveSpeed / 2f;
            }
        }
    }



}
