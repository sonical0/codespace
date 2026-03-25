interface IMazeGenerator
{
    Vec2d Size { get; }
    Vec2d StartPosition { get; }
    Vec2d ExitPosition { get; }

    Cell[,] Generate();
}
