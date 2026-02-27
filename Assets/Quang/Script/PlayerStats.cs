using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    [Header("Stats")]
    public int maxHP = 100;
    public int currentHP;

    private bool isInvincible;
    private PlayerController controller;
    private CameraFollow cam;
    public static bool IsDead = false;

    void Awake()
    {
        Instance = this;
        controller = GetComponent<PlayerController>();
        currentHP = maxHP;

        cam = FindObjectOfType<CameraFollow>();
    }

    public void SetInvincible(bool value) => isInvincible = value;

    public void TakeDamage(int damage)
    {
        if (isInvincible || currentHP <= 0) return;

        currentHP -= damage;
        cam?.TriggerShake();

        if (currentHP <= 0)
        {
            currentHP = 0;
            IsDead = true;
            controller.OnDeath();
        }
        else controller.OnHurt();
    }

    public void Heal(int amount)
    {
        currentHP = Mathf.Min(currentHP + amount, maxHP);
    }
}