using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    [Header("Wizualizacja")]
    public SpriteRenderer weaponRenderer;
    public SpriteRenderer helmetRenderer;
    public SpriteRenderer chestRenderer;
    public SpriteRenderer pantsRenderer;
    public SpriteRenderer bootsRenderer;
    public SpriteRenderer offhandRenderer;

    // --- NAPRAWA: Przywracamy zmienn¹, której szuka PlayerMovement ---
    public ItemData currentWeapon;
    // -----------------------------------------------------------------

    PlayerStats stats;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
    }

    public void UpdateEquipment(ItemData weapon, ItemData helmet, ItemData armor, ItemData legs, ItemData boots, ItemData shield)
    {
        // 1. Zapamiêtujemy broñ (Dla PlayerMovement i logów)
        this.currentWeapon = weapon;

        // 2. Ustawiamy Wygl¹d
        SetRenderer(weaponRenderer, weapon);
        SetRenderer(helmetRenderer, helmet);
        SetRenderer(chestRenderer, armor);
        SetRenderer(pantsRenderer, legs);
        SetRenderer(bootsRenderer, boots);
        SetRenderer(offhandRenderer, shield);

        // 3. Resetujemy statystyki
        stats.equipBaseDmg = 0;
        stats.equipSTR = 0;
        stats.equipWIT = 0;
        stats.equipZR = 0;
        stats.equipINT = 0;
        stats.equipDef = 0;

        // 4. Sumujemy bonusy
        AddBonuses(weapon);
        AddBonuses(helmet);
        AddBonuses(armor);
        AddBonuses(legs);
        AddBonuses(boots);
        AddBonuses(shield);

        // 5. Przeliczamy
        stats.RecalculateStats();
    }

    void SetRenderer(SpriteRenderer sr, ItemData item)
    {
        if (sr == null) return;

        if (item != null)
        {
            sr.sprite = item.icon;
            sr.enabled = true;
        }
        else
        {
            sr.sprite = null;
            sr.enabled = false;
        }
    }

    void AddBonuses(ItemData item)
    {
        if (item == null) return;

        stats.equipBaseDmg += item.damageBonus;
        stats.equipSTR += item.strengthBonus;
        stats.equipWIT += item.vitalityBonus;
        stats.equipZR += item.agilityBonus;
        stats.equipINT += item.intellegenceBonus; 
        stats.equipDef += item.armorBonus;
    }
}