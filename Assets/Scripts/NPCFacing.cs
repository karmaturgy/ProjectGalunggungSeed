using UnityEngine;

public class NPCFacing : MonoBehaviour
{
    public bool alwaysFollow = false;    // Option to always follow the player
    public float followRadius = 3f;    // Radius within which the NPC follows the player

    public Transform player;            // Player's transform (automatically assigned)

    void Start()
    {
        // Automatically assign the player's transform by finding the object named "HEROPLAYER"
        GameObject playerObject = GameObject.Find("HEROPLAYER");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player object with name 'HEROPLAYER' not found in the scene.");
        }
    }

    void Update()
    {
        if (player != null)
        {
            // Calculate the distance between NPC and player
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // Check if NPC should follow the player based on alwaysFollow or distance within radius
            if (alwaysFollow || distanceToPlayer <= followRadius)
            {
                // Make the NPC face the player by rotating only on the Y-axis
                FacePlayer();
            }
        }
    }

    // Function to rotate NPC to face the player
    void FacePlayer()
    {
        // Get direction to player
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0;  // Ignore y-axis for rotation (only rotate on y-axis)

        // Calculate the target rotation towards the player
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

        // Smoothly rotate the NPC towards the target rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5.0f);
    }
}
