using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieBase : MonoBehaviour
{
    public enum State
    {
        Null,
        Idle,
        Scream,
        Move,
        Attack,
        Hit,
        BackHit,
        Death,

        MAX
    }

    protected float currentHP;
    protected float MaxHP;
    protected float walkSpeed;
    protected float runSpeed;
    protected float attackPower;
    protected float jumpPower;

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
    protected const float moveOffset = 10f; // Move position random range
    protected Vector3 desiredPos;

    // View Range
    [SerializeField, Range(0, 20)] private float viewRadius;
    [SerializeField, Range(0, 360)] private float viewAngle;

    // Properties
    public float ViewRadius { get; set; }
    public float ViewAngle { get; set; }

    // LayerMask
    private LayerMask targetMask;
    private LayerMask obstacleMask;

    // Targets which in Radius
    private Collider[] targetsInViewRadius = null;

    // Target mask�� ray hit�� transform�� �����ϴ� ����Ʈ
    private GameObject visibleTarget = null;
    public GameObject VisibleTarget { get; }

    // temp memories
    private Transform targetTransform;
    private Vector3 dir2Target;
    private float distance2Target;

    public virtual void Awake()
    {
        _Agent = GetComponent<NavMeshAgent>();
        _Animator = GetComponent<Animator>();
        _Rigidbody = GetComponent<Rigidbody>();
        targetMask = 1 << LayerMask.NameToLayer("Player");
        obstacleMask = 1 << LayerMask.NameToLayer("Terrain");
    }
    public virtual void Start()
    {
        InitStat();
        currentHP = MaxHP;
        NextState(State.Idle);
    }
    public virtual void InitStat() { }

    public void DecreaseHP(float Damage)
    {
        currentHP -= Damage;
    }
    public void NextState(State NewState) // Get Next Coroutine
    {
        if (NewState == currentState) return; // If new state is same with current state, exit
        if (previousCoroutine != null) StopCoroutine(previousCoroutine); // If there is available previous coroutine, remove it.

        currentState = NewState; // Assign new state
        previousCoroutine = StartCoroutine(currentState.ToString() + "_State");
    }
    public IEnumerator Idle_State()
    {
        findTimer = 0.5f;
        moveTimer = 3f;
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
                desiredPos.Set(Random.Range(transform.position.x - moveOffset, transform.position.x + moveOffset),
                    transform.position.y, Random.Range(transform.position.z - moveOffset, transform.position.z + moveOffset));
                moveTimer = 0f;
                Debug.Log("## 01 IDLE / Set Random Pos : " + desiredPos);
            }
            if(_Agent.pathPending == false)
            {
                _Agent.SetDestination(desiredPos);
            }
            yield return null;
        }
    }
    public IEnumerator Scream_State()
    {
        StopAgent();
        _Animator.SetTrigger("IsScream"); // Do Scream
        yield return screamTime;
        NextState(State.Move);
    }
    public IEnumerator Move_State() // Run State. Chase Player to Attack Range
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

                Debug.Log("## 02 MOVE / Chasing Target : " + visibleTarget.name + " Position : " + visibleTarget.transform.position);
            }
            yield return null;
        }
    }
    public IEnumerator Attack_State()
    {
        StopAgent();
        while (true)
        {
            if (visibleTarget == null)
            {
                _Animator.SetTrigger("MissPlayer");
                Debug.Log("## 03 ATTACK / Name : " + this.name + " Miss Player.");
                NextState(State.Idle);
                yield break;
            }
            if ((visibleTarget.transform.position - transform.position).magnitude <= attackRange) // Player is in my attack range
            {
                _Animator.SetTrigger("IsAttack");
                yield return attackTime;
                Debug.Log("## 03 ATTACK / Name : " + this.name + " Attacked Player!!");
            }
            else // Player is far from attack range
            {
                NextState(State.Move); // Chase
                yield break;
            }
            yield return null;
        }
    }
    public IEnumerator Hit_State()
    {
        StopAgent();
        _Animator.SetTrigger("HitForward");
        Debug.Log("## 04 HIT FORWARD");
        yield return hitBackTime;
        NextState(State.Idle);
    }
    public IEnumerator BackHit_State()
    {
        StopAgent();
        _Animator.SetTrigger("HitBackward");
        Debug.Log("## 04 HIT BACKWARD");
        yield return hitFrontTime;
        NextState(State.Idle);
    }
    public IEnumerator Death_State()
    {
        _Agent.enabled = false;
        yield return null;
    }
    public void OnAttackEvent()
    {

    }
    public void OnHitEvent()
    {
        //NextState(State.Idle);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Weapon") && collision.contacts[0].normal.normalized.z >= 0)
        {
            NextState(State.Hit);
        }
        else if (collision.gameObject.CompareTag("Weapon") && collision.contacts[0].normal.normalized.z < 0)
        {
            NextState(State.BackHit);
        }
    }
    private void FindVisibleTarget()
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
                    Debug.Log("## 01 - 1 FIND / Find Target Name : " + visibleTarget.name + "  Position : " + visibleTarget.transform.position);
                    _Animator.SetBool("IsIdle", false);
                    NextState(State.Scream); // Do Scream
                }
            }
        }
    }
    public void StopAgent() // Stop Agent movement
    {
        _Agent.isStopped = true;
        _Agent.velocity = Vector3.zero;
    }
#if UNITY_EDITOR
    public Vector3 DirFromAngle(float angleDegrees, bool angleIsGlobal)
    {
        if (angleIsGlobal == false)
        {
            angleDegrees += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Cos((-angleDegrees + 90) * Mathf.Deg2Rad), 0, Mathf.Sin((-angleDegrees + 90) * Mathf.Deg2Rad));
    }
#endif
}
