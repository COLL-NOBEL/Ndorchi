using UnityEngine;

public class ZoneRestrictor : MonoBehaviour
{
    // This is the "Box" the player is allowed to stay in.
    // We will drag and drop the Zone Trigger here in the Inspector.
    public BoxCollider assignedZone;

    // This runs AFTER the player moves, to "catch" them if they went too far.
    void LateUpdate()
    {
        // 1. If we haven't assigned a zone yet, don't do anything.
        if (assignedZone == null) return;

        // 2. Get the boundaries of the assigned zone.
        Bounds bounds = assignedZone.bounds;

        // 3. Get the player's current position.
        Vector3 playerPos = transform.position;

        // 4. "Clamp" the position. 
        // This means: If X is greater than the Max X of the box, set it TO the Max X.
        float clampedX = Mathf.Clamp(playerPos.x, bounds.min.x, bounds.max.x);
        float clampedZ = Mathf.Clamp(playerPos.z, bounds.min.z, bounds.max.z);

        // 5. Apply the clamped position back to the player.
        // We keep the Y position (height) the same so they don't sink into the floor.
        transform.position = new Vector3(clampedX, playerPos.y, clampedZ);
    }
}