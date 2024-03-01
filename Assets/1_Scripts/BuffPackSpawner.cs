using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class BuffPackSpawner : NetworkBehaviour
{
    [SerializeField] private EBuffType buffType;
    [SerializeField] private float buffDurationTime = 15f;
    [SerializeField] private float cooldownTime = 10f;
    [SerializeField] private float triggerRadius = 1f;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private GameObject activeObject;
    [SerializeField] private GameObject inactiveObject;

    [Networked] private TickTimer CooldownTimer { get; set; }

    public bool IsActive => CooldownTimer.ExpiredOrNotRunning(Runner);

    private static Collider[] colliders = new Collider[8];

    public override void Spawned()
    {
        activeObject.SetActive(IsActive);
        inactiveObject.SetActive(IsActive == false);
    }

    public override void FixedUpdateNetwork()
    {
        if (IsActive == false)
            return;

        int collisions = Runner.GetPhysicsScene().OverlapSphere(transform.position + Vector3.up, triggerRadius, colliders, layerMask, QueryTriggerInteraction.Ignore);
        for (int i = 0; i < collisions; i++)
        {
            var stat = colliders[i].GetComponentInParent<PlayerStat>();
            if (stat != null)
            {
                stat.RPC_ActivateBuff(buffType, buffDurationTime);
                CooldownTimer = TickTimer.CreateFromSeconds(Runner, cooldownTime);
                break;
            }
        }
    }

    public override void Render()
    {
        activeObject.SetActive(IsActive);
        inactiveObject.SetActive(IsActive == false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + Vector3.up, triggerRadius);
    }
}
