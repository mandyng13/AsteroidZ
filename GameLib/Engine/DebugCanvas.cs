using System;
using System.Collections.Generic;
using System.Linq;
using GameLib.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static System.Int32;

namespace GameLib.Engine;

public static class DebugCanvas
{
    private static readonly List<DebugShape> DebugShapes = [];
    private static readonly List<DebugShape> PermanentDebugShapes = [];

    /// <summary>
    /// Draws a circle on the screen. It will always be rendered in front of game objects.
    /// </summary>
    /// <param name="position">The center position.</param>
    /// <param name="radius">The radius.</param>
    /// <param name="color">The border color.</param>
    /// <param name="lineThickness">The border thickness. If the circle should be
    /// filled just pick a number higher than the radius.</param>
    /// <param name="permanent">If set to true the shape will be drawn for the rest of the game.
    /// Otherwise <c>DrawCircle</c> will have to be called every frame.</param>
    public static void DrawCircle(Vector2 position, int radius, Color color, int lineThickness = 3, bool permanent = false)
    {
        var circleTexture = CreateCircleTexture(radius, lineThickness, color);
        
        var debugShape = new DebugShape(circleTexture, position);
        if (permanent)
            PermanentDebugShapes.Add(debugShape);    
        else
            DebugShapes.Add(debugShape);
    }

    /// <summary>
    /// Draws a square on the screen. It will always be rendered in front of game objects.
    /// </summary>
    /// <param name="position">The center position.</param>
    /// <param name="size">The width and height of the square.</param>
    /// <param name="color">The border color.</param>
    /// <param name="lineThickness">The border thickness. If the square should be
    /// filled just pick a number higher than the size.</param>
    /// <param name="permanent">If set to true the shape will be drawn for the rest of the game.
    /// Otherwise <c>DrawSquare</c> will have to be called every frame.</param>
    public static void DrawSquare(Vector2 position, int size, Color color, int lineThickness = 3, bool permanent = false, float rotation = 0)
    {
        DrawRectangle(position, size, size, color, lineThickness, permanent, rotation);
    }

    /// <summary>
    /// Draws a rectangle on the screen. It will always be rendered in front of game objects.
    /// </summary>
    /// <param name="position">The center position.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <param name="color">The border color.</param>
    /// <param name="lineThickness">The border thickness. If the square should be
    /// filled just pick a number higher than width and height.</param>
    /// <param name="permanent">If set to true the shape will be drawn for the rest of the game.
    /// Otherwise <c>DrawRectangle</c> will have to be called every frame.</param>
    public static void DrawRectangle(Vector2 position, int width, int height, Color color, int lineThickness = 3, bool permanent = false, float rotation = 0)
    {
        var rectTexture = CreateRectTexture(width, height, lineThickness, color);
        var debugShape = new DebugShape(rectTexture, position, rotation);
        if (permanent)
            PermanentDebugShapes.Add(debugShape);    
        else
            DebugShapes.Add(debugShape);
    }

    /// <summary>
    /// Draws a polygon on the screen. Will always render in front of game objects. Can also be used to render a line
    /// by providing just two vertices.
    /// </summary>
    /// <param name="position">The polygon position. The vertices will be positioned relative to this.</param>
    /// <param name="vertices">The vertices of the polygon. They should be ordered clockwise or counterclockwise.</param>
    /// <param name="color">The border color.</param>
    /// <param name="permanent">If set to true the shape will be drawn for the rest of the game.
    /// Otherwise <c>DrawPolygon</c> will have to be called every frame.</param>
    public static void DrawPolygon(Vector2 position, Vector2[] vertices, Color color, bool permanent = false, float rotation = 0)
    {
        var polygonTexture = CreatePolygonTexture(vertices, color);
        var debugShape = new DebugShape(polygonTexture, position, rotation);
        if (permanent)
            PermanentDebugShapes.Add(debugShape);    
        else
            DebugShapes.Add(debugShape);
    }

    public static DebugShape[] Flush()
    {
        var shapeArray = DebugShapes.Concat(PermanentDebugShapes).ToArray();
        DebugShapes.Clear();

        return shapeArray;
    }

