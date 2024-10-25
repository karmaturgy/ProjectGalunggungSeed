using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientSystem : MonoBehaviour
{
    [Header("Global Ambient Settings (BGM)")]
    public AudioSource globalAmbientSource;   // Single AudioSource for Global Ambient/BGM
    public AudioClip[] globalAmbientClips;    // Array of global ambient tracks
    public float bgmVolume = 0.5f;            // Volume for global ambient tracks

    [Header("Local Ambient Settings")]
    public List<LocalAmbient> localAmbients;  // List of LocalAmbient class instances
    public Transform playerTransform;         // Reference to the player (e.g., HEROPLAYER)
    public float fadeSpeed = 1f;              // Speed of volume fade for all local ambients

    private void Start()
    {
        if(playerTransform == null)
        {
            GameObject player = GameObject.Find("HEROPLAYER");
            playerTransform = player.transform;
        }

        // Play a random global ambient clip on loop
        if (globalAmbientSource != null && globalAmbientClips.Length > 0)
        {
            globalAmbientSource.loop = true;
            globalAmbientSource.volume = bgmVolume;
            globalAmbientSource.clip = globalAmbientClips[Random.Range(0, globalAmbientClips.Length)];
            globalAmbientSource.Play();
        }

        // Ensure all local ambient sources are playing, but start with 0 volume
        foreach (LocalAmbient ambient in localAmbients)
        {
            if (ambient.audioSource != null)
            {
                ambient.audioSource.loop = true;
                ambient.audioSource.volume = 0f;  // Start with 0 volume
                ambient.audioSource.spatialBlend = 1f; // Ensure it's a 3D sound
                ambient.audioSource.Play();
            }
        }
    }

    private void Update()
    {
        if (playerTransform == null) return; // Exit if no playerTransform assigned

        // Update volume for each local ambient source based on player's distance
        foreach (LocalAmbient ambient in localAmbients)
        {
            float distance = Vector3.Distance(playerTransform.position, ambient.audioSource.transform.position);

            if (distance <= ambient.maxDistance)
            {
                // Calculate volume based on proximity, fading in as player approaches
                float targetVolume = Mathf.Lerp(0f, 1f, 1f - (distance / ambient.maxDistance));
                ambient.audioSource.volume = Mathf.Lerp(ambient.audioSource.volume, targetVolume, fadeSpeed * Time.deltaTime);
            }
            else
            {
                // Fade out when the player is beyond maxDistance
                ambient.audioSource.volume = Mathf.Lerp(ambient.audioSource.volume, 0f, fadeSpeed * Time.deltaTime);
            }
        }
    }
}

[System.Serializable]
public class LocalAmbient
{
    public AudioSource audioSource; // AudioSource for the local ambient sound
    public float maxDistance = 10f; // Maximum distance where the sound is fully heard
}
