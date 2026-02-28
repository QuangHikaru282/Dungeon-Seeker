// HealthUIManager.cs
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HealthUIManager : MonoBehaviour
{
    public static HealthUIManager Instance;

    [Header("New HP Slider")]
    public Slider hpSlider;              // Slider hiển thị máu 0-100
    public TextMeshProUGUI livesText;

    [Header("Health Bar Settings")]
    public Transform healthBar;
    public TextMeshProUGUI heartCountText; 

    public int slotCount = 3; 

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    public void InitializeHealthBar(int initialHP)
    {
        UpdateHealthUI(initialHP);
    }
    public void UpdateHealthUI(int currentHP)
    {
        if (hpSlider != null)
            hpSlider.value = currentHP;
    }

    // Cập nhật số mạng
    public void UpdateLivesUI(int lives)
    {
        if (livesText != null)
            livesText.text = "x" + lives;
    }
}
