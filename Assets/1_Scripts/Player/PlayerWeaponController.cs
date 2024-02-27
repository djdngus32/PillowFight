using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerWeaponController : NetworkBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform attackTransform;

    private int animIDAttack;
    private int visibleAttackCount;

    [Networked, HideInInspector] public Weapon CurrentWeapon { get; set; }
    [HideInInspector] public Weapon[] allWeapons;

    [Networked] private TickTimer AttackCooldownTimer { get; set; }
    [Networked,HideInInspector] private int AttackCount { get; set; }

    private void Awake()
    {
        allWeapons = GetComponentsInChildren<Weapon>();
    }

    public override void Spawned()
    {
        if(HasStateAuthority && allWeapons.Length > 0)
        {
            CurrentWeapon = allWeapons[0];
        }

        animIDAttack = Animator.StringToHash("Attack");
        visibleAttackCount = AttackCount;
    }

    public override void Render()
    {
        if(visibleAttackCount < AttackCount)
        {
            animator.SetTrigger(animIDAttack);
            animator.SetFloat("AttackRatePerSecond", CurrentWeapon.AttackSpeed);
        }

        visibleAttackCount = AttackCount;
    }

    public void Attack()
    {
        if (CurrentWeapon == null)
            return;
        if (AttackCooldownTimer.ExpiredOrNotRunning(Runner) == false)
            return;

        CurrentWeapon.Attack(attackTransform.position, attackTransform.rotation);
        AttackCount++;

        AttackCooldownTimer = TickTimer.CreateFromSeconds(Runner, CurrentWeapon.AttackCooldownTime);
    }
}
