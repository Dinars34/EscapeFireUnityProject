using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    [Header("Key Settings")]
    [Tooltip("Type of key (e.g., 'RedKey', 'BlueKey', 'MasterKey')")]
    public string keyType = "RedKey";
    
    [Header("Interaction Settings")]
    public KeyCode interactionKey = KeyCode.E;
    
    [Header("UI Settings")]
    public GameObject interactionPrompt; // "Press E to pick up RedKey"
    
    [Header("Visual Settings (Optional)")]
    public float rotationSpeed = 50f; // Make key rotate in place
    public float bobSpeed = 1f; // Make key bob up and down
    public float bobHeight = 0.3f;
    
    [Header("Audio (Optional)")]
    public AudioClip pickupSound;
    
    private bool playerInRange = false;
    private PlayerInventory playerInventory;
    private Vector3 startPosition;
    private AudioSource audioSource;
    
    void Start()
    {
        // Hide interaction prompt at start
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
        
        // Store starting position for bobbing effect
        startPosition = transform.position;
        
        // Setup audio
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // 3D sound
    }
    
    void Update()
    {
        // Rotate key slowly for visual effect
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        
        // Bob up and down
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
        
        // Check for pickup input
        if (playerInRange && Input.GetKeyDown(interactionKey))
        {
            PickupKey();
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            playerInventory = other.GetComponent<PlayerInventory>();
            
            // Show interaction prompt
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(true);
            }
            
            Debug.Log("Press " + interactionKey + " to pick up " + keyType);
        }
    }
    
    void OnTriggerExit(Collider other)
    {
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
    
    void PickupKey()
    {
        if (playerInventory != null)
        {
            // Add key to player inventory
            playerInventory.AddKey(keyType);
            
            // Play pickup sound
            if (pickupSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(pickupSound);
            }
            
            // Hide interaction prompt
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(false);
            }
            
            Debug.Log("Picked up " + keyType + "!");
            
            // Destroy key object after short delay (to let sound play)
            Destroy(gameObject, pickupSound != null ? pickupSound.length : 0.1f);
        }
    }
}