using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

/// <summary>
/// ���÷θ� �����ϰ� �� ������ Ʈ����
/// </summary>
public class DamageTrigger : MonoBehaviour
{
    [SerializeField] private int damage;

    private void OnTriggerEnter(Collider other)
    {
        var playerStat = other.GetComponentInParent<PlayerStat>();
        if (playerStat != null)
        {
            if (playerStat.HasStateAuthority)
            {
                playerStat.RPC_ApplyDamage(PlayerRef.None, damage);
            }
        }
    }
}
