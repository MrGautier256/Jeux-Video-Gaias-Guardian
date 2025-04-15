using UnityEngine;

public class JumpingBossAI : EnemyAI
{
    [Header("Jump Settings")]
    public float jumpHeight = 1.5f;
    public float jumpDuration = 1f;
    public float jumpInterval = 2f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public float initialDelayRandomRange = 5f;

    [Header("Direction")]
    public float horizontalJumpDistance = 2f;

    [Header("Limites de la salle")]
    public Transform leftBoundary;
    public Transform rightBoundary;

    [Header("Activation")]
    public float detectionRadius = 10f;
    private bool isActive = false;

    private float nextJumpTime;
    private bool isJumping = false;
    private float jumpStartTime;
    private Vector3 jumpStartPos;
    private Vector3 jumpEndPos;

    protected override void Start()
    {
        base.Start();
        nextJumpTime = Time.time + Random.Range(0f, initialDelayRandomRange);
    }

    public override void Act()
    {
        if (!isActive)
        {
            if (player != null && Vector2.Distance(transform.position, player.position) <= detectionRadius)
            {
                isActive = true;
            }
            else
            {
                return;
            }
        }

        if (isJumping)
        {
            float t = Mathf.Clamp01((Time.time - jumpStartTime) / jumpDuration);
            float yOffset = Mathf.Sin(t * Mathf.PI) * jumpHeight;

            Vector3 horizontalPos = Vector3.Lerp(jumpStartPos, jumpEndPos, t);
            transform.position = new Vector3(horizontalPos.x, horizontalPos.y + yOffset, transform.position.z);

            if (t >= 1f)
                isJumping = false;
        }
        else if (Time.time >= nextJumpTime && IsGrounded())
        {
            StartJump();
        }
    }

    private void StartJump()
    {
        isJumping = true;
        jumpStartTime = Time.time;
        jumpStartPos = transform.position;
        jumpEndPos = jumpStartPos;

        if (player != null)
        {
            float dirX = Mathf.Sign(player.position.x - transform.position.x);
            float targetX = jumpStartPos.x + dirX * horizontalJumpDistance;

            // Clamp dans la zone
            float minX = leftBoundary != null ? leftBoundary.position.x : float.NegativeInfinity;
            float maxX = rightBoundary != null ? rightBoundary.position.x : float.PositiveInfinity;
            targetX = Mathf.Clamp(targetX, minX, maxX);

            jumpEndPos = new Vector3(targetX, jumpStartPos.y, jumpStartPos.z);

            // Flip visuel
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * dirX;
            transform.localScale = scale;
        }

        nextJumpTime = Time.time + jumpInterval;
    }

    private bool IsGrounded()
    {
        if (groundCheck == null) return true;
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
