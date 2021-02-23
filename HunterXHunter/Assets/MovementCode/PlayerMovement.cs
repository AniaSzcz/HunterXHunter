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
    public bool collisionHappened;
    bool touchingTree;
    public Transform treeDetection;
    public LayerMask treeMask;
    public float climbingSpeed = 3f;
    public GameObject collisionObject;

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
        else
        {
            pressedE = false;
        }
        if (collisionHappened == true && pressedE == true && state != myStates.Climbing)
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

                Vector3 direction = new Vector3(collisionObject.transform.position.x - treeDetection.transform.position.x, 0f, collisionObject.transform.position.z - treeDetection.transform.position.z);
                RaycastHit nextPlace;
                Physics.Raycast(treeDetection.transform.position, direction, out nextPlace);
                if (nextPlace.collider.CompareTag("Tree"))
                {
                    Vector3 newPosition = ((treeDetection.transform.position - nextPlace.point).normalized) * 0.6f;
                    transform.position = nextPlace.point + newPosition;
                }

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
    // Detecting if we've hit any trees
    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Tree")
        {
            collisionHappened = true;
            collisionObject = collision.gameObject;
        }
    }
    void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.tag == "Tree")
        {
            collisionHappened = false;
        }
    }
}
