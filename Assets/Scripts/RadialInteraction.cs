/*
 * Code sourced from tutorial by Speed Tutor
 * https://youtu.be/5xWDKJj1UGY
 * Adapted to work with new input system and use event key inputs
 */

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RadialInteraction : MonoBehaviour {
    [Header("Radial Timers")]
    [SerializeField] private float indicatorTimer = 0.0f;
    public float maxIndicatorTimer = 1.0f;
    [SerializeField] private float blockTimer = 1.0f;
    
    [Header("UI indicator")]
    [SerializeField] private Image radialIndicatorUI;
    

    [Header("Unity Event")]
    [SerializeField] private UnityEvent primaryInteract;
    [SerializeField] private UnityEvent secondaryInteract;

    private UnityEvent _callbackEvent;

    private bool shouldUpdate = false;
    private float blockTime = 0.0f;

    public void OnPrimaryKey(InputAction.CallbackContext value) {
        var keyDown = value.ReadValue<float>() == 1.0f;
        CheckKey(keyDown);
        _callbackEvent = primaryInteract;
    }
    
    public void OnSecondaryKey(InputAction.CallbackContext value) {
        var keyDown = value.ReadValue<float>() == 1.0f;
        CheckKey(keyDown);
        _callbackEvent = secondaryInteract;
    }

    public void CheckKey(bool keyDown) {
        if (keyDown && maxIndicatorTimer > .0f) {
            shouldUpdate = true;
            radialIndicatorUI.enabled = true;
        } else {
            shouldUpdate = false;
            radialIndicatorUI.enabled = false;
            indicatorTimer = .0f;
            radialIndicatorUI.fillAmount = .0f;
        }
    }
    private void Update() {
        if (blockTime > 0) {
            blockTime -= Time.deltaTime;
        } else if (shouldUpdate) {
            indicatorTimer += Time.deltaTime;
            radialIndicatorUI.fillAmount = indicatorTimer / maxIndicatorTimer;

            if (indicatorTimer >= maxIndicatorTimer) {
                indicatorTimer = .0f;
                radialIndicatorUI.fillAmount = .0f;
                _callbackEvent.Invoke();
                blockTime = blockTimer;
            }
        }
    }
}