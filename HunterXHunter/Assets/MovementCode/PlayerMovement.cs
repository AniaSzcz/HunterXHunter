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
    public float groundDistance = 0.1f;
    public LayerMask groundMask;
    Vector3 velocity;
    bool isGrounded;
    // Climbing ---------------------------------------------------------
    bool pressedE = false;
    bool touchingTree;
    public Transform treeDetection;
    public LayerMask treeMask;
    public float climbingSpeed = 3f;

    // States -----------------------------------------------------------
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

        // Climbing state -----------------------------------------------
        
        // Detecting if we've hit "e" to climb the tree and going into climbing state if its true
        if (Input.GetKeyDown(KeyCode.E))
        {
            pressedE = true;
        }
        // Drawing an invisible circle around the empty TreeDetection and seeing when that collides with the tree
        touchingTree = Physics.CheckSphere(treeDetection.position, 1, treeMask);
        // If it's touching, go into the climbing state
        if(touchingTree == true && pressedE == true && state != myStates.Climbing)
        {
            state = myStates.Climbing;
            pressedE = false;
        } 

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

                if (Input.GetKey(KeyCode.W))
                {
                    transform.Translate(0f, climbingSpeed * Time.deltaTime, 0f);
                }





                if (pressedE == true)
                {
                    state = myStates.Walking;
                    pressedE = false;
                }

                break;       
        }
            


    }
}
