using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using UHFPS.Runtime;

public class TeleportTransition : MonoBehaviour
{
    [Header("Fade Settings")]
    public CanvasGroup fadePanel; // Assign your Fade Panel's Image component here
    public float fadeDuration = 1.0f; // Duration of the fade in/out effect
    public Transform playerTransform; // The player's Transform to teleport
    public LookController lookController; // Reference to LookController to adjust camera rotation

    [Header("Teleportation Settings")]
    public Transform teleportDestination; // Destination transform for teleportation
    public Vector2 teleportRotation;
    public bool enableRotation = true; // Option to enable or disable rotation on teleport
    public float playerScale = 1.0f; // Scale of the player after teleportation

    [Header("Time of Day Manager")]
    public TimeOfDayManager timeOfDayManager;

    [Header("Trigger Events")]
    public UnityEvent onEnterTrigger; // Called when entering the trigger zone
    public UnityEvent onStayTrigger;  // Called while staying in the trigger zone
    public UnityEvent onExitTrigger;  // Called when exiting the trigger zone

    [Header("Fade Events")]
    public UnityEvent onFadeOut; // Called right before fading out
    public UnityEvent onTeleport; // Called after teleportation
    public UnityEvent onFadeIn; // Called after fading in

    private bool isTeleporting = false;
    private bool fade = true;
    private string skyBox = "";
    private Vector3 originalScale; // Store the original scale of the player

    // Reference to the AudioSource component of the ambient sound
    private AudioSource ambientAudioSource;

    private void OnEnable()
    {
        // Automatically find and assign the FadePanel Image component
        if (fadePanel == null)
        {
            GameObject fadePanelObject = GameObject.Find("FadePanel");
            if (fadePanelObject != null)
            {
                fadePanel = fadePanelObject.GetComponent<CanvasGroup>();
            }
            else
            {
                Debug.LogError("FadePanel not found! Make sure a GameObject named 'FadePanel' exists in the scene.");
            }
        }

        // Automatically find and assign the player transform
        if (playerTransform == null)
        {
            GameObject playerObject = GameObject.Find("HEROPLAYER");
            if (playerObject != null)
            {
                playerTransform = playerObject.transform;
                originalScale = playerTransform.localScale; // Store the original scale of the player
            }
            else
            {
                Debug.LogError("HEROPLAYER not found! Make sure a GameObject named 'HEROPLAYER' exists in the scene.");
            }
        }

        // Automatically find and assign the LookController
        if (lookController == null)
        {
            GameObject lookControllerObject = FindObjectOfType<LookController>().gameObject;
            if (lookControllerObject != null)
            {
                lookController = lookControllerObject.GetComponent<LookController>();
            }
            else
            {
                Debug.LogError("LookController not found! Make sure an object with LookController exists in the scene.");
            }
        }

        // Automatically find and assign the timeOfDayManager
        if (timeOfDayManager == null)
        {
            GameObject day = FindObjectOfType<TimeOfDayManager>().gameObject;
            if (day != null)
            {
                timeOfDayManager = day.GetComponent<TimeOfDayManager>();
            }
            else
            {
                Debug.LogError("timeOfDayManager not found! Make sure an object with TimeOfDayManager exists in the scene.");
            }
        }

        // Automatically find and assign the ambient AudioSource from the GameManager
        if (ambientAudioSource == null)
        {
            GameObject Ambience = GameObject.Find("GAMEMANAGER/Audios/Ambience"); // Assuming GameManager is in the scene
            if (Ambience != null)
            {
                ambientAudioSource = Ambience.GetComponent<AudioSource>();
                if (ambientAudioSource == null)
                {
                    Debug.LogError("The Ambience object does not have an AudioSource component!");
                }
            }
            else
            {
                Debug.LogError("GameManager or its Audios/Ambience not found!");
            }
        }
    }

    private void Update()
    {
        if (isTeleporting)
        {
            if (fade && fadePanel.alpha < 1f)
            {
                if (fadePanel.alpha == 0f)
                {
                    onFadeOut.Invoke();
                }

                fadePanel.alpha += Time.deltaTime / fadeDuration;

                // Start fading in the ambient sound when fade out reaches 100%
                if (fadePanel.alpha >= 1f)
                {
                    fade = false;
                    onTeleport.Invoke();
                    timeOfDayManager.ApplyTimeOfDay(skyBox);

                    // Teleport and resize the player
                    if (teleportDestination != null)
                    {
                        playerTransform.position = teleportDestination.position;
                        playerTransform.localScale = Vector3.one * playerScale;

                        if (enableRotation && lookController != null)
                        {
                            lookController.LookRotation = teleportRotation;
                        }
                    }
                    else
                    {
                        Debug.LogError("Teleport destination is not assigned!");
                    }

                    // Fade in ambient sound
                    StartCoroutine(FadeAmbientSound(0f, 0.2f, fadeDuration));
                }
            }

            if (!fade && fadePanel.alpha >= 0f)
            {
                fadePanel.alpha -= Time.deltaTime / fadeDuration;

                if (fadePanel.alpha <= 0)
                {
                    fade = true;
                    onFadeIn.Invoke();
                    isTeleporting = false;
                }
            }
        }
    }

    private IEnumerator FadeAmbientSound(float startVolume, float endVolume, float duration)
    {
        float timeElapsed = 0f;
        float initialVolume = startVolume;
        while (timeElapsed < duration)
        {
            ambientAudioSource.volume = Mathf.Lerp(initialVolume, endVolume, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        ambientAudioSource.volume = endVolume;
    }

    public void TeleportPlayer(string skies)
    {
        if (isTeleporting)
            return;

        skyBox = skies;
        isTeleporting = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Invoke the event for when the player enters the trigger
            onEnterTrigger.Invoke();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Invoke the event for when the player stays inside the trigger
            onStayTrigger.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Invoke the event for when the player exits the trigger
            onExitTrigger.Invoke();
        }
    }
}
