using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{
    public Transform launchPoint;
    public GameObject projectilePrefab;
    public float spreadAngle = 10f; // Angle between projectiles for spread (degrees)
    public int numberOfProjectiles = 1; // Number of projectiles to fire

    public void FireProjectile()
    {
        // Calculate the angle step for even distribution
        float angleStep = numberOfProjectiles > 1 ? spreadAngle / (numberOfProjectiles - 1) : 0;
        float startAngle = numberOfProjectiles > 1 ? -spreadAngle / 2 : 0;

        for (int i = 0; i < numberOfProjectiles; i++)
        {
            // Calculate rotation for this projectile
            Quaternion rotation = Quaternion.Euler(0, 0, startAngle + i * angleStep) * projectilePrefab.transform.rotation;
            GameObject projectile = Instantiate(projectilePrefab, launchPoint.position, rotation);

            // Adjust scale based on parent's facing direction
            Vector3 origScale = projectile.transform.localScale;
            projectile.transform.localScale = new Vector3(
                origScale.x * (transform.localScale.x > 0 ? 1 : -1),
                origScale.y,
                origScale.z
            );
        }
    }
}
