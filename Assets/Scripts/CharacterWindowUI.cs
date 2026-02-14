using UnityEngine;
using UnityEngine.UI; // Potrzebne do Image (Avatar)
using TMPro;

public class CharacterWindowUI : MonoBehaviour
{
    public static CharacterWindowUI instance; // Singleton, ¿eby InventoryUI mog³o nas zamkn¹æ

    [Header("G³ówne Elementy")]
    public GameObject characterPanel;
    public PlayerMovement playerMovement; // ¯eby blokowaæ ruch

    [Header("Wizualizacja Nowa")]
    public Image avatarImage;          // <-- Tutaj wrzucisz obrazek twarzy
    public TextMeshProUGUI levelText;  // <-- Tutaj wrzucisz tekst "Poziom 1"

    [Header("Statystyki Tekstowe")]
    public TextMeshProUGUI strText;
    public TextMeshProUGUI witText;
    public TextMeshProUGUI intText;
    public TextMeshProUGUI zrText;

    public TextMeshProUGUI hpText;
    public TextMeshProUGUI dmgText;
    public TextMeshProUGUI defText;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        characterPanel.SetActive(false);
        PlayerStats.instance.onStatsChangedCallback += UpdateUI;
        UpdateUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleCharacterWindow();
        }
    }

    public void ToggleCharacterWindow()
    {
        bool isActive = !characterPanel.activeSelf;

        // 1. Jeœli OTWIERAMY okno postaci -> Zamykamy Ekwipunek
        if (isActive)
        {
            if (InventoryUI.instance != null && InventoryUI.instance.inventoryWindow.activeSelf)
            {
                // Wywo³ujemy funkcjê zamykania w InventoryUI
                InventoryUI.instance.ToggleInventory();
            }
        }

        // 2. W³¹czamy/Wy³¹czamy panel
        characterPanel.SetActive(isActive);

        // 3. Blokujemy/Odblokowujemy ruch gracza
        if (playerMovement != null)
        {
            playerMovement.enabled = !isActive;

            // Jeœli otwieramy okno, zatrzymaj gracza w miejscu (¿eby siê nie œlizga³)
            if (isActive)
            {
                Rigidbody2D rb = playerMovement.GetComponent<Rigidbody2D>();
                if (rb != null) rb.linearVelocity = Vector2.zero;
            }
        }
    }

    void UpdateUI()
    {
        PlayerStats stats = PlayerStats.instance;

        // --- Wyœwietlanie Poziomu ---
        if (levelText != null)
        {
            levelText.text = "Poziom " + stats.level.ToString();
        }

        // --- Wyœwietlanie Avatara (Opcjonalne, pobiera z HUD jeœli nie przypiszesz innego) ---
        // (Tu mo¿na dodaæ logikê zmiany miny, jeœli chcesz te¿ w tym oknie)

        // --- Statystyki ---
        strText.text = FormatAttribute(stats.baseSTR, stats.equipSTR);
        witText.text = FormatAttribute(stats.baseWIT, stats.equipWIT);
        intText.text = FormatAttribute(stats.baseINT, stats.equipINT);
        zrText.text = FormatAttribute(stats.baseZR, stats.equipZR);

        hpText.text = $"P¯: {stats.maxHealth}";
        dmgText.text = $"Atak: {stats.totalDamage}";
        defText.text = $"Obrona: {stats.defense}";
    }

    string FormatAttribute(int baseVal, int bonusVal)
    {
        string bonusText = "";
        if (bonusVal > 0) bonusText = $"<color=green>(+{bonusVal})</color>";
        else if (bonusVal < 0) bonusText = $"<color=red>({bonusVal})</color>";
        else bonusText = $"<color=grey>(+0)</color>";

        return $"{baseVal} {bonusText}";
    }
}