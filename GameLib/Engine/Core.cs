using System;
using System.Collections.Generic;
using System.Diagnostics;
using GameLib.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameLib.Engine;

public class Core : Game
{
    /// <summary>
    /// The graphics device manager used to control the presentation of graphics.
    /// </summary>
    public static GraphicsDeviceManager Graphics { get; private set; }
    
    /// <summary>
    /// The graphics device used to create graphical resources and perform primitive rendering.
    /// </summary>
    public static new GraphicsDevice GraphicsDevice { get; private set; }
    
    /// <summary>
    /// The sprite batch used for all 2D rendering.
    /// </summary>
    public static SpriteBatch SpriteBatch { get; private set; }
    
    /// <summary>
    /// The content manager used to load global assets.
    /// </summary>
    public new static ContentManager Content { get; private set; }
    
    /// <summary>
    /// The game background color.
    /// </summary>
    public Color ClearColor { get; set; } = Color.MonoGameOrange;
    
    /// <summary>
    /// Time since the Game started in seconds.
    /// </summary>
    public static float TimeSinceStart { get; private set; }
    
    /// <summary>
    /// Height of the game window in pixels.
    /// </summary>
    public static int ScreenHeight { get; private set; }
    
    /// <summary>
    /// Width of the game window in pixels.
    /// </summary>
    public static int ScreenWidth { get; private set; }
    
    /// <summary>
    /// List of all active SpriteRenderers to be drawn in the Draw-Function.
    /// </summary>
    public static List<SpriteRenderer>[] RenderLayers = new List<SpriteRenderer>[10];
    /// <summary>
    /// List of all Controllers that handle player input in the Update-Function.
    /// </summary>
    public static readonly List<Controller> Controllers = [];

    /// <summary>
    /// List of all components that get have an Update-Method that gets called every frame.
    /// </summary>
    public static readonly List<UpdateComponent> UpdateComponents = [];
    
    /// <summary>
    /// The PhysicsWorld that applies physics to all RigidBody-Components in the Update-Function.
    /// </summary>
    private PhysicsWorld _physics;

    protected Core(string title, int width, int height)
    {
        ScreenHeight = height;
        ScreenWidth = width;
        
        Graphics = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = width,
            PreferredBackBufferHeight = height,
        };
        
        Window.Title = title;
        Content = base.Content;
        Content.RootDirectory = "Content";

        for (var i = 0; i < RenderLayers.Length; i++)
            RenderLayers[i] = [];

        IsFixedTimeStep = true;
        TargetElapsedTime = TimeSpan.FromSeconds(1 / 60.0);
    }

    protected override void Initialize()
    {
        base.Initialize();
        _physics = new PhysicsWorld();
        GraphicsDevice = base.GraphicsDevice;
        SpriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        TimeSinceStart = (float)gameTime.TotalGameTime.TotalSeconds;
        
        _physics.Update(gameTime.ElapsedGameTime.TotalSeconds);
        foreach (var controller in Controllers)
        {
            controller.HandleInput(Keyboard.GetState(), Mouse.GetState());
        }
        foreach (var updateComponent in UpdateComponents)
        {
            if (updateComponent.DestroyEndOfFrame)
                continue;
            updateComponent.Update(gameTime.ElapsedGameTime.TotalSeconds);
        }

        for (var i = UpdateComponents.Count - 1; i >= 0; i--)
        {
            if (UpdateComponents[i].DestroyEndOfFrame)
                UpdateComponents.RemoveAt(i);
        }

        base.Update(gameTime);
    }

    public static void AddActiveRenderer(SpriteRenderer renderer)
    {
        RenderLayers[renderer.Layer].Add(renderer);
    }

    public static void RemoveActiveRenderer(SpriteRenderer renderer)
    {
        RenderLayers[renderer.Layer].Remove(renderer);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(ClearColor);

        SpriteBatch.Begin();

        for (var layerIndex = 0; layerIndex < RenderLayers.Length; layerIndex++)
        {
            foreach (var s in RenderLayers[layerIndex])
            {
                SpriteBatch.Draw(s.Texture, s.Position, null, s.Color, s.GameObject.Rotation, Vector2.Zero, s.Scale,
                    SpriteEffects.None, 0.0f);
            }
        }
        
        if (Debugger.IsAttached)
            DrawDebugCollisionShapes();
        
        foreach (var shape in DebugCanvas.Flush())
        {
            Vector2 origin = new Vector2(shape.Texture.Width / 2f, shape.Texture.Height / 2f);
            SpriteBatch.Draw(shape.Texture, shape.Position, null, Color.White, shape.Rotation, origin, 1f,
                SpriteEffects.None, 0f);
        }
        
        SpriteBatch.End();

        base.Draw(gameTime);
    }

    protected override void EndDraw()
    {
        base.EndDraw();
        foreach (var gameObject in GameObject.QueuedForDestruction)
        {
            gameObject.DestroyImmediate();
        }
        GameObject.QueuedForDestruction.Clear();
    }

    private void DrawDebugCollisionShapes()
    {
        // foreach (var activeCollider in PhysicsWorld.ActiveColliders)
        // {
        //     if (activeCollider is CircleCollider circle)
        //         DebugCanvas.DrawCircle(activeCollider.Position, (int)circle.Radius, Color.DeepSkyBlue);
        // }
    }
}