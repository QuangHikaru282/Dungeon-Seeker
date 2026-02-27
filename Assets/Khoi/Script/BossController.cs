using System.Collections;
using UnityEngine;

public enum BossState { Idle, Walk, Attack, Stagger, Dead }

[RequireComponent(typeof(Animator), typeof(Rigidbody2D))]
public class BossController : MonoBehaviour
{
    [Header("Stats")]
    public int maxHP = 500;
    public int currentHP;

    [Header("Movement")]
    public float moveSpeed = 2.5f;

    [Header("Detection & Attack")]
    public float detectRange = 8f;
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;

    [Header("Phase 2 Transition")]
    private Vector3 baseScale = new Vector3(1f, 1f, 1f);
    public float staggerDuration = 3f;
    public Vector3 phase2Scale = new Vector3(1.3f, 1.3f, 1.3f);

    [Header("Attack 2 - Ice Projectile")]
    public GameObject iceProjectilePrefab;
    public Transform firePoint;
    public float attack2Range = 6f;
    public float attack2Cooldown = 5f;
    private float attack2Timer = 0f;

    [Header("Attack 3 - AOE")]
    public Transform aoeCenter;
    public float aoeRadius = 2.5f;
    public int aoeDamage = 20;
    public LayerMask playerLayer;

    [Header("Attack Damage")]
    public int attack1Damage = 15;

    // Components
    private Animator anim;
    private Rigidbody2D rb;
    private Transform player;

    // State
    private BossState state = BossState.Idle;
    private bool isPhase2 = false;
    private bool inTransition = false;
    private bool isFacingRight = true;
    private float attackTimer;

    // Animation hashes
    private static readonly int HashWalk    = Animator.StringToHash("Walk");
    private static readonly int HashIdle    = Animator.StringToHash("Idle");
    private static readonly int HashAtk1   = Animator.StringToHash("Attack1");
    private static readonly int HashAtk2   = Animator.StringToHash("Attack2");
    private static readonly int HashAtk3   = Animator.StringToHash("Attack3");
    private static readonly int HashStagger = Animator.StringToHash("Stagger");
    private static readonly int HashHurt   = Animator.StringToHash("Hurt");
    private static readonly int HashDeath  = Animator.StringToHash("Death");

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb   = GetComponent<Rigidbody2D>();
        currentHP = maxHP;

        GameObject go = GameObject.FindGameObjectWithTag("Player");
        if (go) player = go.transform;
        baseScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    void Update()
    {
        if (state == BossState.Dead || inTransition) return;
        if (PlayerStats.IsDead)
        {
            StandIdle();
            return;
        }

        attackTimer -= Time.deltaTime;
        attack2Timer -= Time.deltaTime;
        float dist = player ? Vector2.Distance(transform.position, player.position) : Mathf.Infinity;

        switch (state)
        {
            case BossState.Idle:
            case BossState.Walk:
            float distToPlayer = player ? Vector2.Distance(transform.position, player.position) : Mathf.Infinity;

            if (distToPlayer <= attackRange)
            {
                // Trong tầm gần → Attack 1 hoặc 3
                EnterMeleeAttack();
            }
            else if (distToPlayer <= attack2Range && isPhase2 && attack2Timer <= 0f)
            {
                // Trong tầm xa + Phase 2 + hết cooldown → Attack 2
                EnterRangedAttack();
            }
            else if (distToPlayer <= detectRange)
            {
                // Còn lại → dí theo Player
                ChasePlayer();
            }
            else StandIdle();
            break;

            case BossState.Attack:
                rb.velocity = Vector2.zero;
                break;
        }
    }

    // ── Movement ──────────────────────────────────────────

