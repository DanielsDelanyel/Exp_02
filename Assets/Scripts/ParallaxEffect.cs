using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    [Header("Ustawienia")]
    public Camera cam;

    [Range(0f, 1f)]
    public float parallaxEffect;
    // 0 = Obiekt stoi w miejscu (jak œciana)
    // 1 = Obiekt rusza siê idealnie z kamer¹ (jak przyklejony do ekranu, np. t³o nieba)
    // 0.5 = Rusza siê z po³ow¹ prêdkoœci (dalekie góry)

    private float startPos;
    private float length;

    void Start()
    {
        startPos = transform.position.x;

        if (cam == null) cam = Camera.main;

        // --- POPRAWKA: Szukamy SpriteRenderer w DZIECIACH (Children), a nie na sobie ---
        SpriteRenderer childSprite = GetComponentInChildren<SpriteRenderer>();

        if (childSprite != null)
        {
            length = childSprite.bounds.size.x;
            Debug.Log($"Namierzono t³o! Szerokoœæ wynosi: {length}");
        }
        else
        {
            Debug.LogError("B£¥D: Skrypt ParallaxEffect nie widzi ¿adnego obrazka w dzieciach!");
        }
    }

    void Update()
    {
        // Ile przesunêliœmy siê wzglêdem efektu paralaksy
        float temp = (cam.transform.position.x * (1 - parallaxEffect));

        // Ile mamy przesun¹æ t³o
        float dist = (cam.transform.position.x * parallaxEffect);

        // Przesuwamy obiekt
        transform.position = new Vector3(startPos + dist, transform.position.y, transform.position.z);

        // --- Opcjonalne: Nieskoñczone przewijanie t³a ---
        // Jeœli kamera wyjecha³a za daleko w prawo, przesuñ t³o do przodu
        if (length > 0)
        {
            if (temp > startPos + length) startPos += length;
            else if (temp < startPos - length) startPos -= length;
        }
    }
}
