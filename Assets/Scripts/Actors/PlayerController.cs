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
    [HideInInspector] private Vector3 rotateVector; // Desired Rotation Vector
    [HideInInspector] private Vector3 relativeVector;
    [HideInInspector] private Vector3 finalVector = Vector3.zero;

    private float moveSpeed = 0f; // Use to walk and run speed
    private float turnDirection = 0f; // Use to rotate character
    private float fireTimer = 0f; // Fire timer
    private const float fireTime = 0.6f; // My gun fire rate
    private int bulletCount = 30; // current Ammo count
    private const int maxBulletCount = 30;
    private int haveBulletCount = 0;

    [SerializeField, Range(0f, 50f)] private float WalkSpeed = 4f;
    [SerializeField, Range(0f, 50f)] private float RunSpeed = 8f;
    [SerializeField, Range(0f, 50f)] private float BackSpeed = 4f;

    [SerializeField, Range(0f, 100f)] private float jumpPower = 5.5f;
    [SerializeField, Range(0f, 0.5f)] private float groundClearance = 0.25f;
    [SerializeField, Range(0f, 1f)] private float groundDistance = 0.25f;

    [SerializeField, Range(0f, 200f)] private float mouseSensitivity_X = 100f;
    [SerializeField, Range(0f, 200f)] private float mouseSensitivity_Y = 100f;
    [SerializeField, Range(0f, 500f)] private float turnMultiplier = 300f;
    [SerializeField, Range(0f, 1000f)] private float gunRange = 500f;
    

    private GameObject focusPoint; // Camera point
    // Hand
    private GameObject realAxe = null;
    private GameObject realGun = null;
    // Back
    private GameObject fakeAxe = null;
    private GameObject fakeGun = null;
    // Muzzle effect
    private GameObject muzzleEffect = null;

    private bool IsMove = false;
    private bool IsJump = false;
    private bool IsFalling = false;
    private bool IsAim = false;
    private bool IsGun = false;
    private bool IsFire = false;

    private bool CursorLocked = false;
    private bool CoroutineAlready = false;

    private WaitForSeconds oneSec = new WaitForSeconds(1f);

    // temp
    private RaycastHit hit; // temp hit
    private float xRotate;

    private void Awake()
    {
        _InputManager = GetComponent<InputManager>();
        _Animator = GetComponent<Animator>();
        _Rigidbody = GetComponent<Rigidbody>();

        fakeAxe = GameObject.FindGameObjectWithTag("FakeAxe");
        fakeGun = GameObject.FindGameObjectWithTag("FakeGun");

        realAxe = GameObject.FindGameObjectWithTag("RealAxe");
        realAxe.GetComponent<CapsuleCollider>().isTrigger = true;

        realGun = GameObject.FindGameObjectWithTag("RealGun");

        fakeGun.SetActive(false);
        fakeAxe.SetActive(false);

        muzzleEffect = GameObject.FindGameObjectWithTag("Muzzle");

        muzzleEffect.SetActive(false);
        realGun.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
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
        IsAim = _InputManager.RightClicking;

        Jump();
        Axe();
        Fire();
        Reload();
        MouseCamera();

        SlotChange();
        AnimationPlay();
    }
    private void Movement() // Character Movement And Rotation
    {
        if (IsLanding() == false && IsFalling == false)
        {
            finalVector = Vector3.zero;
            moveVector = transform.forward * _InputManager.Vertical + transform.right * _InputManager.Horizontal;
            moveVector.Normalize();
            finalVector = moveVector;
            if(IsMove)
            {
                _Rigidbody.position += moveSpeed * Time.deltaTime * finalVector; // Do Movement
                transform.Rotate(transform.up * turnDirection * turnMultiplier * Time.deltaTime); // Rotate Character
                focusPoint.transform.parent.Rotate(transform.up * -turnDirection * turnMultiplier * Time.deltaTime);
            }
            /*
            if (_InputManager.Horizontal != 0f) // Has Horizontal Movement
            {

                if (_InputManager.Vertical >= 0f) // Only Forward And Stop + Horizontal
                {
                    rotateVector = transform.up * _InputManager.Horizontal;
                    finalVector += transform.forward;
                    //transform.position += moveSpeed * Time.deltaTime * transform.forward;
                }
                else // BackWard + Horizontal
                {
                    rotateVector = transform.up * -_InputManager.Horizontal;
                    finalVector += -transform.forward;//transform.position += moveSpeed * Time.deltaTime * -transform.forward;
                }

                transform.Rotate(rotateVector * Time.deltaTime * turnMultiplier / 10); // Rotate Character by Horizontal Input
                //focusPoint.transform.parent.Rotate(transform.up * -turnDirection * turnMultiplier * Time.deltaTime);
            }

            if (_InputManager.Vertical != 0f) // Has Forward And Backward Movement
            {
                moveVector = transform.forward * _InputManager.Vertical; // Calculate Vector
                moveVector.Normalize();
                finalVector += moveVector;
                //transform.position += moveSpeed * Time.deltaTime * moveVector;
                transform.Rotate(transform.up * turnDirection * turnMultiplier * Time.deltaTime); // Rotate Character
                focusPoint.transform.parent.Rotate(transform.up * -turnDirection * turnMultiplier * Time.deltaTime);
            }
            finalVector.Normalize(); // Normalize movement vector
            _Rigidbody.position += moveSpeed * Time.deltaTime * finalVector; // Do Movement
            */
        }
    }
    private void Axe()
    {
        if(_InputManager.LeftClick)
        {
            _Animator.SetTrigger("First");
        }
    }
    private bool IsGrounded()
    {
        if (Physics.CheckSphere(new Vector3(transform.position.x, transform.position.y - groundDistance, transform.position.z), groundClearance, 1 << LayerMask.NameToLayer("Terrain")))
        {
            IsFalling = false;
            return true;
        }
        else
        {
            if(CoroutineAlready == false)
            {
                StartCoroutine(FallingCheck());
            }
            return false;
        }
    }
    private bool IsLanding()
    {
        if (_Animator.GetCurrentAnimatorStateInfo(0).IsName("Land") && _Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            return true;
        }
        else return false;
    }
    private IEnumerator FallingCheck()
    {
        CoroutineAlready = true;
        int timer = 0;
        while(true)
        {
            if(IsGrounded() == false)
            {
                timer += 1;
                if(timer >= 3)
                {
                    IsFalling = true;
                    CoroutineAlready = false;
                    yield break;
                }
            }
            else
            {
                IsFalling = false;
                CoroutineAlready = false;
                yield break;
            }
            yield return oneSec;
        }
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
                moveSpeed += 0.05f;
                if (moveSpeed > RunSpeed) moveSpeed = RunSpeed;
            }
            else if (_InputManager.Vertical < 0)
            {
                if(moveSpeed > BackSpeed)
                {
                    moveSpeed = BackSpeed;
                }
                else moveSpeed = BackSpeed;
            }
            else if(_InputManager.Horizontal != 0)
            {
                moveSpeed = WalkSpeed;
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
        if (Input.GetKeyDown(KeyCode.P))
        {
            CursorLocked = CursorLocked ? false : true;
        }
        if (CursorLocked == true) return;

        relativeVector = transform.InverseTransformPoint(focusPoint.transform.position);
        relativeVector /= relativeVector.magnitude;
        turnDirection = (relativeVector.x / relativeVector.magnitude);

        xRotate = focusPoint.transform.localEulerAngles.x - Input.GetAxis("Mouse Y"); // Get X rotate
        xRotate = xRotate > 180 ? xRotate - 360 : xRotate; // Get X rotate when x have minus value
        xRotate = Mathf.Clamp(xRotate, -25, 60); // Clamp angles
        // Vertical
        focusPoint.transform.eulerAngles = new Vector3(xRotate, focusPoint.transform.eulerAngles.y, 0f);
        
        // Horizontal
        focusPoint.transform.parent.Rotate(transform.up * Input.GetAxis("Mouse X") * mouseSensitivity_X * Time.deltaTime);
    }
    private void AnimationPlay()
    {
        _Animator.SetFloat("Vertical", _InputManager.Vertical * moveSpeed);
        _Animator.SetFloat("Horizontal", _InputManager.Horizontal * moveSpeed);
        _Animator.SetBool("IsMove", IsMove);
        _Animator.SetBool("IsGrounded", IsGrounded());
        _Animator.SetBool("IsFalling", IsFalling);
        _Animator.SetBool("Jump", _InputManager.Jump);
        _Animator.SetBool("IsAim", IsAim);
        _Animator.SetBool("IsGun", IsGun);
        _Animator.SetBool("IsFire", IsFire);
    }
    private void ResetLegMovement()
    {
        _Animator.SetTrigger("ResetLower");
    }
    private void AxeOn()
    {
        realAxe.GetComponent<CapsuleCollider>().isTrigger = false;
    }
    private void AxeOff()
    {
        realAxe.GetComponent<CapsuleCollider>().isTrigger = true;
    }
    
    private void Fire()
    {
        if(IsGun && _InputManager.LeftCliking && bulletCount > 0)
        {
            fireTimer = fireTime; // First shot
            fireTimer += Time.deltaTime; // timer Start
            if(fireTimer >= fireTime) // Satisfied fire rate
            {
                fireTimer = 0f; // reset
                muzzleEffect.SetActive(true); // show muzzle
                IsFire = true; // Animation
                Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * gunRange, Color.red);
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, gunRange)) // Shoot ray
                {
                    if (hit.transform.CompareTag("Enemy")) // Zombie ?
                    {
                        EffectPool.Instance.GetEffect("Blood").transform.position = hit.point;
                        hit.transform.gameObject.GetComponent<ZombieBase>().NextState(ZombieBase.State.GunHit); // Give damage
                    }
                    else
                    {
                        EffectPool.Instance.GetEffect("Normal").transform.position = hit.point;
                        return;
                    }
                }
            }
        }
        else
        {
            muzzleEffect.SetActive(false); // hide muzzle
            IsFire = false;
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 10f, Color.green);
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 10f, 1 << LayerMask.NameToLayer("Props")))
            {

                if(_InputManager.E) // Get
                {
                    Debug.Log(hit.transform.name);
                    if (hit.transform.CompareTag("DropGun")) // Is gun?
                    {
                        GunMode(); // Change mode
                        Destroy(hit.transform.gameObject); // destroy grounded prop
                    }
                }
            }
        }
    }
    private void Reload()
    {
        if(_InputManager.Reload) // Input R
        {
            _Animator.SetTrigger("IsReload"); // reload animation
        }
    }
    private void OnReloadEvent() // When start reload animation ends
    {
        int need = maxBulletCount - bulletCount; // How many bullets I need?
        if(need <= haveBulletCount) // I have enough bullets
        {
            bulletCount += need;
            haveBulletCount -= need;
        }
        else // I have not enough bullets
        {
            bulletCount += haveBulletCount;
            haveBulletCount = 0;
        }
    }
    private void SlotChange() // Num1 Num2
    {
        if(_InputManager.Slot1 && IsGun == false) // Not equip gun, Have gun
        {
            GunMode();
        }
        else if(_InputManager.Slot2 && IsGun == true) // Equip Axe
        {
            AxeMode();
        }

    }
    private void GunMode()
    {
        IsGun = true;

        realGun.SetActive(true);
        muzzleEffect.SetActive(false);
        realAxe.SetActive(false);

        fakeGun.SetActive(false);
        fakeAxe.SetActive(true);
    }
    private void AxeMode()
    {
        IsGun = false;

        realGun.SetActive(false);
        realAxe.SetActive(true);

        fakeGun.SetActive(true);
        fakeAxe.SetActive(false);
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
