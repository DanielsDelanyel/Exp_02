using UnityEngine;
using System.Collections; // <-- To jest potrzebne do odliczania czasu!

public class ChestLightEffect : MonoBehaviour
{
    [Header("Ustawienia Promieni")]
    public GameObject rayPrefab;    // Twój nowy prefab (Rodzic + Dziecko)
    public int rayCount = 5;        // Ile promieni?
    public float angleSpread = 60f; // K¹t wachlarza (np. 60 stopni)
    public float widthScale = 0.5f; // Jak chude maj¹ byæ promienie?

    [Header("Animacja")]
    public float pulseSpeed = 2f;   // Szybkoœæ pulsowania
    public float minAlpha = 0.3f;   // Minimalna przezroczystoœæ
    public float maxAlpha = 0.8f;   // Maksymalna przezroczystoœæ

    [Header("Czas trwania")]
    public float duration = 5f; // Po ilu sekundach ma znikn¹æ?

    private SpriteRenderer[] raysRenderers; // Lista do pamiêtania promieni

    // Tê funkcjê wywo³asz ze skrzyni
    public void ActivateEffect(Color rarityColor)
    {
        // 1. Czyœcimy stare (jakby coœ zosta³o)
        foreach (Transform child in transform) Destroy(child.gameObject);

        StopAllCoroutines();

        raysRenderers = new SpriteRenderer[rayCount];

        // 2. Obliczamy k¹ty
        // Zaczynamy od lewej (-po³owa k¹ta) do prawej (+po³owa k¹ta)
        float startAngle = -angleSpread / 2f;
        float angleStep = angleSpread / (rayCount - 1);

        for (int i = 0; i < rayCount; i++)
        {
            // Tworzymy promieñ
            GameObject rayObj = Instantiate(rayPrefab, transform);

            rayObj.transform.localPosition = Vector3.zero;

            // Obliczamy k¹t dla tego konkretnego promienia
            float currentAngle = startAngle + (angleStep * i);

            // Obracamy (Oœ Z to obrót w 2D)
            rayObj.transform.localRotation = Quaternion.Euler(0, 0, -currentAngle);

            // Pobieramy SpriteRenderer z DZIECKA (bo prefab to Pusty Rodzic -> Obrazek)
            SpriteRenderer sr = rayObj.GetComponentInChildren<SpriteRenderer>();

            if (sr != null)
            {
                // Ustawiamy kolor rzadkoœci
                sr.color = rarityColor;

                // Ustawiamy szerokoœæ (¿eby nie by³y grubymi parówkami)
                sr.transform.localScale = new Vector3(widthScale, Random.Range(1f, 1.5f), 1f); // Losowa d³ugoœæ

                raysRenderers[i] = sr;
            }

        }
        StartCoroutine(DisableAfterTime());
    }

    void Update()
    {
        // Efekt "oddychania" œwiat³a (zmienia przezroczystoœæ)
        if (raysRenderers != null && raysRenderers.Length > 0)
        {
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);

            foreach (var sr in raysRenderers)
            {
                if (sr != null)
                {
                    Color c = sr.color;
                    c.a = alpha; // Zmieniamy tylko kana³ Alpha
                    sr.color = c;
                }
            }
        }
    }
    IEnumerator DisableAfterTime()
    {
        // Czekamy tyle sekund, ile wpisa³eœ w Inspektorze
        yield return new WaitForSeconds(duration);

        // Niszczymy wszystkie promienie
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Czyœcimy listê, ¿eby Update przesta³ dzia³aæ
        raysRenderers = null;
    }
}