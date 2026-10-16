using GameLib.Engine;
using GameLib.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AsteroidZ;

public class AsteroidZGame() : Core("AsteroidZ", 1000, 700)
{
    private GameObject.Template _shipTemplate;
    private GameObject.Template _shipFlameTemplate;
    private GameObject.Template _asteroidBuilder;
    
    protected override void Initialize()
    {
        base.Initialize();
        ClearColor = Color.Black;
        
        CreateTemplates();
        var shipFlameObject = _shipFlameTemplate.Instantiate(new Vector2(500, 315));
        var shipFlame = shipFlameObject.GetComponent<ShipFlame>();
        var ship = _shipTemplate.Instantiate(new Vector2(500, 350));
        ship.GetComponent<ShipController>().ShipFlame = shipFlame;
        shipFlame.ShipObject = ship;
        
        _asteroidBuilder.Instantiate(new Vector2(0, 0));
    }

    private void CreateTemplates()
    {
        var shipExplosionTemplate = new GameObject.Template([
            new SpriteRenderer.Builder(Content.Load<Texture2D>("Sprites/Cloud"), 2).WithScale(0.5f).Build(),
            new ShipExplosion()
        ]); 
        _shipTemplate = new GameObject.Template([
            new SpriteRenderer.Builder(Content.Load<Texture2D>("Sprites/SpaceShip")).WithScale(0.3f).Build(),
            // Todo: Add a rigidbody with a mass of 10
            // Todo: Add a circle collider with a radius of 33
            new ShipController(shipExplosionTemplate)
        ]);

        _shipFlameTemplate = new GameObject.Template([
            new SpriteRenderer.Builder(Content.Load<Texture2D>("Sprites/Flame")).WithScale(0.4f).Build(),
            new ShipFlame()
        ]);

        _asteroidBuilder = new GameObject.Template([
            new AsteroidBuilder()
        ]);
    }
}