// Bailey Miller, 4/30/2025
using UnityEngine;
using UnityEngine.AI;

public class baseEnemyBehavior : MonoBehaviour
{
    public Transform player;
    public float stopDistance = 4f;
    public float strafeDistance = 5f;
    public float shootCooldown = 2.5f;
    public float speed = 3f;
    public GameObject projectilePrefab;
    public Transform shootPoint;

    private NavMeshAgent agent;
    private float shootTimer;
    private Vector3 strafeDirection;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        shootTimer = shootCooldown;
        BehaviorAfterShot();
        player = GameObject.Find("player_capsule").transform;
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // look at player
        Vector3 lookPos = player.position - transform.position;
        lookPos.y = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookPos), Time.deltaTime * 5);

        if (distanceToPlayer > stopDistance)
        {
            ApproachPlayer();
        }
        else
        {
            BehaviorAfterApproach(distanceToPlayer);   
        }

        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0)
        {
            ShootAtPlayer();
            shootTimer = shootCooldown;
            BehaviorAfterShot();
        }
    }

    public virtual void ApproachPlayer()
    {
        // run toward player until stop distance
        agent.speed = speed;
        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

    public virtual void BehaviorAfterApproach(float distanceToPlayer)
    {
        // strafe and maintain distance
        agent.isStopped = true;

        Vector3 toPlayer = (transform.position - player.position).normalized;
        Vector3 strafeDir = Quaternion.Euler(0, 90, 0) * toPlayer * (strafeDirection.x > 0 ? 1 : -1);

        // Combine strafe with backup if too close
        Vector3 movementDirection = strafeDir;
        if (distanceToPlayer < stopDistance - 1f)
        {
            movementDirection += toPlayer;
        }

        movementDirection.Normalize();
        agent.Move(movementDirection * speed * Time.deltaTime);
    }

    public virtual void ShootAtPlayer()
    {
        if (projectilePrefab != null && shootPoint != null)
        {
            FireWeapon();
        }
    }

    public virtual void FireWeapon()
    {
        GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 direction = (player.position - shootPoint.position).normalized;
            rb.linearVelocity = direction * 20f;
        }
        Destroy(projectile, 5f);
    }

    public virtual void BehaviorAfterShot()
    {
        strafeDirection = Random.value < 0.5f ? Vector3.left : Vector3.right;
    }
}