using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor
{
    private float horizontal = 0f;
    private float vertical = 0f;
    [SerializeField]
    private readonly float speed = 1.2f; // Walk Speed
    [SerializeField]
    private readonly float runSpeed = 4.5f;

    private float currentVelocity = 0f;
    [SerializeField]
    private float turnSpeed = 2f;
    private float runTimer = 0f;

    private bool IsJump = false;
    private bool IsSprint = false;
    [SerializeField]
    private float jumpPower = 10f;

    private Vector3 desiredPos;
    private Vector3 desiredDir;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        StartCoroutine(PlayerVelocity());
    }
    private void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        IsJump = Input.GetKeyDown(KeyCode.Space);
        IsSprint = Input.GetKey(KeyCode.LeftShift);

        desiredPos.Set(horizontal, 0f, vertical);
        desiredPos.Normalize();


        ChangeAnimation();

        //Debug.Log(rigidBody.velocity.magnitude * 3.6f);
        //RotateMe();
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
        rigidBody.velocity = desiredPos * currentVelocity;
        /*
        if (IsSprint == false)
        {
            
            
        }
        else
        {
            rigidBody.velocity = desiredPos * currentVelocity * 2f;
        }
        
        */
    }
    private IEnumerator PlayerVelocity()
    {
        while (true)
        {
            if(IsSprint)
            {
                runTimer += Time.deltaTime;
                currentVelocity = Mathf.Lerp(speed, runSpeed, runTimer);
                Debug.Log("Lerp : " + currentVelocity);
            }
            else
            {
                runTimer = 0f;
                currentVelocity -= 0.02f;
                if (currentVelocity < speed) currentVelocity = speed;
                Debug.Log("Lerp Release : " + currentVelocity);
            }


            yield return null;
        }
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
