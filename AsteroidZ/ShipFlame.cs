using GameLib.Engine;

namespace AsteroidZ;

public class ShipFlame : UpdateComponent
{
    public GameObject ShipObject { get; set; }

    private SpriteRenderer _flameRenderer;

    private const float YOffset = -35;

    public override void Connect(GameObject gameObject)
    {
        base.Connect(gameObject);
        _flameRenderer = GameObject.GetComponent<SpriteRenderer>();
    }

    public override void Update(double deltaTime)
    {
        if (ShipObject == null)
            return;
        
        GameObject.Position = ShipObject.Position + ShipObject.Forward * YOffset;
        GameObject.Rotation = ShipObject.Rotation;
    }

    public void Show() => _flameRenderer.Enable();
    public void Hide() => _flameRenderer.Disable();
}