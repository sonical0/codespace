readonly record struct ConsoleScreen
{
    public void DrawText(Vec2d position, string text, ConsoleColor? color = null)
    {
        Console.SetCursorPosition(position.X, position.Y);
        if (color.HasValue)
        {
            Console.ForegroundColor = color.Value;
        }

        Console.Write(text);
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