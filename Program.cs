#region Constants
const int width = 50;
const int height = 20;

const int offsetY = 3;
const int offsetX = 0;

const int marginYMessage = 3;
const int messageHeight = 5;

const string sHeader = "🏃 LABYRINTHE ASCII  C# 🏃";
const string sWin = """
🎉  FÉLICITATIONS !  🎉
Vous avez trouvé la sortie !
""";
const string sCanceled = "\n  Partie abandonnée. À bientôt !";
const string sPressKey = "  Appuyez sur une key pour quitter...";

const ConsoleColor SuccessColor     = ConsoleColor.Green;
const ConsoleColor DangerColor      = ConsoleColor.Red;
const ConsoleColor InfoColor        = ConsoleColor.Cyan;
const ConsoleColor InstructionColor = ConsoleColor.DarkCyan;
const ConsoleColor WallColor        = ConsoleColor.DarkGray;
const ConsoleColor CorridorColor    = ConsoleColor.DarkBlue;
const ConsoleColor PlayerColor      = ConsoleColor.Yellow;
const ConsoleColor ExitColor        = ConsoleColor.Green;
#endregion 

var mazeSize = new Vec2d(width, height);
var grid = new CellType[mazeSize.X, mazeSize.Y];
var screen = new ConsoleScreen();
var keyboardController = new KeyboardController();

var playerPosition = Vec2d.Zero;
var mode = State.Playing;

GenerateMaze(grid, playerPosition);
DrawScreen();

while (mode == State.Playing)
{
    var input = keyboardController.ReadInput();
    if (input.IsCanceled)
    {
        mode = State.Canceled;
        continue;
    }

    if (input.MoveDelta == Vec2d.Zero)
    {
        continue;
    }

    var nextPosition = playerPosition + input.MoveDelta;

    if (nextPosition.IsInBounds(mazeSize) && grid[nextPosition.X, nextPosition.Y] != CellType.Wall)
    {
        if (grid[nextPosition.X, nextPosition.Y] == CellType.Exit) mode = State.Won;

        UpdateCell(playerPosition, CellType.Corridor);
        UpdateCell(playerPosition = nextPosition, CellType.Player);
    }
}

if (mode == State.Won)
{
    screen.DrawBoxedText(new Vec2d(0, offsetY + height + marginYMessage), sWin, SuccessColor);
}
else
{
    screen.DrawText(new Vec2d(0, offsetY + height + marginYMessage), sCanceled, DangerColor);
}

screen.DrawText(new Vec2d(0, offsetY + height + marginYMessage + messageHeight), sPressKey);
Console.CursorVisible = true;
Console.ReadKey(true);

#region Functions

void DrawCell(Vec2d cellPosition) => DrawTextColorXY(
    offsetX + cellPosition.X, 
    offsetY + cellPosition.Y,
    grid[cellPosition.X, cellPosition.Y] switch
    {
        CellType.Wall   => ("█", WallColor),
        CellType.Player => ("@", PlayerColor),
        CellType.Exit   => ("★", ExitColor),
        _               => ("·", CorridorColor)
    });

void DrawTextColorXY(int x, int y, (string text, ConsoleColor color) info) =>
    screen.DrawText(new Vec2d(x, y), info);

void UpdateCell(Vec2d cellPosition, CellType type)
{
    grid[cellPosition.X, cellPosition.Y] = type;
    DrawCell(cellPosition);
}

void DrawScreen()
{
    Console.Clear();
    Console.CursorVisible = false;

    screen.DrawBoxedText(new Vec2d(0, 0), sHeader, InfoColor);
    for (var y = 0; y < height; y++)
    {
        for (var x = 0; x < width; x++)
        {
            DrawCell(new Vec2d(x, y));
        }
    }
    screen.DrawText(new Vec2d(0, offsetY + height), keyboardController.InstructionsText, InstructionColor);
}

void GenerateMaze(CellType[,] grid, Vec2d playerStartPosition)
{
    for (var y = 0; y < height; y++)
        for (var x = 0; x < width; x++)
            grid[x, y] = CellType.Wall;

    Vec2d[] directions = [
        new Vec2d(0, -1),
        new Vec2d(1, 0),
        new Vec2d(0, 1),
        new Vec2d(-1, 0)
    ];
    int[][] orders = [
        [ 0, 1, 2, 3 ], [ 0, 1, 3, 2 ], [ 0, 2, 1, 3 ], [ 0, 2, 3, 1 ], [ 0, 3, 1, 2 ], [ 0, 3, 2, 1 ],
        [ 1, 0, 2, 3 ], [ 1, 0, 3, 2 ], [ 1, 2, 0, 3 ], [ 1, 2, 3, 0 ], [ 1, 3, 0, 2 ], [ 1, 3, 2, 0 ],
        [ 2, 0, 1, 3 ], [ 2, 0, 3, 1 ], [ 2, 1, 0, 3 ], [ 2, 1, 3, 0 ], [ 2, 3, 0, 1 ], [ 2, 3, 1, 0 ],
        [ 3, 0, 1, 2 ], [ 3, 0, 2, 1 ], [ 3, 1, 0, 2 ], [ 3, 1, 2, 0 ], [ 3, 2, 0, 1 ], [ 3, 2, 1, 0 ]
    ];
    var rng = new Random();

    GenerateMazeRec(playerStartPosition);

    var outputPosition = new Vec2d((mazeSize.X - 1) & ~1, (mazeSize.Y - 1) & ~1);

    grid[playerStartPosition.X, playerStartPosition.Y] = CellType.Player;
    grid[outputPosition.X, outputPosition.Y] = CellType.Exit;
    
    void GenerateMazeRec(Vec2d currentPosition)
    {
        grid[currentPosition.X, currentPosition.Y] = CellType.Corridor;
        foreach (var dir in orders[rng.Next(orders.Length)])
        {
            if (InMaze(currentPosition, directions[dir], out var nextPosition) &&
                grid[nextPosition.X, nextPosition.Y] == CellType.Wall)
            {
                var between = currentPosition.Midpoint(nextPosition);
                grid[between.X, between.Y] = CellType.Corridor;
                GenerateMazeRec(nextPosition);
            }
        }

        bool InMaze(Vec2d position, Vec2d delta, out Vec2d next) =>
            (next = position + (delta * 2)).IsInBounds(mazeSize);
    }
}
#endregion
