//Bailey Miller 5/2/2025
using UnityEngine;

public class EnemyBulletHandling : MonoBehaviour
{
    bool hasDamaged = false;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
            return;
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            Health targetHealth = collision.gameObject.GetComponent<Health>();
            PlayerControllerScript pcs = collision.gameObject.GetComponent<PlayerControllerScript>();

            if (targetHealth != null &&
                !hasDamaged &&
                pcs != null &&
                pcs.isDodging == false)
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