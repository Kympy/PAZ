using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieBase : MonoBehaviour
{
    public delegate void DecreaseHP();
    public DecreaseHP decreaseHP;
    public enum State
    {
        Null,
        Idle,
        Scream,
        Move,
        Attack,
        GunHit,
        Hit,
        BackHit,
        Death,
        BackDeath,

        MAX
    }
    public string MyName = "";
    [SerializeField] protected float currentHP;
    [SerializeField] protected float MaxHP;
    [SerializeField] protected float walkSpeed;
    [SerializeField] protected float runSpeed;
    [SerializeField] protected float attackPower;
    [SerializeField] protected float jumpPower;

    protected float attackRange;
    protected float rotateSpeed;

    protected bool isDead = false;

    // Components
    protected NavMeshAgent _Agent;
    protected Animator _Animator;
    protected Rigidbody _Rigidbody;
    // AI
    protected Coroutine previousCoroutine = null;
    protected State currentState = State.Null;
    // Timer & Const Values
    protected float findTimer = 0f; // Find Player Timer
    protected float moveTimer = 0f; // Random Move direction set Timer
    protected WaitForSeconds screamTime = new WaitForSeconds(2.5f); // Scream Anim Time
    protected WaitForSeconds attackTime; // Attack Anim Time
    protected WaitForSeconds hitBackTime;
    protected WaitForSeconds hitFrontTime;
    protected WaitForSeconds deadBodyTime = new WaitForSeconds(7f);
    protected WaitForSeconds gunHitTime = new WaitForSeconds(0.5f);
    protected const float moveOffset = 10f; // Move position random range
    protected Vector3 desiredPos;

    // View Range
    protected float viewRadius;
    protected float viewAngle;

    // Properties
    public float ViewRadius { get { return viewRadius; } set { viewRadius = value; } }
    public float ViewAngle { get { return viewAngle; } set { viewAngle = value; } }

    // LayerMask
    protected LayerMask targetMask;
    protected LayerMask obstacleMask;

    // Targets which in Radius
    protected Collider[] targetsInViewRadius = null;

    // Target mask ray hit target
    protected GameObject visibleTarget = null;
    public GameObject VisibleTarget { get { return visibleTarget; } }
    //Icon
    protected GameObject ZombieIcon = null;

    // temp memories
    protected Transform targetTransform;
    protected Vector3 dir2Target;
    protected float distance2Target;
    public float AttackPower { get { return attackPower; } }
    private void OnEnable()
    {
        _Rigidbody.useGravity = true;
        this.GetComponent<Collider>().enabled = true;
        currentHP = MaxHP;
    }
    public virtual void Awake()
    {
        _Agent = GetComponent<NavMeshAgent>();
        _Agent.enabled = false;
        _Animator = GetComponent<Animator>();
        _Rigidbody = GetComponent<Rigidbody>();
        targetMask = 1 << LayerMask.NameToLayer("Player");
        obstacleMask = 1 << LayerMask.NameToLayer("Terrain");
        ZombieIcon = transform.GetChild(0).gameObject;
        ZombieIcon.SetActive(false);
    }
    public virtual void Start()
    {
        decreaseHP += GunDamage;
        decreaseHP += Die;
        _Agent.enabled = true;
        NextState(State.Idle);
    }
    public virtual void InitData(ZombieData myData) // Get Data and set
    {
        MyName = myData.Name;
        attackPower = myData.AttackPower;
        MaxHP = myData.MaxHP;
        currentHP = MaxHP;
        attackRange = myData.AttackRange;
        viewRadius = myData.ViewRadius;
        viewAngle = myData.ViewAngle;
        walkSpeed = myData.WalkSpeed;
        runSpeed = myData.RunSpeed;
    }
    public void GunDamage()
    {
        currentHP -= 500f;
    }
    public void Die()
    {
        if(currentHP <= 0)
        {
            NextState(State.Death);
        }
    }
    public virtual void NextState(State NewState) // Get Next Coroutine
    {
        if (NewState == currentState) return; // If new state is same with current state, exit
        if (previousCoroutine != null) StopCoroutine(previousCoroutine); // If there is available previous coroutine, remove it.

        currentState = NewState; // Assign new state
        previousCoroutine = StartCoroutine(currentState.ToString() + "_State");
    }
    public virtual IEnumerator Idle_State()
    {
        findTimer = 0.5f;
        moveTimer = 5f;
        _Agent.enabled = true;
        _Agent.isStopped = false;
        _Agent.speed = walkSpeed;
        _Animator.SetBool("IsIdle", true); // -> Patrol Animation == walk
        while(true)
        {
            findTimer += Time.deltaTime; // Timer
            moveTimer += Time.deltaTime;
            if(findTimer > 0.5f) // Find time
            {
                FindVisibleTarget(); // Find Player
                findTimer = 0f;
            }
            if(moveTimer > 5f) // Random Move Timer
            {
                desiredPos = new Vector3(Random.Range(transform.position.x - moveOffset, transform.position.x + moveOffset),
                    transform.position.y, Random.Range(transform.position.z - moveOffset, transform.position.z + moveOffset));
                moveTimer = 0f;
            }
            if(_Agent.pathPending == false)
            {
               _Agent.SetDestination(desiredPos);
            }
            yield return null;
        }
    }
    public virtual IEnumerator Scream_State()
    {
        StopAgent();
        ZombieIcon.SetActive(true);
        _Animator.SetTrigger("IsScream"); // Do Scream
        yield return screamTime;
        NextState(State.Move);
    }
    public virtual IEnumerator Move_State() // Run State. Chase Player to Attack Range
    {
        _Agent.isStopped = false;
        _Agent.speed = runSpeed;
        while (true)
        {
            if (visibleTarget == null) // Not find player yet
            {
                NextState(State.Idle);
                yield break;
            }
            if ((visibleTarget.transform.position - transform.position).magnitude <= attackRange) // Player is in my attack range
            {
                NextState(State.Attack);
                yield break;
            }
            else // Chase Player
            {
                if(_Agent.pathPending == false)
                {
                    _Agent.SetDestination(visibleTarget.transform.position);
                }

                //Debug.Log("## 02 MOVE / Chasing Target : " + visibleTarget.name + " Position : " + visibleTarget.transform.position);
            }
            yield return null;
        }
    }
    public virtual IEnumerator Attack_State()
    {
        StopAgent();
        while (true)
        {
            if (visibleTarget == null)
            {
                _Animator.SetTrigger("MissPlayer");
                //Debug.Log("## 03 ATTACK / Name : " + this.name + " Miss Player.");
                NextState(State.Idle);
                yield break;
            }
            else
            {
                // Look Player
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(visibleTarget.transform.position - transform.position), 0.5f);
                //transform.LookAt(visibleTarget.transform);
            }

            if ((visibleTarget.transform.position - transform.position).magnitude <= attackRange) // Player is in my attack range
            {
                _Animator.SetTrigger("IsAttack"); // Attack Animation
                yield return attackTime;
                //Debug.Log("## 03 ATTACK / Name : " + this.name + " Attacked Player!!");
            }
            else // Player is far from attack range
            {
                NextState(State.Move); // Chase
                yield break;
            }
            yield return null;
        }
    }
    public IEnumerator Hit_State() // Not Use
    {
        StopAgent();
        _Animator.SetTrigger("HitForward");
        //Debug.Log("## 04 HIT FORWARD");
        yield return hitBackTime;
        NextState(State.Idle);
    }
    public IEnumerator BackHit_State()
    {
        StopAgent();
        _Animator.SetTrigger("HitBackward");
        //Debug.Log("## 04 HIT BACKWARD");
        yield return hitFrontTime;
        NextState(State.Idle);
    }
    public virtual IEnumerator GunHit_State() // When hit by bullet
    {
        if (currentHP > 0)
        {
            StopAgent();
            GunDamage();
            _Animator.SetTrigger("GunHit");
            yield return gunHitTime;

            if (visibleTarget != null)
            {
                _Animator.SetTrigger("GoMove");
                NextState(State.Move);
            }
            else
            {
                _Animator.SetTrigger("GoIdle");
                NextState(State.Idle);
            }
        }
        else NextState(State.Death);
    }
    public virtual IEnumerator Death_State()
    {
        StopAgent();
        _Agent.enabled = false;
        _Rigidbody.useGravity = false;
        this.GetComponent<Collider>().enabled = false;
        _Animator.SetTrigger("DeathForward");
        yield return deadBodyTime;
        ZombiePool.Instance.ReturnZombie(this);
    }
    public virtual IEnumerator BackDeath_State() 
    {
        StopAgent();
        _Agent.enabled = false;
        _Rigidbody.useGravity = false;
        this.GetComponent<Collider>().enabled = false;
        _Animator.SetTrigger("DeathBackward");
        yield return deadBodyTime;
        ZombiePool.Instance.ReturnZombie(this);
    }
    public void OnCollisionEnter(Collision collision) // Collision by Axe
    {
        if (collision.gameObject.CompareTag("RealAxe")) // If Axe,
        {
            if(collision.contacts[0].normal.normalized.z >= 0) // Normal Vector's z value is larger than zero.
            {
                NextState(State.Death); // Forward Death Animation
            }
            else
            {
                NextState(State.BackDeath); // Backward Death Animation
            }
        }
    }
    public virtual void FindVisibleTarget()
    {
        // Get targetMask collider which in view Radius
        targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            targetTransform = targetsInViewRadius[i].transform;
            dir2Target = (targetTransform.position - transform.position).normalized;

            // Player & Zombie Angle is less than View Angle / 2
            if (Vector3.Angle(transform.forward, dir2Target) < viewAngle / 2)
            {
                distance2Target = Vector3.Distance(transform.position, targetTransform.transform.position);

                // Only targeting when obstacle doesn't exist
                if (!Physics.Raycast(transform.position, dir2Target, distance2Target, obstacleMask))
                {
                    visibleTarget = targetTransform.gameObject;
                    //Debug.Log("## 01 - 1 FIND / Find Target Name : " + visibleTarget.name + "  Position : " + visibleTarget.transform.position);
                    _Animator.SetBool("IsIdle", false);
                    NextState(State.Scream); // Do Scream
                    
                }
            }
        }
    }
    public virtual IEnumerator Null_State() // Not yet
    {
        yield return null;
    }
    public void StopAgent() // Stop Agent movement
    {
        _Agent.isStopped = true;
        _Agent.speed = 0;
    }
#if UNITY_EDITOR
    public Vector3 DirFromAngle(float angleDegrees, bool angleIsGlobal) // Get angle to draw gizmos
    {
        if (angleIsGlobal == false)
        {
            angleDegrees += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Cos((-angleDegrees + 90) * Mathf.Deg2Rad), 0, Mathf.Sin((-angleDegrees + 90) * Mathf.Deg2Rad));
    }
#endif
}
