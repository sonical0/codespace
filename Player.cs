sealed class Player
{
    private readonly Maze _maze;
    private readonly Vec2d _mazeOffset;
    private readonly ConsoleColor _playerColor;
    private readonly ConsoleColor _wallColor;
    private readonly ConsoleColor _corridorColor;
    private readonly ConsoleColor _exitColor;
    private readonly ConsoleColor _startColor;

    public Vec2d Position { get; private set; }

    public Player(
        Maze maze,
        Vec2d mazeOffset,
        ConsoleColor playerColor,
        ConsoleColor wallColor,
        ConsoleColor corridorColor,
        ConsoleColor exitColor,
        ConsoleColor startColor)
    {
        _maze = maze;
        _mazeOffset = mazeOffset;
        _playerColor = playerColor;
        _wallColor = wallColor;
        _corridorColor = corridorColor;
        _exitColor = exitColor;
        _startColor = startColor;

        Position = maze.StartPosition;
    }

    public void Draw(ConsoleScreen screen) =>
        screen.DrawText(_mazeOffset + Position, ("@", _playerColor));

    public bool TryMove(ConsoleScreen screen, Vec2d moveDelta)
    {
        var nextPosition = Position + moveDelta;
        if (!_maze.IsWalkable(nextPosition))
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