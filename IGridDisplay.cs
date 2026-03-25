interface IGridDisplay
{
    void DrawText(Vec2d position, string text, ConsoleColor? color = null);
    void DrawText(Vec2d position, (string text, ConsoleColor color) info);
    void DrawBoxedText(Vec2d position, string text, ConsoleColor? color = null);
}
