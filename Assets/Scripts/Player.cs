using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] private int enemyDamage;
    [SerializeField] private float damageTimer;
    [SerializeField] private StatManager statManager;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameObject deathMessage;

    private int enemiesInRange;
    private float damageTimerCounter;
    private Color originalColor;

    private void Awake()
    {
        enemiesInRange = 0;
        damageTimerCounter = 0f;
        originalColor = spriteRenderer.color;
    }

    private void FixedUpdate()
    {
        if (enemiesInRange > 0)
        {
            damageTimerCounter += Time.deltaTime;
            if (damageTimerCounter >= damageTimer)
            {
                TakeDamage();
                damageTimerCounter = 0f;
            }
        }
        else
        {
            damageTimerCounter = 0f;
        }
        Movement();
    }

    private void Movement()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector2 movement = new Vector2(horizontalInput, verticalInput).normalized * moveSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + movement);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange++;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange--;
            if (enemiesInRange <= 0)
            {
                damageTimerCounter = damageTimer;
            }
        }
    }

    private void TakeDamage()
    {
        Debug.Log("took damage");
        spriteRenderer.color = Color.red;
        Invoke("ResetColor", 0.1f);
        statManager.ModifyStat("Health", -enemyDamage);
        if (statManager.GetStat("Health") <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        deathMessage.SetActive(true);
        Reset();
    }

    private void Reset()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    private void ResetColor()
    {
        spriteRenderer.color = originalColor;
    }
}
