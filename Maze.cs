sealed class Maze
{
    private readonly CellType[,] _grid;

    public Vec2d Size { get; }
    public Vec2d StartPosition { get; }
    public Vec2d ExitPosition { get; }

    public Maze(MazeGen mazeGen)
    {
        Size = mazeGen.Size;
        StartPosition = mazeGen.StartPosition;
        ExitPosition = mazeGen.ExitPosition;
        _grid = mazeGen.Generate();
    }

    public CellType GetCellType(Vec2d position) => _grid[position.X, position.Y];

    public bool IsInBounds(Vec2d position) => position.IsInBounds(Size);

    public bool IsWalkable(Vec2d position) =>
        IsInBounds(position) && GetCellType(position) != CellType.Wall;

    public bool IsExit(Vec2d position) =>
        IsInBounds(position) && GetCellType(position) == CellType.Exit;

    public void Draw(
        ConsoleScreen screen,
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
        ConsoleScreen screen,
        Vec2d offset,
        Vec2d position,
        ConsoleColor wallColor,
        ConsoleColor corridorColor,
        ConsoleColor exitColor,
        ConsoleColor startColor)
    {
        var output = GetCellType(position) switch
        {
            CellType.Wall => ("█", wallColor),
            CellType.Exit => ("★", exitColor),
            CellType.Start => ("S", startColor),
            _ => ("·", corridorColor)
        };

        screen.DrawText(offset + position, output);
    }
}