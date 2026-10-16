using System;
using GameLib.Engine;

namespace GameLib.Physics;

/// <summary>
/// Abstract Components that represents the form of a GameObject.
/// Handles Collisions.
/// </summary>
public abstract class Collider : Component
{
    public RigidBody RigidBody { get; private set; }
    public Action<Collider> Collided;
    
    public void TriggerCollisionWith(Collider other) => Collided?.Invoke(other);
    
    public override void Connect(GameObject gameObject)
    {
        base.Connect(gameObject);
        RigidBody = GameObject.GetComponent<RigidBody>();
        PhysicsWorld.ActiveColliders.Add(this);
    }

    public override void Destroy()
    {
        PhysicsWorld.ActiveColliders.Remove(this);
    }
}