    private static Texture2D CreateCircleTexture(int radius, int lineThickness, Color color)
    {
        var textureSize = radius * 2;
        var texture = new Texture2D(Core.GraphicsDevice, textureSize, textureSize);
        var colorData = new Color[textureSize * textureSize];

        var outerRadiusSqr = radius * radius;
        var innerRadius = Math.Max(0, radius - lineThickness);
        var innerRadiusSqr = innerRadius * innerRadius;

        var center = radius - 0.5f;

        for (var x = 0; x < textureSize; x++)
        {
            var dx = x - center;
            var dxSqr = dx * dx;

            for (var y = 0; y < textureSize; y++)
            {
                var dy = y - center;
                var distSqr = dy * dy + dxSqr;
                if (distSqr < outerRadiusSqr && distSqr > innerRadiusSqr)
                {
                    colorData[x * textureSize + y] = color;
                }
                else
                {
                    colorData[x * textureSize + y] = Color.Transparent;
                }
            }
        }

        texture.SetData(colorData);

        return texture;
    }
    
    private static Texture2D CreateRectTexture(int width, int height, int lineThickness, Color color)
    {
        var texture = new Texture2D(Core.GraphicsDevice, width, height);
        var colorData = new Color[width * height];
        
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                if (x < lineThickness || x >= width - lineThickness || y < lineThickness || y >= height - lineThickness)
                {
                    colorData[y * width + x] = color;
                }
                else
                {
                    colorData[y * width + x] = Color.Transparent;
                }
            }
        }

        texture.SetData(colorData);

        return texture;
    }

    private static Texture2D CreatePolygonTexture(Vector2[] vertices, Color color)
    {
        var textureSize = GetTextureSizeForPolygon(vertices);
        var newTex = new Texture2D(Core.GraphicsDevice, textureSize.X, textureSize.Y);
        var colorData = new Color[textureSize.X * textureSize.Y];
        
        if (vertices.Length < 2)
            throw new ArgumentException("Vertices array must have at least 2 vertices.");

        for (var i = 0; i < vertices.Length; i++)
        {
            var start = vertices[i];
            var end = vertices[(i + 1) % vertices.Length];
            var startFromTopLeft = Vector2Int.FromVector2(start) + new Vector2Int(textureSize.X / 2, textureSize.Y / 2);
            var endFromTopLeft = Vector2Int.FromVector2(end) + new Vector2Int(textureSize.X / 2, textureSize.Y / 2);
            
            DrawBresenhamLine(colorData, startFromTopLeft, endFromTopLeft, textureSize.X, textureSize.Y, color);
        }
        newTex.SetData(colorData);
        return newTex;
    }
    
    private static Vector2Int GetTextureSizeForPolygon(Vector2[] vertices)
    {
        var minX = MaxValue;
        var minY = MaxValue;
        var maxX = MinValue;
        var maxY = MinValue;
        
        foreach (var v in vertices)
        {
            if (v.X < minX)
                minX = (int)v.X;
            if (v.X > maxX)
                maxX = (int)v.X;
            if (v.Y < minY)
                minY = (int)v.Y;
            if (v.Y > maxY)
                maxY = (int)v.Y;
        }

        var width = Math.Max(Math.Abs(maxX), Math.Abs(minX)) * 2 + 1;
        var height = Math.Max(Math.Abs(maxY), Math.Abs(minY)) * 2 + 1;
        return new Vector2Int(width, height);
    }

    private static void DrawBresenhamLine(Color[] pixels, Vector2Int start, Vector2Int end, int textureWidth, int textureHeight, Color color)
    {
        var x0 = start.X;
        var y0 = start.Y;
        var x1 = end.X;
        var y1 = end.Y;
        
        var dx = Math.Abs(end.X - start.X);
        var dy = Math.Abs(end.Y - start.Y);
        var sx = start.X < end.X ? 1 : -1;
        var sy = start.Y < end.Y ? 1 : -1;
        var err = dx - dy;
        
        var maxSteps = dx + dy;

        for (var step = 0; step <= maxSteps; step++)
        {
            if (x0 >= 0 && x0 < textureWidth && y0 >= 0 && y0 < textureHeight)
            {
                pixels[y0 * textureWidth + x0] = color;
            }

            if (x0 == x1 && y0 == y1) break;

            var e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }

    public struct DebugShape(Texture2D texture, Vector2 position, float rotation = 0f)
    {
        public Texture2D Texture = texture;
        public Vector2 Position = position;
        public float Rotation = rotation; 
    }
}