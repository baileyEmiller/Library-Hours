using UnityEngine;

public class splitEnemyBehavior : baseEnemyBehavior
{
    public override void FireWeapon()
    {
        Vector3 baseDirection = (player.position - shootPoint.position).normalized;
        float[] angles = {30f, -30f};

        foreach (float angle in angles)
        {
            GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            if (rb != null)
            {
                Vector3 rotatedDirection = Quaternion.AngleAxis(angle, shootPoint.up) * baseDirection;
                rb.linearVelocity = rotatedDirection * 20f;
            }

            Destroy(projectile, 5f);
        }
    }
}
