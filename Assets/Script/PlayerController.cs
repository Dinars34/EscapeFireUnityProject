using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 9f;
    public float crouchSpeed = 2.5f;
    public float rotationSpeed = 10f;
    
    [Header("Jump Settings")]
    public float jumpHeight = 1.2f;
    public float gravity = -9.81f;
    
    [Header("Roll Settings")]
    public float rollDistance = 5f;
    public float rollDuration = 0.6f;
    
    [Header("References")]
    public Transform cameraTransform;
    
    // Private variables
    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isCrouching;
    private bool isRolling;
    private bool isJumping;
    private float rollTimer;
    private Vector3 rollDirection;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        
        // If camera not assigned, find main camera
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
        
        Debug.Log("PlayerController initialized successfully!");
    }
    
    void Update()
    {
        // Check if grounded
        isGrounded = controller.isGrounded;
        
        if (animator != null)
        {
            animator.SetBool("IsGrounded", isGrounded);
        }
        
        // Reset jumping state when grounded
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small downward force to keep grounded
            isJumping = false;
        }
        
        // Handle rolling - blocks all other actions
        if (isRolling)
        {
            HandleRoll();
            return; // Skip other controls during roll
        }
        
        // Get input using keyboard keys directly
        float horizontal = 0f;
        float vertical = 0f;
        
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            vertical = 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            vertical = -1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            horizontal = 1f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            horizontal = -1f;
        
        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;
        
        // Handle crouch toggle
        if (Input.GetKeyDown(KeyCode.C))
        {
            isCrouching = !isCrouching;
            
            if (animator != null)
            {
                animator.SetBool("IsCrouching", isCrouching);
            }
            
            // Adjust character controller height
            if (isCrouching)
            {
                controller.height = 1f; // Crouch height
                controller.center = new Vector3(0, 0.5f, 0);
            }
            else
            {
                controller.height = 1.8f; // Standing height
                controller.center = new Vector3(0, 0.9f, 0);
            }
        }
        
        // Handle roll (Left Control key)
        if (Input.GetKeyDown(KeyCode.LeftControl) && isGrounded && !isCrouching && !isJumping)
        {
            StartRoll(inputDirection);
            return;
        }
        
        // Handle movement
        if (inputDirection.magnitude >= 0.1f)
        {
            // Calculate movement direction relative to camera
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            
            // Rotate player to face movement direction
            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            
            // Determine speed based on input
            float currentSpeed;
            float animSpeed;
            
            if (isCrouching)
            {
                currentSpeed = crouchSpeed;
                animSpeed = 2.5f;
            }
            else if (Input.GetKey(KeyCode.LeftShift)) // Left Shift to run
            {
                currentSpeed = runSpeed;
                animSpeed = 9f;
                Debug.Log("RUNNING! Speed: " + animSpeed);
            }
            else
            {
                currentSpeed = walkSpeed;
                animSpeed = 5f;
            }
            
            // Move character
            controller.Move(moveDirection * currentSpeed * Time.deltaTime);
            
            // Update animator speed
            if (animator != null)
            {
                animator.SetFloat("Speed", animSpeed);
            }
        }
        else
        {
            // No movement input
            if (animator != null)
            {
                animator.SetFloat("Speed", 0f);
            }
        }
        
        // Handle jump (Spacebar)
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isCrouching && !isJumping)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            isJumping = true;
            
            if (animator != null)
            {
                animator.SetTrigger("Jump");
            }
        }
        
        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    
    void StartRoll(Vector3 inputDirection)
    {
        isRolling = true;
        rollTimer = 0f;
        
        // Determine roll direction
        if (inputDirection.magnitude >= 0.1f)
        {
            // Roll in movement direction
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            rollDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            
            // Face roll direction
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
        }
        else
        {
            // Roll forward if no input
            rollDirection = transform.forward;
        }
        
        if (animator != null)
        {
            animator.SetTrigger("Roll");
        }
    }
    
    void HandleRoll()
    {
        rollTimer += Time.deltaTime;
        
        if (rollTimer < rollDuration)
        {
            // Move character during roll
            float rollSpeed = rollDistance / rollDuration;
            controller.Move(rollDirection * rollSpeed * Time.deltaTime);
            
            // Apply gravity during roll
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }
        else
        {
            isRolling = false;
        }
    }
}