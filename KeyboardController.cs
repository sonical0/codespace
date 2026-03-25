sealed class KeyboardController : IController
{
    public string InstructionsText { get; } = "  [Z/↑] Haut   [S/↓] Bas   [Q/←] Gauche   [D/→] Droite   [Espace] Ramasser   [Échap] Quitter";

    public bool IsUpPressed { get; private set; }
    public bool IsDownPressed { get; private set; }
    public bool IsLeftPressed { get; private set; }
    public bool IsRightPressed { get; private set; }
    public bool IsEscPressed { get; private set; }
    public bool IsPickupPressed { get; private set; }

    public ControllerInput ReadInput()
    {
        var key = Console.ReadKey(true).Key;

        IsUpPressed = key is ConsoleKey.Z or ConsoleKey.UpArrow;
        IsDownPressed = key is ConsoleKey.S or ConsoleKey.DownArrow;
        IsLeftPressed = key is ConsoleKey.Q or ConsoleKey.LeftArrow;
        IsRightPressed = key is ConsoleKey.D or ConsoleKey.RightArrow;
        IsEscPressed = key == ConsoleKey.Escape;
        IsPickupPressed = key == ConsoleKey.Spacebar;

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