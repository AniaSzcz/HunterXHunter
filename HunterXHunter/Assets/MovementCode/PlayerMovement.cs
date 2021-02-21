using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
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
    // Climbing ---------------------------------------------------------
    bool touchingTree;
    public Transform treeDetection;
    public LayerMask treeMask;
    bool climbingState = false;
    // State int --------------------------------------------------------
    int state = 0;
    
    // Update is called once per frame
    void Update()
    {
        // Climbing state -----------------------------------------------
        // Drawing an invisible circle around the empty TreeDetection and seeing when that collides with the tree
        touchingTree = Physics.CheckSphere(treeDetection.position, 1, treeMask);
        // If it's touching, go into the climbing state
        if(touchingTree == true && Input.GetKey(KeyCode.E))
        {
            state = 1;
        } 
        
        

        
        
        if (state == 0)
        {
            Walking();
        }
        else if (state == 1)
        {
            Climbing();
        }
    }
    void Walking()
    {
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
    }
    void Climbing()
    {
        
    }
}
