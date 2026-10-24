using System.Collections.Generic;

namespace GameLib.Physics;

/// <summary>
/// The Core of the Physics Engine. Handles Collisions and updates RigidBodies every frame.
/// </summary>
public class PhysicsWorld()
{
    public static readonly List<Collider> ActiveColliders = [];
    public static readonly List<RigidBody> RigidBodies = [];
    
    /// <summary>
    /// Is called every frame. Is supposed to handle all the physics simulation.
    /// </summary>
    /// <param name="deltaTime"> Time since last frame.</param>
    public void Update(double deltaTime)
    {
        // Todo: Find and handle collisions between ActiveColliders using CollisionDetection.cs
        CollisionDetection.DetectCollisions(ActiveColliders);
        foreach (var body in RigidBodies)
        {
            body.Update(deltaTime);
        }
    }
}
