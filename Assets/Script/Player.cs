using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player Stats")]
    public int MaxHealth = 100;
    public float MaxTime = 180f; // 3 minutes = 180 seconds
    public int currentHealth;
    public float currentTime;
    
    [Header("UI References")]
    public HealthBar healthBar;
    public TimeBar timeBar;
    
    [Header("Game Over")]
    public GameManager gameManager;
    
    private bool isDead = false;

    void Start()
    {
        // Initialize health and time
        currentHealth = MaxHealth;
        currentTime = MaxTime;

        // Set up UI bars
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(MaxHealth);
        }
        
        if (timeBar != null)
        {
            timeBar.SetMaxTime((int)MaxTime);
        }
        
        // Find GameManager if not assigned
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }
        
        // Make sure PlayerInventory component exists
        if (GetComponent<PlayerInventory>() == null)
        {
            gameObject.AddComponent<PlayerInventory>();
            Debug.Log("PlayerInventory component added automatically");
        }
    }

    void Update()
    {
        // Don't update if player is dead
        if (isDead)
            return;
        
        // Countdown timer
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime; // Decrease by real time
            
            if (timeBar != null)
            {
                timeBar.SetTime((int)currentTime);
            }
            
            // Check if time runs out
            if (currentTime <= 0)
            {
                currentTime = 0;
                Die("Time's Up!");
            }
        }
        
        // TEST: Press T to take damage (for testing only)
        if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(20);
        }
        
        // TEST: Press H to heal (for testing only)
        if (Input.GetKeyDown(KeyCode.H))
        {
            Heal(20);
        }
    }
    
    public void TakeDamage(int damage)
    {
        if (isDead)
            return;
        
        currentHealth -= damage;
        
        // Clamp health to 0 minimum
        if (currentHealth < 0)
            currentHealth = 0;
        
        // Update health bar
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }
        
        // Check if player died
        if (currentHealth <= 0)
        {
            Die("You Died!");
        }
        
        Debug.Log("Player took " + damage + " damage. Current health: " + currentHealth);
    }
    
    public void Heal(int healAmount)
    {
        if (isDead)
            return;
        
        currentHealth += healAmount;
        
        // Don't exceed max health
        if (currentHealth > MaxHealth)
            currentHealth = MaxHealth;
        
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }
        
        Debug.Log("Player healed " + healAmount + ". Current health: " + currentHealth);
    }
    
    public void AddTime(float timeAmount)
    {
        if (isDead)
            return;
        
        currentTime += timeAmount;
        
        // Don't exceed max time
        if (currentTime > MaxTime)
            currentTime = MaxTime;
        
        if (timeBar != null)
        {
            timeBar.SetTime((int)currentTime);
        }
        
        Debug.Log("Added " + timeAmount + " seconds. Current time: " + currentTime);
    }
    
    void Die(string reason)
    {
        if (isDead)
            return;
        
        isDead = true;
        
        Debug.Log("Player died: " + reason);
        
        // Disable player controller
        PlayerController controller = GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.enabled = false;
        }
        
        // Trigger game over
        if (gameManager != null)
        {
            gameManager.GameOver(reason);
        }
    }
    
    public bool IsDead()
    {
        return isDead;
    }
}