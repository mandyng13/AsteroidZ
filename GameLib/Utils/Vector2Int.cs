using System;
using Microsoft.Xna.Framework;

namespace GameLib.Utils;

public struct Vector2Int(int x, int y)
{
    public int X = x;
    public int Y = y;
    
    public static Vector2Int FromVector2(Vector2 vector) => new ((int)MathF.Round(vector.X), (int)MathF.Round(vector.Y));

    public static Vector2Int operator +(Vector2Int a, Vector2Int b) => new (a.X + b.X, a.Y + b.Y);
}
