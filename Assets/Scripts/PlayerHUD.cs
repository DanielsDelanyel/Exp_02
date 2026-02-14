using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [Header("Paski")]
    public Slider healthSlider;
    public Slider manaSlider;
    public Slider staminaSlider;

    [Header("Tekst (Opcjonalnie)")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI staminaText; // <-- NOWOŒÆ: Pole na tekst staminy

    [Header("Awatar Postaci")]
    public Image faceImage;
    public Sprite faceHappy;
    public Sprite faceNeutral;
    public Sprite faceSad;

    void Start()
    {
        PlayerStats.instance.onHealthChangedCallback += UpdateHealthUI;
        InitHUD();
    }

    void Update()
    {
        UpdateStaminaUI();
    }

    void InitHUD()
    {
        UpdateHealthUI();
        UpdateStaminaUI();
    }

    void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = PlayerStats.instance.maxHealth;
            healthSlider.value = PlayerStats.instance.currentHealth;
        }

        if (healthText != null)
        {
            healthText.text = $"{PlayerStats.instance.currentHealth} / {PlayerStats.instance.maxHealth}";
        }

        UpdateFace();
    }

    void UpdateStaminaUI()
    {
        // 1. Aktualizacja suwaka
        if (staminaSlider != null)
        {
            staminaSlider.maxValue = PlayerStats.instance.maxStamina;
            staminaSlider.value = PlayerStats.instance.currentStamina;
        }

        // 2. Aktualizacja tekstu (Liczby ca³kowite)
        if (staminaText != null)
        {
            // Rzutowanie na (int) sprawia, ¿e 74.99 wyœwietli siê jako 74
            int current = (int)PlayerStats.instance.currentStamina;
            int max = (int)PlayerStats.instance.maxStamina;

            staminaText.text = $"{current} / {max}";
        }
    }

    void UpdateFace()
    {
        if (faceImage == null) return;

        float maxHP = PlayerStats.instance.maxHealth;
        if (maxHP <= 0) maxHP = 1;

        float percent = (float)PlayerStats.instance.currentHealth / maxHP;

        if (percent > 0.66f)
        {
            if (faceHappy != null) faceImage.sprite = faceHappy;
        }
        else if (percent > 0.33f)
        {
            if (faceNeutral != null) faceImage.sprite = faceNeutral;
        }
        else
        {
            if (faceSad != null) faceImage.sprite = faceSad;
        }
    }
}