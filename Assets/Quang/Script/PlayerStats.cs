using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    [Header("Stats")]
    public int maxHP = 100;
    public int currentHP;

    private bool isInvincible;
    private PlayerController controller;

    void Awake()
    {
        Instance = this;
        controller = GetComponent<PlayerController>();
        currentHP = maxHP;
    }

    public void SetInvincible(bool value) => isInvincible = value;

    public void TakeDamage(int damage)
    {
        if (isInvincible || currentHP <= 0) return;

        currentHP -= damage;

        if (currentHP <= 0)
        {
            currentHP = 0;
            controller.OnDeath();
        }
        else
        {
            controller.OnHurt();
        }

        // TODO: cập nhật UI HP bar
    }

    public void Heal(int amount)
    {
        currentHP = Mathf.Min(currentHP + amount, maxHP);
    }
}