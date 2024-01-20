using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerStat : NetworkBehaviour
{
    [SerializeField] private float maxHP = 100f;
    [SerializeField] private float invincibilityDurationAfterSpawn = 2f;

    public bool IsAlive => CurrentHP > 0f;

    public bool IsInvincibility => invincibilityTimer.ExpiredOrNotRunning(Runner) == false;

    [Networked] public float CurrentHP { get;private set; }
    [Networked] private TickTimer invincibilityTimer { get; set; }

    public override void Spawned()
    {
        if(HasStateAuthority)
        {
            CurrentHP = maxHP;

            invincibilityTimer = TickTimer.CreateFromSeconds(Runner, invincibilityDurationAfterSpawn);

            PlayerManager.Instance.onChangedHP?.Invoke((int)CurrentHP);
        }
    }

    public bool ApplyDamage(PlayerRef dealer, float damage, Vector3 position, Vector3 direction)
    {
        if (CurrentHP <= 0f)
            return false;

        if (IsInvincibility)
            return false;

        CurrentHP -= damage;

        if(CurrentHP<=0f)
        {
            CurrentHP = 0f;

            //磷澜 贸府 せせせせ
            Debug.Log("Dead!");
        }

        

        return true;            
    }

    [Rpc(RpcSources.All , RpcTargets.StateAuthority)]
    public void RPC_ApplyDamage(PlayerRef dealer, float damage)
    {
        if (dealer == Object.StateAuthority)
            return;

        if (CurrentHP <= 0f)
            return;

        if (IsInvincibility)
            return;

        CurrentHP -= damage;

        if (CurrentHP <= 0f)
        {
            CurrentHP = 0f;

            //磷澜 贸府 せせせせ
            Debug.Log("Dead!");
        }

        PlayerManager.Instance.onChangedHP?.Invoke((int)CurrentHP);
    }
}
