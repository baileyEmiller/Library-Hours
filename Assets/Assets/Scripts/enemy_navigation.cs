using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public float stopDistance = 10f;
    public float strafeDistance = 5f;
    public float shootCooldown = 2.5f;
    public float strafeSpeed = 3f;
    public GameObject projectilePrefab;
    public Transform shootPoint;

    private NavMeshAgent agent;
    private float shootTimer;
    private Vector3 strafeDirection;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        shootTimer = shootCooldown;
        ChooseStrafeDirection();
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Look at player
        Vector3 lookPos = player.position - transform.position;
        lookPos.y = 0; // keep rotation on the Y axis only
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookPos), Time.deltaTime * 5);

        // Movement logic
        if (distanceToPlayer > stopDistance)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        else
        {
            agent.isStopped = true;

            // Strafe around player
            Vector3 strafeTarget = player.position + (Quaternion.Euler(0, 90, 0) * strafeDirection) * strafeDistance;
            Vector3 moveDir = (strafeTarget - transform.position).normalized;
            agent.Move(moveDir * strafeSpeed * Time.deltaTime);
        }

        // Shooting logic
        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0)
        {
            ShootAtPlayer();
            shootTimer = shootCooldown;
            ChooseStrafeDirection();
        }
    }

    void ShootAtPlayer()
    {
        if (projectilePrefab != null && shootPoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direction = (player.position - shootPoint.position).normalized;
                rb.velocity = direction * 20f; // example speed
            }
        }
    }

    void ChooseStrafeDirection()
    {
        strafeDirection = Random.value < 0.5f ? Vector3.left : Vector3.right;
    }
}