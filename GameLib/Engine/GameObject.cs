using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace GameLib.Engine
{
    /// <summary>
    /// Any Object with a position, rotation and scale in the game world.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <param name="scale"></param>
    public class GameObject(Vector2 position, float rotation = 0f, float scale = 1f)
    {
        public static List<GameObject> QueuedForDestruction = new();
        
        public Vector2 Position { get; set; } = position;
        public float Rotation { get; set; } = rotation;
        public float Scale { get; set; } = scale;

        public Vector2 Forward
        {
            get
            {
                var forward = new Vector2(0, 1);
                forward.Rotate(Rotation);
                return forward;
            }
        }

        private readonly List<Component> _components = [];

        public T GetComponent<T>() where T : Component
        {
            return (T)_components.FirstOrDefault(c => c is T, null);
        }

        public void Destroy()
        {
            QueuedForDestruction.Add(this);
        }

        public void DestroyImmediate()
        {
            foreach (var component in _components)
            {
                component.Destroy();
            }
        }

        public class Template(List<Component> components)
        {
            public GameObject Instantiate(Vector2 position, float rotation = 0, float scale = 1)
            {
                var gameObject = new GameObject(position, rotation, scale);
                components.ForEach(comp => gameObject._components.Add(comp.CloneComponent()));
                gameObject._components.ForEach(comp => comp.Connect(gameObject));
                return gameObject;
            }
        }
    }
}