    void ChasePlayer()
    {
        float dirX = player.position.x > transform.position.x ? 1f : -1f;
        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);
        FaceDirection(dirX);
        anim.SetBool(HashWalk, true);
        anim.SetBool(HashIdle, false);
        state = BossState.Walk;
    }

    void StandIdle()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
        anim.SetBool(HashWalk, false);
        anim.SetBool(HashIdle, true);
        state = BossState.Idle;
    }

    // ── Attack ────────────────────────────────────────────

    void EnterMeleeAttack()
    {
        if (attackTimer > 0) { StandIdle(); return; }
        attackTimer = attackCooldown;
        state = BossState.Attack;
        anim.SetBool(HashWalk, false);

        if (!isPhase2)
        {
            anim.SetTrigger(HashAtk1);
        }
        else
        {
            int roll = Random.Range(0, 2);
            anim.SetTrigger(roll == 0 ? HashAtk1 : HashAtk3);
        }
    }

    void EnterRangedAttack()
    {
        if (attackTimer > 0) { ChasePlayer(); return; }
        attackTimer  = attackCooldown;
        attack2Timer = attack2Cooldown;
        state = BossState.Attack;
        anim.SetBool(HashWalk, false);
        anim.SetTrigger(HashAtk2);
    }

    // Animation Event — cuối mỗi clip Attack
    public void OnAttackEnd()
    {
        state = BossState.Idle;
    }

    // Animation Event — frame chạm của Attack 1
    public void OnAttack1Hit()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, attackRange, playerLayer);
        hit?.GetComponent<PlayerStats>()?.TakeDamage(attack1Damage);
    }

    // Animation Event — frame bắn của Attack 2
    public void OnAttack2Fire()
    {
        if (!iceProjectilePrefab || !firePoint) return;
        float dirX = isFacingRight ? 1f : -1f;
        GameObject proj = Instantiate(iceProjectilePrefab, firePoint.position, Quaternion.identity);
        proj.GetComponent<IceProjectile>()?.Init(dirX);
    }

    // Animation Event — frame kích hoạt AOE của Attack 3
    public void OnAttack3AOE()
    {
        Collider2D hit = Physics2D.OverlapCircle(aoeCenter.position, aoeRadius, playerLayer);
        hit?.GetComponent<PlayerStats>()?.TakeDamage(aoeDamage);
    }

    // ── Damage / Phase ────────────────────────────────────

    public void TakeDamage(int damage)
    {
        if (state == BossState.Dead || inTransition) return;

        currentHP -= damage;

        if (currentHP <= 0) { Die(); return; }

        if (!isPhase2 && currentHP <= maxHP / 2)
        {
            StartCoroutine(Phase2Transition());
            return;
        }

        state = BossState.Idle;   // reset state trước khi play Hurt
        anim.SetTrigger(HashHurt);
    }

    // Gọi từ Animation Event ở frame cuối clip Hurt
    public void OnHurtEnd()
    {
        if (state != BossState.Dead)
            state = BossState.Idle;
    }

    IEnumerator Phase2Transition()
    {
        inTransition = true;
        state = BossState.Stagger;
        rb.velocity = Vector2.zero;
        anim.SetTrigger(HashStagger);

        float elapsed = 0f;
        float startSize = Mathf.Abs(transform.localScale.x);  // lấy giá trị tuyệt đối
        float targetSize = phase2Scale.x;                      // 1.3

        while (elapsed < staggerDuration)
        {
            elapsed += Time.deltaTime;
            float newSize = Mathf.Lerp(startSize, targetSize, elapsed / staggerDuration);
            // Giữ đúng hướng flip trong khi scale
            transform.localScale = new Vector3(isFacingRight ? newSize : -newSize, newSize, 1f);
            yield return null;
        }

        // Cập nhật baseScale sau transition
        baseScale = new Vector3(targetSize, targetSize, 1f);

        isPhase2 = true;
        inTransition = false;
        state = BossState.Idle;
    }

    void Die()
    {
        state = BossState.Dead;
        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        GetComponent<Collider2D>().enabled = false;
        anim.SetTrigger(HashDeath);
        // TODO: drop đá mở phong ấn
    }

    // ── Utilities ─────────────────────────────────────────

    void FaceDirection(float dirX)
    {
        if (dirX > 0 && !isFacingRight) Flip();
        else if (dirX < 0 && isFacingRight) Flip();
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        // Chỉ flip trục X, giữ nguyên Y và Z theo scale hiện tại
        float currentScaleX = Mathf.Abs(transform.localScale.x);
        transform.localScale = new Vector3(isFacingRight ? currentScaleX : -currentScaleX,
                                        transform.localScale.y,
                                        transform.localScale.z);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = Color.red;    Gizmos.DrawWireSphere(transform.position, attackRange);
        if (aoeCenter) { Gizmos.color = Color.cyan; Gizmos.DrawWireSphere(aoeCenter.position, aoeRadius); }
    }
}