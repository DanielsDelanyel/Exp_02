using UnityEngine;
using TMPro; // Wa¿ne dla obs³ugi tekstu!

public class DamagePopup : MonoBehaviour
{
    private TextMeshPro textMesh;
    private float disappearTimer;
    private Color textColor;


    [Header("Ruch (Lob)")]
    public float initialYVelocity = 7f;  // Si³a wyrzutu w górê
    public float xVelocityRange = 2f;    // Losowy rozrzut na boki
    public float gravity = 15f;          // Jak szybko ma spadaæ (symulowana grawitacja)

    private Vector3 moveVector;

    [Header("Skalowanie")]
    public Vector3 startScale = new Vector3(1f, 1f, 1f); // Pocz¹tkowy rozmiar
    public Vector3 endScale = new Vector3(2f, 2f, 2f);   // Koñcowy rozmiar
    public float animSpeed = 5f; // Jak szybko siê powiêksza

    void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    // Tê funkcjê wywo³asz z poziomu przeciwnika
    public void Setup(int damageAmount, Color color, Vector3 startSize, Vector3 endSize)
    {
        textMesh.text = damageAmount.ToString();
        textMesh.color = color;
        textColor = color;

        // Nadpisujemy rozmiary tymi z inspektora wroga (opcjonalnie)
        if (startSize != Vector3.zero) startScale = startSize;
        if (endSize != Vector3.zero) endScale = endSize;

        transform.localScale = startScale;

        // Ustawiamy losowy wektor ruchu (LOB)
        // Losowo w lewo lub w prawo + mocno w górê
        moveVector = new Vector3(Random.Range(-xVelocityRange, xVelocityRange), initialYVelocity);

        disappearTimer = 1f; // Tekst ¿yje 1 sekundê
    }

    void Update()
    {
        // 1. RUCH (Symulacja fizyki bez Rigidbody)
        transform.position += moveVector * Time.deltaTime;

        // Symulacja grawitacji (zmniejszamy prêdkoœæ Y w dó³)
        moveVector.y -= gravity * Time.deltaTime;

        // 2. SKALOWANIE (Powiêkszanie w czasie)
        // Lerp p³ynnie zmienia wartoœæ od startScale do endScale
        transform.localScale = Vector3.Lerp(transform.localScale, endScale, Time.deltaTime * animSpeed);

        // 3. ZNIKANIE (Fade out)
        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            // Zmniejszamy przezroczystoœæ (Alpha)
            float fadeSpeed = 3f;
            textColor.a -= fadeSpeed * Time.deltaTime;
            textMesh.color = textColor;

            if (textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}