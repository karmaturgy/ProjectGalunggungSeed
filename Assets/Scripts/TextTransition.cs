using System.Collections;
using TMPro;
using UnityEngine;

public class TextTransition : MonoBehaviour
{
    public TextMeshPro textMeshPro; // Reference to TextMeshPro component (normal, not UGUI)

    private bool isFading = false;

    private void Awake()
    {
        // If textMeshPro is not set in the inspector, automatically find the TextMeshPro component on this GameObject
        if (textMeshPro == null)
        {
            textMeshPro = GetComponent<TextMeshPro>();
        }
    }

    // Function to fade in the text over time
    public void FadeIn(float duration)
    {
        StartCoroutine(FadeTextTo(duration, 1.0f)); // Fade to fully visible (alpha = 1)
    }

    // Function to fade out the text over time
    public void FadeOut(float duration)
    {
        StartCoroutine(FadeTextTo(duration, 0.0f)); // Fade to invisible (alpha = 0)
    }

    // New function to handle fade in, stay for a few seconds, then fade out
    public void FadeInAndOut(float visibleDuration)
    {
        StartCoroutine(FadeInOutRoutine(visibleDuration));
    }

    // Coroutine to fade in, wait, and then fade out
    private IEnumerator FadeInOutRoutine(float visibleDuration)
    {
        // Fade in
        yield return StartCoroutine(FadeTextTo(2f, 1.0f)); // Fade to fully visible

        // Stay visible for a certain duration
        yield return new WaitForSeconds(visibleDuration);

        // Fade out
        yield return StartCoroutine(FadeTextTo(2f, 0.0f)); // Fade to invisible
    }

    // Coroutine to handle the fade transition
    private IEnumerator FadeTextTo(float fadeDuration, float targetAlpha)
    {
        if (textMeshPro == null) yield break;
        if (isFading) yield break; // Prevent overlapping fades

        isFading = true; // Prevents starting another fade while one is in progress

        Color currentColor = textMeshPro.color;
        float startAlpha = currentColor.a; // Get the starting alpha value
        float timer = 0;

        // Fade over time
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float blend = Mathf.Clamp01(timer / fadeDuration); // Ensures it stays between 0 and 1
            currentColor.a = Mathf.Lerp(startAlpha, targetAlpha, blend);
            textMeshPro.color = currentColor;
            yield return null; // Wait for the next frame
        }

        // Ensure the final alpha is exactly the target
        currentColor.a = targetAlpha;
        textMeshPro.color = currentColor;
        isFading = false;
    }
}
