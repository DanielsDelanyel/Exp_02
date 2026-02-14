using UnityEngine;
using System.Collections;

public class Grib_enemy_behaviour : MonoBehaviour
{
    [Header("Statystyki")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Atak")]
    public int damageToGive = 15; // Ile zabiera ¿ycia graczowi


    [Header("Ruch i Wykrywanie Œcian")]
    public float speed = 2f;
    public Transform wallCheck;
    public float wallCheckDist = 0.5f;
    public LayerMask obstacleLayer;

    // --- NOWOŒÆ: Wykrywanie Krawêdzi ---
    [Header("Wykrywanie Krawêdzi")]
    public Transform edgeCheck; // Pusty obiekt przed stopami wroga
    public float edgeCheckDist = 1f; // Jak g³êboko sprawdzaæ?
    // -----------------------------------

    [Header("Reakcja na cios")]
    public float knockbackForce = 5f;
    public float stunTime = 0.5f;

    [Header("Œmieræ")]
    public float deathJumpForce = 8f;

    [Header("Floating Text (Popupy)")]
    public GameObject damagePopupPrefab;
    public Color damageColor = Color.yellow;
    public Vector3 popupStartSize = new Vector3(1f, 1f, 1f);
    public Vector3 popupEndSize = new Vector3(2f, 2f, 2f);

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Collider2D col;

    private bool isHurt = false;
    private bool isDead = false;
    private bool facingRight = true;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        if (rb != null) rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void FixedUpdate()
    {
        if (isDead) return;
        if (isHurt) return;

        float kierunek = facingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(kierunek * speed, rb.linearVelocity.y);

        CheckSurroundings();
    }

    // Zmieni³em nazwê z CheckForWalls na CheckSurroundings, bo sprawdza te¿ pod³ogê
    void CheckSurroundings()
    {
        // 1. SPRAWDZANIE ŒCIANY (Stare)
        if (wallCheck != null)
        {
            Vector2 direction = facingRight ? Vector2.right : Vector2.left;
            RaycastHit2D wallHit = Physics2D.Raycast(wallCheck.position, direction, wallCheckDist, obstacleLayer);

            if (wallHit.collider != null)
            {
                Flip();
                return; // Jeœli trafi³ w œcianê, od razu obracamy i koñczymy funkcjê
            }
        }

        // 2. SPRAWDZANIE KRAWÊDZI (Nowe)
        if (edgeCheck != null)
        {
            // Strzelamy promieniem W DÓ£
            RaycastHit2D groundInfo = Physics2D.Raycast(edgeCheck.position, Vector2.down, edgeCheckDist, obstacleLayer);

            // Jeœli promieñ NIE trafi³ w nic (collider == false), to znaczy ¿e jest dziura!
            if (groundInfo.collider == false)
            {
                Flip();
            }
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    public void TakeDamage(int damage, Transform attacker)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (damagePopupPrefab != null)
        {
            GameObject popup = Instantiate(damagePopupPrefab, transform.position, Quaternion.identity);
            DamagePopup popupScript = popup.GetComponent<DamagePopup>();
            if (popupScript != null)
            {
                popupScript.Setup(damage, damageColor, popupStartSize, popupEndSize);
            }
        }

        if (attacker.position.x > transform.position.x && !facingRight) Flip();
        else if (attacker.position.x < transform.position.x && facingRight) Flip();

        float knockbackDir = Mathf.Sign(transform.position.x - attacker.position.x);

        if (currentHealth <= 0) Die(knockbackDir);
        else StartCoroutine(DamageRoutine(knockbackDir));
    }

    IEnumerator DamageRoutine(float direction)
    {
        isHurt = true;
        sr.color = Color.red;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(new Vector2(direction * knockbackForce, knockbackForce / 1.5f), ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.1f);
        sr.color = Color.white;
        yield return new WaitForSeconds(stunTime);
        isHurt = false;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Sprawdzamy czy uderzyliœmy w Gracza
        if (collision.gameObject.CompareTag("Player"))
        {
            // Pobieramy statystyki gracza
            PlayerStats playerStats = collision.gameObject.GetComponent<PlayerStats>();

            if (playerStats != null)
            {
                // Zadajemy obra¿enia
                playerStats.TakeDamage(damageToGive);

                // Opcjonalnie: Odrzuæ gracza (Knockback)
                Rigidbody2D playerRB = collision.gameObject.GetComponent<Rigidbody2D>();
                if (playerRB != null)
                {
                    // Kierunek od grzyba do gracza
                    Vector2 direction = (collision.transform.position - transform.position).normalized;
                    playerRB.AddForce(direction * 5f, ForceMode2D.Impulse); // 5f to si³a odrzutu
                }
            }
        }
    }
    void Die(float knockbackDir)
    {
        isDead = true;
        isHurt = true;
        sr.color = Color.gray;
        col.enabled = false;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(new Vector2(knockbackDir * 2f, deathJumpForce), ForceMode2D.Impulse);
        rb.gravityScale = 3f;
        Destroy(gameObject, 3f);
    }

    // --- RYSOWANIE LINII POMOCNICZYCH (GIZMOS) ---
    private void OnDrawGizmos()
    {
        // Rysujemy liniê œciany (¯ó³ta)
        if (wallCheck != null)
        {
            Gizmos.color = Color.yellow;
            Vector2 direction = facingRight ? Vector2.right : Vector2.left;
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + (Vector3)(direction * wallCheckDist));
        }

        // Rysujemy liniê krawêdzi (Czerwona)
        if (edgeCheck != null)
        {
            Gizmos.color = Color.red;
            // Rysujemy liniê w dó³
            Gizmos.DrawLine(edgeCheck.position, edgeCheck.position + (Vector3.down * edgeCheckDist));
        }
    }
}