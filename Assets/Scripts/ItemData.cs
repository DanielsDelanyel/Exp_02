using UnityEngine;

// To pozwala tworzyæ przedmioty prawym przyciskiem w Project -> Create
[CreateAssetMenu(fileName = "New Item", menuName = "Ekwipunek/Przedmiot")]
public class ItemData : ScriptableObject
{
    public string itemName = "Nowy Przedmiot";
    public Sprite icon;
    [TextArea] public string description = "Opis przedmiotu...";

    public ItemType itemType;

    [Header("Konsumpcja")]
    public int healAmount = 0; // Ile punktów ¿ycia przywraca?

    [Header("Bonusy do Statystyk")]
    public int damageBonus = 0; // Bezpoœrednie obra¿enia
    public int armorBonus = 0;  // Pancerz (Obrona)

    // Statystyki Metinowe
    public int strengthBonus = 0;  // SI£ (Zwiêksza obra¿enia)
    public int vitalityBonus = 0;  // WIT (Zwiêksza HP)
    public int agilityBonus = 0;   // ZR (Uniki/Obrona)
    public int intellegenceBonus = 0; // INT (Mana/Magia)


    [Header("Logika Gry")]
    public GameObject itemPrefab;
}

// Lista typów przedmiotów
public enum ItemType
{
    General,    // Zwyk³y (do plecaka)
    Consumable, // Do wykorzystania jednorazowo
    Weapon,     // Broñ (do rêki)
    Second_Hand,// Druga rêka
    Helmet,     // He³m
    Armor,      // Zbroja
    Legs,       // Spodnie
    Boots       // Buty
}