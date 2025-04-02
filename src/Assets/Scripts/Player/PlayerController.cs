using System.Collections;
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
    public float attackCooldown = 0.3f;
    public int attackDamage = 1;
    public LayerMask enemyLayer;

    public GameObject attackHitboxPrefab;

    private bool canAttack = true;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        abilities = GetComponent<PlayerAbilities>();
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
        }
        else if (doubleJumpAvailable)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
            doubleJumpAvailable = false;
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
        dashTime = Time.time + dashDuration;
        canDash = false;
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed && canAttack)
        {
            canAttack = false;
            StartCoroutine(AttackFlash());

            Vector2 attackOrigin = (Vector2)transform.position + Vector2.right * GetFacingDirection() * attackRange * 0.5f;
            Vector2 boxSize = new Vector2(attackRange, attackWidth);

            // Visual prefab instantiation
            if (attackHitboxPrefab != null)
            {
                GameObject vis = Instantiate(attackHitboxPrefab);
                vis.transform.position = attackOrigin;
                vis.transform.rotation = Quaternion.identity;
                vis.transform.localScale = new Vector3(boxSize.x, boxSize.y, 1f);

                // On peut aussi y ajouter un sprite flip si tu veux visuellement indiquer la direction
                Destroy(vis, 0.1f);
            }

            Collider2D[] hits = Physics2D.OverlapBoxAll(attackOrigin, boxSize, 0f, enemyLayer);

            foreach (Collider2D hit in hits)
            {
                if (hit.TryGetComponent(out EnemyHitbox hitbox))
                {
                    hitbox.ReceiveHit(attackDamage);
                }
            }

            Invoke(nameof(ResetAttack), attackCooldown);
        }
    }

    private void ResetAttack()
    {
        canAttack = true;
    }

    private bool IsGrounded()
    {
        Collider2D groundCollider = Physics2D.OverlapCircle(groundCheck.position, groundLayer);
        return groundCollider != null;
    }

    public int GetFacingDirection()
    {
        return facingDirection;
    }

    private IEnumerator AttackFlash()
    {
        Color originalColor = sr.color;
        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sr.color = originalColor;
    }
}
