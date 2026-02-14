using UnityEngine;
using System.Collections.Generic; // Potrzebne do List

public class Chest : MonoBehaviour
{
    public enum Rarity { Common, Rare, Epic, Legendary }

    [Header("Tryb Skrzyni")]
    public bool isMysteryChest = false; // CZY TO TAJEMNICZA SKRZYNIA?

    [Header("Konfiguracja (Dla Zwyk³ej Skrzyni)")]
    public Rarity fixedRarity;       // U¿ywane, jeœli isMysteryChest = false
    public GameObject fixedLoot;     // U¿ywane, jeœli isMysteryChest = false

    [Header("Konfiguracja (Dla Mystery Chest)")]
    [Range(0, 100)] public float chanceRare = 30f;      // Szansa na Rare
    [Range(0, 100)] public float chanceEpic = 10f;      // Szansa na Epic
    [Range(0, 100)] public float chanceLegendary = 1f;  // Szansa na Legendary
    // Reszta to szansa na Common (100 - suma powy¿szych)

    [Header("Pule Przedmiotów (Drop Tables)")]
    public List<GameObject> commonDrops;
    public List<GameObject> rareDrops;
    public List<GameObject> epicDrops;
    public List<GameObject> legendaryDrops;

    [Header("Wygl¹d i Efekty")]
    public Sprite openSprite;
    public GameObject glowPrefab;
    public GameObject promptE;
    public ChestLightEffect lightEffect;

    [Header("Si³a wyrzutu")]
    public float minForce = 3f;
    public float maxForce = 6f;

    private bool isPlayerClose = false;
    private bool isOpen = false;
    private GameObject activePrompt;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isPlayerClose && Input.GetKeyDown(KeyCode.E) && !isOpen)
        {
            OpenChest();
        }
    }

    void OpenChest()
    {
        isOpen = true;

        // 1. USTALAMY RZADKOŒÆ (Losowa lub Sta³a)
        Rarity finalRarity;

        if (isMysteryChest)
        {
            finalRarity = RollRarity(); // Losujemy!
            Debug.Log("Mystery Chest wylosowa³: " + finalRarity);
        }
        else
        {
            finalRarity = fixedRarity; // Bierzemy ustawion¹ na sztywno
        }

        // 2. DOBIERAMY KOLOR DO WYLOSOWANEJ RZADKOŒCI
        Color finalColor = GetColorFromRarity(finalRarity);

        // 3. EFEKTY WIZUALNE (Cz¹steczki + Promienie)
        SpawnGlow(finalColor);
        if (lightEffect != null)
        {
            lightEffect.ActivateEffect(finalColor);
        }

        // 4. ZMIANA GRAFIKI I INTERFEJS
        if (openSprite != null) sr.sprite = openSprite;
        if (activePrompt != null) Destroy(activePrompt);

        // 5. WYRZUCENIE PRZEDMIOTU
        SpawnLoot(finalRarity);
    }

    // --- LOGIKA LOSOWANIA RZADKOŒCI ---
    Rarity RollRarity()
    {
        float roll = Random.Range(0f, 100f);

        // Sprawdzamy od najrzadszego (¿eby Legendary mia³o priorytet)
        if (roll <= chanceLegendary) return Rarity.Legendary;
        if (roll <= chanceLegendary + chanceEpic) return Rarity.Epic;
        if (roll <= chanceLegendary + chanceEpic + chanceRare) return Rarity.Rare;

        // Jeœli nic nie wypad³o, to Common
        return Rarity.Common;
    }

    // --- LOGIKA WYBORU PRZEDMIOTU ---
    void SpawnLoot(Rarity rarityToSpawn)
    {
        GameObject itemToSpawn = null;

        if (isMysteryChest)
        {
            // Wybieramy losowy przedmiot z odpowiedniej listy
            List<GameObject> pool = null;

            switch (rarityToSpawn)
            {
                case Rarity.Common: pool = commonDrops; break;
                case Rarity.Rare: pool = rareDrops; break;
                case Rarity.Epic: pool = epicDrops; break;
                case Rarity.Legendary: pool = legendaryDrops; break;
            }

            // Losujemy z listy (jeœli lista nie jest pusta)
            if (pool != null && pool.Count > 0)
            {
                int index = Random.Range(0, pool.Count);
                itemToSpawn = pool[index];
            }
            else
            {
                Debug.LogWarning($"Pula przedmiotów dla {rarityToSpawn} jest pusta!");
            }
        }
        else
        {
            // Stary tryb: jeden konkretny przedmiot
            itemToSpawn = fixedLoot;
        }

        // Fizyczne stworzenie przedmiotu
        if (itemToSpawn != null)
        {
            // Tworzymy przedmiot lekko nad skrzyni¹
            GameObject loot = Instantiate(itemToSpawn, transform.position + Vector3.up * 0.5f, Quaternion.identity);

            Rigidbody2D rb = loot.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 direction = new Vector2(Random.Range(-1f, 1f), 1f).normalized;
                float force = Random.Range(minForce, maxForce);
                rb.AddForce(direction * force, ForceMode2D.Impulse);
            }
        }
    }

    Color GetColorFromRarity(Rarity r)
    {
        switch (r)
        {
            case Rarity.Common: return Color.white;
            case Rarity.Rare: return Color.blue;
            case Rarity.Epic: return new Color(0.6f, 0f, 1f); // Fiolet
            case Rarity.Legendary: return Color.yellow; // Z³oty
            default: return Color.white;
        }
    }

    void SpawnGlow(Color colorToUse)
    {
        if (glowPrefab != null)
        {
            GameObject glow = Instantiate(glowPrefab, transform.position, Quaternion.identity);
            ParticleSystem ps = glow.GetComponent<ParticleSystem>();

            if (ps != null)
            {
                var mainSettings = ps.main;
                mainSettings.startColor = colorToUse;
            }
            Destroy(glow, 5f);
        }
    }

    // --- Trigger Enter/Exit bez zmian ---
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isOpen)
        {
            isPlayerClose = true;
            if (activePrompt == null && promptE != null)
            {
                activePrompt = Instantiate(promptE, transform.position + Vector3.up * 1f, Quaternion.identity);
                activePrompt.transform.SetParent(transform);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerClose = false;
            if (activePrompt != null) Destroy(activePrompt);
        }
    }
}