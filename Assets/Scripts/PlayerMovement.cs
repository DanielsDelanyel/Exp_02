using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 17f;

    private Rigidbody2D rb;
    private float moveInput;
    private bool czyNaZiemi = false;
    private Animator anim;

    [Header("Walka")]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask Enemy;

    [Tooltip("Ile czasu mija zanim uderzenie zadaje obrażenia")]
    public float impactDelay = 0.4f;
    [Tooltip("Ile czasu musisz odczekać przed kolejnym atakiem")]
    public float attackCooldown = 0.8f;

    // --- KOSZTY STAMINY ---
    public float jumpStaminaCost = 20f;
    public float attackStaminaCost = 50f;
    // ----------------------

    private float nextAttackTime = 0f;
    private PlayerEquipment equipment;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        equipment = GetComponent<PlayerEquipment>();
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (moveInput > 0) transform.localScale = new Vector3(2.6f, 2.5f, 2.6f);
        else if (moveInput < 0) transform.localScale = new Vector3(-2.6f, 2.5f, 2.6f);

        anim.SetFloat("Speed", Mathf.Abs(moveInput));

        // --- ATAK Z KOSZTEM STAMINY ---
        if (Input.GetButtonDown("Fire1"))
        {
            if (Time.time >= nextAttackTime)
            {
                // Sprawdzamy czy mamy staminę (50)
                if (PlayerStats.instance.HasStamina(attackStaminaCost))
                {
                    StartCoroutine(AttackRoutine());
                    nextAttackTime = Time.time + attackCooldown;

                    // Zużywamy staminę
                    PlayerStats.instance.UseStamina(attackStaminaCost);
                }
                else
                {
                    Debug.Log("Za mało staminy na atak!");
                }
            }
        }

        // --- SKOK Z KOSZTEM STAMINY ---
        if (Input.GetButtonDown("Jump") && czyNaZiemi)
        {
            // Sprawdzamy czy mamy staminę (20)
            if (PlayerStats.instance.HasStamina(jumpStaminaCost))
            {
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

                // Zużywamy staminę
                PlayerStats.instance.UseStamina(jumpStaminaCost);
            }
            else
            {
                Debug.Log("Za mało staminy na skok!");
            }
        }
    }

    IEnumerator AttackRoutine()
    {
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(impactDelay);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, Enemy);

        foreach (Collider2D enemy in hitEnemies)
        {
            Grib_enemy_behaviour enemyScript = enemy.GetComponent<Grib_enemy_behaviour>();

            if (enemyScript != null)
            {
                int totalDamage = PlayerStats.instance.totalDamage;
                string weaponName = "Pięści";
                int weaponBonus = 0;

                if (equipment != null && equipment.currentWeapon != null)
                {
                    weaponName = equipment.currentWeapon.itemName;
                    weaponBonus = equipment.currentWeapon.damageBonus;
                }

                Debug.Log($"ATAK: {enemy.name} | Broń: {weaponName} (+{weaponBonus}) | SUMA DMG: {totalDamage}");
                enemyScript.TakeDamage(totalDamage, transform);
            }
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Podloga")) czyNaZiemi = true;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Podloga")) czyNaZiemi = false;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}