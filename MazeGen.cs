sealed class MazeGen : IMazeGenerator
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
    public double CoinProbability { get; }
    public double DoorProbability { get; }

    public MazeGen(Vec2d size, Vec2d? startPosition = null, double coinProbability = 0.1, double doorProbability = 0.15)
    {
        if (size.X < 3 || size.Y < 3)
        {
            throw new ArgumentOutOfRangeException(nameof(size), "Le labyrinthe doit faire au moins 3x3.");
        }

        if (coinProbability < 0 || coinProbability > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(coinProbability), "La probabilité doit être entre 0 et 1.");
        }

        if (doorProbability < 0 || doorProbability > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(doorProbability), "La probabilité doit être entre 0 et 1.");
        }

        Size = size;
        StartPosition = NormalizeInteriorPosition(startPosition ?? new Vec2d(1, 1));
        ExitPosition = NormalizeInteriorPosition(new Vec2d(Size.X - 2, Size.Y - 2));
        CoinProbability = coinProbability;
        DoorProbability = doorProbability;
    }

    public Cell[,] Generate()
    {
        var grid = new Cell[Size.X, Size.Y];
        var visitedPath = new List<Vec2d>();

        for (var y = 0; y < Size.Y; y++)
            for (var x = 0; x < Size.X; x++)
                grid[x, y] = new Wall();

        var rng = new Random();
        GenerateMazeRec(StartPosition, visitedPath);

        for (var y = 0; y < Size.Y; y++)
        {
            for (var x = 0; x < Size.X; x++)
            {
                if (grid[x, y] is Room room && rng.NextDouble() < CoinProbability && room.Content == null)
                {
                    room.Content = new Coin();
                }
            }
        }

        var availableRooms = new List<Vec2d>();
        for (var y = 0; y < Size.Y; y++)
        {
            for (var x = 0; x < Size.X; x++)
            {
                var pos = new Vec2d(x, y);
                if (grid[x, y] is Room && !pos.Equals(StartPosition) && !pos.Equals(ExitPosition))
                {
                    availableRooms.Add(pos);
                }
            }
        }

        var roomCount = availableRooms.Count;
        var doorStartIndex = Math.Max(2, roomCount / 2);

        // First pass: determine how many doors we'll actually place
        var potentialDoors = new List<int>();
        for (var i = doorStartIndex; i < availableRooms.Count; i++)
        {
            if (rng.NextDouble() < DoorProbability && potentialDoors.Count < 5)
            {
                potentialDoors.Add(i);
            }
        }

        for (var doorId = 0; doorId < potentialDoors.Count; doorId++)
        {
            var doorPos = availableRooms[potentialDoors[doorId]];
            grid[doorPos.X, doorPos.Y] = new Door(doorId);
        }

        var keysPlaced = 0;
        var doorsNeeded = potentialDoors.Count;

        if (doorsNeeded > 0)
        {
            var keyRoomPositions = new List<int>();
            for (var i = 0; i < doorStartIndex && keyRoomPositions.Count < doorsNeeded; i++)
            {
                if (grid[availableRooms[i].X, availableRooms[i].Y] is Room room && room.Content == null)
                {
                    keyRoomPositions.Add(i);
                }
            }

            foreach (var roomIndex in keyRoomPositions)
            {
                var keyPos = availableRooms[roomIndex];
                if (grid[keyPos.X, keyPos.Y] is Room room && room.Content == null)
                {
                    room.Content = new Key(keysPlaced);
                    keysPlaced++;
                }
            }
        }

        grid[StartPosition.X, StartPosition.Y] = new Start();
        grid[ExitPosition.X, ExitPosition.Y] = new Exit();

        return grid;

        void GenerateMazeRec(Vec2d currentPosition, List<Vec2d> path)
        {
            path.Add(currentPosition);
            grid[currentPosition.X, currentPosition.Y] = new Room();
            foreach (var directionIndex in Orders[rng.Next(Orders.Length)])
            {
                var delta = Directions[directionIndex];
                var nextPosition = currentPosition + (delta * 2);
                if (IsInterior(nextPosition) &&
                    grid[nextPosition.X, nextPosition.Y] is Wall)
                {
                    var between = currentPosition.Midpoint(nextPosition);
                    grid[between.X, between.Y] = new Room();
                    GenerateMazeRec(nextPosition, path);
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