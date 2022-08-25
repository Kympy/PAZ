using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputManager))]
public class PlayerController : MonoBehaviour
{
    [HideInInspector] private InputManager _InputManager;
    [HideInInspector] private Animator _Animator;
    [HideInInspector] private Rigidbody _Rigidbody;

    [HideInInspector] private Vector3 moveVector; // Desired Movement Vector
    [HideInInspector] private Vector3 relativeVector;

    private float moveSpeed = 0f;
    [SerializeField, Range(0f, 50f)] private float WalkSpeed = 2f;
    [SerializeField, Range(0f, 50f)] private float RunSpeed = 8f;
    [SerializeField, Range(0f, 50f)] private float BackSpeed = 2f;
    [SerializeField, Range(0f, 50f)] private float RunBackSpeed = 4f;

    [SerializeField, Range(0f, 100f)] private float jumpPower = 10f;
    [SerializeField, Range(0f, 0.5f)] private float groundClearance = 0.25f;
    [SerializeField, Range(0f, 1f)] private float groundDistance = 0.25f;
    private float turnDirection = 0f;
    [SerializeField, Range(0f, 200f)] private float mouseSensitivity_X = 100f;
    [SerializeField, Range(0f, 200f)] private float mouseSensitivity_Y = 100f;
    [SerializeField, Range(0f, 500f)] private float turnMultiplier = 100f;

    private GameObject focusPoint;

    private bool IsMove = false;
    private bool IsJump = false;
    private bool IsSprint = false;
    private bool IsBrake = false;
    private bool CursorLocked = false;

    private const float gravityForce = -9.8f;
    Vector3 playerVeolcity = Vector3.zero;
    private void Awake()
    {
        _InputManager = GetComponent<InputManager>();
        _Animator = GetComponent<Animator>();
        _Rigidbody = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        focusPoint = GameObject.Find("Focus");
        StartCoroutine(GetPlayerVelocity());
    }
    private void FixedUpdate()
    {
        Movement();
    }
    private void Update()
    {
        IsMove = _InputManager.HasVerticalInput || _InputManager.HasHorizontalInput;
        IsJump = _InputManager.Jump;
        AnimationPlay();
        MouseCamera();
        Jump();
    }
    private void Movement()
    {
        if (IsLanding() == false)
        {
            moveVector = transform.forward * _InputManager.Vertical + transform.right * _InputManager.Horizontal; // Calculate Vector
            if (IsMove)
            {
                transform.position += moveSpeed * Time.deltaTime * moveVector;
                transform.Rotate(transform.up * turnDirection * turnMultiplier * Time.deltaTime); // Rotate Character
                focusPoint.transform.parent.Rotate(transform.up * -turnDirection * turnMultiplier * Time.deltaTime);
            }
        }


    }
    private bool IsGrounded()
    {
        //Debug.Log($"## center {new Vector3(transform.position.x, transform.position.y - groundDistance, transform.position.z).ToString()} {groundClearance}");
        return Physics.CheckSphere(new Vector3(transform.position.x, transform.position.y - groundDistance, transform.position.z), groundClearance, ~LayerMask.NameToLayer("Terrain"));
    }
    private bool IsLanding()
    {
        if (_Animator.GetCurrentAnimatorStateInfo(0).IsName("Land") && _Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            return true;
        }
        else return false;
    }
    private void Jump()
    {
        if (IsJump && IsGrounded())
        {
            _Rigidbody.AddForce(transform.up * jumpPower, ForceMode.VelocityChange);
        }
        
    }
    private IEnumerator GetPlayerVelocity()
    {
        while(true)
        {
            if (_InputManager.Vertical > 0 && _InputManager.Sprint == false)
            {
                if(moveSpeed > WalkSpeed)
                {
                    moveSpeed -= 0.03f;
                    if (moveSpeed < WalkSpeed) moveSpeed = WalkSpeed;
                }
                else moveSpeed = WalkSpeed;
            }
            else if (_InputManager.Vertical > 0 && _InputManager.Sprint)
            {
                moveSpeed += 0.03f;
                if (moveSpeed > RunSpeed) moveSpeed = RunSpeed;
            }
            else if (_InputManager.Vertical < 0 && _InputManager.Sprint == false)
            {
                if(moveSpeed > BackSpeed)
                {
                    moveSpeed -= 0.03f;
                    if (moveSpeed < BackSpeed) moveSpeed = BackSpeed;
                }
                else moveSpeed = BackSpeed;
            }
            else if (_InputManager.Vertical < 0 && _InputManager.Sprint)
            {
                moveSpeed += 0.03f;
                if (moveSpeed > RunBackSpeed) moveSpeed = RunBackSpeed;
            }
            else
            {
                moveSpeed -= 0.03f;
                if (moveSpeed < 0f) moveSpeed = 0f;
            }
            yield return null;
        }
    }
    private void MouseCamera()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            CursorLocked = CursorLocked ? false : true;
        }
        if (CursorLocked == false) return;

        relativeVector = transform.InverseTransformPoint(focusPoint.transform.position);
        relativeVector /= relativeVector.magnitude;
        turnDirection = (relativeVector.x / relativeVector.magnitude);
        // Vertical
        focusPoint.transform.eulerAngles = new Vector3(focusPoint.transform.eulerAngles.x + -Input.GetAxis("Mouse Y"), focusPoint.transform.eulerAngles.y, 0f);
        // Horizontal
        focusPoint.transform.parent.Rotate(transform.up * Input.GetAxis("Mouse X") * mouseSensitivity_X * Time.deltaTime);
    }
    private void AnimationPlay()
    {
        _Animator.SetFloat("Vertical", _InputManager.Vertical * moveSpeed);
        _Animator.SetFloat("Horizontal", _InputManager.Horizontal * moveSpeed);
        _Animator.SetBool("IsMove", IsMove);
        _Animator.SetBool("IsGrounded", IsGrounded());
        //_Animator.SetFloat("Jump", _InputManager.Jump);
        //_Animator.SetBool("IsBrake", _InputManager.Brake && moveSpeed < 2f);
        _Animator.SetBool("Jump", _InputManager.Jump);
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - groundDistance, transform.position.z), groundClearance);
    }
    private void OnGUI()
    {
        float h = 50;
        GUI.Label(new Rect(20, h, 200, 20), "Is Ground : " + IsGrounded());
        h += 30;
        GUI.Label(new Rect(20, h, 200, 20), "CursorLocked : " + CursorLocked);
        h += 30;
        GUI.Label(new Rect(20, h, 200, 20), "Turn Direction : " + turnDirection);
    }
#endif
}