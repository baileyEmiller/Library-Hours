//Bailey Miller, 4/17/25
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 700f;
    public Camera mainCamera;
    public GameObject bulletPrefab;
    public Transform shootPoint;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (!mainCamera) mainCamera = Camera.main; // default to the main camera if not assigned
    }

    void Update()
    {
        // WASD
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical"); 
        Vector3 moveDirection = new Vector3(moveX, 0, moveZ).normalized;

        if (moveDirection.magnitude >= 0.1f)
        {
            // move operation
            rb.MovePosition(transform.position + moveDirection * moveSpeed * Time.deltaTime);
        }

        RotateCharacterToMouse();

        if (Input.GetMouseButtonDown(0))
        {
            FireBullet();
        }
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
                // Use shootPoint's forward direction to fire the bullet
                Vector3 direction = shootPoint.forward;
                direction.y = 0;
                direction.Normalize();
                bulletRb.linearVelocity = direction * 10f;
            }
            Destroy(bullet, 3f);
        }
    }
}