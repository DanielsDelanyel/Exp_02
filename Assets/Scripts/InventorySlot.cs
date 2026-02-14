using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Potrzebne do klikania

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Image iconDisplay;

    // Zmieniamy 'item' na publiczny (lub dodajemy getter), ¿eby ³atwo sprawdzaæ co tu jest
    public ItemData item;

    [Header("Konfiguracja Slotu")]
    public bool isBackpackSlot = true; // Czy to zwyk³y plecak? (Przyjmuje wszystko)
    public ItemType allowedType;       // Jeœli NIE plecak, to co przyjmuje? (np. Weapon)

    void Start()
    {
        if (iconDisplay == null || iconDisplay.gameObject.activeInHierarchy == false)
        {
            iconDisplay = transform.Find("Icon").GetComponent<Image>();
        }
        if (item == null) ClearSlot();
    }

    public void AddItem(ItemData newItem)
    {
        item = newItem;
        iconDisplay.sprite = item.icon;
        iconDisplay.preserveAspect = true;
        iconDisplay.enabled = true;
    }

    public void ClearSlot()
    {
        item = null;
        iconDisplay.sprite = null;
        if (iconDisplay != null) iconDisplay.enabled = false;
    }

    // --- LOGIKA KLIKANIA (PRZENOSZENIE) ---
    public void OnPointerClick(PointerEventData eventData)
    {
        // 1. Sprawdzamy co trzyma gracz na myszce
        ItemData mouseItem = InventoryUI.instance.draggedItem;

        // SCENARIUSZ A: Gracz nic nie trzyma, a w slocie coœ jest -> PODNIEŒ
        if (mouseItem == null)
        {
            if (item != null)
            {
                InventoryUI.instance.SetDraggedItem(item); // Daj przedmiot na myszkê
                ClearSlot(); // Wyczyœæ slot
                InventoryTooltip.instance.HideTooltip(); // Ukryj opis, ¿eby nie przeszkadza³
            }

        }
        // SCENARIUSZ B: Gracz trzyma coœ na myszce -> SPRÓBUJ PO£O¯YÆ / ZAMIENIÆ
        else
        {
            // Najpierw sprawdzamy, czy ten slot w ogóle przyjmuje taki typ przedmiotu!
            if (CheckIfItemFits(mouseItem))
            {
                // Zapamiêtujemy, co by³o w slocie (do ewentualnej zamiany)
                ItemData itemInSlot = item;

                // K³adziemy nowy przedmiot w slocie
                AddItem(mouseItem);

                // Co robimy z tym, co by³o w slocie?
                if (itemInSlot != null)
                {
                    // ZAMIANA: To co by³o w slocie, trafia na myszkê
                    InventoryUI.instance.SetDraggedItem(itemInSlot);
                }
                else
                {
                    // PO£O¯ENIE: Slot by³ pusty, wiêc myszka jest teraz pusta
                    InventoryUI.instance.ClearDraggedItem();
                }
            }
            else
            {
                Debug.Log("Ten przedmiot tu nie pasuje!");
            }
        }

        if (isBackpackSlot == false)
        {
            // Powiedz InventoryUI, ¿eby odœwie¿y³ wygl¹d gracza
            InventoryUI.instance.OnEquipmentChanged();
        }
    }


    // Funkcja sprawdzaj¹ca czy przedmiot pasuje do slotu
    bool CheckIfItemFits(ItemData itemToCheck)
    {
        // Jeœli to slot plecaka -> przyjmuje wszystko
        if (isBackpackSlot) return true;

        // Jeœli to slot wyposa¿enia -> sprawdŸ typ
        if (itemToCheck.itemType == allowedType) return true;

        return false;
    }

    // --- Tooltipy (Bez zmian) ---
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null && InventoryUI.instance.draggedItem == null) // Pokazuj tylko jak nic nie niesiemy
        {
            InventoryTooltip.instance.ShowTooltip(item);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryTooltip.instance.HideTooltip();
    }
}