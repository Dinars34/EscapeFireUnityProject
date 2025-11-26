using UnityEngine;

public class TowelPickup : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Tooltip("Key to press to pick up towel")]
    public KeyCode interactionKey = KeyCode.E;
    
    [Header("UI Settings")]
    public GameObject interactionPrompt; // UI prompt: "Press E to pick up towel"
    
    private bool playerInRange = false;
    private PlayerInventory playerInventory;
    
    void Start()
    {
        // Hide interaction prompt at start
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }
    
    void Update()
    {
        // Check for interaction input
        if (playerInRange && Input.GetKeyDown(interactionKey))
        {
            PickupTowel();
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Check if player entered trigger zone
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            playerInventory = other.GetComponent<PlayerInventory>();
            
            // Show interaction prompt
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(true);
            }
            
            Debug.Log("Press " + interactionKey + " to pick up towel");
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        // Player left trigger zone
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            playerInventory = null;
            
            // Hide interaction prompt
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(false);
            }
        }
    }
    
    void PickupTowel()
    {
        if (playerInventory != null)
        {
            // Add towel to player inventory
            playerInventory.PickupTowel();
            
            // Hide interaction prompt
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(false);
            }
            
            // Destroy or hide the towel object
            gameObject.SetActive(false);
            // Or use: Destroy(gameObject);
            
            Debug.Log("Towel picked up!");
        }
    }
}