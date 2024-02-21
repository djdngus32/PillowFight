using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Weapon : NetworkBehaviour
{
    public float Damage = 10f;
    public int FireRate = 100;
    public LayerMask HitMask;
    public float MaxHitDistance = 100f;

    public int localFireCount;

    private float fireCoolTime;

    [Networked, HideInInspector] public int FireCount { get; set; }
    [Networked] private TickTimer fireCooldown { get; set; }

    public float FireCoolTime => fireCoolTime;
    public float RemainingFireCoolTime => fireCooldown.IsRunning ? fireCooldown.RemainingTime(Runner).Value : 0;
    public float FireRatePerSecond { get; private set; }

    public override void Spawned()
    {
        fireCoolTime = 60f / FireRate;

        FireRatePerSecond = 1 / fireCoolTime;

        localFireCount = FireCount;
    }

    public void Equip(PlayerController ownerPlayer)
    {

    }

    public void Fire(Vector3 firePosition, Vector3 fireDirection)
    {
        if (fireCooldown.ExpiredOrNotRunning(Runner) == false)
            return;

        Attack(firePosition, fireDirection);
        FireCount++;
        fireCooldown = TickTimer.CreateFromSeconds(Runner, fireCoolTime);
    }

    private void Attack(Vector3 firePosition, Vector3 fireDirection)
    {
        Debug.DrawRay(firePosition, fireDirection* MaxHitDistance, Color.red, 5f);
        if (Physics.Raycast(firePosition, fireDirection, out var hit, MaxHitDistance, HitMask))            
        {
            var enemyStat = hit.transform.GetComponent<PlayerStat>();
            if (enemyStat == null || enemyStat.IsAlive == false)
                return;

            enemyStat.RPC_ApplyDamage(Object.InputAuthority, Damage);
        }
    }
}
