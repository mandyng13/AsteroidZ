using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameLib.Engine
{
    /// <summary>
    /// Handles the texture and visual appearance of a GameObject.
    /// </summary>
    public class SpriteRenderer(Texture2D texture, int layer) : Component
    {
        public int Layer { get; } = layer;
        public Texture2D Texture { get; set; } = texture;
        public Color Color { get; set; } = Color.White;
        public Vector2 Offset { get; private set; } = new(-texture.Width / 2, -texture.Height / 2);
        public bool Enabled { get; private set; }
        public float Scale => _localScale * GameObject.Scale;
        public Vector2 Position {
            get 
            {
                var rotatedOffset = Offset * GameObject.Scale;
                rotatedOffset.Rotate(GameObject.Rotation);
                return GameObject.Position + rotatedOffset;   
            }
        }
        
        private float _localScale = 1.0f;

        public override void Connect(GameObject gameObject)
        {
            Enable();
            base.Connect(gameObject);
        }

        public override void Destroy()
        {
            Disable();
        }

        public void Enable()
        {
            if (Enabled)
                return; 
            Enabled = true;
            Core.AddActiveRenderer(this);
        }

        public void Disable()
        {
            if (!Enabled)
                return;
            Enabled = false;
            Core.RemoveActiveRenderer(this);
        }

        public class Builder(Texture2D texture, int layer = 0)
        {
            private SpriteRenderer _renderer = new(texture, layer);
            private Vector2 _customCenterOffset = Vector2.Zero;

            public Builder WithColor(Color color)
            {
                _renderer.Color = color;
                return this;
            }

            public Builder WithScale(float scale)
            {
                _renderer._localScale = scale;
                return this;
            }

            public Builder WithCenterOffset(Vector2 offset)
            {
                _customCenterOffset = offset;
                return this;
            }
            
            public SpriteRenderer Build() {
                _renderer.Offset = new Vector2(-texture.Width / 2, -texture.Height / 2) * _renderer._localScale + _customCenterOffset;
                return _renderer;
            }
        }
    }
}