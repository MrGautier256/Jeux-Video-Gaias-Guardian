using UnityEngine;
using System.Collections.Generic;

public class TeleportingBossAI : EnemyAI, IEnemySlowable
{
    [Header("Teleportation Settings")]
    public List<Transform> teleportPoints = new();
    public float teleportInterval = 3f;
    public float teleportDelayRandomRange = 1f;
    private float nextTeleportTime;

    [Header("Attack Settings")]
    public bool canShoot = true;
    public GameObject projectilePrefab;
    public float projectileSpeed = 5f;
    public float shootCooldown = 3f;

    [Header("Shooting Pattern")]
    public PatrollingAI.ShootPatternMode shootMode = PatrollingAI.ShootPatternMode.Dynamic;

    public bool fireHorizontal = true;
    public bool followPlayerDirectionHorizontal = false;
    public bool fireVertical = false;
    public bool followPlayerDirectionVertical = false;
    public bool fireDiagonal = false;
    public bool followPlayerDirectionDiagonal = false;

    [Header("Sprite Settings")]
    public bool spriteFacesLeftByDefault = true;

    [Header("Detection Zone")]
    public float rangeLeft = 8f, rangeRight = 8f, rangeUp = 5f, rangeDown = 2f;
    public Vector2 detectionOffset;

    [Header("VortexPollen Reaction")]
    private float originalCooldown;
    private float originalTeleportInterval;
    private bool isSlowed = false;
    private SpriteRenderer sr;

    private float nextShootTime;
    private Vector3 originalScale;
    private bool isActivated = false;


    protected override void Start()
    {
        base.Start();
        nextTeleportTime = Time.time + Random.Range(0, teleportDelayRandomRange);
        nextShootTime = Time.time + Random.Range(0, shootCooldown);

        originalCooldown = shootCooldown;
        originalTeleportInterval = teleportInterval;

        sr = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;
    }

    public override void Act()
    {
        if (!isActivated && IsPlayerInDetectionZone())
        {
            isActivated = true;
        }

        if (!isActivated) return;

        if (Time.time >= nextTeleportTime && teleportPoints.Count > 0)
        {
            TeleportToRandomPoint();
            nextTeleportTime = Time.time + teleportInterval;
        }

        FacePlayer();

        if (canShoot && player != null && Time.time >= nextShootTime)
        {
            ShootProjectiles();
            nextShootTime = Time.time + shootCooldown;
        }
    }


    private void TeleportToRandomPoint()
    {
        int index = Random.Range(0, teleportPoints.Count);
        transform.position = teleportPoints[index].position;
    }

    private void FacePlayer()
    {
        if (player == null) return;

        float dirToPlayer = player.position.x - transform.position.x;
        if (Mathf.Abs(dirToPlayer) > 0.1f)
        {
            ApplyFlip(-dirToPlayer);
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

        if (shootMode == PatrollingAI.ShootPatternMode.Dynamic)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            CreateProjectile(spawnPos, direction);
        }
        else
        {
            if (fireHorizontal) ShootFixedHorizontal(spawnPos);
            if (fireVertical) ShootFixedVertical(spawnPos);
            if (fireDiagonal) ShootFixedDiagonal(spawnPos);
        }
    }

    private void ShootFixedHorizontal(Vector3 spawnPos)
    {
        float dx = player.position.x - transform.position.x;
        if (followPlayerDirectionHorizontal)
        {
            if (Mathf.Abs(dx) >= 0.1f)
                CreateProjectile(spawnPos, dx < 0 ? Vector2.left : Vector2.right);
        }
        else
        {
            CreateProjectile(spawnPos, Vector2.left);
            CreateProjectile(spawnPos, Vector2.right);
        }
    }

    private void ShootFixedVertical(Vector3 spawnPos)
    {
        float dy = player.position.y - transform.position.y;
        if (followPlayerDirectionVertical)
        {
            if (Mathf.Abs(dy) >= 0.1f)
                CreateProjectile(spawnPos, dy < 0 ? Vector2.down : Vector2.up);
        }
        else
        {
            CreateProjectile(spawnPos, Vector2.up);
            CreateProjectile(spawnPos, Vector2.down);
        }
    }

    private void ShootFixedDiagonal(Vector3 spawnPos)
    {
        if (followPlayerDirectionDiagonal)
        {
            Vector2 to = player.position - transform.position;
            Vector2 diagDir = new Vector2(Mathf.Sign(to.x), Mathf.Sign(to.y)).normalized;
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

    public void ApplySlow(float factor, float duration)
    {
        Debug.Log("AVANT");

        if (isSlowed) return;
        Debug.Log($"TeleportingBossAI: Slow applied. Shoot cooldown: {shootCooldown}, Teleport interval: {teleportInterval}");

        isSlowed = true;
        shootCooldown /= factor;
        teleportInterval *= factor;
        sr.color = new Color(1f, 0.85f, 0.2f);
        Invoke(nameof(RemoveSlow), duration);
    }

    private void RemoveSlow()
    {
        shootCooldown = originalCooldown;
        teleportInterval = originalTeleportInterval;
        sr.color = Color.white;
        isSlowed = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector2 center = (Vector2)transform.position + detectionOffset;
        Vector2 size = new Vector2(rangeLeft + rangeRight, rangeUp + rangeDown);
        Vector3 gizmoCenter = new Vector3(center.x + (rangeRight - rangeLeft) / 2f, center.y + (rangeUp - rangeDown) / 2f, 0f);
        Gizmos.DrawWireCube(gizmoCenter, size);
    }
}
