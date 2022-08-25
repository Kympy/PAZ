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
        Death,

        MAX
    }

    protected float currentHP;
    protected float MaxHP;
    protected float walkSpeed;
    protected float runSpeed;
    protected float attackPower;
    protected float jumpPower;

    protected bool isDead = false;

    protected Coroutine previousCoroutine = null;
    protected State currentState = State.Null;

    public virtual void Start()
    {
        currentHP = MaxHP;
        NextState(State.Idle);
    }

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
        yield return null;
    }
    public IEnumerator Move_State()
    {
        yield return null;
    }
    public IEnumerator Attack_State()
    {
        yield return null;
    }
    public IEnumerator Hit_State()
    {
        yield return null;
    }
    public IEnumerator Death_State()
    {
        yield return null;
    }
    public void OnAttackEvent()
    {

    }
}
