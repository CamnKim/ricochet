﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Inspector settable values
    public int nonGroundedJumps = 1;
    // Player's speed
    float speed;
    public float walkSpeed = 5;
    public float sprintSpeed = 15;
    // Player's sensitivity
    [Range(0, 10)]
    public float sens = 5;
    float sensMod = 25;
    public float crouchingHeight = 1f;
    public float jumpForce = 10;
    public int invert = -1;
    public float viewRange = 1;

    public bool isDead = false;


    
    private Rigidbody rb;
    private CapsuleCollider cc;
    private Animator anim;
    private Camera cam;
    public Animator P_arms;
    public Animator R_arms;
    private bool isJumping = false;
    private bool isSprinting = false;
    private int numJumps;
    private Vector3 movement;
    private float standingHeight;
    private float verticalRotation;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CapsuleCollider>();
        anim = GetComponent<Animator>();
        cam = GetComponentInChildren<Camera>();
        speed = walkSpeed;

        standingHeight = cc.height;
        numJumps = nonGroundedJumps;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            return;
        }
        // Horizontal mouse movement
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * sens);

        // Vertical mouse movement
        verticalRotation += Input.GetAxisRaw("Mouse Y") * sens * invert;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        cam.transform.eulerAngles = new Vector3(verticalRotation, transform.eulerAngles.y, 0);

        
        

        // Resetting double jump when grounded
        if(isGrounded())
        {
            numJumps = nonGroundedJumps;
        }

        // Jump
        if (numJumps > 0 && Input.GetButtonDown("Jump"))
        {
            isJumping = true;
            if(!isGrounded())
            {
                numJumps -= 1;
            }
        }

        // Crouch
        if (Input.GetButtonDown("Crouch"))
        {
            cc.height = crouchingHeight;
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            cc.height = standingHeight;
        }

        // Inspect
        //if (Input.GetButtonDown("Inspect"))
        //{
        //    P_arms.SetTrigger("Inspect");
        //}

        // Sprint
        if (Input.GetButtonDown("Sprint"))
        {
            speed = sprintSpeed;
            isSprinting = true;
        }
        if(Input.GetButtonUp("Sprint"))
        {
            speed = walkSpeed;
            isSprinting = false;
        }
    }

    void FixedUpdate()
    {
        // Movement
        if (isDead) return;
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        move(h, v);
        Animating(h, v);
        // Jump
        if (isJumping)
        {
            isJumping = false;
            GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void move(float h, float v)
    {
        movement = new Vector3(h, 0, v);
        movement = rb.rotation * movement;
        rb.MovePosition(transform.position + movement.normalized * Time.deltaTime * speed);
    }

    private void Animating(float h, float v)
    {
        if (v != 0f || h != 0f)
        {
            P_arms.SetBool("Walk", true);
            R_arms.SetBool("Walk", true);
            if (isSprinting)
            {
                P_arms.SetBool("Walk", false);
                P_arms.SetBool("Run", true);
                R_arms.SetBool("Walk", false);
                R_arms.SetBool("Run", true);
            }
        }
        else
        {
            P_arms.SetBool("Walk", false);
            P_arms.SetBool("Run", false);
            R_arms.SetBool("Walk", false);
            R_arms.SetBool("Run", false);
        }

    }

    // refered to solution given in this forum post https://answers.unity.com/questions/196381/how-do-i-check-if-my-rigidbody-player-is-grounded.html
    private bool isGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, cc.bounds.extents.y + 0.1f);
    }
}
