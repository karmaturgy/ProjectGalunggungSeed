using UnityEngine;

public class ShowingCursor : MonoBehaviour
{
    [Header("Cursor Settings")]
    public bool cursorVisible = true;           // Set to true to show the cursor
    public CursorLockMode cursorLockMode = CursorLockMode.None; // None for unrestricted cursor

    private void Start()
    {
        // Apply the cursor settings
        SetCursorState();
    }

    private void SetCursorState()
    {
        // Show or hide the cursor based on the cursorVisible setting
        Cursor.visible = cursorVisible;

        // Set the cursor locking mode
        Cursor.lockState = cursorLockMode;
    }

    // Optional: Use this if you need to reset cursor settings when returning to this scene
    private void OnEnable()
    {
        SetCursorState();
    }

    public void SetCursorVisible(bool visible)
    {
        Cursor.visible = visible;
    }

    public void SetCursorLockMode(CursorLockMode lockMode)
    {
        Cursor.lockState = lockMode;
    }
}
