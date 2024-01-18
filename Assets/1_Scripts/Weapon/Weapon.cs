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
        fireCooldown = TickTimer.CreateFromSeconds(Runner, fireTicks);
    }

    private void Attack(Vector3 firePosition, Vector3 fireDirection)
    {
        var hitOptions = HitOptions.IncludePhysX | HitOptions.IgnoreInputAuthority;

        if(Runner.LagCompensation.Raycast(firePosition, fireDirection, MaxHitDistance, Object.InputAuthority, out var hit, HitMask, hitOptions))
        {

        }
    }
}
