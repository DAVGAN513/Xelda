using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 6.0f;
    public float turnSpeed = 120.0f;
    public float jumpForce = 6.0f;
    public float groundCheckDistance = 0.25f;

    public int attackDamage = 20;
    public float attackRange = 3.5f;
    public float attackCooldown = 0.5f;
    public string bossTag = "Boss";

    public float horizontalInput;
    public float forwardInput;

    private Rigidbody rb;
    private EntityHealth health;
    private bool isGrounded = true;
    private float nextAttackTime = 0f;
    private bool isAttackAnimating = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        health = GetComponent<EntityHealth>();

        if (rb != null)
        {
            rb.freezeRotation = true;
        }
    }

    void Update()
    {
        if (health != null && health.currentHealth <= 0) return;

        horizontalInput = Input.GetAxis("Horizontal");
        forwardInput = Input.GetAxis("Vertical");

        if (!isAttackAnimating)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * speed * forwardInput);
            transform.Rotate(Vector3.up, Time.deltaTime * turnSpeed * horizontalInput);
        }

        if (Input.GetKeyDown(KeyCode.Space) && IsGroundedNow() && rb != null)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }

        if (Input.GetMouseButtonDown(0) && Time.time >= nextAttackTime && !isAttackAnimating)
        {
            nextAttackTime = Time.time + attackCooldown;
            StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttackAnimating = true;
        Vector3 originalPos = transform.localPosition;
        Vector3 lungePos = originalPos + transform.forward * 0.15f;

        float elapsed = 0f;
        float duration = 0.08f;
        while (elapsed < duration)
        {
            transform.localPosition = Vector3.Lerp(originalPos, lungePos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Attack();

        elapsed = 0f;
        while (elapsed < duration)
        {
            transform.localPosition = Vector3.Lerp(lungePos, originalPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
        isAttackAnimating = false;
    }

    private void Attack()
    {
        Vector3 attackCenter = transform.position + transform.forward * (attackRange * 0.5f);
        Collider[] hits = Physics.OverlapSphere(attackCenter, attackRange * 0.75f);

        foreach (Collider hit in hits)
        {
            if (hit.gameObject == gameObject || hit.transform.IsChildOf(transform))
                continue;

            bool isBoss = hit.CompareTag(bossTag) ||
                          (hit.transform.root != null && hit.transform.root.CompareTag(bossTag));

            if (isBoss)
            {
                EntityHealth bossHealth = hit.GetComponentInParent<EntityHealth>();
                if (bossHealth == null && hit.transform.root != null)
                {
                    bossHealth = hit.transform.root.GetComponentInChildren<EntityHealth>();
                }

                if (bossHealth != null)
                {
                    bossHealth.TakeDamage(attackDamage);
                    break;
                }
            }
        }
    }

    private bool IsGroundedNow()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 0.05f;
        RaycastHit[] hits = Physics.RaycastAll(rayOrigin, Vector3.down, groundCheckDistance);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject != gameObject && !hit.collider.transform.IsChildOf(transform))
            {
                return true;
            }
        }

        return false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.contacts.Length > 0)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                if (contact.normal.y > 0.3f)
                {
                    isGrounded = true;
                    break;
                }
            }
        }
    }
}