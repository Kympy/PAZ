using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor
{
    #region Player Movement Value
    private float horizontal = 0f;
    private float vertical = 0f;
    [SerializeField]
    private readonly float speed = 1.2f; // Walk Speed
    [SerializeField]
    private readonly float runSpeed = 9f;
    private float turnSpeed = 2f;
    private float rotateSpeed = 100f;
    #endregion

    #region Player Bone
    private Transform upperSpine;
    private Transform Neck;
    private GameObject Head;
    #endregion
    private float currentVelocity = 0f;
    [SerializeField]

    private float runTimer = 0f;

    private bool IsStop = true;
    private bool IsMove = false;
    private bool IsJump = false;
    private bool IsSprint = false;
    private bool AltInput = false;
    private bool MouseInput = false;
    [SerializeField]
    private float jumpPower = 10f;

    private Vector3 moveDir;
    private Vector3 cameraDir;
    private Quaternion cameraRot;
    private Quaternion lastCameraRot;
    private Vector3 LastMousePos;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();
        upperSpine = animator.GetBoneTransform(HumanBodyBones.UpperChest);
        Neck = animator.GetBoneTransform(HumanBodyBones.Neck);
        Head = GameObject.Find("bb_male_Neck");
        Debug.Log(upperSpine.name);
        Debug.Log(Neck.name);
    }
    private void Start()
    {
        StartCoroutine(PlayerVelocity());
    }
    private void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        IsMove = !Mathf.Approximately(horizontal, 0f) || !Mathf.Approximately(vertical, 0f);
        IsStop = !IsMove;
        IsJump = Input.GetKeyDown(KeyCode.Space);
        IsSprint = Input.GetKey(KeyCode.LeftShift);
        AltInput = Input.GetKey(KeyCode.LeftAlt);
        MouseInput = Input.GetAxis("Mouse X") != 0f || Input.GetAxis("Mouse Y") != 0f;

        moveDir = new Vector3(horizontal, 0f, vertical);
        moveDir.Normalize();
        cameraDir = Camera.main.transform.forward;
        cameraDir.y = 0f;
        cameraRot = Camera.main.transform.rotation;
        cameraRot.x = 0f;
        cameraRot.z = 0f;
        LastMousePos = Input.mousePosition;

        ChangeAnimation();
    }
    private void FixedUpdate()
    {
        Movement();
        Jump();
        RotateMe();
    }
    private void LateUpdate()
    {
        //RotateMe();
    }
    private void Jump()
    {
        if(IsJump)
        rigidBody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
    }
    private void Movement()
    {
        Debug.Log(IsStop);
        Debug.DrawRay(Camera.main.transform.position, cameraDir * 4f, Color.red);
        if (vertical > 0 && rigidBody.velocity.magnitude < 0.1f)
        {
            Debug.Log("Success");
            //rigidBody.velocity = cameraDir * currentVelocity;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(cameraDir), Time.deltaTime * 13f);
            transform.rotation = Quaternion.LookRotation(cameraDir);
        }
        if (IsMove) // Camera stop
        {
            rigidBody.velocity = transform.forward * currentVelocity;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDir), Time.deltaTime * 13f);
        }


        if(IsMove == false)
        {
            rigidBody.velocity = Vector3.zero;
        }



    }
    private IEnumerator PlayerVelocity()
    {
        while (true)
        {
            if(IsSprint)
            {
                runTimer += Time.deltaTime;
                currentVelocity = Mathf.Lerp(speed, runSpeed, runTimer * 0.5f);
                //Debug.Log("Lerp : " + currentVelocity);
            }
            else
            {
                runTimer = 0f;
                currentVelocity -= 0.03f;
                if (currentVelocity < speed) currentVelocity = speed;
                //Debug.Log("Lerp Release : " + currentVelocity);
            }

            yield return null;
        }
    }
    private void RotateMe()
    {


    }
    private void ChangeAnimation()
    {
        animator.SetBool("IsMove", IsMove);
        animator.SetFloat("MoveSpeed", rigidBody.velocity.magnitude);
    }
}
