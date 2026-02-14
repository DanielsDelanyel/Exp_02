using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryUseArea : MonoBehaviour, IDropHandler, IPointerDownHandler
{
    // Obs³uga upuszczenia (Drag & Drop)
    public void OnDrop(PointerEventData eventData)
    {
        TryUseItem();
    }

    // Obs³uga klikniêcia (Dla spójnoœci z systemem "Click & Place")
    public void OnPointerDown(PointerEventData eventData)
    {
        TryUseItem();
    }

    void TryUseItem()
    {
        // Sprawdzamy, czy trzymamy przedmiot na myszce
        if (InventoryUI.instance.draggedItem != null)
        {
            // Wywo³ujemy funkcjê U¯YCIA (UseItem), a nie Wyrzucenia
            InventoryUI.instance.UseItem(InventoryUI.instance.draggedItem);
        }
    }
}