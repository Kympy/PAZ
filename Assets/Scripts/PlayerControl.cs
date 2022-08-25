using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputManager))]
[RequireComponent(typeof(CharacterController))]
public class PlayerControl : MonoBehaviour
{
    [HideInInspector] private InputManager _InputManager;
    [HideInInspector] private CharacterController _CharacterController;
    [HideInInspector] private Animator _Animator;

    [HideInInspector] private Vector3 moveVector; // Desired Movement Vector
    [HideInInspector] private Vector3 gravityVector;
    [HideInInspector] private Vector3 relativeVector;

    private float moveSpeed = 0f;
    [SerializeField, Range(1f, 10f)] private float WalkSpeed = 2f;
    [SerializeField, Range(1f, 10f)] private float RunSpeed = 4f;
    [SerializeField, Range(1f, 10f)] private float BackSpeed = 1f;

    [SerializeField, Range(0f, 100f)] private float jumpPower = 10f;
    [SerializeField, Range(0f, 0.5f)] private float groundClearance = 0.25f;
    [SerializeField, Range(0f, 1f)] private float groundDistance = 0.25f;
    private float turnDirection = 0f;
    [SerializeField, Range(0f, 200f)] private float mouseSensitivity_X = 100f;
    [SerializeField, Range(0f, 200f)] private float mouseSensitivity_Y = 100f;
    [SerializeField, Range(0f, 500f)] private float turnMultiplier = 100f;

    private GameObject focusPoint;

    private bool IsMove = false;
    private bool IsRun = false;
    private bool CursorLocked = false;

    private const float gravityForce = -9.8f;
    Vector3 playerVeolcity = Vector3.zero;
    private void Awake()
    {
        _InputManager = GetComponent<InputManager>();
        _CharacterController = GetComponent<CharacterController>();
        _Animator = GetComponent<Animator>();
    }
    private void Start()
    {
        focusPoint = GameObject.Find("Focus");
    }
    private void Update()
    {
        IsMove = _InputManager.HasVerticalInput || _InputManager.HasHorizontalInput;
        Movement();
        AnimationPlay();
        MouseCamera();
    }
    private void Movement()
    {
        if (IsGrounded() && gravityVector.y < 0)
        {
            gravityVector.y = -2f;
            playerVeolcity.y = 0f;
        }
        gravityVector.x = 0f; gravityVector.z = 0f;
        gravityVector.y += gravityForce * Time.deltaTime;

        _CharacterController.Move(gravityVector * Time.deltaTime);

        if (IsGrounded())
        {
            moveVector = transform.forward * _InputManager.Vertical + transform.right * _InputManager.Horizontal; // Calculate Vector
            if (IsMove)
            {
                if (_InputManager.Vertical < 0) moveSpeed = BackSpeed;
                else if (_InputManager.Vertical > 0) moveSpeed = WalkSpeed;
                _CharacterController.Move(moveSpeed * Time.deltaTime * moveVector); // Do Movement
                transform.Rotate(transform.up * turnDirection * turnMultiplier * Time.deltaTime); // Rotate Character
                focusPoint.transform.parent.Rotate(transform.up * -turnDirection * turnMultiplier * Time.deltaTime);
            }

        }
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            playerVeolcity.y += Mathf.Sqrt(jumpPower * 2 * -gravityForce);
        }
        playerVeolcity.y += gravityForce * Time.deltaTime;
        _CharacterController.Move(playerVeolcity * Time.deltaTime);
    }
    private bool IsGrounded()
    {
        //Debug.Log($"## center {new Vector3(transform.position.x, transform.position.y - groundDistance, transform.position.z).ToString()} {groundClearance}");
        return Physics.CheckSphere(new Vector3(transform.position.x, transform.position.y - groundDistance, transform.position.z), groundClearance, ~LayerMask.NameToLayer("Terrain"));
    }
    private void Jump()
    {

        //_CharacterController.Move((transform.up + moveVector).normalized * (jumpPower * 2 * -gravityForce) * Time.deltaTime);
        _CharacterController.Move(playerVeolcity * Time.deltaTime);
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
        _Animator.SetFloat("Vertical", _InputManager.Vertical);
        _Animator.SetFloat("Horizontal", _InputManager.Horizontal);
        _Animator.SetBool("IsGrounded", IsGrounded());
        //_Animator.SetFloat("Jump", _InputManager.Jump);
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
