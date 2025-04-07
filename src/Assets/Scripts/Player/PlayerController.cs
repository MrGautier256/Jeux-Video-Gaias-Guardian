using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Components")]
    public Rigidbody2D rb;
    private SpriteRenderer sr;
    private PlayerAbilities abilities;

    [Header("Movement")]
    public float speed = 2f;
    public float jumpingPower = 3f;
    private int facingDirection = 1;
    private float horizontal;

    [Header("Dash")]
    public float dashSpeed = 10f;
    public float dashDuration = 0.2f;
    private bool isDashing = false;
    private float dashTime;
    private bool canDash = true;

    [Header("Jumping")]
    private bool doubleJumpAvailable;

    [Header("Checks")]
    public Transform groundCheck;
    public Transform wallCheck;
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    public float groundCheckRadius = 0.2f;


    [Header("Attack")]
    public float attackRange = 1f;
    public float attackWidth = 0.5f;
    public float attackCooldown = 0.5f;
    public int attackDamage = 1;
    public LayerMask enemyLayer;

    private Animator animator;
    private float jumpBuffer = 0f;


    private bool canAttack = true;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        abilities = GetComponent<PlayerAbilities>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {

        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        }


        if (jumpBuffer > 0f)
        {
            jumpBuffer -= Time.deltaTime;
        }
        else if (animator.GetBool("Isjumping") && IsGrounded())
        {
            Debug.Log("Reset Isjumping");
            animator.SetBool("Isjumping", false);
        }

        if (isDashing)
        {
            if (Time.time >= dashTime)
            {
                isDashing = false;
                animator.SetBool("IsDashing", false);
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

            if (abilities != null && abilities.CanDoubleJump)
            {
                doubleJumpAvailable = true;
            }
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
            facingDirection = (int)Mathf.Sign(horizontal);
            sr.flipX = horizontal < 0;
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!context.performed) return;


        if (IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
            doubleJumpAvailable = abilities != null && abilities.CanDoubleJump;
            animator.SetBool("Isjumping", true);
            jumpBuffer = 0.15f;


        }
        else if (doubleJumpAvailable)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
            doubleJumpAvailable = false;
            animator.SetBool("Isjumping", true);
        }
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (!context.performed || !canDash || isDashing) return;

        if (abilities == null)
        {
            abilities = GetComponent<PlayerAbilities>();
            if (abilities == null)
            {
                Debug.LogWarning("[Player] PlayerAbilities est manquant !");
                return;
            }
        }

        if (!abilities.CanDash) return;


        isDashing = true;
        animator.SetTrigger("DashTrigger");
        animator.SetBool("IsDashing", true);
        dashTime = Time.time + dashDuration;
        canDash = false;
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (!context.performed || !canAttack || abilities == null || !abilities.CanUseSword) return;

        canAttack = false;

        if (animator != null)
        {
            animator.SetTrigger("AttackTrigger");
            animator.SetBool("IsAttacking", true);

        }

        Vector2 attackOrigin = (Vector2)transform.position + Vector2.right * GetFacingDirection() * attackRange * 0.5f;
        Vector2 boxSize = new Vector2(attackRange, attackWidth);

        Collider2D[] hits = Physics2D.OverlapBoxAll(attackOrigin, boxSize, 0f, enemyLayer);

        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent(out EnemyHitbox hitbox))
            {
                hitbox.ReceiveHit(attackDamage);
            }
        }
        float attackDuration = animator.runtimeAnimatorController.animationClips
    .FirstOrDefault(clip => clip.name == "attack")?.length ?? 0.5f;


        attackCooldown = attackDuration;
        Invoke(nameof(ResetAttack), attackCooldown);
    }


    private void ResetAttack()
    {
        canAttack = true;

        if (animator != null)
        {
            animator.SetBool("IsAttacking", false);
        }
    }


    private bool IsGrounded()
    {
        Collider2D groundCollider = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        return groundCollider != null;
    }

    public int GetFacingDirection()
    {
        return facingDirection;
    }
}
