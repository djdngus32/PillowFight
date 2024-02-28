using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Unity.VisualScripting;

public class RecoveryPackSpawner : NetworkBehaviour
{
    [SerializeField] private float recoveryValue = 50f;
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

        // Get all colliders around pickup within Radius.
        int collisions = Runner.GetPhysicsScene().OverlapSphere(transform.position + Vector3.up, triggerRadius, colliders, layerMask, QueryTriggerInteraction.Ignore);
        for (int i = 0; i < collisions; i++)
        {
            // Check for Health component on collider game object or any parent.
            var stat = colliders[i].GetComponentInParent<PlayerStat>();
            if (stat != null && stat.TryRecoveryHp(recoveryValue))
            {
                // Pickup was successful, activating timer.
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
