using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 0.15f; // Jak szybko kamera nadrabia zaleg³oœci (0 = natychmiast, 0.5 = bardzo miêkko)
    public Vector3 offset;

    private Vector3 velocity = Vector3.zero;

    // U¿ywamy LateUpdate zamiast Update
    // Update: Gracz siê porusza.
    // LateUpdate: Kamera rusza siê DOPIERO po tym, jak gracz skoñczy³ ruch.
    // To zapobiega drganiu kamery (jitter).
    void LateUpdate()
    {
        // Sprawdzamy, czy przypisaliœmy gracza, ¿eby unikn¹æ b³êdów
        if (target != null)
        {
            // Gdzie chcemy, ¿eby kamera by³a? (Pozycja gracza + przesuniêcie)
            Vector3 targetPosition = target.position + offset;

            // P³ynne przejœcie z obecnej pozycji do pozycji docelowej
            // SmoothDamp to funkcja idealna do kamer - dzia³a jak sprê¿yna
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
    }


}
