using System;

// LABYRINTHE ASCII - C# Console

const int WIDTH = 50;
const int HEIGHT = 20;
const int OFFSET_X = 0;
const int OFFSET_Y = 3;
const int CELL_WIDTH = WIDTH / 2;
const int CELL_HEIGHT = HEIGHT / 2;

const ConsoleColor COLOR_WALL = ConsoleColor.DarkGray;
const ConsoleColor COLOR_PLAYER = ConsoleColor.Yellow;
const ConsoleColor COLOR_EXIT = ConsoleColor.Green;
const ConsoleColor COLOR_CORRIDOR = ConsoleColor.DarkBlue;
const ConsoleColor COLOR_TITLE = ConsoleColor.Cyan;
const ConsoleColor COLOR_INSTRUCTIONS = ConsoleColor.DarkCyan;
const ConsoleColor COLOR_WIN = ConsoleColor.Green;
const ConsoleColor COLOR_QUIT = ConsoleColor.Red;

const string MSG_TITLE = """
    ╔══════════════════════════════════════════════════╗
    ║          🏃 LABYRINTHE ASCII  C#  🏃             ║
    ╚══════════════════════════════════════════════════╝
    """;

const string MSG_INSTRUCTIONS = "  [Z/^] Haut   [S/v] Bas   [Q/<] Gauche   [D/>] Droite   [Echap] Quitter";

const string MSG_WIN = """
      ╔════════════════════════════════╗
      ║   🎉  FÉLICITATIONS !  🎉      ║
      ║   Vous avez trouvé la sortie ! ║
      ╚════════════════════════════════╝
    """;

const string MSG_QUIT = "\n  Partie abandonnée. À bientôt !";
const string MSG_EXIT = "  Appuyez sur une key pour quitter...";

var grid = new CellType[WIDTH, HEIGHT];

GenerateMaze(grid, out var playerX, out var playerY, out var exitX, out var exitY);
DrawInitialScreen(grid);

void DrawCell(int cx, int cy)
{
    Console.SetCursorPosition(OFFSET_X + cx, OFFSET_Y + cy);
    var cell = grid[cx, cy];
    var (color, pattern) = cell switch
    {
        CellType.Wall => (COLOR_WALL, "█"),
        CellType.Player => (COLOR_PLAYER, "@"),
        CellType.Exit => (COLOR_EXIT, "★"),
        CellType.Corridor => (COLOR_CORRIDOR, " "),
        _ => (COLOR_CORRIDOR, " ")
    };
    Console.ForegroundColor = color;
    Console.Write(pattern);
    Console.ResetColor();
}

var won = false;

while (!won)
{
    ConsoleKey key = Console.ReadKey(true).Key;

    var nx2 = playerX;
    var ny2 = playerY;

    switch (key)
    {
        case ConsoleKey.Z or ConsoleKey.UpArrow:
            ny2--;
            break;
        case ConsoleKey.S or ConsoleKey.DownArrow:
            ny2++;
            break;
        case ConsoleKey.Q or ConsoleKey.LeftArrow:
            nx2--;
            break;
        case ConsoleKey.D or ConsoleKey.RightArrow:
            nx2++;
            break;
        case ConsoleKey.Escape:
            won = false;
            break;
    }

    if (key == ConsoleKey.Escape) break;

    if (nx2 >= 0 && nx2 < WIDTH && ny2 >= 0 && ny2 < HEIGHT && grid[nx2, ny2] != CellType.Wall)
    {
        if (grid[nx2, ny2] == CellType.Exit) won = true;

        grid[playerX, playerY] = CellType.Corridor;
        DrawCell(playerX, playerY);

        playerX = nx2;
        playerY = ny2;
        grid[playerX, playerY] = CellType.Player;
        DrawCell(playerX, playerY);
    }
}

if (won)
{
    DrawTextXY(0, OFFSET_Y + HEIGHT + 3, MSG_WIN, COLOR_WIN);
}
else
{
    DrawTextXY(0, OFFSET_Y + HEIGHT + 3, MSG_QUIT, COLOR_QUIT);
}

DrawTextXY(0, OFFSET_Y + HEIGHT + 8, MSG_EXIT);
Console.CursorVisible = true;
Console.ReadKey(true);

void DrawTextXY(int x, int y, string text, ConsoleColor? color = null)
{
    Console.SetCursorPosition(x, y);
    if (color != null)
    {
        Console.ForegroundColor = color.Value;
    }
    Console.Write(text);
    Console.ResetColor();
}

void GenerateMaze(CellType[,] grid, out int playerX, out int playerY, out int exitX, out int exitY)
{
    // Initialize grid with walls
    for (var y = 0; y < HEIGHT; y++)
        for (var x = 0; x < WIDTH; x++)
            grid[x, y] = CellType.Wall;

    var stackX = new int[CELL_WIDTH * CELL_HEIGHT];
    var stackY = new int[CELL_WIDTH * CELL_HEIGHT];
    var stackTop = 0;

    var visited = new bool[CELL_WIDTH, CELL_HEIGHT];

    var dx = new[] { 0, 1, 0, -1 };
    var dy = new[] { -1, 0, 1, 0 };

    var rng = new Random();

    var startCX = 0; var startCY = 0;
    visited[startCX, startCY] = true;
    grid[startCX * 2, startCY * 2] = CellType.Corridor;

    stackX[stackTop] = startCX;
    stackY[stackTop] = startCY;
    stackTop++;

    while (stackTop > 0)
    {
        var cx = stackX[stackTop - 1];
        var cy = stackY[stackTop - 1];

        var directions = new[] { 0, 1, 2, 3 };
        rng.Shuffle(directions);

        var found = false;
        foreach (var dir in directions)
        {
            var nx = cx + dx[dir];
            var ny = cy + dy[dir];
            if (nx >= 0 && nx < CELL_WIDTH && ny >= 0 && ny < CELL_HEIGHT && !visited[nx, ny])
            {
                grid[cx * 2 + dx[dir], cy * 2 + dy[dir]] = CellType.Corridor;
                grid[nx * 2, ny * 2] = CellType.Corridor;
                visited[nx, ny] = true;
                stackX[stackTop] = nx;
                stackY[stackTop] = ny;
                stackTop++;
                found = true;
                break;
            }
        }
        if (!found) stackTop--;
    }

    // Set player and exit positions
    playerX = 0; 
    playerY = 0;
    exitX = (CELL_WIDTH - 1) * 2;
    exitY = (CELL_HEIGHT - 1) * 2;

    grid[playerX, playerY] = CellType.Player;
    grid[exitX, exitY] = CellType.Exit;
}

void DrawInitialScreen(CellType[,] grid)
{
    Console.Clear();
    Console.CursorVisible = false;

    DrawTextXY(0, 0, MSG_TITLE, COLOR_TITLE);

    for (var y = 0; y < HEIGHT; y++)
        for (var x = 0; x < WIDTH; x++)
            DrawCell(x, y);

    DrawTextXY(0, OFFSET_Y + HEIGHT + 1, MSG_INSTRUCTIONS, COLOR_INSTRUCTIONS);
}

enum CellType
{
    Corridor = 0,
    Wall = 1,
    Player = 2,
    Exit = 3
}
