//Bailey Miller 5/2/2025
using UnityEngine;

public class EnemyBulletHandling : MonoBehaviour
{
    bool hasDamaged = false;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Health targetHealth = collision.gameObject.GetComponent<Health>();
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();

            if (targetHealth != null &&
                !hasDamaged &&
                (playerController == null || playerController.isDodging == false))
            {

                targetHealth.health -= 1;
                hasDamaged = true;

                if (targetHealth.health <= 0)
                {
                    Destroy(collision.gameObject);
                }
            }
        }

        Destroy(gameObject);
    }
}