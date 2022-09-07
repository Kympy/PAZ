using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalZombie : ZombieBase
{
    private BoxCollider attackCollider = null;
    public override void Awake()
    {
        base.Awake();
        attackCollider = GetComponentInChildren<BoxCollider>();
        attackCollider.enabled = false;
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
    public void AttackTriggerOn() // Enable Arm Collider
    {
        attackCollider.enabled = true;
    }
    public void AttackTriggerOff() // Disable Arm Collider
    {
        attackCollider.enabled = false;
    }
}
