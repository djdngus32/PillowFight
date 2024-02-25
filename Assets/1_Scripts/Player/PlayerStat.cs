using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerStat : NetworkBehaviour
{
    [SerializeField] private float maxHP = 100f;
    [SerializeField] private float invincibilityDurationAfterSpawn = 2f;

    public int localDamagedCount;

    public bool IsAlive => CurrentHP > 0f;
    public bool IsInvincibility => invincibilityTimer.ExpiredOrNotRunning(Runner) == false;

    [Networked] public int DamagedCount { get; private set; }
    [Networked] public float CurrentHP { get; private set; }
    [Networked] private TickTimer invincibilityTimer { get; set; }

    public override void Spawned()
    {
        if(HasStateAuthority)
        {
            CurrentHP = maxHP;

            invincibilityTimer = TickTimer.CreateFromSeconds(Runner, invincibilityDurationAfterSpawn);

            PlayerManager.Instance.onChangedHP?.Invoke((int)CurrentHP);
        }

        localDamagedCount = DamagedCount;
    }

    [Rpc(RpcSources.All , RpcTargets.StateAuthority)]
    public void RPC_ApplyDamage(PlayerRef instigator, float damage)
    {
        if (instigator == Object.StateAuthority)
            return;

        if (CurrentHP <= 0f)
            return;

        if (IsInvincibility)
            return;

        CurrentHP -= damage;

        if (CurrentHP <= 0f)
        {
            CurrentHP = 0f;

            //避擠 籀葬 六六六六
            Debug.Log("Dead!");
            StartCoroutine(PlayerManager.Instance.CoRespawnPlayer(5f));
            FightGameManager.Instance.RPC_OnKilledPlayer(instigator, Object.StateAuthority);
        }
        else
        {
            DamagedCount++;
        }

        PlayerManager.Instance.onChangedHP?.Invoke((int)CurrentHP);
    }
}
