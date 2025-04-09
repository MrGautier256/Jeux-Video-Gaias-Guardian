using UnityEngine;

public class DashingChaserAI : EnemyAI
{
    [Header("Movement")]
    public float walkSpeed = 0.7f;
    public float dashSpeed = 6f;
    public float dashDuration = 0.3f;
    public float dashCooldown = 3f;
    public Transform pointA;
    public Transform pointB;

    [Header("Attack Settings")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 5f;
    public float shootCooldown = 3f;

    [Header("Detection")]
    public float detectionRadius = 12f;

    private float nextDashTime;
    private float dashEndTime;
    private bool isDashing = false;

    private float nextShootTime;
    private Vector3 originalScale;

    private bool goingToB = true;
    private int dashDirection = 1;
    public bool spriteFacesLeftByDefault = true;


    protected override void Start()
    {
        base.Start();
        originalScale = transform.localScale;
        nextDashTime = Time.time + Random.Range(0f, dashCooldown);
        nextShootTime = Time.time + Random.Range(0f, shootCooldown);
    }

    public override void Act()
    {
        if (player == null || pointA == null || pointB == null) return;

        float minX = Mathf.Min(pointA.position.x, pointB.position.x);
        float maxX = Mathf.Max(pointA.position.x, pointB.position.x);
        float currentX = transform.position.x;
        float playerX = player.position.x;

        Vector2 moveDir;

        // Dash logic
        if (isDashing)
        {
            moveDir = new Vector2(dashDirection, 0f);
            if (Time.time >= dashEndTime)
                isDashing = false;
        }
        else
        {
            // Patrol logic
            Transform target = goingToB ? pointB : pointA;
            float distance = target.position.x - currentX;

            if (Mathf.Abs(distance) < 0.1f)
                goingToB = !goingToB;

            moveDir = new Vector2(Mathf.Sign(distance), 0f);

            // Trigger dash randomly
            if (Time.time >= nextDashTime && Random.value < 0.5f)
            {
                isDashing = true;
                dashDirection = Random.value < 0.5f ? -1 : 1;
                dashEndTime = Time.time + dashDuration;
                nextDashTime = Time.time + dashCooldown;
            }
        }

        // Flip sprite based on player
        if (Mathf.Abs(playerX - currentX) > 0.1f)
        {
            float flipFactor = spriteFacesLeftByDefault ? -1f : 1f;
            float flipX = Mathf.Sign(playerX - currentX) * flipFactor * Mathf.Abs(originalScale.x);
            transform.localScale = new Vector3(flipX, originalScale.y, originalScale.z);
        }

        // Move
        float speed = isDashing ? dashSpeed : walkSpeed;
        Vector3 newPos = transform.position + new Vector3(moveDir.x * speed * Time.deltaTime, 0f, 0f);
        newPos.x = Mathf.Clamp(newPos.x, minX, maxX);

        // Stop dash if clamped
        if (isDashing && (newPos.x <= minX || newPos.x >= maxX))
        {
            isDashing = false;
        }

        transform.position = newPos;

        // Shoot if player is within range
        if (Mathf.Abs(playerX - currentX) <= detectionRadius && Time.time >= nextShootTime)
        {
            Vector2 shootDir = (playerX < currentX) ? Vector2.left : Vector2.right;
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