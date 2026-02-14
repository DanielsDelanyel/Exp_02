using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("Dane Przedmiotu")]
    public ItemData itemData; // <-- To musisz przypisaæ w Inspektorze!

    [Header("Ustawienia")]
    public GameObject promptE;
    private bool isPlayerClose = false;
    private GameObject activePrompt;

    void Update()
    {
        if (isPlayerClose && Input.GetKeyDown(KeyCode.E))
        {
            PickUp();
        }
    }

    void PickUp()
    {
        // Próbujemy dodaæ do ekwipunku
        bool pickedUp = InventoryUI.instance.Add(itemData);

        if (pickedUp)
        {
            Debug.Log("Podniesiono: " + itemData.itemName);
            Destroy(activePrompt);
            Destroy(gameObject);
        }
    }

    // ... Reszta kodu OnTriggerEnter/Exit bez zmian ...
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerClose = true;
            if (activePrompt == null && promptE != null)
            {
                activePrompt = Instantiate(promptE, transform.position + Vector3.up * 0.5f, Quaternion.identity);
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