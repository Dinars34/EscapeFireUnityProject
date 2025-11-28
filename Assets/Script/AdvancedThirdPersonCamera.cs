using UnityEngine;

public class AdvancedThirdPersonCamera : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("The player transform to follow")]
    public Transform target;
    
    [Tooltip("Offset from player position (height adjustment)")]
    public Vector3 targetOffset = new Vector3(0f, 1.5f, 0f);
    
    [Header("Camera Distance")]
    [Tooltip("Normal distance from player")]
    public float normalDistance = 5f;
    
    [Tooltip("Minimum distance (when zoomed in or colliding)")]
    public float minDistance = 1f;
    
    [Tooltip("Maximum distance")]
    public float maxDistance = 8f;
    
    [Header("Camera Rotation")]
    [Tooltip("Mouse sensitivity for horizontal rotation")]
    public float mouseSensitivityX = 3f;
    
    [Tooltip("Mouse sensitivity for vertical rotation")]
    public float mouseSensitivityY = 3f;
    
    [Tooltip("Minimum vertical angle (looking down)")]
    public float minVerticalAngle = -30f;
    
    [Tooltip("Maximum vertical angle (looking up)")]
    public float maxVerticalAngle = 70f;
    
    [Header("Camera Smoothing")]
    [Tooltip("How smooth the camera follows (lower = smoother)")]
    public float positionSmoothing = 10f;
    
    [Tooltip("How smooth the camera rotates")]
    public float rotationSmoothing = 10f;
    
    [Header("Collision Settings")]
    [Tooltip("Check for walls/obstacles between camera and player")]
    public bool checkCollision = true;
    
    [Tooltip("Layers that block the camera (e.g., walls)")]
    public LayerMask collisionLayers = -1;
    
    [Tooltip("Radius of collision sphere")]
    public float collisionRadius = 0.3f;
    
    [Tooltip("How close to wall before adjusting")]
    public float collisionBuffer = 0.2f;
    
    [Header("Input Settings")]
    [Tooltip("Invert vertical mouse axis")]
    public bool invertY = false;
    
    [Tooltip("Lock/unlock cursor at start")]
    public bool lockCursor = true;
    
    // Private variables
    private float currentDistance;
    private float currentX = 0f;
    private float currentY = 20f;
    private Vector3 currentVelocity;
    
    void Start()
    {
        // Find player if not assigned
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
            else
            {
                Debug.LogError("AdvancedThirdPersonCamera: No target assigned and no Player found!");
                enabled = false;
                return;
            }
        }
        
        // Initialize distance
        currentDistance = normalDistance;
        
        // Lock cursor
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        // Set initial rotation based on camera's starting position
        Vector3 angles = transform.eulerAngles;
        currentX = angles.y;
        currentY = angles.x;
    }
    
    void LateUpdate()
    {
        if (target == null)
            return;
        
        // Handle cursor lock toggle (press ESC to unlock)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
        
        // Get mouse input only if cursor is locked
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivityX;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivityY;
            
            // Apply mouse input
            currentX += mouseX;
            currentY -= invertY ? -mouseY : mouseY;
            
            // Clamp vertical rotation
            currentY = Mathf.Clamp(currentY, minVerticalAngle, maxVerticalAngle);
        }
        
        // Handle mouse scroll for zoom
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            normalDistance -= scroll * 2f;
            normalDistance = Mathf.Clamp(normalDistance, minDistance, maxDistance);
        }
        
        // Calculate desired position
        Vector3 targetPosition = target.position + targetOffset;
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0f);
        Vector3 direction = rotation * Vector3.back;
        
        // Calculate camera position
        Vector3 desiredPosition = targetPosition + direction * normalDistance;
        
        // Check for collisions
        float finalDistance = normalDistance;
        
        if (checkCollision)
        {
            RaycastHit hit;
            Vector3 rayStart = targetPosition;
            Vector3 rayDirection = direction;
            float rayDistance = normalDistance + collisionBuffer;
            
            // Perform sphere cast for better collision detection
            if (Physics.SphereCast(rayStart, collisionRadius, rayDirection, out hit, rayDistance, collisionLayers))
            {
                // Hit something, move camera closer
                finalDistance = Mathf.Max(hit.distance - collisionBuffer, minDistance);
                desiredPosition = targetPosition + direction * finalDistance;
            }
        }
        
        // Smooth camera movement
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref currentVelocity,
            1f / positionSmoothing
        );
        
        // Smooth camera rotation
        Quaternion desiredRotation = Quaternion.LookRotation(targetPosition - transform.position);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            desiredRotation,
            rotationSmoothing * Time.deltaTime
        );
        
        // Update current distance for next frame
        currentDistance = finalDistance;
    }
    
    // Optional: Method to set target from other scripts
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
    
    // Optional: Reset camera position
    public void ResetCamera()
    {
        if (target != null)
        {
            Vector3 targetPosition = target.position + targetOffset;
            Vector3 direction = Quaternion.Euler(currentY, currentX, 0f) * Vector3.back;
            transform.position = targetPosition + direction * normalDistance;
        }
    }
}