//Bailey Miller, 4/17/25
using UnityEngine;

public class PlayerControllerScript : MonoBehaviour
{
    // editor obj assignemnts
    public Camera mainCamera;
    public GameObject bulletPrefab;
    public Transform shootPoint;

    // behavior variables
    public float moveSpeed = 5f;
    public float rotationSpeed = 720f;
    public float dodgeForce = 15f;
    public float dodgeDuration = 0.2f;
    public float dodgeCooldown = 2f;
    public bool isDodging = false;

    //calculation variables
    private Rigidbody rb;
    private Vector3 moveDirection;
    private Vector3 dodgeDirection;
    private Quaternion targetRot;
    private float dodgeTimer = 0f;
    private float dodgeCooldownTimer = 0f;
    private float moveX;
    private float moveZ;



    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (!mainCamera)
        {
            mainCamera = Camera.main;
        }
    }



    void Update() // updates every frame, good for player input and time calculations
    {
        // movement input
        CalculateMovement();

        // bullet fire
        if (Input.GetMouseButtonDown(0))
        {
            FireBullet();
        }

        // rotation input
        CalculateTargetRotationFromMouse();

        // dodge input
        if (Input.GetKeyDown(KeyCode.Space) && !isDodging && dodgeCooldownTimer <= 0f && moveDirection != Vector3.zero)
        {
            CalculateDodge(moveDirection);
        }

        // dodge cooldown
        if (dodgeCooldownTimer > 0f)
        {
            CalculateDodgeCooldown();
        }
    }



    void FixedUpdate() //updates every .02 seconds, used only for physics calculations
    {
        if (isDodging)
        {
            //dodge action
            DodgePlayer();
        }
        else
        {
            //movement action
            MovePlayer();
        }

        //rotation action
        RotateCharacterToMouse();
    }


    //start dodge
    private void CalculateDodge(Vector3 direction)
    {
        isDodging = true;
        dodgeDirection = direction.normalized;
        dodgeTimer = dodgeDuration;
    }


    //do dodge
    private void DodgePlayer()
    {
        rb.MovePosition(rb.position + dodgeDirection * dodgeForce * Time.fixedDeltaTime);
        dodgeTimer -= Time.fixedDeltaTime;
        if (dodgeTimer <= 0f)
        {
            isDodging = false;
            dodgeCooldownTimer = dodgeCooldown;
        }
    }


    //start movement
    private void CalculateMovement()
    {
        moveX = Input.GetAxisRaw("Horizontal");
        moveZ = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector3(moveX, 0, moveZ).normalized;
    }


    //do movement
    private void MovePlayer()
    {
        if (moveDirection.magnitude > 0.1f)
        {
            rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
        }
    }


    //start rotate
    private void CalculateTargetRotationFromMouse()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, transform.position);

        if (groundPlane.Raycast(ray, out float hitDist))
        {
            Vector3 targetPoint = ray.GetPoint(hitDist);
            Vector3 lookDir = (targetPoint - transform.position);
            lookDir.y = 0;

            if (lookDir.sqrMagnitude > 0.001f)
            {
                targetRot = Quaternion.LookRotation(lookDir);
            }
        }
    }


    //do rotate
    private void RotateCharacterToMouse()
    {
        rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime));
    }


    //do time decrement
    public void CalculateDodgeCooldown()
    {
        dodgeCooldownTimer -= Time.deltaTime;
    }


    //do weapon fire
    private void FireBullet()
    {
        if (bulletPrefab && shootPoint)
        {
            GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

            if (bulletRb)
            {
                bulletRb.linearVelocity = shootPoint.forward * 10f;
            }

            Destroy(bullet, 5f);
        }
    }
}