using UnityEngine;

public class BatEnemy : BaseEnemy
{
    [Header("Bat - Projectile")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 6f;

    [Header("Bat - Fly")]
    public float flyAmplitude = 0.5f;   // biên độ lắc lư trên/dưới khi patrol
    public float flyFrequency = 2f;

    private static readonly int HashFly = Animator.StringToHash("Fly");

    protected override void Awake()
    {
        base.Awake();
        // Bat bay tự do, không bị gravity
        rb.gravityScale = 0f;
    }

    protected override void Update()
    {
        base.Update();

        // Luôn áp lắc lư trục Y (trừ khi chết)
        if (state != EnemyState.Dead)
            rb.velocity = new Vector2(rb.velocity.x,
                Mathf.Sin(Time.time * flyFrequency) * flyAmplitude);
    }

    // Bat di chuyển cả trục Y (bay)
    protected override void SetVelocityX(float vx)
    {
        float vy = Mathf.Sin(Time.time * flyFrequency) * flyAmplitude;
        rb.velocity = new Vector2(vx, vy);
    }

    protected override void OnPatrolMove(float dirX)
    {
        anim.SetBool(HashFly, true);
    }

    protected override void OnChaseMove(float dirX)
    {
        // Chase chỉ trục X, giữ bay lắc lư trục Y như patrol
        anim.SetBool(HashFly, true);
    }

    protected override void DoIdle()
    {
        rb.velocity = Vector2.zero;
        anim.SetBool(HashFly, true);
    }

    protected override void TriggerAttack()
    {
        anim.SetTrigger(HashAttack);
        // Animation Event sẽ gọi OnFireProjectile()
    }

    // Gọi từ Animation Event
    public void OnFireProjectile()
    {
        if (!projectilePrefab || !player) return;
        Vector2 dir = ((Vector2)player.position - (Vector2)firePoint.position).normalized;
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        proj.GetComponent<Projectile>()?.Init(dir, projectileSpeed);
    }
}