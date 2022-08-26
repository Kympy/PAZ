using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieBase : MonoBehaviour
{
    public enum State
    {
        Null,
        Idle,
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

    protected Animator _Animator;

    protected bool isDead = false;

    protected Coroutine previousCoroutine = null;
    protected State currentState = State.Null;
    protected WaitForSeconds findDelay = new WaitForSeconds(0.5f);
    protected Rigidbody _Rigidbody;

    // View Range
    [SerializeField, Range(0, 10)] private float viewRadius;
    [SerializeField, Range(0, 360)] private float viewAngle;

    public float ViewRadius { get { return viewRadius; } }
    public float ViewAngle { get { return viewAngle; } }

    // LayerMask
    private LayerMask targetMask;
    private LayerMask obstacleMask;

    // Targets which in Radius
    private Collider[] targetsInViewRadius = null;

    // Target mask에 ray hit된 transform을 보관하는 리스트
    private Transform visibleTargets = null;
    public Transform VisibleTargets { get { return visibleTargets; } }

    // temp memories
    private Transform targetTransform;
    private Vector3 dir2Target;
    private float distance2Target;
    public virtual void Awake()
    {
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
        Debug.Log("Idle !!");
        while(true)
        {
            FindVisibleTarget();
            yield return findDelay;
            //yield return null;
        }
    }
    public IEnumerator Move_State()
    {
        Debug.Log("Move!!");

        while(true)
        {
            if (visibleTargets == null)
            {
                NextState(State.Idle);
                yield break;
            }
            if ((visibleTargets.position - transform.position).magnitude > 10f)
            {
                NextState(State.Idle);
                yield break;
            }
            if((visibleTargets.position - transform.position).magnitude < attackRange)
            {
                NextState(State.Attack);
            }
            else
            {
                transform.position += (visibleTargets.position - transform.position).normalized * walkSpeed * Time.deltaTime;
            }
            Debug.Log(visibleTargets.name);
            yield return null;
        }
    }
    public IEnumerator Attack_State()
    {
        Debug.Log("Attack!!");
        yield return null;
    }
    public IEnumerator Hit_State()
    {
        _Animator.SetTrigger("HitForward");
        yield return null;
    }
    public IEnumerator BackHit_State()
    {
        _Animator.SetTrigger("HitBackward");
        yield return null;
    }
    public IEnumerator Death_State()
    {
        yield return null;
    }
    public void OnAttackEvent()
    {

    }
    public void OnHitEvent()
    {
        NextState(State.Idle);
    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.contacts[0].normal.normalized);
        if (collision.gameObject.CompareTag("Weapon") && collision.contacts[0].normal.normalized.z >= 0)
        {
            NextState(State.Hit);
        }
        else if(collision.gameObject.CompareTag("Weapon") && collision.contacts[0].normal.normalized.z < 0)
        {
            NextState(State.BackHit);
        }
    }
    private void FindVisibleTarget()
    {
        // Get targetMask collider which in view Radius
        targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
        Debug.Log(targetMask.value);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            targetTransform = targetsInViewRadius[i].transform;
            dir2Target = (targetTransform.position - transform.position).normalized;

            // Player & Zombie Angle is less than View Angle / 2
            if (Vector3.Angle(transform.forward, dir2Target) < viewAngle / 2)
            {
                distance2Target = Vector3.Distance(transform.position, targetTransform.transform.position);

                // 타겟으로 가는 레이캐스트에 obstacleMask가 걸리지 않으면 visibleTargets에 Add
                if (!Physics.Raycast(transform.position, dir2Target, distance2Target, obstacleMask))
                {
                    visibleTargets = targetTransform;
                    Debug.Log(visibleTargets.name);
                    NextState(State.Move);
                }
            }
        }
    }
    public Vector3 DirFromAngle(float angleDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleDegrees += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Cos((-angleDegrees + 90) * Mathf.Deg2Rad), 0, Mathf.Sin((-angleDegrees + 90) * Mathf.Deg2Rad));
    }

}
