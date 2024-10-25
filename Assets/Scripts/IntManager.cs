using System;
using UnityEngine;
using UnityEngine.Events;

public class IntManager : MonoBehaviour
{
    // Define the class that holds name, maxInt, currentInt, and the event
    [Serializable]
    public class IntData
    {
        public string name;
        public int maxInt;
        public int currentInt;

        // Each IntData has its own UnityEvent
        public UnityEvent onMaxIntReached;

        // Constructor for the class
        public IntData(string name, int maxInt, int currentInt)
        {
            this.name = name;
            this.maxInt = maxInt;
            this.currentInt = currentInt;
            this.onMaxIntReached = new UnityEvent(); // Initialize the event
        }

        // Method to increment currentInt
        public void IncrementInt()
        {
            if (currentInt < maxInt)
            {
                currentInt++;
                Debug.Log($"Incremented {name}: {currentInt}/{maxInt}");

                if (currentInt == maxInt)
                {
                    Debug.Log($"{name} has reached its max value!");
                    onMaxIntReached.Invoke(); // Invoke the event when maxInt is reached
                }
            }
            else
            {
                Debug.Log($"{name} has already reached max value!");
            }
        }
    }

    // Array to hold multiple IntData objects
    public IntData[] intDataArray;

    // Function to increment an IntData object by name
    public void IncrementIntByName(string name)
    {
        foreach (IntData data in intDataArray)
        {
            if (data.name == name)
            {
                data.IncrementInt(); // Call the increment function on the specific IntData
                return;
            }
        }
        Debug.Log($"No entry found for name: {name}");
    }
}
