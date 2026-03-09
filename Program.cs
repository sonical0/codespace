#region Constants
const int width = 50;
const int height = 20;

const int offsetY = 3;
const int offsetX = 0;

const int marginYMessage = 3;
const int messageHeight = 5;

const string sHeader = """
    ╔══════════════════════════════════════════════════╗
    ║          🏃 LABYRINTHE ASCII  C#  🏃             ║
    ╚══════════════════════════════════════════════════╝
    """;
const string sInstructions = "  [Z/↑] Haut   [S/↓] Bas   [Q/←] Gauche   [D/→] Droite   [Échap] Quitter";
const string sWin = """
    ╔════════════════════════════════╗
    ║   🎉  FÉLICITATIONS !  🎉      ║
    ║   Vous avez trouvé la sortie ! ║
    ╚════════════════════════════════╝
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

var grid = new CellType[width, height];

var playerX = 0;
var playerY = 0;
var mode = State.Playing;

GenerateMaze(grid, playerX, playerY);
DrawScreen();

while (mode == State.Playing)
{
    var key = Console.ReadKey(true).Key;

    var nx2 = playerX;
    var ny2 = playerY;

    switch (key)
    {
        case ConsoleKey.Z or ConsoleKey.UpArrow:    ny2--; break;
        case ConsoleKey.S or ConsoleKey.DownArrow:  ny2++; break;
        case ConsoleKey.Q or ConsoleKey.LeftArrow:  nx2--; break;
        case ConsoleKey.D or ConsoleKey.RightArrow: nx2++; break;
        case ConsoleKey.Escape: mode = State.Canceled; break;
    }
    if (InBound(nx2, width) && InBound(ny2, height) && grid[nx2, ny2] != CellType.Wall)
    {
        if (grid[nx2, ny2] == CellType.Exit) mode = State.Won;

        UpdateCell(playerX      , playerY      , CellType.Corridor);
        UpdateCell(playerX = nx2, playerY = ny2, CellType.Player  );
    }
}

DrawTextColorXY(0, offsetY + height + marginYMessage,
    mode == State.Won 
    ? (sWin, SuccessColor) 
    : (sCanceled, DangerColor)
);
DrawTextXY(0, offsetY + height + marginYMessage + messageHeight, sPressKey);
Console.CursorVisible = true;
Console.ReadKey(true);

#region Functions

void DrawTextXY(int x, int y, string text, ConsoleColor? color = null)
{
    Console.SetCursorPosition(x, y);
    if(color.HasValue)
    {
        Console.ForegroundColor = color.Value;
    }
    Console.Write(text);
    Console.ResetColor();
}

void DrawTextColorXY(int x, int y, (string text, ConsoleColor color) info) =>
    DrawTextXY(x, y, info.text, info.color);

void DrawCell(int cx, int cy) => DrawTextColorXY(
    offsetX + cx, 
    offsetY + cy,
    grid[cx, cy] switch
    {
        CellType.Wall   => ("█", WallColor),
        CellType.Player => ("@", PlayerColor),
        CellType.Exit   => ("★", ExitColor),
        _               => ("·", CorridorColor)
    });

void UpdateCell(int cx, int cy, CellType type)
{
    grid[cx, cy] = type;
    DrawCell(cx, cy);
}

void DrawScreen()
{
    Console.Clear();
    Console.CursorVisible = false;

    DrawTextXY(0, 0, sHeader, InfoColor);
    for (var y = 0; y < height; y++)
    {
        for (var x = 0; x < width; x++)
        {
            DrawCell(x, y);
        }
    }
    DrawTextXY(0, offsetY + height, sInstructions, InstructionColor);
}

bool InBound(int val, int max) => val >= 0 && val < max;

void GenerateMaze(CellType[,] grid, int playerStartX, int playerStartY)
{
    for (var y = 0; y < height; y++)
        for (var x = 0; x < width; x++)
            grid[x, y] = CellType.Wall;

    int[] dx = [ 0, 1, 0, -1 ];
    int[] dy = [ -1, 0, 1, 0 ];
    int[][] orders = [
        [ 0, 1, 2, 3 ], [ 0, 1, 3, 2 ], [ 0, 2, 1, 3 ], [ 0, 2, 3, 1 ], [ 0, 3, 1, 2 ], [ 0, 3, 2, 1 ],
        [ 1, 0, 2, 3 ], [ 1, 0, 3, 2 ], [ 1, 2, 0, 3 ], [ 1, 2, 3, 0 ], [ 1, 3, 0, 2 ], [ 1, 3, 2, 0 ],
        [ 2, 0, 1, 3 ], [ 2, 0, 3, 1 ], [ 2, 1, 0, 3 ], [ 2, 1, 3, 0 ], [ 2, 3, 0, 1 ], [ 2, 3, 1, 0 ],
        [ 3, 0, 1, 2 ], [ 3, 0, 2, 1 ], [ 3, 1, 0, 2 ], [ 3, 1, 2, 0 ], [ 3, 2, 0, 1 ], [ 3, 2, 1, 0 ]
    ];
    var rng = new Random();

    GenerateMazeRec(playerStartX, playerStartY);

    var outX = (width  - 1) & ~1;
    var outY = (height - 1) & ~1;

    grid[playerStartX, playerStartY] = CellType.Player;
    grid[outX, outY] = CellType.Exit;
    
    void GenerateMazeRec(int x, int y)
    {
        grid[x, y] = CellType.Corridor;
        foreach (var dir in orders[rng.Next(orders.Length)])
        {
            if( InMaze(x, dx[dir], width , out var nx) && 
                InMaze(y, dy[dir], height, out var ny) && 
                grid[nx, ny] == CellType.Wall)
            {
                grid[(x + nx) / 2, (y + ny) / 2] = CellType.Corridor;
                GenerateMazeRec(nx, ny);
            }
        }
        bool InMaze(int val, int delta, int max, out int next) => 
            InBound(next = val + delta * 2, max);
    }
}
#endregion

#region Enums
enum State
{
    Playing,
    Won,
    Canceled
}
enum CellType
{
    Corridor = 0,
    Wall = 1,
    Player = 2,
    Exit = 3
}
#endregion