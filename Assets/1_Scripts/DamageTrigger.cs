using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class DamageTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<PlayerStat>(out var playerStat))
        {
            playerStat.RPC_ApplyDamage(PlayerRef.None, 1000f);
        }
    }
}
