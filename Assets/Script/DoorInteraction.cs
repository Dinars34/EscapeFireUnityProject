using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    [Header("Door Settings")]
    [Tooltip("Is the door currently open?")]
    public bool isOpen = false;
    
    [Tooltip("Key to press to interact with door")]
    public KeyCode interactionKey = KeyCode.E;
    
    [Header("Door Animation")]
    [Tooltip("How far the door opens (degrees)")]
    public float openAngle = 90f;
    
    [Tooltip("Speed of door opening/closing")]
    public float doorSpeed = 2f;
    
    [Tooltip("Which axis to rotate (X, Y, or Z)")]
    public RotationAxis rotationAxis = RotationAxis.Y;
    
    [Header("UI Settings")]
    public GameObject interactionPrompt; // UI prompt: "Press E to open/close door"
    
    private bool playerInRange = false;
    private Vector3 closedRotation;
    private Vector3 openRotation;
    private bool isAnimating = false;
    
    public enum RotationAxis
    {
        X,
        Y,
        Z
    }
    
    void Start()
    {
        // Store initial rotation as closed position
        closedRotation = transform.eulerAngles;
        
        // Calculate open rotation based on selected axis
        openRotation = closedRotation;
        switch (rotationAxis)
        {
            case RotationAxis.X:
                openRotation.x += openAngle;
                break;
            case RotationAxis.Y:
                openRotation.y += openAngle;
                break;
            case RotationAxis.Z:
                openRotation.z += openAngle;
                break;
        }
        
        // Hide interaction prompt at start
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }
    
    void Update()
    {
        // Check for interaction input
        if (playerInRange && Input.GetKeyDown(interactionKey) && !isAnimating)
        {
            ToggleDoor();
        }
        
        // Animate door opening/closing
        if (isAnimating)
        {
            Vector3 targetRotation = isOpen ? openRotation : closedRotation;
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, targetRotation, doorSpeed * Time.deltaTime);
            
            // Check if animation is complete
            float angle = Vector3.Angle(transform.eulerAngles, targetRotation);
            if (angle < 0.5f)
            {
                transform.eulerAngles = targetRotation;
                isAnimating = false;
            }
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Check if player entered trigger zone
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            
            // Show interaction prompt
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(true);
            }
            
            string action = isOpen ? "close" : "open";
            Debug.Log("Press " + interactionKey + " to " + action + " door");
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        // Player left trigger zone
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            
            // Hide interaction prompt
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(false);
            }
        }
    }
    
    void ToggleDoor()
    {
        isOpen = !isOpen;
        isAnimating = true;
        
        string action = isOpen ? "Opening" : "Closing";
        Debug.Log(action + " door...");
    }
    
    // Public method to open door from other scripts
    public void OpenDoor()
    {
        if (!isOpen)
        {
            isOpen = true;
            isAnimating = true;
        }
    }
    
    // Public method to close door from other scripts
    public void CloseDoor()
    {
        if (isOpen)
        {
            isOpen = false;
            isAnimating = true;
        }
    }
}