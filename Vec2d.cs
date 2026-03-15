readonly record struct Vec2d(int X, int Y)
{
    public static readonly Vec2d Zero = new(0, 0);

    public static Vec2d operator +(Vec2d left, Vec2d right) =>
        new(left.X + right.X, left.Y + right.Y);

    public static Vec2d operator -(Vec2d left, Vec2d right) =>
        new(left.X - right.X, left.Y - right.Y);

    public static Vec2d operator *(Vec2d value, int factor) =>
        new(value.X * factor, value.Y * factor);

    public bool IsInBounds(Vec2d size) =>
        X >= 0 && X < size.X && Y >= 0 && Y < size.Y;

    public Vec2d Midpoint(Vec2d other) =>
        new((X + other.X) / 2, (Y + other.Y) / 2);
}