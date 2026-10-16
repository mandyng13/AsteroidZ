using System;
using GameLib.Engine;
using GameLib.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AsteroidZ;

public class AsteroidBuilder : UpdateComponent
{
    private Texture2D[] _asteroidTextures;
    private float _lastBuildTime;
    
    private const int StartAsteroidCount = 3;
    private const float AsteroidBuildInterval = 2.5f;
    private const float MaxMass = 30f;
    private const float MinMass = 10f;
    private const float MaxScale = 0.5f;
    private const float MinScale = 0.2f;

    public override void Connect(GameObject gameObject)
    {
        base.Connect(gameObject);
        _asteroidTextures =
        [
            Core.Content.Load<Texture2D>("Sprites/spaceMeteors_001"),
            Core.Content.Load<Texture2D>("Sprites/spaceMeteors_002"),
            Core.Content.Load<Texture2D>("Sprites/spaceMeteors_003"),
            Core.Content.Load<Texture2D>("Sprites/spaceMeteors_004")
        ];
        
        for (var i = 0; i < StartAsteroidCount; i++)
            BuildRandomAsteroid();
    }

    public override void Update(double deltaTime)
    {
        if (Core.TimeSinceStart - _lastBuildTime > AsteroidBuildInterval)
            BuildRandomAsteroid();
    }

    private void BuildRandomAsteroid()
    {
        var textureIndex = Random.Shared.Next(0, _asteroidTextures.Length - 1);
        var asteroidSize = GetRandomFloat(0, 1f);
        var scale = MathHelper.Lerp(MinScale, MaxScale, asteroidSize);
        var mass = MathHelper.Lerp(MinMass, MaxMass, asteroidSize);
        var radius = 210f * scale;
        
        var randomTemplate = new GameObject.Template([
            new SpriteRenderer.Builder(_asteroidTextures[textureIndex], 1).Build(),
            // Todo: Add rigidbody
            // Todo: Add Circle collider
        ]);

        var startPosition = GetRandomAsteroidStartPosition();
        var newAsteroid = randomTemplate.Instantiate(startPosition, 0, scale);
        var direction = GetRandomInwardDirectionFromPosition(startPosition);
        // Todo: give the newAsteroid a starting push of 250000N in direction
        
        _lastBuildTime = Core.TimeSinceStart;
    }

    private Vector2 GetRandomAsteroidStartPosition()
    {
        return GetRandomPositionFromCenter(750, 900);
    }

    private Vector2 GetRandomInwardDirectionFromPosition(Vector2 position)
    {
        var centerPoint = GetRandomPositionFromCenter(0, 300f);
        var inwardDirection = centerPoint - position;
        inwardDirection.Normalize();
        
        return inwardDirection;
    }
    
    private Vector2 GetRandomPositionFromCenter(float minRadius, float maxRadius)
    {
        var position = new Vector2(0, 1);
        var radius = GetRandomFloat(minRadius, maxRadius);
        position *= radius;
        var rotation = MathHelper.ToRadians(GetRandomFloat(0, 360));
        position.Rotate(rotation);
        
        return position + new Vector2(Core.ScreenWidth, Core.ScreenHeight) * 0.5f;
    }
    
    private float GetRandomFloat(float min, float max)
    {
        return min + (float)Random.Shared.NextDouble() * (max - min);
    }
}