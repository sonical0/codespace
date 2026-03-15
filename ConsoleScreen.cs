readonly record struct ConsoleScreen
{
    public void DrawText(Vec2d position, string text, ConsoleColor? color = null)
    {
        if (position.X < 0 || position.Y < 0)
        {
            return;
        }

        if (position.Y >= Console.BufferHeight || position.X >= Console.BufferWidth)
        {
            return;
        }

        var drawableLength = Math.Min(text.Length, Console.BufferWidth - position.X);
        if (drawableLength <= 0)
        {
            return;
        }

        var drawableText = drawableLength == text.Length ? text : text[..drawableLength];

        Console.SetCursorPosition(position.X, position.Y);
        if (color.HasValue)
        {
            Console.ForegroundColor = color.Value;
        }

        Console.Write(drawableText);
        Console.ResetColor();
    }

    public void DrawText(Vec2d position, (string text, ConsoleColor color) info) =>
        DrawText(position, info.text, info.color);

    public void DrawBoxedText(Vec2d position, string text, ConsoleColor? color = null)
    {
        var lines = text.Replace("\r", string.Empty).Split('\n');
        var contentWidth = lines.Max(static line => line.Length);

        DrawText(position, $"╔{new string('═', contentWidth + 2)}╗", color);

        for (var index = 0; index < lines.Length; index++)
        {
            var line = lines[index];
            var paddedLine = line.PadRight(contentWidth);
            DrawText(new Vec2d(position.X, position.Y + index + 1), $"║ {paddedLine} ║", color);
        }

        DrawText(new Vec2d(position.X, position.Y + lines.Length + 1), $"╚{new string('═', contentWidth + 2)}╝", color);
    }
}