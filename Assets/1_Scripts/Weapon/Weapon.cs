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

    private int fireTicks;

    [Networked] private int fireCount { get; set; }
    [Networked] private TickTimer fireCooldown { get; set; }

    public override void Spawned()
    {
        float fireTime = 60f / FireRate; ;
        fireTicks = Mathf.CeilToInt(fireTime / Runner.DeltaTime);
    }

    public void Fire(Vector3 firePosition, Vector3 fireDirection)
    {
        if (fireCooldown.ExpiredOrNotRunning(Runner) == false)
            return;

        Attack(firePosition, fireDirection);
        fireCooldown = TickTimer.CreateFromSeconds(Runner, 0.1f);
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
