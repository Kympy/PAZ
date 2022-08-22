using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor
{
    private float horizontal = 0f;
    private float vertical = 0f;
    private float speed = 10f;
    private float turnSpeed = 2f;

    private bool IsJump = false;
    private float jumpPower = 10f;

    private Vector3 desiredPos;
    private Vector3 desiredDir;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        IsJump = Input.GetKeyDown(KeyCode.Space);

        desiredPos.Set(horizontal, 0f, vertical);
        desiredPos.Normalize();

        

        ChangeAnimation();

        Debug.Log(rigidBody.velocity.magnitude);
        RotateMe();
    }
    private void FixedUpdate()
    {
        Movement();
        Jump();

    }
    private void Jump()
    {
        if(IsJump)
        rigidBody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
    }
    private void Movement()
    {
        rigidBody.AddForce(desiredPos * speed, ForceMode.Acceleration);

    }
    private void RotateMe()
    {
        if(vertical > 0)
        {
            desiredDir = Vector3.RotateTowards(transform.forward, desiredPos, turnSpeed * Time.deltaTime, 0f);
            transform.rotation = Quaternion.LookRotation(desiredDir);
        }
        else
        {
            desiredDir = Vector3.RotateTowards(transform.forward, -desiredPos, turnSpeed * Time.deltaTime, 0f);
            transform.rotation = Quaternion.LookRotation(desiredDir);
        }
    }
    private void ChangeAnimation()
    {
        if(vertical < 0)
        {
            animator.SetBool("IsBack", true);
            animator.SetBool("IsMove", false);
        }
        else if(vertical > 0)
        {
            animator.SetBool("IsBack", false);
            animator.SetBool("IsMove", true);
        }
        else
        {
            animator.SetBool("IsBack", false);
            animator.SetBool("IsMove", false);
        }
        animator.SetFloat("MoveSpeed", rigidBody.velocity.magnitude);
    }
}
