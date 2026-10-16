using GameLib.Engine;

namespace AsteroidZ;

public class ShipExplosion : UpdateComponent
{
    private const float AnimationDuration = 2f;
    private float _animationTimer = 0f;

    public override void Connect(GameObject gameObject)
    {
        base.Connect(gameObject);
        _animationTimer = AnimationDuration;
    }

    public override void Update(double deltaTime)
    {
        if (_animationTimer < 0)
        {
            GameObject.Destroy();
            return;
        }
        _animationTimer -= (float)deltaTime;
    }
}