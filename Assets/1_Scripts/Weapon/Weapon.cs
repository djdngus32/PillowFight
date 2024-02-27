using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Weapon : NetworkBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private float attackSpeed = 1f;
    [SerializeField] private float damageBoxSize = 1f;
    [SerializeField] private float damageBoxDistance = 2f;
    [SerializeField] private LayerMask hitLayerMask;

    public float AttackSpeed => attackSpeed;
    public float AttackCooldownTime => 1 / attackSpeed;

    public void Attack(Vector3 attackPosition, Quaternion attackRotation)
    {
        float halfBoxSize = damageBoxSize * 0.5f;
        float halfBoxDistance = damageBoxDistance * 0.5f;

        Vector3 center = attackPosition + (attackRotation * Vector3.forward * halfBoxDistance);
        Vector3 halfExtents = new Vector3(halfBoxSize, halfBoxSize, halfBoxDistance);
        Collider[] colliders = new Collider[8];

        int collisions = Runner.GetPhysicsScene().OverlapBox(center, halfExtents, colliders, attackRotation, hitLayerMask);
        if(collisions > 0)
        {
            for(int i = 0; i < collisions; i++)
            {
                //KCC�� ����ϸ� KCC Collider Component�� KKC ������Ʈ�� ���� ������Ʈ�� �ڽ����� �����Ǳ� ������ �ؿ� �ڵ�ó�� �÷��̾� ��ũ��Ʈ�� ã���ش�.
                var enemyStat = colliders[i].GetComponentInParent<PlayerStat>();
                if (enemyStat == null || enemyStat.HasStateAuthority == Object.HasStateAuthority || enemyStat.IsAlive == false)
                    continue;

                enemyStat.RPC_ApplyDamage(Object.StateAuthority, damage);
            }
        }
    }
}
