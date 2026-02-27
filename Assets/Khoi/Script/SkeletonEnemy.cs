using UnityEngine;

public class SkeletonEnemy : BaseEnemy
{
    [Header("Skeleton - Melee")]
    public Transform attackPoint;
    public float meleeRange = 0.5f;
    public int meleeDamage = 10;
    public LayerMask playerLayer;

    private static readonly int HashWalk = Animator.StringToHash("Walk");
    private static readonly int HashIdle = Animator.StringToHash("Idle");

    protected override void OnPatrolMove(float dirX)
    {
        anim.SetBool(HashWalk, true);
        anim.SetBool(HashIdle, false);
    }

    protected override void OnChaseMove(float dirX)
    {
        anim.SetBool(HashWalk, true);
        anim.SetBool(HashIdle, false);
    }

    protected override void DoIdle()
    {
        base.DoIdle();
        anim.SetBool(HashWalk, false);
        anim.SetBool(HashIdle, true);
    }

    protected override void TriggerAttack()
    {
        anim.SetTrigger(HashAttack);
        // Animation Event gọi OnMeleeHit()
    }

    // Gọi từ Animation Event tại frame chạm
    public void OnMeleeHit()
    {
        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, meleeRange, playerLayer);
        hit?.GetComponent<PlayerStats>()?.TakeDamage(meleeDamage);
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(attackPoint.position, meleeRange);
        }
    }
}