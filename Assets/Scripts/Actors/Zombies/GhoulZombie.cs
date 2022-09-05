using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhoulZombie : ZombieBase
{
    private BoxCollider attackCollider = null;

    private WaitForSeconds getUpTime = new WaitForSeconds(2f);

    private float jumpAttackRange;

    private ZombieData data = new ZombieData();
    public override void Awake()
    {
        base.Awake();
        attackCollider = GetComponentInChildren<BoxCollider>();
        data = DataManager.Instance.GetZombieData("Ghoul");
        InitData(data);
    }
    public override void Start()
    {
        // Time
        _Agent.stoppingDistance = attackRange;
        rotateSpeed = 5f;
        attackTime = new WaitForSeconds(1.2f);
        hitFrontTime = new WaitForSeconds(1.2f);
        hitBackTime = new WaitForSeconds(2.15f);
        base.Start();
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
        jumpAttackRange = attackRange * 6f;
    }
    public override IEnumerator Idle_State() // Lay
    {
        findTimer = 0.5f;
        _Agent.enabled = true;
        _Agent.isStopped = true;
        _Agent.speed = 0f;

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
                }
            }
        }
    }
    public override IEnumerator Scream_State() // >> GetUp_State
    {
        StopAgent();
        _Animator.SetTrigger("GetUp"); // Get up
        yield return getUpTime;
        NextState(State.Attack);
    }
    public override IEnumerator Move_State() // Run State. Chase Player to Attack Range
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
            if ((visibleTarget.transform.position - transform.position).magnitude <= jumpAttackRange) // Player is in my attack range
            {
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
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(visibleTarget.transform.position - transform.position), 0.5f);
            }

            if ((visibleTarget.transform.position - transform.position).magnitude <= jumpAttackRange) // Player is in my attack range
            {
                _Animator.SetTrigger("JumpAttack");
                _Rigidbody.AddForce(transform.forward * jumpPower, ForceMode.Impulse);
                yield return attackTime;
            }
            else if ((visibleTarget.transform.position - transform.position).magnitude <= attackRange)
            {
                _Animator.SetTrigger("IsAttack"); // Attack Animation
                yield return attackTime;
            }
            else // Player is far from attack range
            {
                NextState(State.Move);
                yield break;
            }
            yield return null;
        }
    }
    public override IEnumerator Death_State()
    {
        StopAgent();
        _Agent.enabled = false;
        _Rigidbody.useGravity = false;
        this.GetComponent<Collider>().enabled = false;
        _Animator.SetTrigger("DeathForward");
        yield return deadBodyTime;
        Destroy(this.gameObject);
    }
    public override IEnumerator BackDeath_State()
    {
        StopAgent();
        _Agent.enabled = false;
        _Rigidbody.useGravity = false;
        this.GetComponent<Collider>().enabled = false;
        _Animator.SetTrigger("DeathBackward");
        yield return deadBodyTime;
        Destroy(this.gameObject);
    }
    public IEnumerator Null_State()
    {
        yield return null;
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
