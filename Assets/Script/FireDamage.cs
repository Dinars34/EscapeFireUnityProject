using UnityEngine;

public class FireDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damageAmount = 10;
    public float damageInterval = 1f; // Damage every 1 second
    
    private bool playerInFire = false;
    private float damageTimer = 0f;
    private Player player;
    
    void Update()
    {
        // If player is in fire zone
        if (playerInFire && player != null)
        {
            damageTimer += Time.deltaTime;
            
            // Deal damage at intervals
            if (damageTimer >= damageInterval)
            {
                player.TakeDamage(damageAmount);
                damageTimer = 0f;
                
                Debug.Log("Fire is burning player!");
            }
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Check if player entered fire
        if (other.CompareTag("Player"))
        {
            player = other.GetComponent<Player>();
            
            if (player != null)
            {
                playerInFire = true;
                damageTimer = 0f;
                Debug.Log("Player entered fire zone!");
            }
        }
    }
    
    void OnTriggerStay(Collider other)
    {
        // Make sure we keep tracking player
        if (other.CompareTag("Player") && player == null)
        {
            player = other.GetComponent<Player>();
            playerInFire = true;
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        // Player left fire
        if (other.CompareTag("Player"))
        {
            playerInFire = false;
            player = null;
            damageTimer = 0f;
            Debug.Log("Player left fire zone!");
        }
    }
}