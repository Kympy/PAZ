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
        // Time
        _Agent.stoppingDistance = attackRange;
        rotateSpeed = 5f;
        attackTime = new WaitForSeconds(4f);
        hitFrontTime = new WaitForSeconds(1.2f);
        hitBackTime = new WaitForSeconds(2.15f);
    }
}
