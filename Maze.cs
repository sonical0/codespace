sealed class Maze
{
    private readonly Cell[,] _grid;

    public Vec2d Size { get; }
    public Vec2d StartPosition { get; }
    public Vec2d ExitPosition { get; }

    public Maze(IMazeGenerator mazeGen)
    {
        Size = mazeGen.Size;
        StartPosition = mazeGen.StartPosition;
        ExitPosition = mazeGen.ExitPosition;
        _grid = mazeGen.Generate();
    }

    public Cell GetCell(Vec2d position) => _grid[position.X, position.Y];

    public bool IsInBounds(Vec2d position) => position.IsInBounds(Size);

    public bool IsWalkable(Vec2d position) =>
        IsInBounds(position) && !(_grid[position.X, position.Y] is Wall);

    public bool IsWalkable(Vec2d position, IReadOnlyList<ICollectable> inventory)
    {
        if (!IsInBounds(position))
            return false;

        var cell = _grid[position.X, position.Y];

        if (cell is Wall)
            return false;

        if (cell is Door door)
        {
            // Check if player has the key for this door
            return inventory.OfType<Key>().Any(k => k.DoorId == door.DoorId);
        }

        return true;
    }

    public bool IsExit(Vec2d position) =>
        IsInBounds(position) && _grid[position.X, position.Y] is Exit;

    public void Draw(
        IGridDisplay screen,
        Vec2d offset,
        ConsoleColor wallColor,
        ConsoleColor corridorColor,
        ConsoleColor exitColor,
        ConsoleColor startColor)
    {
        for (var y = 0; y < Size.Y; y++)
        {
            for (var x = 0; x < Size.X; x++)
            {
                DrawCell(
                    screen,
                    offset,
                    new Vec2d(x, y),
                    wallColor,
                    corridorColor,
                    exitColor,
                    startColor);
            }
        }
    }

    public void DrawCell(
        IGridDisplay screen,
        Vec2d offset,
        Vec2d position,
        ConsoleColor wallColor,
        ConsoleColor corridorColor,
        ConsoleColor exitColor,
        ConsoleColor startColor)
    {
        var cell = _grid[position.X, position.Y];
        var output = cell.GetDisplay(wallColor, corridorColor, exitColor, startColor);
        screen.DrawText(offset + position, output);
    }
}