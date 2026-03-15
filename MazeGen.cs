sealed class MazeGen
{
    private static readonly Vec2d[] Directions =
    [
        new(0, -1),
        new(1, 0),
        new(0, 1),
        new(-1, 0)
    ];

    private static readonly int[][] Orders =
    [
        [0, 1, 2, 3], [0, 1, 3, 2], [0, 2, 1, 3], [0, 2, 3, 1], [0, 3, 1, 2], [0, 3, 2, 1],
        [1, 0, 2, 3], [1, 0, 3, 2], [1, 2, 0, 3], [1, 2, 3, 0], [1, 3, 0, 2], [1, 3, 2, 0],
        [2, 0, 1, 3], [2, 0, 3, 1], [2, 1, 0, 3], [2, 1, 3, 0], [2, 3, 0, 1], [2, 3, 1, 0],
        [3, 0, 1, 2], [3, 0, 2, 1], [3, 1, 0, 2], [3, 1, 2, 0], [3, 2, 0, 1], [3, 2, 1, 0]
    ];

    public Vec2d Size { get; }
    public Vec2d StartPosition { get; }
    public Vec2d ExitPosition { get; }

    public MazeGen(Vec2d size, Vec2d? startPosition = null)
    {
        if (size.X < 3 || size.Y < 3)
        {
            throw new ArgumentOutOfRangeException(nameof(size), "Le labyrinthe doit faire au moins 3x3.");
        }

        Size = size;
        StartPosition = NormalizeInteriorPosition(startPosition ?? new Vec2d(1, 1));
        ExitPosition = NormalizeInteriorPosition(new Vec2d(Size.X - 2, Size.Y - 2));
    }

    public CellType[,] Generate()
    {
        var grid = new CellType[Size.X, Size.Y];

        for (var y = 0; y < Size.Y; y++)
            for (var x = 0; x < Size.X; x++)
                grid[x, y] = CellType.Wall;

        var rng = new Random();
        GenerateMazeRec(StartPosition);

        grid[StartPosition.X, StartPosition.Y] = CellType.Start;
        grid[ExitPosition.X, ExitPosition.Y] = CellType.Exit;

        return grid;

        void GenerateMazeRec(Vec2d currentPosition)
        {
            grid[currentPosition.X, currentPosition.Y] = CellType.Corridor;
            foreach (var directionIndex in Orders[rng.Next(Orders.Length)])
            {
                var delta = Directions[directionIndex];
                var nextPosition = currentPosition + (delta * 2);
                if (IsInterior(nextPosition) &&
                    grid[nextPosition.X, nextPosition.Y] == CellType.Wall)
                {
                    var between = currentPosition.Midpoint(nextPosition);
                    grid[between.X, between.Y] = CellType.Corridor;
                    GenerateMazeRec(nextPosition);
                }
            }
        }
    }

    private bool IsInterior(Vec2d position) =>
        position.X > 0 && position.X < Size.X - 1 &&
        position.Y > 0 && position.Y < Size.Y - 1;

    private Vec2d NormalizeInteriorPosition(Vec2d position) =>
        new(
            NormalizeAxis(position.X, Size.X),
            NormalizeAxis(position.Y, Size.Y));

    private static int NormalizeAxis(int value, int axisSize)
    {
        var min = 1;
        var max = axisSize - 2;

        var clamped = Math.Clamp(value, min, max);
        if ((clamped & 1) == 0)
        {
            clamped = clamped == max ? clamped - 1 : clamped + 1;
        }

        return clamped;
    }
}