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
    float speed;
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
    float smoothingSpeed = 3f;
    float intializeSmoothingSpeed = 10f;
    Vector3 normalDirection;
    bool initializeClimbing = false;
    Vector3 checkingVector;
    float rightLeftSpeed = 70f;
    float upDownSpeed = 2f;
    // Running ----------------------------------------------------------
    bool pressedCtrl = false;
    // Debugging --------------------------------------------------------
    bool pressedR = false;
    Vector3 getUp;
    Vector3 newStart;
    // States -----------------------------------------------------------
    public enum myStates
    {
        Standing = 0,
        Walking,
        Climbing,
        Running,
        Debugging,
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
            Physics.Raycast(transform.position, firstDirection, out hit, 1f);
            if (hit.collider != null && hit.collider.gameObject.tag == "Tree")
            {          
                // making the player face the hit point
                lookAt = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                transform.LookAt(lookAt);

                normalDirection = hit.normal.normalized; 

                state = myStates.Climbing;
                pressedE = false;
                initializeClimbing = true;
            }
            else if (hit.collider == null)
            {
                Debug.Log("Not in range");
            }
        }

        // Running state -----------------------------------------------------

        if (Input.GetKey(KeyCode.LeftControl))
        {
            pressedCtrl = true;
        }
        else
        {
            pressedCtrl = false;
        }
        
        if (pressedCtrl == true)
        {
            state = myStates.Running;
        }

        switch (state)
        {
            case myStates.Walking:

                transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
                speed = 12f; 
                isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance);
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

                if (initializeClimbing == true)
                {
                    //Making the player go to the point 0.6 away from the hit point (so there's no clipping)
                    Vector3 newPosition = hit.point + (hit.normal.normalized * radiusOfPlayer);
                    initialPosition = new Vector3(newPosition.x, transform.position.y, newPosition.z);
                    Vector3 initialPosition2 = Vector3.Lerp(transform.position, initialPosition, Time.deltaTime * intializeSmoothingSpeed);
                    transform.position = initialPosition2;

                    lookAt = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                    transform.LookAt(lookAt);

                    checkingVector = new Vector3(hit.point.x, 0, hit.point.z);

                    if (Vector3.Distance(transform.position, initialPosition) <= 0.01f)
                    {
                        initializeClimbing = false;
                    }
                }

                if (initializeClimbing == false)
                {
                    if (hit.collider != null && hit.collider.gameObject.tag == "Tree")
                    {
                        normalDirection = Vector3.Lerp(normalDirection, hit.normal.normalized, Time.deltaTime * smoothingSpeed);          
                        //Making the player go to the point 0.6 away from the hit point (so there's no clipping)                       
                        Vector3 newPosition = hit.point + (normalDirection * radiusOfPlayer);  
                        Vector3 newPosition1 = new Vector3(newPosition.x, transform.position.y, newPosition.z);
                        transform.position = newPosition1;

                        transform.LookAt(hit.point);
                        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0));

                    }
                                     
                    Debug.DrawRay(transform.position, direction, Color.yellow);

                    if (Input.GetKey(KeyCode.W))
                    {
                        ClimbingUp();
                    }

                    if (Input.GetKey(KeyCode.D))
                    {
                        ClimbRightLeft(true);
                    }

                    if (Input.GetKey(KeyCode.A))
                    {
                        ClimbRightLeft(false);
                    }

                    if (Input.GetKey(KeyCode.S))
                    {
                        ClimbingDown();
                    }

                    if (pressedE == true)
                    {
                        state = myStates.Walking;
                        pressedE = false;
                    }
                }

                break;
                
            case myStates.Running:

                speed = 24f;

                isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance);
                if(isGrounded && velocity.y < 0)
                {
                    velocity.y = -2f;
                }

                float runningX = Input.GetAxis("Horizontal");
                float runningZ = Input.GetAxis("Vertical");

                Vector3 runningMove = transform.right * runningX + transform.forward * runningZ;

                controller.Move(runningMove * speed * Time.deltaTime);

                velocity.y += gravity * Time.deltaTime;

                controller.Move(velocity * Time.deltaTime);

                if (Input.GetKey(KeyCode.LeftControl))
                {
                    pressedCtrl = true;
                }
                else
                {
                    pressedCtrl = false;
                }
                if (pressedCtrl == false)
                {
                    state = myStates.Walking;
                }                
                
                break;
            case myStates.Debugging:

                Debug.DrawRay(newStart, direction, Color.magenta);
                
                if (Input.GetKeyDown(KeyCode.R))
                {
                    pressedR = true;
                }
                else
                {
                    pressedR = false;
                }
                
                if (pressedR == true)
                {
                    state = myStates.Walking;
                    pressedR = false;
                }

                break;
        }   
    }
    void ClimbRightLeft(bool right) // tr
    {
        direction = hit.point - transform.position;
        Physics.Raycast(transform.position, direction, out hit, 0.8f);         
        if (hit.collider != null && hit.collider.gameObject.tag == "Tree")
        {
            direction = Quaternion.Euler(0, Time.deltaTime * rightLeftSpeed * (right ? 1f : -1f), 0) * direction;
            Physics.Raycast(transform.position, direction, out hit, 0.8f);
            if (hit.collider == null)
            {
                direction = Quaternion.Euler(0, Time.deltaTime * rightLeftSpeed * (right ? -1f : 1f), 0) * direction;
                Physics.Raycast(transform.position, direction, out hit, 0.8f);    
            }
        }  
    }
    
    void ClimbingUp()
    {
        direction = hit.point - transform.position;
        Physics.Raycast(transform.position, direction, out hit, 0.8f);         
        if (hit.collider != null && hit.collider.gameObject.tag == "Tree")
        {
            direction = new Vector3(direction.x, direction.y + (Time.deltaTime * upDownSpeed * 1f), direction.z);
            Physics.Raycast(transform.position, direction, out hit, 0.8f);
            if (hit.collider != null && hit.collider.gameObject.tag == "Tree")
            {
                normalDirection = Vector3.Lerp(normalDirection, hit.normal.normalized, Time.deltaTime * smoothingSpeed);          
                //Making the player go to the point 0.6 away from the hit point (so there's no clipping)                       
                Vector3 newPosition = hit.point + (normalDirection * radiusOfPlayer);  
                transform.position = newPosition;
            }
            else if (hit.collider == null)
            {            
                direction = new Vector3(direction.x, direction.y - (Time.deltaTime * rightLeftSpeed * 1f) - 0.09f, direction.z);
                newStart = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
                Physics.Raycast(newStart, direction, out hit, 0.8f);
                getUp = hit.point - transform.position;
                getUp = new Vector3(getUp.x, getUp.y + 1.801f, getUp.z);
                controller.Move(getUp);

                state = myStates.Walking;
            }  
        }
    }
    void ClimbingDown()
    {
        direction = hit.point - transform.position;
        Physics.Raycast(transform.position, direction, out hit, 0.8f);         
        if (hit.collider != null && hit.collider.gameObject.tag == "Tree")
        {
            direction = new Vector3(direction.x, direction.y - 0.01f, direction.z);
            Physics.Raycast(transform.position, direction, out hit, 0.8f);
            if (hit.collider != null && hit.collider.gameObject.tag == "Tree")
            {
                normalDirection = Vector3.Lerp(normalDirection, hit.normal.normalized, Time.deltaTime * smoothingSpeed);          
                //Making the player go to the point 0.6 away from the hit point (so there's no clipping)                       
                Vector3 newPosition = hit.point + (normalDirection * radiusOfPlayer);  
                transform.position = newPosition;
            }
            else if (hit.collider == null)
            {                
                direction = new Vector3(direction.x, direction.y + 0.01f, direction.z);
                Physics.Raycast(transform.position, direction, out hit, 0.8f);         
            }    
        }       
    }
    
}
