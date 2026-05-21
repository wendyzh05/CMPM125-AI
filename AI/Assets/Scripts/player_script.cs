using UnityEngine;
using UnityEngine.InputSystem;

public class WalkthroughCamera : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float lookSpeed = 0.08f;
    public Transform cameraTransform;

    private CharacterController controller;
    private float verticalLookRotation = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (cameraTransform == null)
        {
            cameraTransform = GetComponentInChildren<Camera>().transform;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Vector2 moveInput = Vector2.zero;

        if (Keyboard.current.wKey.isPressed) moveInput.y += 1;
        if (Keyboard.current.sKey.isPressed) moveInput.y -= 1;
        if (Keyboard.current.dKey.isPressed) moveInput.x += 1;
        if (Keyboard.current.aKey.isPressed) moveInput.x -= 1;

        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * moveSpeed * Time.deltaTime);

        Vector2 lookInput = Mouse.current.delta.ReadValue();

        transform.Rotate(Vector3.up * lookInput.x * lookSpeed);

        verticalLookRotation -= lookInput.y * lookSpeed;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -75f, 75f);

        cameraTransform.localEulerAngles = new Vector3(verticalLookRotation, 0f, 0f);

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
