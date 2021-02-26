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
    Vector3 direction;
    public RaycastHit hit;
    float radiusOfPlayer = 0.6f;
    public Camera camera1;
    Vector3 initialPosition;
    Vector3 lookAt;
    Vector3 rightLookAt;
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
        if (pressedE && state != myStates.Climbing)
        {
            Vector3 firstDirection = new Vector3(camera1.transform.forward.x, 0, camera1.transform.forward.z);
            Physics.Raycast(transform.position, firstDirection, out hit, 0.8f);
                if (hit.collider != null && hit.collider.gameObject.tag == "Tree")
                {          
                    //Making the player go to the point 0.6 away from the hit point (so there's no clipping)                       
                    Vector3 newPosition = hit.point + (hit.normal.normalized * radiusOfPlayer);
                    initialPosition = new Vector3(newPosition.x, transform.position.y, newPosition.z);
                    transform.position = initialPosition;

                    // making the player face the hit point
                    lookAt = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                    transform.LookAt(lookAt);

                    state = myStates.Climbing;
                    pressedE = false;
                }
                else if (hit.collider == null)
                {
                    Debug.Log("Not in range");
                }
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

                Debug.DrawRay(transform.position, direction, Color.yellow);

                if (Input.GetKey(KeyCode.W))
                {
                    ClimbingUp();
                }

                if (Input.GetKey(KeyCode.D))
                {
                    ClimbingRight();
                }

                if (Input.GetKey(KeyCode.A))
                {
                    ClimbingLeft();
                }

                if (pressedE == true)
                {
                    state = myStates.Walking;
                    pressedE = false;
                }

                break;     
        }
    }
    void ClimbingRight()
    {
        direction = hit.point - transform.position;
        Physics.Raycast(transform.position, direction, out hit, 0.8f);         
        if (hit.collider != null && hit.collider.gameObject.tag == "Tree")
        {
            direction = Quaternion.Euler(0, 0.5f, 0) * direction;
            Physics.Raycast(transform.position, direction, out hit, 0.8f);
            if (hit.collider != null && hit.collider.gameObject.tag == "Tree")
            {          
                //Making the player go to the point 0.6 away from the hit point (so there's no clipping)                       
                Vector3 newPosition = hit.point + (hit.normal.normalized * radiusOfPlayer);
                transform.position = new Vector3(newPosition.x, transform.position.y, newPosition.z);
            }  
        }
    }
    void ClimbingLeft()
    {
        direction = hit.point - transform.position;
        Physics.Raycast(transform.position, direction, out hit, 0.8f);         
        if (hit.collider != null && hit.collider.gameObject.tag == "Tree")
        {
            direction = Quaternion.Euler(0, -0.5f, 0) * direction;
            Physics.Raycast(transform.position, direction, out hit, 0.8f);
            if (hit.collider != null && hit.collider.gameObject.tag == "Tree")
            {          
                //Making the player go to the point 0.6 away from the hit point (so there's no clipping)                       
                Vector3 newPosition = hit.point + (hit.normal.normalized * radiusOfPlayer);
                transform.position = new Vector3(newPosition.x, transform.position.y, newPosition.z);
            }  
        }
    }
    void ClimbingUp()
    {
        direction = hit.point - transform.position;
        Physics.Raycast(transform.position, direction, out hit, 0.8f);         
        if (hit.collider != null && hit.collider.gameObject.tag == "Tree")
        {
            direction = new Vector3(direction.x, direction.y + 0.01f, direction.z); //<-------- this needs work
            Physics.Raycast(transform.position, direction, out hit, 0.8f);
            if (hit.collider != null && hit.collider.gameObject.tag == "Tree")
            {          
                //Making the player go to the point 0.6 away from the hit point (so there's no clipping)                       
                Vector3 newPosition = hit.point + (hit.normal.normalized * radiusOfPlayer);
                transform.position = newPosition;
            }  
        }
    }
}
