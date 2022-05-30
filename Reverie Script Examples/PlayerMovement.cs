using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Vector3 movement;
    public float speed;
    public float sprintSpeed;
    public float gravity = -9.81f;
    float x;
    float y;
    float z;
    public CharacterController myController;
    Vector3 velocity;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public float jumpHeight = 10f;
    public LayerMask groundMask;
    bool grounded;
    float startGravity;
    float climbSpeed;
    public bool crouching;
    public float crouchHeight;
    public float standingHeight;

    public Vector3 crouchVector;

    public Vector3 standVector;
   public bool onLadder;


    void Start()
    {
        startGravity = gravity;
        standingHeight = myController.height;
        standVector = myController.center;
    }

    // Update is called once per frame
    void Update()
    {
        grounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (onLadder)
        {
            y = Input.GetAxis("Vertical") * speed *1.5f * Time.deltaTime;
        }
        else
        {
            z = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        }
        x = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        
        if(grounded && velocity.y < 0 && !onLadder)
        {
            velocity.y = -5f;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = sprintSpeed;
        }
        else
        {
            speed = sprintSpeed/2;
        }

        if (Input.GetButtonDown("Jump") && grounded)
        {

            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        if (onLadder)
        {
            movement = transform.right * x + transform.up * y;
        }
        else
        {
            movement = transform.right * x + transform.forward * z;
        }
        myController.Move(movement);
        velocity.y += gravity * Time.deltaTime;
        myController.Move(velocity * Time.deltaTime);
        if(Input.GetKey(KeyCode.LeftControl) && !crouching)
        {
            crouching = true;
            Crouch();

        }
        else if(!Input.GetKey(KeyCode.LeftControl) && crouching)
        {
            crouching= false;
            Stand();
        }
    }

    public void PlayerWarp(Vector3 newSpot)
    {
        myController.enabled = false;
        transform.position = newSpot;
        myController.enabled = true;
        
    }

    public void Crouch()
{
 
    groundCheck.position += Vector3.up *.5f;
    myController.height = crouchHeight;
  
   


}
    public void Stand()
{
    groundCheck.position += Vector3.down *.5f;
    myController.center = standVector;
    myController.height = standingHeight;

   
}
    public void ToggleControl()
    {
        myController.enabled = !myController.enabled;
    }
    public void RemoveControl()
    {
        myController.enabled = false;
    }

    public void AddControl()
    {
        myController.enabled = true;
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.tag == "Ladder")
        {
            OnLadder();
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if(other.tag == "Ladder")
        {
            OffLadder();
        }
    }
    public void OnLadder()
    {
        gravity = 0f;
        grounded = true;
        onLadder = true;
    }
    public void OffLadder()
    {
        gravity = startGravity;
        grounded = false;
        onLadder = false;
    }
}
