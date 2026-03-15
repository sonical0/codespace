sealed class KeyboardController
{
    public string InstructionsText { get; } = "  [Z/↑] Haut   [S/↓] Bas   [Q/←] Gauche   [D/→] Droite   [Échap] Quitter";

    public KeyboardInput ReadInput()
    {
        var key = Console.ReadKey(true).Key;

        return key switch
        {
            ConsoleKey.Z or ConsoleKey.UpArrow => new KeyboardInput(new Vec2d(0, -1), false),
            ConsoleKey.S or ConsoleKey.DownArrow => new KeyboardInput(new Vec2d(0, 1), false),
            ConsoleKey.Q or ConsoleKey.LeftArrow => new KeyboardInput(new Vec2d(-1, 0), false),
            ConsoleKey.D or ConsoleKey.RightArrow => new KeyboardInput(new Vec2d(1, 0), false),
            ConsoleKey.Escape => new KeyboardInput(Vec2d.Zero, true),
            _ => new KeyboardInput(Vec2d.Zero, false)
        };
    }
}

readonly record struct KeyboardInput(Vec2d MoveDelta, bool IsCanceled);