using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(InputManager))]
public class PlayerController : MonoBehaviour
{
    [HideInInspector] private InputManager _InputManager;
    [HideInInspector] private Animator _Animator;
    [HideInInspector] private Rigidbody _Rigidbody;

    [HideInInspector] private Vector3 moveVector; // Desired Movement Vector
    [HideInInspector] private Vector3 relativeVector;
    [HideInInspector] private Vector3 finalVector = Vector3.zero;
    private Vector3 originFocusPoint; // Original position of focus point

    private Vector3 gunRayOffset = Vector3.zero;

    private float moveSpeed = 0f; // Use to walk and run speed
    private float turnDirection = 0f; // Use to rotate character
    private float fireTimer = 0f; // Fire timer
    private const float fireTime = 0.15f; // My gun fire rate
    // Bullet
    public int bulletCount = 40; // current Ammo count
    private const int maxBulletCount = 40; // Max Ammo
    public int haveBulletCount = 0;
    // Health
    public int itemCount = 2;
    // HP
    public float currentHP;
    public float MaxHP = 1000f;
    // Key
    public int keyCount = 0;

    [SerializeField, Range(0f, 50f)] private float WalkSpeed = 4f;
    [SerializeField, Range(0f, 50f)] private float RunSpeed = 8f;
    [SerializeField, Range(0f, 50f)] private float BackSpeed = 4f;

    [SerializeField, Range(0f, 100f)] private float jumpPower = 5.5f;
    [SerializeField, Range(0f, 0.5f)] private float groundClearance = 0.25f;
    [SerializeField, Range(0f, 1f)] private float groundDistance = 0.25f;

    [SerializeField, Range(0f, 200f)] private float mouseSensitivity_X = 100f;
    //[SerializeField, Range(0f, 200f)] private float mouseSensitivity_Y = 100f;
    [SerializeField, Range(0f, 500f)] private float turnMultiplier = 300f;
    [SerializeField, Range(0f, 1000f)] private float gunRange = 500f;

    // Player spine
    private Transform upperBody = null;

    private GameObject focusPoint; // Camera point
    // Hand
    private GameObject realAxe = null;
    private GameObject realGun = null;
    // Back
    private GameObject fakeAxe = null;
    private GameObject fakeGun = null;
    // Muzzle effect
    private GameObject muzzleEffect = null;
    // Casing
    private GameObject casingEffect = null;

    private bool IsMove = false;
    private bool IsJump = false;
    private bool IsFalling = false;
    private bool IsAim = false;
    private bool IsGun = false;
    private bool IsFire = false;
    private bool Reloading = false;
    private bool IsDead = false;
    public bool HasGun = false;

