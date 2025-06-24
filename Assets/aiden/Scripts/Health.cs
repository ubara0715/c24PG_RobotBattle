using UnityEngine;
using UnityEngine.UI;

public class HealthAndArmor : MonoBehaviour
{
    public int maxHealth = 100;
    public int maxArmor = 50;

    private int currentHealth;
    private int currentArmor;

    public int damageOnCollision = 10;
    public int thrownArmorBonus = 15;

    [Header("Critical Hit Settings")]
    public float critChance = 0.2f; 
    public float critMultiplier = 2.0f;

    public Text healthText;
    public Text armorText;
    public Text criticalHitText;

    void Start()
    {
        currentHealth = maxHealth;
        currentArmor = maxArmor;
        UpdateUI();
    }

    void OnCollisionEnter(Collision collision)
    { 
        if (collision.gameObject.CompareTag("ThrownArmor"))
        {
            int totalDamage = damageOnCollision + thrownArmorBonus;
            Debug.Log("Hit by thrown armor! Total damage: " + totalDamage);
            TakeDamage(totalDamage);

            
            Destroy(collision.gameObject);
        }       
    }

    public void TakeDamage(int amount)
    {
        bool isCrit = Random.value < critChance;

        if (isCrit)
        {
            amount = Mathf.RoundToInt(amount * critMultiplier);
            Debug.Log("CRITICAL HIT! Damage: " + amount);
            ShowCriticalHitText();
        }
       
        if (currentArmor > 0)
        {
            int armorAbsorbed = Mathf.Min(currentArmor, amount);
            currentArmor -= armorAbsorbed;
            amount -= armorAbsorbed;
        }
   
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void ShowCriticalHitText()
    {
        if (criticalHitText == null) return;

        criticalHitText.gameObject.SetActive(true);
        CancelInvoke("HideCriticalHitText");
        Invoke("HideCriticalHitText", 1f); 
    }

    void HideCriticalHitText()
    {
        if (criticalHitText != null)
            criticalHitText.gameObject.SetActive(false);
    }


    void UpdateUI()
    {
        if (healthText != null)
            healthText.text = "Health: " + currentHealth;
        if (armorText != null)
            armorText.text = "Armor: " + currentArmor;
    }

    void Die()
    {
        Debug.Log(gameObject.name + " has died!");     
        Destroy(gameObject);
    }
}
