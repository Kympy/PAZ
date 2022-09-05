using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalAttack : MonoBehaviour
{
    private float attackPower;
    private void Start() // Because data is initialized at Awake()
    {
        attackPower = GetComponentInParent<ZombieBase>().AttackPower;
        Debug.Log("Attack power : " + attackPower);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            other.gameObject.SendMessage("DecreaseHP", attackPower);
        }
    }
}
