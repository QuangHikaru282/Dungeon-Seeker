using UnityEngine;

// Script cơ bản để PlayerController biên dịch được
// Sẽ mở rộng khi làm Enemy
public class EnemyStats : MonoBehaviour
{
    public int maxHP = 50;
    public int currentHP;

    void Awake() => currentHP = maxHP;

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0) Die();
    }

    void Die()
    {
        // TODO: animation, drop item
        Destroy(gameObject);
    }
}