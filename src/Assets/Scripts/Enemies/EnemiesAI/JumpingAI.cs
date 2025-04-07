using UnityEngine;

public class JumpingAI : EnemyAI
{
    [Header("Jump Settings")]
    public float jumpHeight = 1.5f;
    public float jumpDuration = 1f;
    public float jumpInterval = 2f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public float initialDelayRandomRange = 5f;

    private float nextJumpTime;
    private bool isJumping = false;
    private float jumpStartTime;
    private Vector3 startPos;

    protected override void Start()
    {
        base.Start();
        nextJumpTime = Time.time + Random.Range(0f, initialDelayRandomRange);
    }

    public override void Act()
    {
        if (isJumping)
        {
            float t = Mathf.Clamp01((Time.time - jumpStartTime) / jumpDuration);

            float yOffset = Mathf.Sin(t * Mathf.PI) * jumpHeight;
            transform.position = startPos + new Vector3(0f, yOffset, 0f);

            if (t >= 1f)
            {
                transform.position = startPos; // Fix du "vol progressif"
                isJumping = false;
            }
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
        startPos = transform.position;
        nextJumpTime = Time.time + jumpInterval;
    }

    private bool IsGrounded()
    {
        if (groundCheck == null) return true;
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
