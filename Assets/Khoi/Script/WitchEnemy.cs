using UnityEngine;

public class WitchEnemy : BaseEnemy
{
    [Header("Witch - Projectile")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 7f;

    private static readonly int HashWalk  = Animator.StringToHash("Walk");
    private static readonly int HashIdle  = Animator.StringToHash("Idle");

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
        // Animation Event gọi OnFireProjectile()
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