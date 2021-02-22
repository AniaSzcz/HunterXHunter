using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    // Hack was to share variables--------------------------------------
    public static PlayerMovement hack;
    void Awake()
    {
        hack = this;
    }
    // Walking ---------------------------------------------------------
    public CharacterController controller;
    public float speed = 12f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    Vector3 velocity;
    bool isGrounded;

    // States ------------------------------------------------------------
    public enum myStates
    {
        Standing = 0,
        Walking,
        Climbing,
    }
    public myStates state = myStates.Walking;
    

    // Update is called once per frame
    void Update()
    {
        // Climbing state activation happens remotely through individual trees
        
        
        
        switch (state)
        {
            case myStates.Walking:  
                
                isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
                if(isGrounded && velocity.y < 0)
                {
                    velocity.y = -2f;
                }

                float x = Input.GetAxis("Horizontal");
                float z = Input.GetAxis("Vertical");

                Vector3 move = transform.right * x + transform.forward * z;

                controller.Move(move * speed * Time.deltaTime);

                if(Input.GetButtonDown("Jump") && isGrounded)
                {
                    velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
                }

                velocity.y += gravity * Time.deltaTime;

                controller.Move(velocity * Time.deltaTime);
                
                break;
            
            case myStates.Climbing:

                Debug.Log("It worked");

                break;

                
        }
    }

}
