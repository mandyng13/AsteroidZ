using GameLib.Engine;
using GameLib.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AsteroidZ;

public class ShipController(GameObject.Template shipExplosionTemplate) : Controller
{
    public ShipFlame ShipFlame { get; set; }

    private RigidBody _rigidBody;
    private Collider _collider;
    private GameObject.Template _shipExplosionTemplate = shipExplosionTemplate;
        
    private const float BoostForce = 500f;

    public override void Connect(GameObject gameObject)
    {
        base.Connect(gameObject);
        _rigidBody = gameObject.GetComponent<RigidBody>();
        _collider = GameObject.GetComponent<Collider>();
        if (_collider != null)
            _collider.Collided += OnCollision;
    }

    public override void HandleInput(KeyboardState state, MouseState _)
    {
        //Todo Input fügt _rigidBody Kräfte hinzu
        if (state.IsKeyDown(Keys.A) || state.IsKeyDown(Keys.Left))
        {
            _rigidBody.AddForce(new Vector2(-BoostForce, 0));
        }
        if (state.IsKeyDown(Keys.D) || state.IsKeyDown(Keys.Right))
        {
            _rigidBody.AddForce(new Vector2(BoostForce, 0));
        }
        if (state.IsKeyDown(Keys.W) || state.IsKeyDown(Keys.Up))
        {
            _rigidBody.AddForce(new Vector2(0, -BoostForce));
        }
        if (state.IsKeyDown(Keys.S) || state.IsKeyDown(Keys.Down))
        {
            _rigidBody.AddForce(new Vector2(0, BoostForce));
        }
    }
    
    private void OnCollision(Collider other)
    {
        _shipExplosionTemplate.Instantiate(GameObject.Position);
        ShipFlame.GameObject.Destroy();
        GameObject.Destroy();
    }
}