using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GameLib.Physics;

/// <summary>
/// Detects Collisions amongst a set of Colliders
/// </summary>
public static class CollisionDetection
{
    // Todo: Find Collisions
    // Todo: Trigger found collisions with Collider.TriggerCollisionsWith
    public static void DetectCollisions(List<Collider> colliders)
    {
        for (int i = 0; i < colliders.Count; i++)
        {
            for (int j = i + 1; j < colliders.Count; j++)
            {
                // Collider-Paar prüfen
                if (colliders[i] is CircleCollider a && colliders[j] is CircleCollider b)
                {
                    Vector2 difference = a.GameObject.Position - b.GameObject.Position;
                    
                    float radius = a.Radius + b.Radius;

                    if (difference.LengthSquared() <= radius * radius)
                    {
                        a.TriggerCollisionWith(b);
                        b.TriggerCollisionWith(a);
                    }
                }
            }
        }
    }
}