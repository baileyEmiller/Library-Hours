using UnityEditor;
using UnityEngine;

public class PlayerSpriteHandling : MonoBehaviour
{
    [Tooltip("Reference to the player's transform (typically the capsule parent)")]
    public Transform playerTransform;

    [Tooltip("Animator on the hat GameObject")]
    public Animator animator;

    [Tooltip("Movement speed threshold below which the player is considered idle")]
    public float idleThreshold = 0.1f;

    private Rigidbody playerRb;

    void Start()
    {
        if (playerTransform == null)
            playerTransform = transform.parent;

        playerRb = playerTransform.GetComponent<Rigidbody>();

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 1. Determine if the player is moving
        float speed = playerRb.linearVelocity.magnitude;
        bool isMoving = speed > idleThreshold;
        animator.SetBool("IsMoving", isMoving);

        // 2. Determine direction the player is facing
        Vector3 forward = playerTransform.forward;
        forward.y = 0f;
        forward.Normalize();

        float angle = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
        if (angle < 0f) angle += 360f;

        int directionIndex;

        if (angle >= 45f && angle < 135f)
            directionIndex = 1; // East
        else if (angle >= 135f && angle < 225f)
            directionIndex = 2; // South
        else if (angle >= 225f && angle < 315f)
            directionIndex = 3; // West
        else
            directionIndex = 0; // North

        animator.SetInteger("Direction", directionIndex);
    }

    void LateUpdate()
    {
        // Invert the parent's rotation to cancel it out
        transform.rotation = Quaternion.identity;
    }
}
