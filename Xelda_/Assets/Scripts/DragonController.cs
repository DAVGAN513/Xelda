using System.Collections;
using UnityEngine;

public class DragonController : MonoBehaviour
{
    [Header("Movement & Landing")]
    public Transform[] airPoints;
    public Transform landingPoint;
    public float moveDuration = 2.0f;
    public float minAirTime = 5.0f;
    public float maxAirTime = 9.0f;
    public float minGroundedTime = 3.5f;
    public float maxGroundedTime = 6.0f;

    [Header("Projectile Attack")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public Transform playerTarget;
    public float fireInterval = 1.6f;

    public bool isVulnerable = false;

    private EntityHealth health;
    private int currentAirIndex = 0;
    private float nextFireTime = 0f;
    private bool isInAirPhase = true;

    void Start()
    {
        health = GetComponent<EntityHealth>();

        if (playerTarget == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerTarget = playerObj.transform;
            }
        }

        StartCoroutine(BossCycleRoutine());
    }

    void Update()
    {
        if (health != null && health.currentHealth <= 0)
        {
            StopAllCoroutines();
            return;
        }

        if (isInAirPhase && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireInterval;
            ShootProjectile();
        }
    }

    private void ShootProjectile()
    {
        if (projectilePrefab == null || firePoint == null) return;

        GameObject projObj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        DragonProjectile projScript = projObj.GetComponent<DragonProjectile>();

        if (projScript != null && playerTarget != null)
        {
            projScript.SetDirection(playerTarget.position + Vector3.up * 0.8f);
        }
    }

    private IEnumerator BossCycleRoutine()
    {
        while (true)
        {
            isInAirPhase = true;
            isVulnerable = false;

            float airDuration = Random.Range(minAirTime, maxAirTime);
            float airTimer = 0f;

            while (airTimer < airDuration)
            {
                if (airPoints != null && airPoints.Length > 0)
                {
                    Transform targetAir = airPoints[currentAirIndex];
                    if (targetAir != null)
                    {
                        yield return StartCoroutine(MoveToPosition(targetAir.position, moveDuration));
                    }
                    currentAirIndex = (currentAirIndex + 1) % airPoints.Length;
                }

                airTimer += moveDuration;
                yield return null;
            }

            isInAirPhase = false;

            if (landingPoint != null)
            {
                yield return StartCoroutine(MoveToPosition(landingPoint.position, moveDuration));
            }

            isVulnerable = true;

            float groundedDuration = Random.Range(minGroundedTime, maxGroundedTime);
            yield return new WaitForSeconds(groundedDuration);

            isVulnerable = false;

            if (airPoints != null && airPoints.Length > 0 && airPoints[currentAirIndex] != null)
            {
                yield return StartCoroutine(MoveToPosition(airPoints[currentAirIndex].position, moveDuration));
            }
        }
    }

    private IEnumerator MoveToPosition(Vector3 targetPos, float duration)
    {
        Vector3 startPos = transform.position;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
    }
}