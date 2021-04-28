using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Rigidbody2DExt
{
    public static void AddExplosionForce(this Rigidbody2D rb, float explosionForce, Vector2 explosionPosition, float explosionRadius, float upwardsModifier = 0.0F, ForceMode2D mode = ForceMode2D.Force)
    {
        Debug.LogError("ExploForce called");
        Vector2 explosionDir = rb.position - explosionPosition;
        float explosionDistance = explosionDir.magnitude;
        explosionDir = explosionDir.normalized;

        rb.AddForce((1 - (explosionDistance / explosionRadius)) * explosionForce * explosionDir * rb.mass);

        //if (upwardsModifier == 0)
        //    explosionDir /= explosionDistance;
        //else
        //{
        //    explosionDir.y += upwardsModifier;
        //    explosionDir.Normalize();
        //}

        //rb.AddForce(Mathf.Lerp(0, explosionForce, (1 - explosionDistance)) * explosionDir, mode);
    }
}
