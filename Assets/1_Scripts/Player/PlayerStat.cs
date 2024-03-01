using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerStat : NetworkBehaviour
{
    [SerializeField] private float maxHP = 100f;
    [SerializeField] private float invincibilityDurationAfterSpawn = 2f;
    [SerializeField] private GameObject damageBuffEffect;
    [SerializeField] private GameObject moveSpeedBuffEffect;

    public int localDamagedCount;

    public bool IsAlive => CurrentHP > 0f;
    public bool IsInvincibility => InvincibilityTimer.ExpiredOrNotRunning(Runner) == false;
    public bool IsActiveDamageBuff => DamageBuffTimer.ExpiredOrNotRunning(Runner) == false;
    public bool IsActiveMoveSpeedBuff => MoveSpeedBuffTimer.ExpiredOrNotRunning(Runner) == false;

    [Networked] public int DamagedCount { get; private set; }
    [Networked] public float CurrentHP { get; private set; }
    [Networked] private TickTimer InvincibilityTimer { get; set; }
    [Networked] private TickTimer DamageBuffTimer { get; set; }
    [Networked] private TickTimer MoveSpeedBuffTimer { get; set; }

    public override void Spawned()
    {
        if(HasStateAuthority)
        {
            CurrentHP = maxHP;

            InvincibilityTimer = TickTimer.CreateFromSeconds(Runner, invincibilityDurationAfterSpawn);

            PlayerManager.Instance.onChangedHP?.Invoke((int)CurrentHP);
        }

        localDamagedCount = DamagedCount;
    }

    public override void Render()
    {
        if(damageBuffEffect.activeSelf != IsActiveDamageBuff)
        {
            damageBuffEffect.SetActive(IsActiveDamageBuff);
        }

        if(moveSpeedBuffEffect.activeSelf != IsActiveMoveSpeedBuff)
        {
            moveSpeedBuffEffect.SetActive(IsActiveMoveSpeedBuff);
        }
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

    public bool TryRecoveryHp(float recoveryValue)
    {
        if(CurrentHP <= 0f)
            return false;
        if(CurrentHP >= maxHP)
            return false;

        RPC_RecoveryHp(recoveryValue);

        return true;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RecoveryHp(float recoveryValue)
    {
        if (HasStateAuthority == false)
            return;

        CurrentHP = Mathf.Min(CurrentHP + recoveryValue, maxHP);

        PlayerManager.Instance.onChangedHP?.Invoke((int)CurrentHP);
    }

    public void ActivateBuff(EBuffType buffType, float buffDurationTime)
    {
        switch (buffType)
        {
            case EBuffType.DAMAGE:
                DamageBuffTimer = TickTimer.CreateFromSeconds(Runner, buffDurationTime);
                break;
            case EBuffType.MOVESPEED:
                MoveSpeedBuffTimer = TickTimer.CreateFromSeconds(Runner, buffDurationTime);
                break;
        }
    }
}
