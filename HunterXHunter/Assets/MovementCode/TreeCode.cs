using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerMovement;

public class TreeCode : MonoBehaviour
{
    public bool collisionHappened;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    { 
        if (collisionHappened == true && Input.GetKey(KeyCode.E))
        {
            PlayerMovement.hack.state = myStates.Climbing;    
        }
    }
    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.name == "First Person Player")
        {
            collisionHappened = true;    
        }
    }
    void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.name == "First Person Player")
        {
            collisionHappened = false;
            PlayerMovement.hack.state = myStates.Walking;
        }
    }

    
}
