using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed        = 5f;
    public float jumpHeight       = 1.4f;
    public float gravity          = -20f;
    public float sprintMultiplier = 1.6f;

    [Header("Look")]
    public float     mouseSensitivity = 2f;
    public Transform cameraHolder;

    CharacterController controller;
    Vector3 velocity;
    float   xRotation;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;

        var stats = GetComponent<CharacterStats>();
        if (stats?.classData != null)
        {
            moveSpeed  = stats.classData.moveSpeed;
            jumpHeight = stats.classData.jumpHeight;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) ToggleCursor();
        HandleLook();
        HandleMovement();
    }

    void HandleLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        xRotation = Mathf.Clamp(xRotation - mouseY, -80f, 80f);
        if (cameraHolder != null)
            cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleMovement()
    {
        bool grounded = controller.isGrounded;
        if (grounded && velocity.y < 0) velocity.y = -2f;

        float speed = moveSpeed * (Input.GetKey(KeyCode.LeftShift) ? sprintMultiplier : 1f);
        Vector3 move = transform.right   * Input.GetAxis("Horizontal")
                     + transform.forward * Input.GetAxis("Vertical");
        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && grounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void ToggleCursor()
    {
        bool locked = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = locked ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible   = locked;
    }
}
