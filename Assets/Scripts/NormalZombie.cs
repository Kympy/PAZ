using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalZombie : ZombieBase
{
    public override void Awake()
    {
        base.Awake();
    }
    public override void Start()
    {
        base.Start();
    }
    public override void InitStat()
    {
        //View
        ViewRadius = 12f;
        ViewAngle = 360f;
        // Range
        attackRange = 2f;
        _Agent.stoppingDistance = attackRange;
        // Speed
        walkSpeed = 1f;
        runSpeed = 5f;
        rotateSpeed = 5f;
        // Time
        attackTime = new WaitForSeconds(4.7f);
        hitFrontTime = new WaitForSeconds(1.2f);
        hitBackTime = new WaitForSeconds(2.15f);
    }
}
