using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;
using UnityEngine.InputSystem;

[RequireComponent(typeof(VideoPlayer))]
public class VideoEvent : MonoBehaviour
{
    [Header("Video Player Component")]
    public VideoPlayer videoPlayer; // Optional VideoPlayer component reference

    [Header("Input Settings")]
    public PlayerInput playerInput;           // Reference to the PlayerInput component
    public string actionName = "Tap";

    [Header("Events")]
    public UnityEvent onVideoStarted;
    public UnityEvent onLoopPointReached;
    public UnityEvent onVideoPaused;
    public UnityEvent onVideoContinued;
    public UnityEvent onVideoSkipped; // Event for when the video is skipped

    private bool isPaused = false;      // To track pause/continue state
    private bool canSkip = false;       // Determines if the video can be skipped
    private float playTime = 0f;        // Tracks how long the video has been playing
    private InputAction skipAction;     // Reference to the input action for skipping

    private void Awake()
    {
        // Automatically get the VideoPlayer component if not assigned
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        // Ensure the VideoPlayer component is present
        if (videoPlayer == null)
        {
            Debug.LogError("No VideoPlayer component found. Please assign one in the Inspector.");
            return;
        }

        // Ensure the PlayerInput component is assigned
        if (playerInput == null)
        {
            Debug.LogError("PlayerInput component not assigned. Please assign it in the Inspector.");
            return;
        }

        // Find the specified action in the PlayerInput asset
        skipAction = playerInput.actions[actionName];
        if (skipAction != null)
        {
            skipAction.performed += OnSkipAction; // Subscribe to the action
        }
        else
        {
            Debug.LogError($"Action '{actionName}' not found in PlayerInput.");
        }

        // Subscribe to VideoPlayer events
        videoPlayer.started += OnVideoStarted;
        videoPlayer.loopPointReached += OnLoopPointReached;
    }

    private void Update()
    {
        if (videoPlayer.isPlaying)
        {
            playTime += Time.deltaTime; // Increment playtime if video is playing

            // Allow skipping after 3 seconds of playtime
            if (playTime >= 3f)
            {
                canSkip = true;
            }

            // Handle video continuation after a pause
            if (isPaused)
            {
                isPaused = false;
                onVideoContinued?.Invoke();
            }
        }
        else
        {
            if (!isPaused) // Video was playing but is now paused
            {
                isPaused = true;
                onVideoPaused?.Invoke();
            }
        }
    }

    private void OnSkipAction(InputAction.CallbackContext context)
    {
        // Check if skip is allowed and if the action is performed
        if (canSkip && context.performed)
        {
            SkipVideo();
        }
    }

    private void SkipVideo()
    {
        onVideoSkipped?.Invoke(); // Trigger skip event
        videoPlayer.Stop();
        canSkip = false;          // Reset skip state
    }

    private void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        if (videoPlayer != null)
        {
            videoPlayer.started -= OnVideoStarted;
            videoPlayer.loopPointReached -= OnLoopPointReached;
        }

        if (skipAction != null)
        {
            skipAction.performed -= OnSkipAction; // Unsubscribe from input action
        }
    }

    // Event Handlers
    private void OnVideoStarted(VideoPlayer vp)
    {
        playTime = 0f;           // Reset playtime at the start of the video
        canSkip = false;         // Reset skip state at the start
        onVideoStarted?.Invoke();
    }

    private void OnLoopPointReached(VideoPlayer vp)
    {
        onLoopPointReached?.Invoke();
    }
}
