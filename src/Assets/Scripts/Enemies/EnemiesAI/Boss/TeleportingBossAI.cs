using UnityEngine;
using System.Collections.Generic;
using System.Collections;

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

    [Header("Portal FX")]
    public GameObject portalPrefab;
    public Vector3 portalOffset = new Vector3(0f, 0.5f, 0f);
    public Vector3 portalScale = Vector3.one;
    public float exitPortalYOffsetCorrection = -0.5f;

    [Header("Phase 2 Settings")]
    public float phaseTwoTeleportInterval = 0.5f;
    public float phaseTwoShootCooldown = 0.75f;
    public Sprite phaseTwoSprite;


    private float nextShootTime;
    private Vector3 originalScale;
    private bool isActivated = false;
    private int lastIndex = -1;
    private bool isTeleporting = false;
    private Transform lastTeleportPoint;
    private EnemyHealth enemyHealth;
    private int lastKnownHP;
    private bool isInPhaseTwo = false;
    private GameObject exitPortal;
    private GameObject entryPortal;

    protected override void Start()
    {
        base.Start();

        enemyHealth = GetComponent<EnemyHealth>();
        if (enemyHealth != null)
            lastKnownHP = enemyHealth.MaxHealth;


        nextTeleportTime = Time.time + Random.Range(0, teleportDelayRandomRange);
        nextShootTime = Time.time + Random.Range(0, shootCooldown);

        originalCooldown = shootCooldown;
        originalTeleportInterval = teleportInterval;

        sr = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;
        sr.sortingOrder = 5;
    }

    public override void Act()
    {
        if (!isActivated && IsPlayerInDetectionZone())
        {
            isActivated = true;
        }

        if (enemyHealth != null && enemyHealth.IsDead)
        {
            ForceDestroyPortals();
            return;
        }

        if (!isActivated || isTeleporting) return;

        if (Time.time >= nextTeleportTime && teleportPoints.Count > 0)
        {
            TeleportToRandomPoint();
        }

        if (!isInPhaseTwo && enemyHealth != null && enemyHealth.CurrentHealth <= enemyHealth.MaxHealth / 2)
        {
            EnterPhaseTwo();
        }


        CheckHealth();
        FacePlayer();

        if (canShoot && player != null && Time.time >= nextShootTime)
        {
            ShootProjectiles();
            nextShootTime = Time.time + shootCooldown;
        }
    }

    private void EnterPhaseTwo()
    {
        isInPhaseTwo = true;
        teleportInterval *= phaseTwoTeleportInterval;
        shootCooldown *= phaseTwoShootCooldown;
        if (phaseTwoSprite != null)
            sr.sprite = phaseTwoSprite;

        sr.sortingOrder = 5;
    }


    private void CheckHealth()
    {
        if (enemyHealth != null && !isTeleporting)
        {
            int currentHP = Mathf.Max(0, enemyHealth.CurrentHealth);
            if (lastKnownHP - currentHP >= 2)
            {
                lastKnownHP = currentHP;
                TeleportToRandomPoint();
                return;
            }
            lastKnownHP = currentHP;
        }

    }


    private void TeleportToRandomPoint()
    {
        int index;
        do
        {
            index = Random.Range(0, teleportPoints.Count);
        }
        while (teleportPoints.Count > 1 && index == lastIndex);

        Vector3 targetPos = teleportPoints[index].position;

        if (lastTeleportPoint == teleportPoints[index])
        {
            nextTeleportTime = Time.time + teleportInterval;
            return;
        }

        lastIndex = index;
        lastTeleportPoint = teleportPoints[index];
        StartCoroutine(HandleTeleportSequence(transform.position, teleportPoints[index].position));
    }




    private IEnumerator HandleTeleportSequence(Vector3 from, Vector3 to)
    {
        isTeleporting = true;

        // === Portail de sortie (position actuelle)
        if (portalPrefab != null)
        {
            exitPortal = Instantiate(portalPrefab, from + portalOffset + new Vector3(0f, exitPortalYOffsetCorrection, 0f), Quaternion.identity);
            exitPortal.transform.localScale = portalScale;
            exitPortal.GetComponent<PortalEffect>()?.PlayOpen();
        }

        // === Portail d'entrée (destination)
        if (portalPrefab != null)
        {
            entryPortal = Instantiate(portalPrefab, to + portalOffset, Quaternion.identity);
            entryPortal.transform.localScale = portalScale;
            entryPortal.GetComponent<PortalEffect>()?.PlayOpen();
        }

        yield return new WaitForSeconds(0.5f);

        // === Disparition du boss avec fondu
        yield return StartCoroutine(DisableBossTemporarily());

        yield return new WaitForSeconds(0.5f);

        // === Réapparition avec fondu
        yield return StartCoroutine(EnableBossWithFadeIn(to + Vector3.up * 0.5f));

        nextTeleportTime = Time.time + teleportInterval;

        // === Fermeture des portails
        if (exitPortal != null)
        {
            var exitFx = exitPortal.GetComponent<PortalEffect>();
            exitFx?.PlayClose();
            exitFx?.DeactivateAfter(1f);
        }

        if (entryPortal != null)
        {
            var entryFx = entryPortal.GetComponent<PortalEffect>();
            entryFx?.PlayClose();
            entryFx?.DeactivateAfter(1f);
        }

        isTeleporting = false;
    }


    private IEnumerator DisableBossTemporarily(float fadeDuration = 0.3f)
    {
        float elapsed = 0f;
        Color originalColor = sr.color;

        while (elapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        sr.color = Color.white;
        sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        sr.enabled = false;


        SetAllCollidersEnabled(false);
        SetAllScriptsEnabled(false);
    }

    private IEnumerator EnableBossWithFadeIn(Vector3 newPosition, float fadeDuration = 0.3f)
    {
        transform.position = newPosition;

        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0f);
        sr.enabled = true;

        SetAllCollidersEnabled(true);
        SetAllScriptsEnabled(true);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
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

    private void SetAllCollidersEnabled(bool value)
    {
        foreach (var col in GetComponentsInChildren<Collider2D>())
            col.enabled = value;
    }

    private void SetAllScriptsEnabled(bool value)
    {
        foreach (var comp in GetComponents<MonoBehaviour>())
        {
            if (comp != this)
                comp.enabled = value;
        }
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
        if (isSlowed) return;
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

    private void ForceDestroyPortals()
    {
        if (exitPortal != null)
        {
            Destroy(exitPortal);
            exitPortal = null;
        }

        if (entryPortal != null)
        {
            Destroy(entryPortal);
            entryPortal = null;
        }
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
