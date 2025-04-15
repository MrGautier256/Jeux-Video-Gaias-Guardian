using UnityEngine;

public class FlyingChaserAI : EnemyAI, IEnemySlowable
{
    [Header("Flight Settings")]
    public float speed = 3f;
    public float detectionRadius = 5f;
    public float swoopArcHeight = 1.5f;
    public float cooldownBetweenAttacks = 1f;

    private Vector2 targetPosition;
    private bool hasLockedTarget = false;
    private float attackCooldown = 0f;
    private float swoopProgress = 0f;
    private Vector2 swoopStartPos;


    [Header("VortexPollen Reaction")]
    private float originalSpeed;
    private float originalCooldown;
    private bool isSlowed = false;
    private SpriteRenderer sr;

    protected override void Start()
    {
        base.Start();
        originalSpeed = speed;
        originalCooldown = cooldownBetweenAttacks;
        sr = GetComponent<SpriteRenderer>();
    }

    public override void Act()
    {
        if (player == null) return;

        attackCooldown -= Time.deltaTime;

        if (!hasLockedTarget && attackCooldown <= 0f)
        {
            float dist = Vector2.Distance(transform.position, player.position);
            if (dist < detectionRadius)
            {
                hasLockedTarget = true;
                swoopStartPos = transform.position;
                targetPosition = player.position;
                swoopProgress = 0f;
            }
        }

        if (hasLockedTarget)
        {
            // Avance du swoop dans le temps
            swoopProgress += Time.deltaTime * (speed / Vector2.Distance(swoopStartPos, targetPosition));

            Vector2 direction = Vector2.Lerp(swoopStartPos, targetPosition, swoopProgress);
            float arc = Mathf.Sin(Mathf.PI * swoopProgress) * swoopArcHeight;
            Vector2 curvedTarget = new Vector2(direction.x, direction.y + arc);

            transform.position = Vector2.Lerp(transform.position, curvedTarget, Time.deltaTime * speed);

            // Flip visuel
            Vector2 dir = curvedTarget - (Vector2)transform.position;
            if (dir.x != 0)
            {
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Sign(dir.x) * Mathf.Abs(scale.x);
                transform.localScale = scale;
            }

            // Fin de l’attaque
            if (swoopProgress >= 1f)
            {
                hasLockedTarget = false;
                attackCooldown = cooldownBetweenAttacks;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.4f);
    }


    public void ApplySlow(float factor, float duration)
    {
        if (isSlowed) return;

        isSlowed = true;
        speed *= factor;
        cooldownBetweenAttacks /= factor;
        sr.color = new Color(1f, 1f, 0.6f); // effet jaune

        Invoke(nameof(RemoveSlow), duration);
    }

    private void RemoveSlow()
    {
        speed = originalSpeed;
        cooldownBetweenAttacks = originalCooldown;
        sr.color = Color.white;
        isSlowed = false;
    }
}
