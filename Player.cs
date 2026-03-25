sealed class Player
{
    private readonly Maze _maze;
    private readonly IController _controller;
    private readonly Vec2d _mazeOffset;
    private readonly ConsoleColor _playerColor;
    private readonly ConsoleColor _wallColor;
    private readonly ConsoleColor _corridorColor;
    private readonly ConsoleColor _exitColor;
    private readonly ConsoleColor _startColor;
    private int _points;
    private readonly List<ICollectable> _inventory = [];

    public Vec2d Position { get; private set; }
    public int Points => _points;
    public IReadOnlyList<ICollectable> Inventory => _inventory;

    public event EventHandler? PointsChanged;
    public event EventHandler? InventoryChanged;

    public Player(
        Maze maze,
        IController controller,
        Vec2d mazeOffset,
        ConsoleColor playerColor,
        ConsoleColor wallColor,
        ConsoleColor corridorColor,
        ConsoleColor exitColor,
        ConsoleColor startColor)
    {
        _maze = maze;
        _controller = controller;
        _mazeOffset = mazeOffset;
        _playerColor = playerColor;
        _wallColor = wallColor;
        _corridorColor = corridorColor;
        _exitColor = exitColor;
        _startColor = startColor;
        _points = 0;

        Position = maze.StartPosition;
    }

    public void Draw(IGridDisplay screen) =>
        screen.DrawText(_mazeOffset + Position, ("@", _playerColor));

    public bool TryPickup()
    {
        if (_maze.GetCell(Position) is Room room && room.Content != null)
        {
            var item = room.Content;
            _points += item.Points;
            PointsChanged?.Invoke(this, EventArgs.Empty);

            if (item.IsPersistent)
            {
                _inventory.Add(item);
                InventoryChanged?.Invoke(this, EventArgs.Empty);
            }

            room.Content = null;
            return true;
        }

        return false;
    }

    public bool TryMove(IGridDisplay screen, Vec2d moveDelta)
    {
        var nextPosition = Position + moveDelta;
        if (!_maze.IsWalkable(nextPosition, _inventory))
        {
            return false;
        }

        _maze.DrawCell(
            screen,
            _mazeOffset,
            Position,
            _wallColor,
            _corridorColor,
            _exitColor,
            _startColor);

        Position = nextPosition;
        Draw(screen);

        return _maze.IsExit(Position);
    }
}