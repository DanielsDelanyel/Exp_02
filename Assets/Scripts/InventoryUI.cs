using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI instance;

    [Header("UI")]
    public GameObject inventoryWindow;
    public GameObject tooltipWindow;
    public Transform backpackArea;
    public PlayerMovement playerMovement;

    [Header("Sloty Ekwipunku (Equipment Area)")]
    // --- NOWE POLA DO PRZYPISANIA ---
    public InventorySlot weaponSlot;    // RightHand
    public InventorySlot helmetSlot;    // Head
    public InventorySlot armorSlot;     // Chest
    public InventorySlot legsSlot;      // Legs
    public InventorySlot bootsSlot;     // Boots
    public InventorySlot offhandSlot;   // LeftHand

    [Header("Drag & Drop")]
    public Image dragImage;
    public ItemData draggedItem;

    InventorySlot[] slots;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        slots = backpackArea.GetComponentsInChildren<InventorySlot>();

        if (dragImage != null) dragImage.enabled = false;
        foreach (var slot in slots) slot.ClearSlot();

        inventoryWindow.SetActive(false);
        if (tooltipWindow != null) tooltipWindow.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) ToggleInventory();

        if (draggedItem != null && dragImage != null)
        {
            dragImage.transform.position = Input.mousePosition;
        }
    }

    // Zmieñ zmienn¹ inventoryWindow na publiczn¹, jeœli by³a prywatna, 
    // aby CharacterWindowUI móg³ sprawdziæ czy jest otwarta!

    public void ToggleInventory() // Upewnij siê, ¿e funkcja jest publiczna
    {
        bool isActive = !inventoryWindow.activeSelf;

        // --- NOWOŒÆ: Jeœli otwieramy plecak -> Zamykamy okno postaci ---
        if (isActive)
        {
            if (CharacterWindowUI.instance != null && CharacterWindowUI.instance.characterPanel.activeSelf)
            {
                CharacterWindowUI.instance.ToggleCharacterWindow();
            }
        }
        // ---------------------------------------------------------------

        inventoryWindow.SetActive(isActive);

        if (!isActive)
        {
            if (tooltipWindow != null) tooltipWindow.SetActive(false);
            if (draggedItem != null)
            {
                Add(draggedItem);
                ClearDraggedItem();
            }
        }

        if (playerMovement != null)
        {
            playerMovement.enabled = !isActive;
            if (isActive) playerMovement.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }
    }

    public void OnEquipmentChanged()
    {
        PlayerEquipment playerEq = playerMovement.GetComponent<PlayerEquipment>();

        if (playerEq != null)
        {
            // Pobieramy przedmioty ze slotów (lub null, jeœli slot pusty)
            ItemData w = (weaponSlot != null) ? weaponSlot.item : null;
            ItemData h = (helmetSlot != null) ? helmetSlot.item : null;
            ItemData a = (armorSlot != null) ? armorSlot.item : null;
            ItemData l = (legsSlot != null) ? legsSlot.item : null;
            ItemData b = (bootsSlot != null) ? bootsSlot.item : null;
            ItemData s = (offhandSlot != null) ? offhandSlot.item : null;

            // Wysy³amy komplet do gracza
            playerEq.UpdateEquipment(w, h, a, l, b, s);
        }
    }
    public void UseItem(ItemData item)
    {
        if (item == null) return;

        // 1. Sprawdzamy, czy to w ogóle jest jadalne?
        if (item.itemType != ItemType.Consumable)
        {
            Debug.Log("Tego nie da siê zjeœæ! (Typ przedmiotu musi byæ Consumable)");
            // Opcjonalnie: Zwracamy przedmiot do plecaka, zamiast go usuwaæ
            ClearDraggedItem();
            // Ponownie dodajemy, ¿eby nie znikn¹³ (anulowanie akcji)
            Add(item);
            return;
        }

        // 2. Efekt dzia³ania (Leczenie)
        if (item.healAmount > 0)
        {
            PlayerStats.instance.Heal(item.healAmount);
            Debug.Log($"U¿yto: {item.itemName}. Uleczono: {item.healAmount}");
        }

        // 3. Usuwanie z ekwipunku (Logika podobna do ThrowItem, ale bez spawnowania)
        // Szukamy slotu, z którego wziêliœmy przedmiot i go czyœcimy
        foreach (InventorySlot slot in slots)
        {
            if (slot.item == item)
            {
                slot.ClearSlot();
                break;
            }
        }

        // Jeœli by³by w slocie paska skrótów (jeœli kiedyœ zrobisz), tu te¿ trzeba by sprawdziæ.

        // 4. Czyœcimy kursor (przedmiot znika bezpowrotnie - zosta³ zu¿yty)
        ClearDraggedItem();
    }

    // ... Reszta funkcji (Add, SetDraggedItem, ClearDraggedItem) bez zmian ...
    public bool Add(ItemData item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].iconDisplay.enabled == false)
            {
                slots[i].AddItem(item);
                return true;
            }
        }
        return false;
    }

    public void SetDraggedItem(ItemData item)
    {
        draggedItem = item;
        dragImage.sprite = item.icon;
        dragImage.preserveAspect = true;
        dragImage.enabled = true;
    }

    public void ClearDraggedItem()
    {
        draggedItem = null;
        dragImage.enabled = false;
    }
    public void ThrowItem(ItemData item)
    {
        if (item == null) return;

        bool foundAndRemoved = false;

        // 1. Sprawdzamy PLECAK (Stary kod)
        foreach (InventorySlot slot in slots)
        {
            if (slot.item == item)
            {
                slot.ClearSlot();
                foundAndRemoved = true;
                break;
            }
        }

        // 2. NOWOŒÆ: Jeœli nie znaleziono w plecaku, sprawdzamy EKWIPUNEK (Zbrojê itp.)
        if (!foundAndRemoved)
        {
            if (weaponSlot != null && weaponSlot.item == item) { weaponSlot.ClearSlot(); foundAndRemoved = true; }
            else if (helmetSlot != null && helmetSlot.item == item) { helmetSlot.ClearSlot(); foundAndRemoved = true; }
            else if (armorSlot != null && armorSlot.item == item) { armorSlot.ClearSlot(); foundAndRemoved = true; }
            else if (legsSlot != null && legsSlot.item == item) { legsSlot.ClearSlot(); foundAndRemoved = true; }
            else if (bootsSlot != null && bootsSlot.item == item) { bootsSlot.ClearSlot(); foundAndRemoved = true; }
            else if (offhandSlot != null && offhandSlot.item == item) { offhandSlot.ClearSlot(); foundAndRemoved = true; }
        }

        // 3. Fizyczne wyrzucenie (Spawn)
        if (item.itemPrefab != null && playerMovement != null)
        {
            float kierunek = Mathf.Sign(playerMovement.transform.localScale.x);
            Vector3 spawnPos = playerMovement.transform.position + new Vector3(kierunek * 1.5f, 1f, 0);

            GameObject droppedItem = Instantiate(item.itemPrefab, spawnPos, Quaternion.identity);

            Rigidbody2D rb = droppedItem.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 throwForce = new Vector2(kierunek * 5f, 3f);
                rb.AddForce(throwForce, ForceMode2D.Impulse);
            }
        }
        else
        {
            // To jest ten komunikat z Diagnozy 1!
            Debug.LogWarning("Nie mo¿na wyrzuciæ! Brak przypisanego 'Item Prefab' w ItemData: " + item.itemName);
        }

        // 4. Czyœcimy kursor i odœwie¿amy statystyki gracza
        ClearDraggedItem();
        OnEquipmentChanged(); // <-- WA¯NE: ¯eby odjê³o statystyki po wyrzuceniu zbroi!
    }
}