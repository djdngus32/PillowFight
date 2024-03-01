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

    public float DamageBoxDistance => damageBoxDistance;
    public Vector3 DamageBoxCenter { get; private set; }
    public Vector3 DamageBoxSize => new Vector3(damageBoxSize, damageBoxSize, damageBoxDistance);

    public void Attack(Vector3 attackPosition, Quaternion attackRotation,float damageMultiplier)
    {
        float halfBoxSize = damageBoxSize * 0.5f;
        float halfBoxDistance = damageBoxDistance * 0.5f;

        DamageBoxCenter = attackPosition + (attackRotation * Vector3.forward * halfBoxDistance);
        Collider[] colliders = new Collider[8];

        int collisions = Runner.GetPhysicsScene().OverlapBox(DamageBoxCenter, DamageBoxSize * 0.5f, colliders, attackRotation, hitLayerMask);
        if(collisions > 0)
        {
            for(int i = 0; i < collisions; i++)
            {
                //KCC�� ����ϸ� KCC Collider Component�� KKC ������Ʈ�� ���� ������Ʈ�� �ڽ����� �����Ǳ� ������ �ؿ� �ڵ�ó�� �÷��̾� ��ũ��Ʈ�� ã���ش�.
                var enemyStat = colliders[i].GetComponentInParent<PlayerStat>();
                if (enemyStat == null || enemyStat.HasStateAuthority == Object.HasStateAuthority || enemyStat.IsAlive == false)
                    continue;

                enemyStat.RPC_ApplyDamage(Object.StateAuthority, damage * damageMultiplier);
            }
        }
    }
}
