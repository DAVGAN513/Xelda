using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public int attackDamage = 20;
    public float attackRange = 3.5f;
    public float attackCooldown = 0.5f;
    public string bossTag = "Boss";

    private float nextAttackTime = 0f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;
            AttackBoss();
        }
    }

    private void AttackBoss()
    {
        Vector3 attackCenter = transform.position + transform.forward * (attackRange * 0.5f);
        Collider[] hits = Physics.OverlapSphere(attackCenter, attackRange * 0.6f);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag(bossTag) || (hit.transform.root != null && hit.transform.root.CompareTag(bossTag)))
            {
                EntityHealth bossHealth = hit.GetComponentInParent<EntityHealth>();
                if (bossHealth != null)
                {
                    bossHealth.TakeDamage(attackDamage);
                    break;
                }
            }
        }
    }
}