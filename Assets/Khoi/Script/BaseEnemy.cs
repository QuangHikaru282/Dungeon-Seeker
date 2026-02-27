using UnityEngine;

public enum EnemyState { Patrol, Chase, Attack, Dead }

[RequireComponent(typeof(Animator), typeof(Rigidbody2D))]
public abstract class BaseEnemy : MonoBehaviour
{
    [Header("Stats")]
    public int maxHP = 50;
    protected int currentHP;

    [Header("Patrol")]
    public Transform pointA;
    public Transform pointB;
    public float patrolSpeed = 2f;

    [Header("Detection")]
    public float detectRange = 5f;
    public float attackRange = 1.5f;
    public float chaseSpeed = 3.5f;

    [Header("Attack")]
    public float attackCooldown = 1.5f;

    // Components
    protected Animator anim;
    protected Rigidbody2D rb;
    protected Transform player;

    // State
    protected EnemyState state = EnemyState.Patrol;
    protected bool isFacingRight = true;
    protected float attackTimer;

    // Patrol
    private Transform patrolTarget;

    // Animation hashes
    protected static readonly int HashHurt   = Animator.StringToHash("Hurt");
    protected static readonly int HashDeath  = Animator.StringToHash("Death");
    protected static readonly int HashAttack = Animator.StringToHash("Attack");

    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
        rb   = GetComponent<Rigidbody2D>();
        currentHP = maxHP;

        GameObject go = GameObject.FindGameObjectWithTag("Player");
        if (go) player = go.transform;

        patrolTarget = pointB;
    }

    protected virtual void Update()
    {
        if (state == EnemyState.Dead) return;
        if (PlayerStats.IsDead)
        {
            DoPatrol();
            return;
        }

        attackTimer -= Time.deltaTime;
        float distToPlayer = player ? Vector2.Distance(transform.position, player.position) : Mathf.Infinity;

        switch (state)
        {
            case EnemyState.Patrol:
                DoPatrol();
                if (distToPlayer <= detectRange) state = EnemyState.Chase;
                break;

            case EnemyState.Chase:
                DoChase();
                if (distToPlayer <= attackRange)       state = EnemyState.Attack;
                else if (distToPlayer > detectRange)   state = EnemyState.Patrol;
                break;

            case EnemyState.Attack:
                DoIdle();
                if (distToPlayer > attackRange)        state = EnemyState.Chase;
                else if (attackTimer <= 0f)
                {
                    attackTimer = attackCooldown;
                    TriggerAttack();
                }
                break;
        }
    }

    // ── Patrol ────────────────────────────────────────────

    void DoPatrol()
    {
        if (patrolTarget == null) return;

        Vector2 dir = (patrolTarget.position - transform.position).normalized;
        MoveHorizontal(dir.x, patrolSpeed);

        if (Vector2.Distance(transform.position, patrolTarget.position) < 0.2f)
            patrolTarget = patrolTarget == pointA ? pointB : pointA;

        OnPatrolMove(dir.x);
    }

    // ── Chase ─────────────────────────────────────────────

    void DoChase()
    {
        if (!player) return;
        float dirX = player.position.x > transform.position.x ? 1f : -1f;
        // Chỉ set velocity X, giữ nguyên Y (gravity không bị ảnh hưởng)
        rb.velocity = new Vector2(dirX * chaseSpeed, rb.velocity.y);
        FaceDirection(dirX);
        OnChaseMove(dirX);
    }

    // ── Helpers ───────────────────────────────────────────

    protected void MoveHorizontal(float dirX, float speed)
    {
        rb.velocity = new Vector2(dirX * speed, rb.velocity.y);
        FaceDirection(dirX);
    }

    protected virtual void SetVelocityX(float vx)
    {
        rb.velocity = new Vector2(vx, rb.velocity.y);
    }

    protected void FaceDirection(float dirX)
    {
        if (dirX > 0 && !isFacingRight) Flip();
        else if (dirX < 0 && isFacingRight) Flip();
    }

    protected void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    protected virtual void DoIdle() { rb.velocity = new Vector2(0, rb.velocity.y); }

    // ── Overrideable ──────────────────────────────────────

    protected virtual void OnPatrolMove(float dirX) { }   // con cập nhật anim riêng
    protected virtual void OnChaseMove(float dirX)  { }
    protected abstract void TriggerAttack();

    // ── Damage / Death ────────────────────────────────────

    public virtual void TakeDamage(int damage)
    {
        if (state == EnemyState.Dead) return;
        currentHP -= damage;

        if (currentHP <= 0) Die();
        else anim.SetTrigger(HashHurt);
    }

    protected virtual void Die()
    {
        state = EnemyState.Dead;
        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        GetComponent<Collider2D>().enabled = false;
        anim.SetTrigger(HashDeath);
        Destroy(gameObject, 2f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = Color.red;    Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}