using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuActions : MonoBehaviour {
    public GameObject startMenu;

    public void LockCursor() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UnlockCursor() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ToggleCursor() {
        if (Cursor.lockState == CursorLockMode.Locked) {
            UnlockCursor();
        } else {
            LockCursor();
        }
    }
    
    public void TogglePause() {
        if (startMenu.activeSelf)
            return;
        Time.timeScale = 1 - Time.timeScale;
        gameObject.SetActive(!gameObject.activeSelf);
        ToggleCursor();
    }

    public void PauseGame() {
        Time.timeScale = 0;
        gameObject.SetActive(true);
        UnlockCursor();
    }

    public void ResumeGame() {
        Time.timeScale = 1;
        gameObject.SetActive(false);
        LockCursor();
    }
    
    public void QuitGame() {
        Application.Quit();
    }
}