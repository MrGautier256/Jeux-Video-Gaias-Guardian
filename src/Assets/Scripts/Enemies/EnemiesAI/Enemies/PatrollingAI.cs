using UnityEngine;

public class PatrollingAI : EnemyAI
{
    public enum ShootPatternMode { Dynamic, Fixed }

    [Header("Patrol Settings")]
    public float speed = 2f;
    public float stopDistance = 0.05f;
    public float waitTimeAtPoint = 0.5f;
    public float initialDelayRandomRange = 1.5f;
    public Transform pointA;
    public Transform pointB;
    public bool canMove = true;

    [Header("Attack Settings")]
    public bool canShoot = true;
    public GameObject projectilePrefab;
    public float projectileSpeed = 5f;
    public float shootCooldown = 3f;


    [Header("Shooting Pattern")]
    public ShootPatternMode shootMode = ShootPatternMode.Dynamic;

    [Header("Fixed Pattern Directions")]
    public bool fireHorizontal = true;
    public bool followPlayerDirectionHorizontal = false;

    public bool fireVertical = false;
    public bool followPlayerDirectionVertical = false;

    public bool fireDiagonal = false;
    public bool followPlayerDirectionDiagonal = false;


    [Header("Sprite Settings")]
    public bool spriteFacesLeftByDefault = true;

    [Header("Detection Zone (Rectangular)")]
    public float rangeLeft = 8f;
    public float rangeRight = 12f;
    public float rangeUp = 5f;
    public float rangeDown = 2f;
    public Vector2 detectionOffset;

    private Transform currentTarget;
    private float waitTimer = 0f;
    private float nextShootTime;
    private Vector3 originalScale;

    protected override void Start()
    {
        base.Start();
        currentTarget = pointA;
        waitTimer = -Random.Range(0f, initialDelayRandomRange);
        nextShootTime = Time.time + Random.Range(0f, shootCooldown);
        originalScale = transform.localScale;
    }

    public override void Act()
    {
        if (canMove)
        {
            Patrol();
        }
        else
        {
            FacePlayer();
        }

        if (canShoot && player != null && Time.time >= nextShootTime && IsPlayerInDetectionZone())
        {
            ShootProjectiles();
            nextShootTime = Time.time + shootCooldown;
        }
    }

    private void Patrol()
    {
        if (pointA == null || pointB == null) return;

        Vector2 currentPos = transform.position;
        Vector2 targetPos = currentTarget.position;
        float distance = Vector2.Distance(currentPos, targetPos);

        if (distance < stopDistance)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTimeAtPoint)
            {
                currentTarget = (currentTarget == pointA) ? pointB : pointA;
                waitTimer = 0f;
            }
            return;
        }

        transform.position = Vector2.MoveTowards(currentPos, targetPos, speed * Time.deltaTime);

        Vector3 dir = currentTarget.position - transform.position;
        if (dir.x != 0)
        {
            ApplyFlip(dir.x);
        }
    }

    private void FacePlayer()
    {
        if (player == null) return;

        float dirToPlayer = player.position.x - transform.position.x;
        if (Mathf.Abs(dirToPlayer) > 0.1f)
        {
            ApplyFlip(dirToPlayer);
        }
    }

    private void ApplyFlip(float directionX)
    {
        float sign = Mathf.Sign(directionX);
        float flipFactor = spriteFacesLeftByDefault ? -1f : 1f;
        transform.localScale = new Vector3(sign * flipFactor * Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
    }

    private void ShootProjectiles()
    {
        if (projectilePrefab == null) return;

        Vector3 spawnPos = transform.position;

        if (shootMode == ShootPatternMode.Dynamic)
        {
            ShootDynamic(spawnPos);
        }
        else if (shootMode == ShootPatternMode.Fixed)
        {
            ShootFixedHorizontal(spawnPos);
            ShootFixedVertical(spawnPos);
            ShootFixedDiagonal(spawnPos);
        }
    }

    private void ShootDynamic(Vector3 spawnPos)
    {
        Vector2 direction = (player.position - transform.position).normalized;
        CreateProjectile(spawnPos, direction);
    }

    private void ShootFixedHorizontal(Vector3 spawnPos)
    {
        if (!fireHorizontal) return;

        float dx = player.position.x - transform.position.x;

        if (followPlayerDirectionHorizontal)
        {
            if (Mathf.Abs(dx) >= 0.1f)
            {
                Vector2 dir = dx < 0 ? Vector2.left : Vector2.right;
                CreateProjectile(spawnPos, dir);
            }
        }
        else
        {
            CreateProjectile(spawnPos, Vector2.left);
            CreateProjectile(spawnPos, Vector2.right);
        }
    }

    private void ShootFixedVertical(Vector3 spawnPos)
    {
        if (!fireVertical) return;

        float dy = player.position.y - transform.position.y;

        if (followPlayerDirectionVertical)
        {
            if (Mathf.Abs(dy) >= 0.1f)
            {
                Vector2 dir = dy < 0 ? Vector2.down : Vector2.up;
                CreateProjectile(spawnPos, dir);
            }
        }
        else
        {
            CreateProjectile(spawnPos, Vector2.up);
            CreateProjectile(spawnPos, Vector2.down);
        }
    }

    private void ShootFixedDiagonal(Vector3 spawnPos)
    {
        if (!fireDiagonal) return;

        if (followPlayerDirectionDiagonal)
        {
            Vector2 to = player.position - transform.position;
            float dx = Mathf.Sign(to.x);
            float dy = Mathf.Sign(to.y);
            Vector2 diagDir = new Vector2(dx, dy).normalized;
            CreateProjectile(spawnPos, diagDir);
        }
        else
        {
            CreateProjectile(spawnPos, (Vector2.left + Vector2.up).normalized);
            CreateProjectile(spawnPos, (Vector2.right + Vector2.up).normalized);
            CreateProjectile(spawnPos, (Vector2.left + Vector2.down).normalized);
            CreateProjectile(spawnPos, (Vector2.right + Vector2.down).normalized);
        }
    }



    private void CreateProjectile(Vector3 position, Vector2 direction)
    {
        GameObject proj = Instantiate(projectilePrefab, position, Quaternion.identity);
        Projectile p = proj.GetComponent<Projectile>();
        if (p != null)
        {
            p.Launch(direction, projectileSpeed);
        }
    }

    private bool IsPlayerInDetectionZone()
    {
        Vector2 zoneCenter = (Vector2)transform.position + detectionOffset;
        Vector2 playerPos = player.position;

        float leftBound = zoneCenter.x - rangeLeft;
        float rightBound = zoneCenter.x + rangeRight;
        float bottomBound = zoneCenter.y - rangeDown;
        float topBound = zoneCenter.y + rangeUp;

        return playerPos.x >= leftBound &&
               playerPos.x <= rightBound &&
               playerPos.y >= bottomBound &&
               playerPos.y <= topBound;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Vector2 center = Application.isPlaying ? (Vector2)transform.position + detectionOffset
                                               : (Vector2)transform.position + detectionOffset;

        Vector2 size = new Vector2(rangeLeft + rangeRight, rangeUp + rangeDown);
        Vector3 gizmoCenter = new Vector3(center.x + (rangeRight - rangeLeft) / 2f, center.y + (rangeUp - rangeDown) / 2f, 0f);

        Gizmos.DrawWireCube(gizmoCenter, size);
    }
}
