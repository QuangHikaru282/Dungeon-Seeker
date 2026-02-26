using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;

    [Header("Roll")]
    public float rollSpeed = 8f;
    public float rollDuration = 0.4f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;

    [Header("Attack")]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public int attackDamage = 10;
    public LayerMask enemyLayer;

    // Components
    private Rigidbody2D rb;
    private Animator anim;

    // State
    private float moveInput;
    private bool isGrounded;
    private bool isRolling;
    private bool isDead;
    private bool isFacingRight = true;
    private float rollTimer;

    // Animation hashes
    private static readonly int HashSpeed    = Animator.StringToHash("Speed");
    private static readonly int HashIsGround = Animator.StringToHash("IsGrounded");
    private static readonly int HashJump     = Animator.StringToHash("Jump");
    private static readonly int HashRoll     = Animator.StringToHash("Roll");
    private static readonly int HashAttack   = Animator.StringToHash("Attack");
    private static readonly int HashHurt     = Animator.StringToHash("Hurt");
    private static readonly int HashDeath    = Animator.StringToHash("Death");

    void Awake()
    {
        rb   = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDead) return;

        CheckGround();

        if (!isRolling)
        {
            HandleMove();
            HandleJump();
            HandleRoll();
            HandleAttack();
        }
        else
        {
            // Giữ vận tốc roll
            rb.velocity = new Vector2((isFacingRight ? 1 : -1) * rollSpeed, rb.velocity.y);
            rollTimer -= Time.deltaTime;
            if (rollTimer <= 0f) EndRoll();
        }

        UpdateAnimations();
    }

    // ── Movement ──────────────────────────────────────────

    void HandleMove()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        if (moveInput > 0 && !isFacingRight) Flip();
        else if (moveInput < 0 && isFacingRight) Flip();
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            anim.SetTrigger(HashJump);
        }
    }

    void HandleRoll()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && isGrounded)
        {
            isRolling = true;
            rollTimer = rollDuration;
            anim.SetTrigger(HashRoll);
            // Bật invincible
            PlayerStats.Instance?.SetInvincible(true);
        }
    }

    void EndRoll()
    {
        isRolling = false;
        PlayerStats.Instance?.SetInvincible(false);
    }

    void HandleAttack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            anim.SetTrigger(HashAttack);
        }
    }

    // Gọi từ Animation Event tại frame hit của Attack animation
    public void OnAttackHit()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
        foreach (var hit in hits)
        {
            hit.GetComponent<EnemyStats>()?.TakeDamage(attackDamage);
        }
    }

    // ── Utilities ─────────────────────────────────────────

    void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    void UpdateAnimations()
    {
        anim.SetFloat(HashSpeed, Mathf.Abs(moveInput));
        anim.SetBool(HashIsGround, isGrounded);
    }

    // ── Called by PlayerStats ──────────────────────────────

    public void OnHurt()
    {
        anim.SetTrigger(HashHurt);
    }

    public void OnDeath()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        anim.SetTrigger(HashDeath);
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint) { Gizmos.color = Color.red;   Gizmos.DrawWireSphere(attackPoint.position, attackRange); }
        if (groundCheck) { Gizmos.color = Color.green; Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius); }
    }
}