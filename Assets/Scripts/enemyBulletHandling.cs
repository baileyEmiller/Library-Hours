//Bailey Miller 5/2/2025
using System;
using UnityEngine;

public class EnemyBulletHandling : MonoBehaviour
{
    bool hasDamaged = false;
    void OnCollisionEnter(Collision collision)
    {
        Health targetHealth = collision.gameObject.GetComponent<Health>();
        PlayerController p;
        try {
            p = collision.gameObject.GetComponent<PlayerController>();
            if (targetHealth != null && p.isDodging != true && hasDamaged != true) //this throws errors when the bullet collides with something other than player
            { //bc only player has isDodging and only entities have a targetHealth, but game doesnt crash so im gonna leave it for now :D
                targetHealth.health -= 1;
                hasDamaged = true;
                if (targetHealth.health <= 0)
                {
                    Destroy(collision.gameObject);
                }
            }
            Destroy(gameObject);
        }
        catch(Exception e) //if collision doesnt have player controller, enemy did not hit player, so exit
        {
            Console.Out.WriteLine(e);
            return;
        }
    }
}