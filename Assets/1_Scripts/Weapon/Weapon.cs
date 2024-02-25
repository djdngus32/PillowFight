using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Weapon : NetworkBehaviour
{
    [SerializeField] private DamageTrigger damageTrigger;
    [SerializeField] private int damage = 10;
    [SerializeField] private float attackSpeed = 1f;
    
    public float AttackSpeed => attackSpeed;
    public float AttackCooldownTime => 1 / attackSpeed;

    public override void Spawned()
    { 
        if(damageTrigger.IsEnable == true)
        {
            damageTrigger.DisableDamageTrigger();
        }
    }
    
    public void StartAttack()
    {
        if(damageTrigger != null)
        {
            //데미지 트리거 활성화
            damageTrigger.EnableDamageTrigger(Object.StateAuthority, damage);
        }
    }

    public void EndAttack()
    {
        if (damageTrigger != null)
        {
            damageTrigger.DisableDamageTrigger();
        }
    }
}
