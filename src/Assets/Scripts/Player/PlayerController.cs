using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Collider2D playerCollider;
    private Collider2D grappleTargetCollider;

    private Vector2 lastPosition;
    private float stuckTimer = 0f;
    public float maxStuckTime = 0.25f;
    private bool isPlayingRetractSound = false;


    [Header("Components")]
    public Rigidbody2D rb;
    private SpriteRenderer sr;
    private PlayerAbilities abilities;

    [Header("Movement")]
    public float speed = 2f;
    public float jumpingPower = 3f;
    private int facingDirection = 1;
    private float horizontal;

    [Header("Grapple SFX")]
    public AudioClip grappleLaunchClip;
    public AudioClip grappleHitClip;
    public AudioClip grappleRetractClip;

    private AudioSource audioSource;

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

    [Header("Grapple")]
    public LayerMask grappleLayer;
    public float grappleRange = 10f;
    public float grappleSpeed = 5f;
    private Vector2 grappleTarget;
    private bool isGrappling = false;
    private bool canGrapple = true;
    private LineRenderer grappleLine;

    [Header("Special Attack - Pollen Vortex")]
    public GameObject vortexProjectilePrefab;
    public Transform shootPoint; 
    public float vortexCooldown = 3f;

    private float lastVortexTime = -999f;


    [Header("Attack")]
    public float attackRange = 1.5f;
    public float attackWidth = 0.8f;
    public float attackCooldown = 0.5f;
    public int attackDamage = 1;
    public LayerMask enemyLayer;

    private Animator animator;
    private float jumpBuffer = 0f;


    private bool canAttack { get; set; } = false; 


    void Start()
    {
        playerCollider = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        abilities = GetComponent<PlayerAbilities>();
        animator = GetComponent<Animator>();

        grappleLine = GetComponent<LineRenderer>();
        grappleLine.positionCount = 0;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        SetControlsEnabled(true);
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

        if (isGrappling)
        {
            grappleLine.SetPosition(0, transform.position);
            grappleLine.SetPosition(1, grappleTarget);
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

        if (isGrappling)
        {
            Vector2 toTarget = grappleTarget - rb.position;
            Vector2 direction = toTarget.normalized;
            float distance = toTarget.magnitude;

            //Lancer le son de rembobinage si ce n�est pas d�j� fait
            if (!isPlayingRetractSound)
            {
                audioSource.clip = grappleRetractClip;
                audioSource.loop = false;
                audioSource.Play();
                isPlayingRetractSound = true;
            }

            // Annule si un obstacle bloque le chemin
            RaycastHit2D hit = Physics2D.Raycast(rb.position, direction, distance, groundLayer);
            if (hit.collider != null)
            {
                EndGrapple();
                return;
            }

            //Mouvement vers la cible
            if (distance < 0.3f)
            {
                EndGrapple();
            }
            else
            {
                Vector2 newPos = rb.position + direction * grappleSpeed * Time.fixedDeltaTime;
                rb.MovePosition(newPos);
            }

            //D�blocage automatique si bloqu�
            if (Vector2.Distance(rb.position, lastPosition) < 0.01f)
            {
                stuckTimer += Time.fixedDeltaTime;

                if (stuckTimer >= maxStuckTime)
                {
                    Vector2 escapeDir = ((grappleTarget - rb.position).normalized + Vector2.up * 0.75f).normalized;
                    Vector2 newPos = rb.position + escapeDir * 0.5f;
                    rb.position = newPos;
                    stuckTimer = 0f;
                }
            }
            else
            {
                stuckTimer = 0f;
            }

            lastPosition = rb.position;
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
        if (isGrappling)
        {
            EndGrapple();
        }

        if (!context.performed || !canDash || isDashing || !abilities.CanDash) return;

        isDashing = true;
        animator.SetTrigger("DashTrigger");
        animator.SetBool("IsDashing", true);
        dashTime = Time.time + dashDuration;
        canDash = false;
    }

    public void Grapple(InputAction.CallbackContext context)
    {
        audioSource.PlayOneShot(grappleLaunchClip);

        if (!context.performed || !canGrapple || isGrappling || !abilities.CanGrapple) return;

        Vector2 direction = new Vector2(facingDirection, 0.5f).normalized;
        Vector2 endPoint = (Vector2)transform.position + direction * grappleRange;

        // Visuel direct pour feedback
        grappleLine.positionCount = 2;
        grappleLine.SetPosition(0, transform.position);
        grappleLine.SetPosition(1, endPoint);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, grappleRange, grappleLayer);
        Debug.DrawRay(transform.position, direction * grappleRange, Color.green, 1f);

        if (hit.collider != null)
        {
            grappleTarget = hit.point;
            isGrappling = true;
            audioSource.PlayOneShot(grappleHitClip);
            rb.gravityScale = 0;

            grappleTargetCollider = hit.collider;
            Physics2D.IgnoreCollision(playerCollider, grappleTargetCollider, true);
        }
        else
        {
            Invoke(nameof(ClearGrappleLine), 0.1f);
        }
    }

    public void SpecialAttack(InputAction.CallbackContext context)
    {
        if (!context.performed || !abilities.CanUsePollenVortex || Time.time < lastVortexTime + vortexCooldown) return;

        lastVortexTime = Time.time;

        Vector2 shootDir = new Vector2(facingDirection, 0f);
        GameObject vortex = Instantiate(vortexProjectilePrefab, shootPoint.position, Quaternion.identity);
        vortex.GetComponent<PollenVortexProjectile>().Launch(shootDir);
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (!context.performed || !canAttack || abilities == null || !abilities.CanUseSword) return;

        canAttack = false;

        if (animator != null)
        {
            if (animator.GetBool("Isjumping"))
            {
                animator.SetTrigger("JumpMeleeTrigger");
            }
            else
            {
                animator.SetTrigger("AttackTrigger");
            }

            animator.SetBool("IsAttacking", true);
            animator.speed = 2f;
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
        float attackDuration = 0.2f;


        attackCooldown = attackDuration;
        Invoke(nameof(ResetAttack), attackCooldown);
    }


    private void ResetAttack()
    {
        canAttack = true;

        if (animator != null)
        {
            animator.SetBool("IsAttacking", false);
            animator.speed = 1f;
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

    private void ClearGrappleLine()
    {
        if (!isGrappling)
        {
            grappleLine.positionCount = 0;
        }
    }

    private void EndGrapple()
    {
        if (isPlayingRetractSound)
        {
            audioSource.Stop();
            isPlayingRetractSound = false;
        }

        if (grappleTargetCollider != null)
        {
            Physics2D.IgnoreCollision(playerCollider, grappleTargetCollider, false);
            grappleTargetCollider = null;
        }

        isGrappling = false;
        rb.gravityScale = 1;
        grappleLine.positionCount = 0;
    }

    public void SetControlsEnabled(bool enabled)
    {
        canAttack = enabled;
        canDash = enabled;
        canGrapple = enabled;
    }




}
