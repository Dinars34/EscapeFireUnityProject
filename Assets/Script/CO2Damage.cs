using UnityEngine;

public class CO2Damage : MonoBehaviour
{
    [Header("CO2 Damage Settings")]
    [Tooltip("Damage per second from CO2")]
    public int damagePerSecond = 5;
    
    [Tooltip("Time before CO2 starts damaging (grace period in seconds)")]
    public float gracePeriod = 5f;
    
    [Header("Towel Protection")]
    [Tooltip("Damage reduction when player has towel (0-1, where 0.5 = 50% reduction)")]
    public float towelProtection = 0.7f; // 70% damage reduction
    
    private Player player;
    private float damageTimer = 0f;
    private float graceTimer = 0f;
    private bool graceEnded = false;

    void Start()
    {
        // Find the player in the scene
        player = FindObjectOfType<Player>();
        
        if (player == null)
        {
            Debug.LogError("CO2Damage: Player not found in scene!");
        }
    }

    void Update()
    {
        if (player == null || player.IsDead())
            return;

        // Handle grace period
        if (!graceEnded)
        {
            graceTimer += Time.deltaTime;
            
            if (graceTimer >= gracePeriod)
            {
                graceEnded = true;
                Debug.Log("CO2 grace period ended - CO2 damage is now active!");
            }
            return; // Don't deal damage during grace period
        }

        // Deal CO2 damage over time
        damageTimer += Time.deltaTime;
        
        if (damageTimer >= 1f) // Every 1 second
        {
            int actualDamage = damagePerSecond;
            
            // Check if player has towel equipped
            PlayerInventory inventory = player.GetComponent<PlayerInventory>();
            if (inventory != null && inventory.HasTowel())
            {
                // Reduce damage if towel is equipped
                actualDamage = Mathf.RoundToInt(damagePerSecond * (1f - towelProtection));
                Debug.Log("Towel protecting! CO2 damage reduced to: " + actualDamage);
            }
            else
            {
                Debug.Log("No towel protection! Taking full CO2 damage: " + actualDamage);
            }
            
            player.TakeDamage(actualDamage);
            damageTimer = 0f;
        }
    }
    
    // Optional: Method to reset grace period (useful for testing)
    public void ResetGracePeriod()
    {
        graceTimer = 0f;
        graceEnded = false;
        Debug.Log("CO2 grace period reset!");
    }
}