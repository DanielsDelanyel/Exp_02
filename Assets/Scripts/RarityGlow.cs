using UnityEngine;

public class RarityGlow : MonoBehaviour
{
    [Header("Konfiguracja")]
    public GameObject rayPrefab;   // Tu wrzucisz swój prefab "Capsule"
    public int rayCount = 8;       // Ile promieni chcesz mieæ?
    public float spinSpeed = 30f;  // Jak szybko maj¹ siê krêciæ?

    [Header("Wygl¹d")]
    public Color glowColor = Color.yellow; // G³ówny kolor poœwiaty

    // Lista, ¿ebyœmy mogli potem zmieniaæ ich kolor w locie
    private SpriteRenderer[] spawnedRays;

    void Start()
    {
        SpawnRays();
    }

    void Update()
    {
        // Efekt krêcenia siê ca³ej poœwiaty
        transform.Rotate(Vector3.forward * spinSpeed * Time.deltaTime);
    }

    void SpawnRays()
    {
        spawnedRays = new SpriteRenderer[rayCount];
        float angleStep = 360f / rayCount; // Dzielimy ko³o na równe kawa³ki

        for (int i = 0; i < rayCount; i++)
        {
            // 1. Tworzymy nowy promieñ jako dziecko tego obiektu
            GameObject newRay = Instantiate(rayPrefab, transform);

            // 2. Obracamy go o odpowiedni k¹t
            // Quaternion.Euler(0, 0, k¹t) to obrót w 2D
            float currentAngle = i * angleStep;
            newRay.transform.rotation = Quaternion.Euler(0, 0, currentAngle);

            // 3. Kolorowanie
            SpriteRenderer sr = newRay.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = glowColor; // Przypisujemy kolor
                spawnedRays[i] = sr;  // Zapamiêtujemy go
            }
        }
    }

    // Tê funkcjê wywo³amy póŸniej z innego skryptu, ¿eby zmieniæ kolor
    // np. na fioletowy dla epickich przedmiotów
    public void SetRarityColor(Color newColor)
    {
        glowColor = newColor;

        // Jeœli promienie ju¿ istniej¹, zaktualizuj ich kolor
        if (spawnedRays != null)
        {
            foreach (var sr in spawnedRays)
            {
                if (sr != null) sr.color = newColor;
            }
        }
    }
}