//Bailey Miller, 4/17/25
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float rotationSpeed = 5000f;
    public Camera mainCamera;
    public GameObject bulletPrefab;
    public Transform shootPoint;
    private Rigidbody rb;
    public float dodgeForce = 75f;
    public float dodgeDuration = 0.2f;
    public float dodgeCooldown = 2f;
    public bool isDodging = false;
    private float dodgeTimer = 0f;
    private float dodgeCooldownTimer = 0f;
    private Vector3 dodgeDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (!mainCamera) mainCamera = Camera.main;
    }

    void Update()
    {
        // WASD
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        Vector3 moveDirection = new Vector3(moveX, 0, moveZ).normalized;

        if (moveDirection.magnitude >= 0.1f)
        {
            rb.MovePosition(transform.position + moveDirection * moveSpeed * Time.deltaTime);
        }

        RotateCharacterToMouse();

        if (Input.GetMouseButtonDown(0))
        {
            FireBullet();
        }
        // dodge
        if (Input.GetKeyDown(KeyCode.Space) && !isDodging && dodgeCooldownTimer <= 0f)
        {
            if (moveDirection != Vector3.zero)
            {
                StartDodge(moveDirection);
            }
        }

        if (isDodging)
        {
            rb.MovePosition(transform.position + dodgeDirection * dodgeForce * Time.deltaTime);
            dodgeTimer -= Time.deltaTime;

            if (dodgeTimer <= 0f)
            {
                isDodging = false;
                dodgeCooldownTimer = dodgeCooldown;
            }
        }

        if (dodgeCooldownTimer > 0f)
        {
            dodgeCooldownTimer -= Time.deltaTime;
        }
    }

    private void StartDodge(Vector3 direction)
    {
        isDodging = true;
        dodgeTimer = dodgeDuration;
        dodgeDirection = direction;
    }

    private void RotateCharacterToMouse()
    {
        // get mouse position in world space
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane playerPlane = new Plane(Vector3.up, transform.position);
        float rayLength;

        // check where the ray intersects with the plane
        if (playerPlane.Raycast(ray, out rayLength))
        {
            Vector3 pointToLookAt = ray.GetPoint(rayLength); // get mouse pos
            Vector3 dirToLookAt = pointToLookAt - transform.position; // direction to look at
            dirToLookAt.y = 0; // only Y rotation
            if (dirToLookAt.sqrMagnitude > 0.1f)
            {
                // rotation operation
                Quaternion targetRotation = Quaternion.LookRotation(dirToLookAt);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }

    private void FireBullet()
    {
        if (bulletPrefab && shootPoint)
        {
            GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

            if (bulletRb)
            {
                Vector3 direction = shootPoint.forward;
                direction.y = 0;
                direction.Normalize();
                bulletRb.linearVelocity = direction * 10f;
            }
            Destroy(bullet, 5f);
        }
    }
}