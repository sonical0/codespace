sealed class KeyboardController
{
    public string InstructionsText { get; } = "  [Z/↑] Haut   [S/↓] Bas   [Q/←] Gauche   [D/→] Droite   [Échap] Quitter";

    public ControllerInput ReadInput()
    {
        var key = Console.ReadKey(true).Key;

        return key switch
        {
            ConsoleKey.Z or ConsoleKey.UpArrow => new ControllerInput(new Vec2d(0, -1), false),
            ConsoleKey.S or ConsoleKey.DownArrow => new ControllerInput(new Vec2d(0, 1), false),
            ConsoleKey.Q or ConsoleKey.LeftArrow => new ControllerInput(new Vec2d(-1, 0), false),
            ConsoleKey.D or ConsoleKey.RightArrow => new ControllerInput(new Vec2d(1, 0), false),
            ConsoleKey.Escape => new ControllerInput(Vec2d.Zero, true),
            _ => new ControllerInput(Vec2d.Zero, false)
        };
    }
}

readonly record struct ControllerInput(Vec2d MoveDelta, bool IsCanceled);