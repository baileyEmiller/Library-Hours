using UnityEngine;

public class PlayerSpriteHandling : MonoBehaviour
{
    public Animator animator;
    public Transform playerCapsule;
    private PlayerControllerScript pcs;
     void Start()
    {
        pcs = playerCapsule.GetComponent<PlayerControllerScript>();
    }
    void Update()
    {
        Vector3 forward = pcs.exportVector;

        int dir = GetDirectionIndex(forward);
        animator.SetInteger("direction", dir);
        animator.SetBool("isWalking", IsPlayerMoving());
        animator.SetBool("isShooting", IsPlayerShooting());
        animator.SetBool("isDodging", IsPlayerDodging());
        transform.position = playerCapsule.position;
    }

    int GetDirectionIndex(Vector3 dir)
    {
        // Project to XZ plane (ignore vertical axis)
        Vector3 flatDir = new Vector3(dir.x, 0f, dir.z).normalized;

        // Calculate angle in degrees relative to +X axis
        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        angle = (angle + 360f + 90f) % 360f; // Normalize to 0–360 (90 added bc the player sprite gets rotated 90 degrees on play)

        if (angle >= 45f && angle < 135f)
            return 0; // Up
        else if (angle >= 135f && angle < 225f)
            return 1; // Right
        else if (angle >= 225f && angle < 315f)
            return 2; // Down
        else
            return 3; // Left
    }

    bool IsPlayerMoving()
    {
        return Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0; //only checks player input
    }

    bool IsPlayerShooting()
    {
        return Input.GetMouseButton(0); //only returns player click
    }

    bool IsPlayerDodging()
    {
        return pcs.isDodging;
    }
}
