using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class JumpingAI : EnemyAI
{
    [Header("Jump Settings")]
    public float jumpForce = 5f;
    public float jumpInterval = 2f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public float initialDelayRandomRange = 5f;

    private Rigidbody2D rb;
    private float nextJumpTime;

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();

        nextJumpTime = Time.time + Random.Range(0f, initialDelayRandomRange);
    }

    public override void Act()
    {
        if (Time.time >= nextJumpTime && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); 
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            nextJumpTime = Time.time + jumpInterval;
        }
    }

    private bool IsGrounded()
    {
        if (groundCheck == null) return true;
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }
}
