using GameLib.Engine;
using Microsoft.Xna.Framework;

namespace GameLib.Physics;

/// <summary>
/// Component that represents a gameobjects physical presence in the world.
/// Does not define size or bounds of the object. 
/// Handles Kinematics and Force Application.
/// </summary>
/// <param name="Mass"></param>
public class RigidBody : Component
{
    public float Mass = 1f;
    public Vector2 Velocity = Vector2.Zero;
    public Vector2 Acceleration = Vector2.Zero;
    public Vector2 Force = Vector2.Zero;
    
    public override void Connect(GameObject gameObject)
    {
        PhysicsWorld.RigidBodies.Add(this);
        base.Connect(gameObject);
    }
    
    /// <summary>
    /// Is called every frame. Is supposed to handle all the physics simulation.
    /// </summary>
    /// <param name="deltaTime"> Time since last frame.</param>
    public void Update(double deltaTime)
    {
        Acceleration = Force / Mass;
        
        Velocity += Acceleration * (float)deltaTime;
        
        GameObject.Position += Velocity * (float)deltaTime;

        Force = Vector2.Zero;
    }

    public override void Destroy()
    {
        PhysicsWorld.RigidBodies.Remove(this);
    }

    public void AddForce(Vector2 force)
    {
        Force += force;
    }
}