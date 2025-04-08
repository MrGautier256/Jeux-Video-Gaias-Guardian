using UnityEngine;

public class DashingChaserAI : EnemyAI
{
    [Header("Movement")]
    public float walkSpeed = 1.5f;
    public float dashSpeed = 6f;
    public float dashDuration = 0.3f;
    public float dashCooldown = 3f;
    public Transform pointA;
    public Transform pointB;

    [Header("Attack Settings")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 6f;
    public float shootCooldown = 2f;

    [Header("Detection")]
    public float detectionRadius = 12f;

    private float nextDashTime;
    private float dashEndTime;
    private bool isDashing = false;

    private float nextShootTime;
    private Vector3 originalScale;

    protected override void Start()
    {
        base.Start();
        originalScale = transform.localScale;
        nextDashTime = Time.time + Random.Range(0f, 1f);
        nextShootTime = Time.time + Random.Range(0f, shootCooldown);
    }

    public override void Act()
    {
        if (player == null) return;

        Vector2 toPlayer = player.position - transform.position;

        // Flip direction
        if (Mathf.Abs(toPlayer.x) > 0.1f)
        {
            float flipX = Mathf.Sign(toPlayer.x) * Mathf.Abs(originalScale.x);
            transform.localScale = new Vector3(flipX, originalScale.y, originalScale.z);
        }

        // Determine movement direction
        Vector2 moveDir = toPlayer.normalized;
        float moveSpeed = isDashing ? dashSpeed : walkSpeed;
        Vector3 newPos = transform.position + (Vector3)(moveDir * moveSpeed * Time.deltaTime);

        // Clamp to patrol zone (X only)
        if (pointA != null && pointB != null)
        {
            float minX = Mathf.Min(pointA.position.x, pointB.position.x);
            float maxX = Mathf.Max(pointA.position.x, pointB.position.x);

            newPos.x = Mathf.Clamp(newPos.x, minX, maxX);

            // Stop dash if clamped
            if (isDashing && (newPos.x <= minX || newPos.x >= maxX))
            {
                isDashing = false;
            }
        }

        transform.position = newPos;

        // Dash timer logic
        if (isDashing)
        {
            if (Time.time >= dashEndTime)
                isDashing = false;
        }
        else
        {
            if (Time.time >= nextDashTime)
            {
                isDashing = true;
                dashEndTime = Time.time + dashDuration;
                nextDashTime = Time.time + dashCooldown;
            }
        }

        // Horizontal shoot
        if (Time.time >= nextShootTime && Mathf.Abs(toPlayer.x) < detectionRadius)
        {
            Vector2 shootDir = toPlayer.x < 0 ? Vector2.left : Vector2.right;
            Shoot(shootDir);
            nextShootTime = Time.time + shootCooldown;
        }
    }

    private void Shoot(Vector2 direction)
    {
        if (projectilePrefab == null) return;

        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Projectile p = proj.GetComponent<Projectile>();
        if (p != null)
        {
            p.Launch(direction, projectileSpeed);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }
    }
}
