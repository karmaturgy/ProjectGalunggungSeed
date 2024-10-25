using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class TimeOfDayManager : MonoBehaviour
{
    public TimeOfDay[] timeOfDaySettings;
    public Light directionalLight;
    public string MainTime = "";

    [System.Serializable]
    public class TimeOfDay
    {
        public string name; // e.g., "Morning", "Afternoon", "Evening"
        public Material skyboxMaterial;
        public float lightIntensity;
        public Vector3 lightRotation;
    }

    private void Awake()
    {
        // Automatically find and assign the directional light (sun)
        if (directionalLight == null)
        {
            GameObject sunObject = GameObject.Find("Sun Light");
            directionalLight = sunObject.GetComponent<Light>();
            if (directionalLight == null || !directionalLight.type.Equals(LightType.Directional))
            {
                Debug.LogError("Directional Light not found! Make sure there is a directional light in the scene.");
            }
        }
    }

    public void Start()
    {
        ApplyTimeOfDay(MainTime);
    }

    public void ApplyTimeOfDay(string timeOfDayName)
    {
        // Check if there are any settings defined
        if (timeOfDaySettings.Length == 0)
        {
            Debug.LogError("No time of day settings found.");
            return;
        }

        // Loop through the settings to find the one with the matching name
        foreach (TimeOfDay timeOfDay in timeOfDaySettings)
        {
            if (timeOfDay.name.Equals(timeOfDayName, System.StringComparison.OrdinalIgnoreCase))
            {
                // Apply the settings
                RenderSettings.skybox = timeOfDay.skyboxMaterial;
                directionalLight.intensity = timeOfDay.lightIntensity;
                directionalLight.transform.eulerAngles = timeOfDay.lightRotation;

                return;
            }
        }

        // If no matching name was found
        Debug.LogError("Time of day with name '" + timeOfDayName + "' not found.");
    }
}
