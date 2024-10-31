using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    [Header("Movement")]
    [SerializeField] float moveSpeed = 6f;
    [SerializeField] float airMultiplier = 0.4f;
    private float movementMultiplier = 10f;

    [Header("Sprinting")]
    [SerializeField] float walkSpeed = 4f;
    [SerializeField] float sprintSpeed = 6f;
    [SerializeField] float acceleration = 10f;

    [Header("Sprinting Effects")]
    [SerializeField] Camera cam;
    [SerializeField] float sprintFOV = 100f;

    [Header("Jumping")]
    public float jumpForce = 50f;
    public float jumpRate = 15f;

    [Header("Keybinds")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode sprintKey = KeyCode.LeftControl;

    [Header("Drag")]
    [SerializeField] float groundDrag = 6f;
    [SerializeField] float airDrag = 2f;

    [Header("Ground Detection")]
    [SerializeField] LayerMask groundMask;
    public bool isGrounded { get; private set; }

    [HideInInspector] public bool isSprinting;
    [HideInInspector] public bool isMoving;

    [SerializeField] Transform orientation;

    private Vector3 moveDirection;
    private Vector2 movement;
    private Rigidbody rb;
    private float nextTimeToJump = 0f;

    private void Start() {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update() {
        isMoving = (moveDirection != Vector3.zero);

        HandleInput();
        ControlDrag();
        ControlSpeed();

        if (Input.GetKey(jumpKey) && isGrounded) {
            nextTimeToJump = Time.time + 1f / jumpRate;
            HandleJump();
        }

        if (isSprinting && isMoving) {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, sprintFOV, 8f * Time.deltaTime);
        } else {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 90f, 8f * Time.deltaTime);
        }
    }

    private void FixedUpdate() {
        MovePlayer();
    }

    void HandleInput() {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        moveDirection = orientation.forward * movement.y + orientation.right * movement.x;
    }

    void HandleJump() {
        if (isGrounded) {
            Debug.Log("Jump Triggered");
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void ControlSpeed() {
        float targetSpeed = Input.GetKey(sprintKey) && isMoving ? sprintSpeed : walkSpeed;
        isSprinting = Input.GetKey(sprintKey) && isMoving;
        moveSpeed = Mathf.Lerp(moveSpeed, targetSpeed, acceleration * Time.deltaTime);
    }

    void ControlDrag() {
        rb.drag = isGrounded ? groundDrag : airDrag;
    }

    void MovePlayer() {
        float multiplier = isGrounded ? movementMultiplier : airMultiplier * movementMultiplier;
        rb.AddForce(multiplier * moveSpeed * moveDirection.normalized, ForceMode.Acceleration);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Block")) {
            if (isSprinting) {
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 90f, 8f * Time.deltaTime);
                isSprinting = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint col in collision.contacts)
        {
            Debug.Log(col.point.y);
            Debug.Log(transform.position.y + 0.1);
            if (col.point.y <= transform.position.y + 0.1 && !isGrounded)
            {
                isGrounded = true;
            }
        }
    }
}
