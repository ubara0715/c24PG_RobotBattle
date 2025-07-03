using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 50;
    public int maxArmor = 20;

    private int currentHealth;
    private int currentArmor;

    public int damageFromPlayerCollision = 10;

    public Text healthText;
    public Text armorText;

    public float critChance = 0.2f; 
    public float critMultiplier = 1.5f;
    public GameObject criticalHitText;

    void Start()
    {
        currentHealth = maxHealth;
        currentArmor = maxArmor;
        UpdateUI();

        if (criticalHitText != null)
            criticalHitText.SetActive(false);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            TakeDamage(damageFromPlayerCollision, 0);
            Debug.Log("damagetaken");
        }
    }

    public void TakeDamage(int baseDamage, int bonusDamage = 0)
    {
        int totalDamage = baseDamage + bonusDamage;

        if (Random.value < critChance)
        {
            totalDamage = Mathf.RoundToInt(totalDamage * critMultiplier);
            Debug.Log("Critical hit on enemy!");

            if (criticalHitText != null)
            {
                criticalHitText.SetActive(true);
                CancelInvoke("HideCritText");
                Invoke("HideCritText", 1.0f);
            }
        }

        if (currentArmor > 0)
        {
            int absorbed = Mathf.Min(currentArmor, totalDamage);
            currentArmor -= absorbed;
            totalDamage -= absorbed;
        }

        currentHealth -= totalDamage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void HideCritText()
    {
        if (criticalHitText != null)
            criticalHitText.SetActive(false);
    }

    void UpdateUI()
    {
        if (healthText != null)
            healthText.text = "Enemy Health: " + currentHealth;

        if (armorText != null)
            armorText.text = "Enemy Armor: " + currentArmor;
    }

    void Die()
    {
        Debug.Log(gameObject.name + " died!");
        Destroy(gameObject);
    }
}
