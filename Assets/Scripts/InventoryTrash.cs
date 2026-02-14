using UnityEngine;
using UnityEngine.EventSystems; // Wa¿ne dla obs³ugi zdarzeñ UI

// Dodajemy IPointerDownHandler - to reaguje na zwyk³e klikniêcie!
public class InventoryTrash : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        // Sprawdzamy, czy gracz trzyma coœ "na myszce"
        if (InventoryUI.instance.draggedItem != null)
        {
            Debug.Log("Klikniêto kosz! Wyrzucam: " + InventoryUI.instance.draggedItem.itemName);

            // Wywo³ujemy wyrzucanie
            InventoryUI.instance.ThrowItem(InventoryUI.instance.draggedItem);
        }
    }
}