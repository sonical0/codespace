abstract class Cell
{
    public abstract (string symbol, ConsoleColor color) GetDisplay(
        ConsoleColor wallColor,
        ConsoleColor corridorColor,
        ConsoleColor exitColor,
        ConsoleColor startColor);
}

sealed class Wall : Cell
{
    public override (string symbol, ConsoleColor color) GetDisplay(
        ConsoleColor wallColor,
        ConsoleColor corridorColor,
        ConsoleColor exitColor,
        ConsoleColor startColor)
        => ("█", wallColor);
}

sealed class Room : Cell
{
    public ICollectable? Content { get; set; }

    public override (string symbol, ConsoleColor color) GetDisplay(
        ConsoleColor wallColor,
        ConsoleColor corridorColor,
        ConsoleColor exitColor,
        ConsoleColor startColor)
    {
        if (Content == null)
            return ("·", corridorColor);

        return Content switch
        {
            Key => ("#", ConsoleColor.Yellow),
            Coin => ("$", ConsoleColor.DarkYellow),
            _ => ("·", corridorColor)
        };
    }
}

sealed class Exit : Cell
{
    public override (string symbol, ConsoleColor color) GetDisplay(
        ConsoleColor wallColor,
        ConsoleColor corridorColor,
        ConsoleColor exitColor,
        ConsoleColor startColor)
        => ("★", exitColor);
}

sealed class Start : Cell
{
    public override (string symbol, ConsoleColor color) GetDisplay(
        ConsoleColor wallColor,
        ConsoleColor corridorColor,
        ConsoleColor exitColor,
        ConsoleColor startColor)
        => ("S", startColor);
}

sealed class Door : Cell
{
    public int DoorId { get; }
    public bool IsOpen { get; set; }

    public Door(int doorId = 0)
    {
        DoorId = doorId;
        IsOpen = false;
    }

    public override (string symbol, ConsoleColor color) GetDisplay(
        ConsoleColor wallColor,
        ConsoleColor corridorColor,
        ConsoleColor exitColor,
        ConsoleColor startColor)
        => (IsOpen ? "O" : "D", ConsoleColor.Magenta);
}
