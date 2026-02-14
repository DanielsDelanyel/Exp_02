using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance;

    [Header("Rozwój Postaci")]
    public int level = 1;      
    public int currentExp = 0;
    public int expToNextLevel = 100;

    [Header("Statystyki Bazowe")]
    public int baseSTR = 5;
    public int baseWIT = 5;
    public int baseINT = 5;
    public int baseZR = 5;

    [Header("Bonusy z Ekwipunku")]
    public int equipSTR = 0;
    public int equipWIT = 0;
    public int equipINT = 0;
    public int equipZR = 0;
    public int equipBaseDmg = 0;
    public int equipDef = 0;

    [Header("Statystyki Koñcowe")]
    public int totalDamage;
    public int maxHealth;
    public int currentHealth;
    public float maxStamina;
    public float currentStamina;
    public int defense;

    public float staminaRegen = 3f;

    [Header("Walka i Popupy")]
    public GameObject damagePopupPrefab;
    public float invincibilityTime = 1f;
    private float invincibilityTimer;

    public delegate void OnStatsChanged();
    public OnStatsChanged onStatsChangedCallback;

    public delegate void OnHealthChanged();
    public OnHealthChanged onHealthChangedCallback;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        RecalculateStats();
        currentHealth = maxHealth;
        currentStamina = maxStamina;
    }

    void Update()
    {
        if (invincibilityTimer > 0) invincibilityTimer -= Time.deltaTime;

        if (currentStamina < maxStamina)
        {
            currentStamina += staminaRegen * Time.deltaTime;
            if (currentStamina > maxStamina) currentStamina = maxStamina;
        }
    }

    public void RecalculateStats()
    {
        int finalSTR = baseSTR + equipSTR;
        int finalWIT = baseWIT + equipWIT;
        int finalZR = baseZR + equipZR;

        // --- Logika Poziomów (Tutaj w przysz³oœci dodamy wiêcej matematyki) ---
        // Na razie poziom tylko wyœwietlamy, nie wp³ywa na statystyki

        // --- Stamina ---
        maxStamina = 75f + (finalZR * 5f);
        if (currentStamina > maxStamina) currentStamina = maxStamina;

        // --- Zdrowie ---
        int oldMaxHealth = maxHealth;
        maxHealth = finalWIT * 10;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        // --- Walka ---
        totalDamage = (finalSTR * 2) + equipBaseDmg;
        defense = (finalZR * 1) + equipDef;

        if (onStatsChangedCallback != null) onStatsChangedCallback.Invoke();
        if (onHealthChangedCallback != null) onHealthChangedCallback.Invoke();
    }

    // --- Reszta funkcji (TakeDamage, Heal, itp.) bez zmian ---
    public bool HasStamina(float amount) { return currentStamina >= amount; }
    public void UseStamina(float amount) { currentStamina -= amount; if (currentStamina < 0) currentStamina = 0; }

    public void TakeDamage(int damage)
    {
        if (invincibilityTimer > 0) return;
        damage = damage - (defense / 2);
        if (damage < 1) damage = 1;
        currentHealth -= damage;

        if (damagePopupPrefab != null)
        {
            GameObject popup = Instantiate(damagePopupPrefab, transform.position, Quaternion.identity);
            DamagePopup popupScript = popup.GetComponent<DamagePopup>();
            if (popupScript != null) popupScript.Setup(damage, Color.red, Vector3.one, Vector3.one * 1.5f);
        }
        if (onHealthChangedCallback != null) onHealthChangedCallback.Invoke();
        invincibilityTimer = invincibilityTime;
        if (currentHealth <= 0) Debug.Log("Zgon");
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        if (damagePopupPrefab != null)
        {
            GameObject popup = Instantiate(damagePopupPrefab, transform.position, Quaternion.identity);
            DamagePopup popupScript = popup.GetComponent<DamagePopup>();
            if (popupScript != null) popupScript.Setup(amount, Color.green, Vector3.one, Vector3.one * 1.5f);
        }
        if (onHealthChangedCallback != null) onHealthChangedCallback.Invoke();
    }
}