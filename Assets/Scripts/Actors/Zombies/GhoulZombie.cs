using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhoulZombie : ZombieBase
{
    private BoxCollider attackCollider = null;

    private WaitForSeconds getUpTime = new WaitForSeconds(7f);
    private WaitForSeconds jumpAttackTime = new WaitForSeconds(3.21f);

    private float jumpAttackRange;
    private bool secondDeath = false;

    private ZombieData data = new ZombieData();

    public override void Awake()
    {
        base.Awake();
        attackCollider = GetComponentInChildren<BoxCollider>();
        attackCollider.enabled = false;
        data = DataManager.Instance.GetZombieData("Ghoul");
        currentState = State.Null;
        _Agent.enabled = true;
        StopAgent();
        InitData(data);
    }
    public override void Start()
    {
        // Time
        _Agent.stoppingDistance = attackRange;
        rotateSpeed = 5f;
        attackTime = new WaitForSeconds(2.2f);
        hitFrontTime = new WaitForSeconds(1.2f);
        hitBackTime = new WaitForSeconds(2.15f);
        NextState(State.Idle);
    }
    public override void InitData(ZombieData myData) // Get Data
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
        jumpPower = 5f;
        jumpAttackRange = 10f;
    }
    public override void NextState(State NewState)
    {
        base.NextState(NewState);
    }
    public override IEnumerator Idle_State() // Lay
    {
        findTimer = 0.5f;
        StopAgent();

        while (true)
        {
            findTimer += Time.deltaTime; // Timer

            if (findTimer > 0.5f) // Find time
            {
                FindVisibleTarget(); // Find Player
                findTimer = 0f;
            }
            yield return null;
        }
    }
    public override void FindVisibleTarget()
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
                    NextState(State.Scream); // Do Get up state
                    break;
                }
            }
        }
    }
    public override IEnumerator Scream_State() // >> GetUp_State
    {
        StopAgent();
        ZombieIcon.SetActive(true);
        _Animator.SetTrigger("GetUp"); // Get up
        yield return getUpTime;
        NextState(State.Move);
    }
    public override IEnumerator Move_State() // Run State. Chase Player to Attack Range
    {
        Debug.Log("Move");
        _Agent.isStopped = false;
        _Agent.speed = runSpeed;
        _Animator.SetBool("IsMove", true);
        while (true)
        {
            if (visibleTarget == null) // Not find player yet
            {
                _Animator.SetBool("IsMove", false);
                NextState(State.Idle);
                yield break;
            }
            if ((visibleTarget.transform.position - transform.position).magnitude <= jumpAttackRange) // Player is in my attack range
            {
                _Animator.SetBool("IsMove", false);
                NextState(State.Attack);
                yield break;
            }
            else // Chase Player
            {
                if (_Agent.pathPending == false)
                {
                    _Agent.SetDestination(visibleTarget.transform.position);
                }
            }
            yield return null;
        }
    }
    public override IEnumerator Attack_State()
    {
        Debug.Log("Attack");
        StopAgent();
        while (true)
        {
            if (visibleTarget == null)
            {
                _Animator.SetTrigger("MissPlayer");
                NextState(State.Idle);
                yield break;
            }
            else
            {
                // Look Player
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(visibleTarget.transform.position - transform.position), 3f);
                //transform.LookAt(visibleTarget.transform.position);
            }

            if ((visibleTarget.transform.position - transform.position).magnitude <= attackRange) // Melee Attack range
            {
                _Animator.SetTrigger("IsAttack"); // Attack Animation
                yield return attackTime;
            }
            else if ((visibleTarget.transform.position - transform.position).magnitude <= jumpAttackRange) // Far enough >> Jump Attack
            {
                _Animator.SetTrigger("JumpAttack");

                //if(jumpCoroutine == null)
                //{
                    //jumpCoroutine = StartCoroutine(JumpAttack());
                //}
                yield return jumpAttackTime;
            }
            else // Player is far but, closer than jump attack range
            {
                NextState(State.Move);
                yield break;
            }
            yield return null;
        }
    }
    public override IEnumerator Null_State()
    {
        _Animator.SetBool("IsCrawl", true);
        _Agent.enabled = true;
        _Agent.isStopped = false;
        _Agent.speed = walkSpeed;
        _Rigidbody.useGravity = true;
        _Rigidbody.isKinematic = false;
        this.GetComponent<Collider>().isTrigger = false;
        //this.GetComponent<Collider>().enabled = true;
        while (true)
        {
            if (visibleTarget == null) // Not find player yet
            {
                _Animator.SetBool("IsCrawl", false);
                Destroy(this.gameObject, 3f);
                yield break;
            }
            else // Chase Player
            {
                if (_Agent.pathPending == false)
                {
                    _Agent.SetDestination(visibleTarget.transform.position);
                }
            }
            yield return null;
        }
    }
    public override IEnumerator GunHit_State()
    {
        if (currentHP > 0)
        {
            StopAgent();
            GunDamage();
        }
        else
        {
            NextState(State.Death);
        }
        yield return null;
    }
    public override IEnumerator Death_State()
    {
        if (secondDeath)
        {
            StopAgent();
            _Agent.enabled = false;
            _Rigidbody.useGravity = false;
            this.GetComponent<Collider>().enabled = false;
            _Animator.SetBool("IsCrawl", false);
            yield return deadBodyTime;
            Destroy(this.gameObject);
        }
        else
        {
            StopAgent();
            _Agent.enabled = false;
            _Rigidbody.isKinematic = true;
            this.GetComponent<Collider>().isTrigger = true;
            _Animator.SetTrigger("DeathForward");
            if (RandomCase() == 0)
            {
                yield return deadBodyTime;
                Destroy(this.gameObject);
            }
            else
            {
                yield return deadBodyTime;
                secondDeath = true;
                NextState(State.Null);
            }
        }
    }
    public override IEnumerator BackDeath_State()
    {
        NextState(State.Death);
        yield return null;
    }
    private int RandomCase()
    {
        return Random.Range(0, 2);
    }
    public void AttackTriggerOn()
    {
        attackCollider.enabled = true;
    }
    public void AttackTriggerOff()
    {
        attackCollider.enabled = false;
    }
}