    private bool Stop = false;
    private bool CursorLocked = false;
    private bool CoroutineAlready = false;
    // timer
    private WaitForSecondsRealtime oneSec = new WaitForSecondsRealtime(1f);
    // Cam
    private CinemachineVirtualCamera followCam = null;
    private CinemachineVirtualCamera deathCam = null; // When player die
    // temp
    private RaycastHit hit; // temp hit
    private float xRotate;
    private Bullet temp;
    private Vector3 originFocus;
    private void OnDestroy()
    {
        StopAllCoroutines();
    }
    private void Awake()
    {
        currentHP = MaxHP;
        _InputManager = GetComponent<InputManager>();
        _Animator = GetComponent<Animator>();
        _Rigidbody = GetComponent<Rigidbody>();
        followCam = GameObject.FindGameObjectWithTag("FollowCam").GetComponent<CinemachineVirtualCamera>();
        deathCam = GameObject.FindGameObjectWithTag("DeathCam").GetComponent<CinemachineVirtualCamera>();
        deathCam.enabled = false;
        //GetComponent<Collider>().enabled = true;

        fakeAxe = GameObject.FindGameObjectWithTag("FakeAxe");
        fakeGun = GameObject.FindGameObjectWithTag("FakeGun");

        realAxe = GameObject.FindGameObjectWithTag("RealAxe");
        realAxe.GetComponent<CapsuleCollider>().isTrigger = true;

        realGun = GameObject.FindGameObjectWithTag("RealGun");

        fakeGun.SetActive(false);
        fakeAxe.SetActive(false);

        muzzleEffect = GameObject.FindGameObjectWithTag("Muzzle");
        casingEffect = GameObject.FindGameObjectWithTag("Casing");

        muzzleEffect.SetActive(false);
        casingEffect.SetActive(false);
        realGun.SetActive(false);

        upperBody = _Animator.GetBoneTransform(HumanBodyBones.Spine);
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void Start()
    {
        focusPoint = GameObject.Find("Focus");
        StartCoroutine(GetPlayerVelocity());
    }
    private void FixedUpdate()
    {
        if (IsDead == false && Stop == false)
        {
            Movement();
        }
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            GameManager.Instance.Test();
        }
        if (IsDead == false && Stop == false)
        {
            IsMove = _InputManager.HasVerticalInput || _InputManager.HasHorizontalInput;
            IsJump = _InputManager.Jump;
            IsAim = _InputManager.RightClicking;

            MouseCamera();
            Jump();
            Aim();
            Fire();
            Reload();
            if (Input.GetKeyDown(KeyCode.F))
            {

                DecreaseHP(100f);
            }

            SlotChange();
            ShowMap();
            AnimationPlay();
            ResetLegMovement();
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Stop = !Stop;
            UIManager.Instance.ShowESC(Stop);
        }
    }
    private void LateUpdate()
    {
        if (IsDead == false && Stop == false)
        {
            UpperBodyRotate();
        }
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
                transform.Rotate(Time.deltaTime * turnDirection * turnMultiplier * transform.up); // Rotate Character
                focusPoint.transform.parent.Rotate(Time.deltaTime * -turnDirection * turnMultiplier * transform.up);
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
            if(IsAim) // When aiming, only can walk
            {
                moveSpeed = WalkSpeed;
            }
            else // Not aiming can run
            {
                if (_InputManager.Vertical > 0 && _InputManager.Sprint == false)
                {
                    if (moveSpeed > WalkSpeed)
                    {
                        moveSpeed -= 0.1f;
                        if (moveSpeed < WalkSpeed) moveSpeed = WalkSpeed;
                    }
                    else moveSpeed = WalkSpeed;
                }
                else if (_InputManager.Vertical > 0 && _InputManager.Sprint)
                {
                    moveSpeed += 0.1f;
                    if (moveSpeed > RunSpeed) moveSpeed = RunSpeed;
                }
                else if (_InputManager.Vertical < 0)
                {
                    if (moveSpeed > BackSpeed)
                    {
                        moveSpeed = BackSpeed;
                    }
                    else moveSpeed = BackSpeed;
                }
                else if (_InputManager.Horizontal != 0)
                {
                    moveSpeed = WalkSpeed;
                }
                else
                {
                    moveSpeed -= 0.1f;
                    if (moveSpeed < 0f) moveSpeed = 0f;
                }
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

        xRotate = focusPoint.transform.localEulerAngles.x - Input.GetAxis("Mouse Y") * Time.deltaTime * 100f; // Get X rotate
        xRotate = xRotate > 180 ? xRotate - 360 : xRotate; // Get X rotate when x have minus value
        xRotate = Mathf.Clamp(xRotate, -25, 60); // Clamp angles

        // Vertical
        focusPoint.transform.eulerAngles = new Vector3(xRotate, focusPoint.transform.eulerAngles.y, 0f);
        
        // Horizontal
        focusPoint.transform.parent.Rotate(Input.GetAxis("Mouse X") * mouseSensitivity_X * Time.deltaTime * transform.up);

        if(Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            followCam.m_Lens.FieldOfView = -Input.GetAxis("Mouse ScrollWheel") * 10f + followCam.m_Lens.FieldOfView;
        }
    }
    private void UpperBodyRotate()
    {
        upperBody.Rotate(0f, 0f, xRotate);
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
        if(_InputManager.RightClickUp || _InputManager.ShiftUp || _InputManager.Slot1 || _InputManager.Slot2) // Aim Down and Stop run
            _Animator.SetTrigger("ResetLower"); // Leg movement reset
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
        if (_InputManager.LeftClick && IsGun == false)
        {
            _Animator.SetTrigger("First");
            return;
        }
        if (_InputManager.LeftClicking && IsAim == false) // Rotate Forward
        {
            transform.Rotate(Time.deltaTime * turnDirection * turnMultiplier * 5f * transform.up); // Rotate Character
            focusPoint.transform.parent.Rotate(Time.deltaTime * -turnDirection * turnMultiplier * 5f * transform.up);
        }
        if (IsGun && _InputManager.LeftClick && bulletCount > 0 && Reloading == false) // First shoot
        {
            fireTimer = fireTime;
        }
        if (IsGun && _InputManager.LeftClicking && bulletCount > 0 && Reloading == false) // Shooting
        {
            fireTimer += Time.deltaTime; // timer Start
            if(fireTimer >= fireTime) // Satisfied fire rate
            {
                fireTimer = 0f; // reset
                bulletCount--; // Count down bullet
                UIManager.Instance.SetBulletUI(bulletCount, haveBulletCount); // UI Update

                muzzleEffect.SetActive(true); // show muzzle
                casingEffect.SetActive(true); // Show case
                IsFire = true; // Animation

                temp = BulletPool.Instance.GetBullet().GetComponent<Bullet>(); // Get Bullet obj
                temp.transform.position = muzzleEffect.transform.position; // set pos
                temp.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward * gunRange); // set rot
     
                Debug.DrawRay(followCam.transform.position + gunRayOffset, followCam.transform.forward * gunRange, Color.red);
                if (Physics.Raycast(Camera.main.transform.position + gunRayOffset, Camera.main.transform.forward, out hit, gunRange)) // Shoot ray
                {
                    if (hit.transform.CompareTag("Enemy")) // Is that a Zombie ?
                    {
                        EffectPool.Instance.GetEffect("Blood").transform.position = hit.point;
                        hit.transform.gameObject.GetComponent<ZombieBase>().NextState(ZombieBase.State.GunHit); // Give a damage
                    }
                    else // Hit ground
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
            casingEffect.SetActive(false); // hide casing
            IsFire = false;
            Debug.DrawRay(Camera.main.transform.position + gunRayOffset, Camera.main.transform.forward * 13f, Color.green);
            if (Physics.Raycast(Camera.main.transform.position + gunRayOffset, Camera.main.transform.forward, out hit, 13f, 1 << LayerMask.NameToLayer("Props")))
            {

                if(_InputManager.E) // Get
                {
                    Debug.Log(hit.transform.name);
                    if (hit.transform.CompareTag("DropGun") && HasGun == false) // Is gun? & I dont have a gun
                    {
                        HasGun = true;
                        GunMode(); // Change mode
                        Destroy(hit.transform.gameObject); // destroy grounded prop
                    }
                    else if(hit.transform.CompareTag("DropHealth")) // Health recover items
                    {
                        itemCount++;
                        UIManager.Instance.UpdateItemCount(itemCount);
                        Destroy(hit.transform.gameObject);
                    }
                    else if(hit.transform.CompareTag("DropBullet")) // Bullet items
                    {
                        haveBulletCount += (int)(Random.Range(15, 51));
                        UIManager.Instance.SetBulletUI(bulletCount, haveBulletCount);
                        Destroy(hit.transform.gameObject);
                    }
                    else if(hit.transform.CompareTag("Key"))
                    {
                        keyCount++;
                        Destroy(hit.transform.gameObject);
                    }
                }
            }
        }
    }
    private void Aim()
    {
        if(IsAim)
        {
            StopCoroutine(nameof(AimDown));
            followCam.m_Lens.FieldOfView = Mathf.Lerp(followCam.m_Lens.FieldOfView, 35f, Time.deltaTime * 8f);
            transform.Rotate(Time.deltaTime * turnDirection * turnMultiplier * 5f * transform.up); // Rotate Character
            focusPoint.transform.parent.Rotate(Time.deltaTime * -turnDirection * turnMultiplier * 5f * transform.up);
        }
        else if(Input.GetMouseButtonUp(1))
        {
            StartCoroutine(AimDown());
        }
    }
    private IEnumerator AimDown()
    {
        while(true)
        {
            followCam.m_Lens.FieldOfView += 5f;
            if (followCam.m_Lens.FieldOfView > 50f)
            {
                followCam.m_Lens.FieldOfView = 50f;
                yield break;
            }
            yield return null;
        }
    }
    private void Reload()
    {
        if(IsGun && _InputManager.Reload && bulletCount < maxBulletCount && Reloading == false && haveBulletCount > 0) // Input R
        {
            _Animator.SetTrigger("IsReload"); // reload animation
            Reloading = true;
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
        Reloading = false;
        UIManager.Instance.SetBulletUI(bulletCount, haveBulletCount);
    }
    private void SlotChange() // Num1 Num2
    {
        if(_InputManager.Slot1 && IsGun == false && HasGun) // Not equip gun, Have gun
        {
            GunMode();
        }
        else if(_InputManager.Slot2 && IsGun == true) // Equip Axe
        {
            AxeMode();
        }
        else if(_InputManager.Slot3 && itemCount > 0) // Item Use
        {
            currentHP += MaxHP * 0.5f;
            if (currentHP > MaxHP) currentHP = MaxHP;
            UIManager.Instance.UpdateBar(currentHP, MaxHP);
        }
    }
    public void GunMode()
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
    private void ShowMap()
    {
        if (_InputManager.TabKey)
        {
            UIManager.Instance.ShowMap(true);
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            UIManager.Instance.ShowMap(false);
        }
    }
    public void DecreaseHP(float damage) // Get Damage
    {
        currentHP -= damage;
        _Animator.SetTrigger("IsHit");
        UIManager.Instance.StopUpdateBar();
        UIManager.Instance.UpdateBar(currentHP, MaxHP);
        Die();
    }
    public void Die()
    {
        if(currentHP <= 0)
        {
            IsDead = true;
            GetComponent<Collider>().enabled = false;
            _Rigidbody.useGravity = false;
            _Animator.SetTrigger("IsDead");
            deathCam.enabled = true;
            followCam.enabled = false;
        }
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
