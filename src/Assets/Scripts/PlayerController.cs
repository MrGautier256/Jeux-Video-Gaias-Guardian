using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Components")]
    public Rigidbody2D rb;
    SpriteRenderer sr;

    [Header("Movement")]
    public float speed = 2f;
    public float jumpingPower = 3f;
    int facingDirection = 1;

    [Header("Dash")]
    public float dashSpeed = 10;
    public float dashDuration  = 0.2f;

    bool isDashing = false;
    float dashTime;

    [Header("Jumping")]
    bool canDoubleJump;

    [Header("Checks")]
    public Transform groundCheck;
    public Transform wallCheck; 
    public LayerMask groundLayer;
    public LayerMask wallLayer; 
    public float groundCheckRadius = 0.2f;
    float horizontal;
    bool canDash = true;



    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {

        if (isDashing)
        {
            if (Time.time >= dashTime)
            {
                isDashing = false;
            }
            else
            {
                rb.linearVelocity = new Vector2(facingDirection * dashSpeed, 0);
                return;
            }
        }
    }

    void FixedUpdate()
    {
        if (IsGrounded() && !isDashing)
        {
            canDash = true;
        }
        if (!isDashing)
        {
            rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;

        if (horizontal != 0)
        {
            facingDirection = (int)Mathf.Sign(horizontal); // -1 ou 1
            sr.flipX = horizontal < 0;
        }
    }


    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (IsGrounded())
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
                canDoubleJump = true;
            }
            else if (canDoubleJump)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
                canDoubleJump = false;
            }
        }
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash && !isDashing)
        {
            isDashing = true;
            dashTime = Time.time + dashDuration;
            canDash = false;
        }
    }


    private bool IsGrounded()
    {
        Collider2D groundCollider = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        return groundCollider != null;
    }
}