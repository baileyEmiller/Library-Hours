// Bailey Miller, 4/30/2025
using System.Collections;
using UnityEngine;

public class burstEnemyBehavior : baseEnemyBehavior
{
    public float TimeBetweenBurst = .2f;
    IEnumerator BurstFire()
    {
        FireWeapon();
        yield return new WaitForSeconds(TimeBetweenBurst);
        FireWeapon();
        yield return new WaitForSeconds(TimeBetweenBurst);
        FireWeapon();
    }

    public override void ShootAtPlayer()
    {
        if (projectilePrefab != null && shootPoint != null)
        {
            StartCoroutine(BurstFire());
        }
    }
}