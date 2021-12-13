using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
/*
 * Taken from unit tutorials T3 & T4
 * I have adapted it to work with the new unity input system
 */
public class MouseLook : MonoBehaviour {
    [Header("Sensitivity Variables")]
    [Tooltip("The overall sensitivity of the mouse.")]
    public float mouseSensitivity = 3.0f;
    [Tooltip("The horizontal sensitivity of the mouse")]
    public float horizontalSensitivity = 3.0f;
    [Tooltip("The vertical sensitivity of the mouse")]
    public float verticalSensitivity = 3.0f;
    
    [Header("Bounding Variables")]
    [Tooltip("The minimum rotation to keep.")]
    public float minimumVert = -60.0f;
    [Tooltip("The maximum rotation to keep.")]
    public float maximumVert = 45.0f;

    private float mouseLocked;
    private float rotationX;

    void Start() {
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
    }

    void Update() {
        float mouseX = 0, mouseY = 0;

        if (Mouse.current != null) {
            var delta = Mouse.current.delta.ReadValue() / 15.0f;
            mouseX += delta.x;
            mouseY += delta.y;
        }
        if (Gamepad.current != null) {
            var value = Gamepad.current.rightStick.ReadValue() * 2;
            mouseX += value.x;
            mouseY += value.y;
        }
        
        mouseX *= mouseSensitivity * Time.deltaTime;
        mouseY *= mouseSensitivity * Time.deltaTime;
        
        rotationX -= mouseY * mouseSensitivity * horizontalSensitivity;
        float deltaY = mouseX * mouseSensitivity * verticalSensitivity;
        rotationX = Mathf.Clamp(rotationX, minimumVert, maximumVert);
        float rotationY = transform.localEulerAngles.y + deltaY;
        transform.localEulerAngles = new Vector3(rotationX, rotationY, 0);
    }
}