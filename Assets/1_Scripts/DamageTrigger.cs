using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

/// <summary>
/// 로컬로만 동작하게 할 데미지 트리거
/// </summary>
public class DamageTrigger : MonoBehaviour
{
    [SerializeField] private int damage;

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<PlayerStat>(out var playerStat))
        {
            if(playerStat.Object.HasStateAuthority)
            {
                playerStat.RPC_ApplyDamage(PlayerRef.None, damage);
            }            
        }
    }
}
