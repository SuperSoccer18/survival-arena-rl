using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 5f;

    [Header("Damage Protection")]
    [SerializeField] private float invulnerabilityDuration = 0.75f;

    [Header("UI")]
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private SpriteRenderer playerSprite;

    private float currentHealth;
    private bool isInvulnerable;
    private bool isDead;

    private void Awake()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(float damage)
    {
        if (isDead || isInvulnerable)
        {
            return;
        }

        currentHealth = Mathf.Max(0f, currentHealth - damage);
        UpdateHealthUI();

        if (currentHealth <= 0f)
        {
            Die();
            return;
        }

        StartCoroutine(InvulnerabilityRoutine());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Enemy"))
        {
            return;
        }

        Enemy enemy = collision.gameObject.GetComponent<Enemy>();

        float damage = enemy != null
            ? enemy.ContactDamage
            : 1f;

        TakeDamage(damage);
    }

    private IEnumerator InvulnerabilityRoutine()
    {
        isInvulnerable = true;

        yield return new WaitForSeconds(invulnerabilityDuration);

        isInvulnerable = false;
    }

    private void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text =
                $"Health: {currentHealth:0}/{maxHealth:0}";
        }
    }

    private void Die()
    {
        isDead = true;

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}