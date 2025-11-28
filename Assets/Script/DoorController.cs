using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Door Settings")]
    [Tooltip("Does this door require a key?")]
    public bool requiresKey = false;
    
    [Tooltip("What type of key is needed (e.g., 'RedKey', 'BlueKey')")]
    public string requiredKeyType = "RedKey";
    
    [Tooltip("The hinge/pivot point of the door")]
    public Transform doorHinge;
    
    [Tooltip("Door opens inward (towards player) or outward")]
    public bool opensInward = true;
    
    [Tooltip("Angle the door opens to (90 degrees is standard)")]
    public float openAngle = 90f;
    
    [Tooltip("How fast the door opens/closes")]
    public float doorSpeed = 2f;
    
    [Header("Interaction Settings")]
    public KeyCode interactionKey = KeyCode.E;
    
    [Header("UI Settings")]
    public GameObject interactionPrompt; // "Press E to open"
    public GameObject lockedPrompt; // "Door is locked. Need RedKey"
    
    [Header("Audio (Optional)")]
    public AudioClip doorOpenSound;
    public AudioClip doorCloseSound;
    public AudioClip lockedSound;
    
    // Private variables
    private bool isOpen = false;
    private bool isMoving = false;
    private bool playerInRange = false;
    private PlayerInventory playerInventory;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private AudioSource audioSource;
    
    void Start()
    {
        // Setup audio source
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // 3D sound
        
        // If no hinge is assigned, use the door itself
        if (doorHinge == null)
        {
            doorHinge = transform;
        }
        
        // Store the closed rotation
        closedRotation = doorHinge.localRotation;
        
        // Calculate open rotation
        float angle = opensInward ? openAngle : -openAngle;
        openRotation = closedRotation * Quaternion.Euler(0f, angle, 0f);
        
        // Hide all prompts at start
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
        
        if (lockedPrompt != null)
            lockedPrompt.SetActive(false);
    }
    
    void Update()
    {
        // Handle door interaction
        if (playerInRange && Input.GetKeyDown(interactionKey) && !isMoving)
        {
            TryOpenDoor();
        }
        
        // Animate door movement
        if (isMoving)
        {
            AnimateDoor();
        }
    }
    
    void TryOpenDoor()
    {
        // Check if door is locked
        if (requiresKey && !isOpen)
        {
            if (playerInventory != null && playerInventory.HasKey(requiredKeyType))
            {
                // Player has the key, unlock and open
                Debug.Log("Door unlocked with " + requiredKeyType);
                OpenDoor();
            }
            else
            {
                // Door is locked
                Debug.Log("Door is locked! Need: " + requiredKeyType);
                PlaySound(lockedSound);
                ShowLockedMessage();
            }
        }
        else
        {
            // Door doesn't require key or is already open
            ToggleDoor();
        }
    }
    
    void ToggleDoor()
    {
        if (isOpen)
        {
            CloseDoor();
        }
        else
        {
            OpenDoor();
        }
    }
    
    void OpenDoor()
    {
        isOpen = true;
        isMoving = true;
        PlaySound(doorOpenSound);
        Debug.Log("Opening door...");
    }
    
    void CloseDoor()
    {
        isOpen = false;
        isMoving = true;
        PlaySound(doorCloseSound);
        Debug.Log("Closing door...");
    }
    
    void AnimateDoor()
    {
        Quaternion targetRotation = isOpen ? openRotation : closedRotation;
        
        doorHinge.localRotation = Quaternion.Slerp(
            doorHinge.localRotation,
            targetRotation,
            doorSpeed * Time.deltaTime
        );
        
        // Check if door reached target rotation
        if (Quaternion.Angle(doorHinge.localRotation, targetRotation) < 0.5f)
        {
            doorHinge.localRotation = targetRotation;
            isMoving = false;
            Debug.Log("Door " + (isOpen ? "opened" : "closed"));
        }
    }
    
    void ShowLockedMessage()
    {
        if (lockedPrompt != null)
        {
            // Show locked message temporarily
            lockedPrompt.SetActive(true);
            Invoke("HideLockedMessage", 2f);
        }
    }
    
    void HideLockedMessage()
    {
        if (lockedPrompt != null)
        {
            lockedPrompt.SetActive(false);
        }
    }
    
    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            playerInventory = other.GetComponent<PlayerInventory>();
            
            // Show appropriate prompt
            if (requiresKey && !isOpen)
            {
                if (interactionPrompt != null)
                {
                    interactionPrompt.SetActive(true);
                }
            }
            else
            {
                if (interactionPrompt != null)
                {
                    interactionPrompt.SetActive(true);
                }
            }
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            playerInventory = null;
            
            // Hide prompts
            if (interactionPrompt != null)
                interactionPrompt.SetActive(false);
            
            if (lockedPrompt != null)
                lockedPrompt.SetActive(false);
        }
    }
    
    // Public method to unlock door from external scripts
    public void UnlockDoor()
    {
        requiresKey = false;
        Debug.Log("Door unlocked!");
    }
}