using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class DamageTrigger : MonoBehaviour
{
    [SerializeField] private int damage;

    private PlayerRef ownerPlayer = PlayerRef.None;
    private Collider triggerCollider;

    public bool IsEnable => triggerCollider != null && triggerCollider.enabled;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider>();
    }

    private void Start()
    {
        if(triggerCollider != null && triggerCollider.isTrigger == false)
        {
            triggerCollider.isTrigger = true;
        }
    }

    public void EnableDamageTrigger(PlayerRef ownerPlayer, int damage)
    {
        if(triggerCollider != null)
        {
            this.damage = damage;
            this.ownerPlayer = ownerPlayer;
            triggerCollider.enabled = true;
        }
    }

    public void DisableDamageTrigger()
    {
        if (triggerCollider != null)
        {
            triggerCollider.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<PlayerStat>(out var playerStat))
        {
            if(ownerPlayer != playerStat.Object.StateAuthority)
            {
                playerStat.RPC_ApplyDamage(ownerPlayer, damage);
            }            
        }
    }
}
