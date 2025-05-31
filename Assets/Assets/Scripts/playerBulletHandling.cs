//Bailey Miller 5/20/2025
using UnityEngine;

public class PlayerBulletHandling : MonoBehaviour
{
    bool hasDamaged = false;
    void OnCollisionEnter(Collision collision)
    {
        Health targetHealth = collision.gameObject.GetComponent<Health>();

        if (targetHealth != null && hasDamaged != true)
        {
            targetHealth.health -= 1;
            hasDamaged = true;
            if (targetHealth.health <= 0)
            {
                Destroy(collision.gameObject);
            }
        }

        Destroy(gameObject);
    }
}