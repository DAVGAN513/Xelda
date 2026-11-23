using UnityEngine;

public class DragonProjectile : MonoBehaviour
{
    public float speed = 12.0f;
    public int damage = 15;
    public float lifeTime = 5.0f;
    public string playerTag = "Player";

    private Vector3 moveDirection = Vector3.forward;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(moveDirection * Time.deltaTime * speed, Space.World);
    }

    public void SetDirection(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        if (direction.sqrMagnitude > 0.001f)
        {
            moveDirection = direction.normalized;
            transform.LookAt(targetPosition);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) || (other.transform.root != null && other.transform.root.CompareTag(playerTag)))
        {
            EntityHealth playerHealth = other.GetComponentInParent<EntityHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}