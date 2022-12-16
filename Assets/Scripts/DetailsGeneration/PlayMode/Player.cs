using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float walkingSpeed = 12f;
    [SerializeField] private float runningSpeed = 20f;

    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float jetpackTrust = 5f;


    private float velocity;

    private float speed;


    private float gravity = -9.81f;

    private bool canJump = false;

    private Vector3 direction;

    CharacterController controller;


    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {


        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = Mathf.Lerp(speed, runningSpeed, 5 * Time.deltaTime);
        }
        else
        {
            speed = Mathf.Lerp(speed, walkingSpeed, 5 * Time.deltaTime);
        }

        direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * speed;


        if (!controller.isGrounded)
        {
            velocity += gravity * Time.deltaTime;
        }
        else
        {
            canJump = true;
            velocity = 0;
        }

        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            velocity = Mathf.Sqrt(jumpForce * -2f * gravity);
            canJump = false;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            velocity += jetpackTrust * Time.deltaTime;
            
        }

        if (velocity > 7) velocity = 7;

        direction.y = -0.1f + velocity;

        controller.Move(direction * Time.deltaTime);
    }
}
