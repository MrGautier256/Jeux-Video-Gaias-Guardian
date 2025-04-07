using UnityEngine;

public class FlyingChaserAI : EnemyAI
{
    [Header("Flight Settings")]
    public float speed = 3f;
    public float detectionRadius = 5f;
    public float swoopArcHeight = 1.5f;
    public float cooldownBetweenAttacks = 2f;

    private Vector2 targetPosition;
    private bool hasLockedTarget = false;
    private float attackCooldown = 0f;
    private float swoopProgress = 0f;
    private Vector2 swoopStartPos;

    protected override void Start()
    {
        base.Start();
        //attackCooldown = Random.Range(0f, 1f); // pour désynchroniser les crânes
    }

    public override void Act()
    {
        if (player == null) return;

        //attackCooldown -= Time.deltaTime;

        // Si en cooldown ou en vol
        if (!hasLockedTarget && Vector2.Distance(transform.position, player.position) < detectionRadius)
        {
            hasLockedTarget = true;
            swoopStartPos = transform.position;
            targetPosition = player.position; //  lock la position du joueur
            swoopProgress = 0f;
        }

        if (hasLockedTarget)
        {
            swoopProgress += Time.deltaTime * (speed / Vector2.Distance(swoopStartPos, targetPosition));

            Vector2 direction = Vector2.Lerp(swoopStartPos, targetPosition, swoopProgress);
            float arc = Mathf.Sin(Mathf.PI * swoopProgress) * swoopArcHeight;

            // Ajoute l’arc en Y pour un effet de courbe (comme un oiseau)
            Vector2 curvedTarget = new Vector2(direction.x, direction.y + arc);
            transform.position = Vector2.Lerp(transform.position, curvedTarget, Time.deltaTime * speed);

            // Flip sprite
            if ((curvedTarget - (Vector2)transform.position).x != 0)
                transform.localScale = new Vector3(Mathf.Sign(curvedTarget.x - transform.position.x), 1f, 1f);

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
    }

}
