using UnityEngine;
using UnityEngine.UI;
using TMPro;// Pamiêtaj o tym dla UI!

public class InventoryTooltip : MonoBehaviour
{
    // Singleton - ³atwy dostêp z ka¿dego miejsca
    public static InventoryTooltip instance;

    [Header("Komponenty")]
    public TextMeshProUGUI headerField;   // Tytu³ (Nazwa)
    public TextMeshProUGUI contentField;  // Opis

    [Header("Ustawienia")]
    public float offsetX = 15f; // Odsuniêcie od kursora, ¿eby go nie zas³aniaæ
    public float offsetY = -15f;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // Ukryj na start
        gameObject.SetActive(false);
    }

    void Update()
    {
        // Tooltip pod¹¿a za myszk¹ + ma³e przesuniêcie
        transform.position = Input.mousePosition + new Vector3(offsetX, offsetY, 0);
    }

    public void ShowTooltip(ItemData item)
    {
        headerField.text = item.itemName;
        contentField.text = item.description;

        // Opcjonalnie: Mo¿esz tu dodaæ kolory w zale¿noœci od rzadkoœci (item.itemType)

        gameObject.SetActive(true);
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }
}