using System;

// ============================================================
//  ASCII MAZE - C# Console
//  Array : int[50, 20]  (width=50, height=20)
//  0 = corridor   1 = wall   2 = player   3 = exit
//  Movement : Z/Q/S/D or arrow keys
//  ✅ Optimized: only modified cells are redrawn
//               via Console.SetCursorPosition()
// ============================================================

int[,] grid = new int[50, 20];
int width = 50;
int height = 20;

// Vertical offset in the console (number of title lines)
int offsetY = 3;
int offsetX = 0;

// ── Maze generation using "recursive backtracker" ──
int cellW = width / 2;   // 25
int cellH = height / 2;  // 10

for (int y = 0; y < height; y++)
    for (int x = 0; x < width; x++)
        grid[x, y] = 1;

int[] stackX = new int[cellW * cellH];
int[] stackY = new int[cellW * cellH];
int stackTop = 0;

bool[,] visited = new bool[cellW, cellH];

int[] dx = { 0, 1, 0, -1 };
int[] dy = { -1, 0, 1, 0 };

Random rng = new Random();

int startCX = 0, startCY = 0;
visited[startCX, startCY] = true;
grid[startCX * 2, startCY * 2] = 0;

stackX[stackTop] = startCX;
stackY[stackTop] = startCY;
stackTop++;

while (stackTop > 0)
{
    int cx = stackX[stackTop - 1];
    int cy = stackY[stackTop - 1];

    int[] order = { 0, 1, 2, 3 };
    for (int i = 3; i > 0; i--)
    {
        int j = rng.Next(i + 1);
        int tmp = order[i]; order[i] = order[j]; order[j] = tmp;
    }

    bool found = false;
    for (int d = 0; d < 4; d++)
    {
        int nx = cx + dx[order[d]];
        int ny = cy + dy[order[d]];
        if (nx >= 0 && nx < cellW && ny >= 0 && ny < cellH && !visited[nx, ny])
        {
            grid[cx * 2 + dx[order[d]], cy * 2 + dy[order[d]]] = 0;
            grid[nx * 2, ny * 2] = 0;
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

// ── Player position and exit ──
int playerX = 0, playerY = 0;
int exitX = (cellW - 1) * 2;
int exitY = (cellH - 1) * 2;

grid[playerX, playerY] = 2;
grid[exitX, exitY] = 3;

// ── Full initial drawing (once only) ──
Console.Clear();
Console.CursorVisible = false;

Console.SetCursorPosition(0, 0);
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("╔══════════════════════════════════════════════════╗");
Console.WriteLine("║          🏃 LABYRINTHE ASCII  C#  🏃             ║");
Console.WriteLine("╚══════════════════════════════════════════════════╝");
Console.ResetColor();

for (int y = 0; y < height; y++)
{
    for (int x = 0; x < width; x++)
    {
        Console.SetCursorPosition(offsetX + x, offsetY + y);
        int cell = grid[x, y];
        if (cell == 1) { Console.ForegroundColor = ConsoleColor.DarkGray; Console.Write("█"); }
        else if (cell == 2) { Console.ForegroundColor = ConsoleColor.Yellow; Console.Write("@"); }
        else if (cell == 3) { Console.ForegroundColor = ConsoleColor.Green; Console.Write("★"); }
        else { Console.ForegroundColor = ConsoleColor.DarkBlue; Console.Write("·"); }
    }
}

Console.SetCursorPosition(0, offsetY + height + 1);
Console.ForegroundColor = ConsoleColor.DarkCyan;
Console.Write("  [Z/↑] Haut   [S/↓] Bas   [Q/←] Gauche   [D/→] Droite   [Échap] Quitter");
Console.ResetColor();

// ── Local action: redraw ONE single cell via SetCursorPosition ──
void DrawCell(int cx, int cy)
{
    Console.SetCursorPosition(offsetX + cx, offsetY + cy);
    int cell = grid[cx, cy];
    if (cell == 1) { Console.ForegroundColor = ConsoleColor.DarkGray; Console.Write("█"); }  // wall
    else if (cell == 2) { Console.ForegroundColor = ConsoleColor.Yellow; Console.Write("@"); } // player
    else if (cell == 3) { Console.ForegroundColor = ConsoleColor.Green; Console.Write("★"); }  // exit
    else { Console.ForegroundColor = ConsoleColor.DarkBlue; Console.Write(" "); }              // empty cell
    Console.ResetColor();
}

// ── Game loop ──
bool won = false;

while (!won)
{
    ConsoleKey key = Console.ReadKey(true).Key;

    int nx2 = playerX;
    int ny2 = playerY;

    if (key == ConsoleKey.Z || key == ConsoleKey.UpArrow) ny2--;
    else if (key == ConsoleKey.S || key == ConsoleKey.DownArrow) ny2++;
    else if (key == ConsoleKey.Q || key == ConsoleKey.LeftArrow) nx2--;
    else if (key == ConsoleKey.D || key == ConsoleKey.RightArrow) nx2++;
    else if (key == ConsoleKey.Escape) break;

    if (nx2 >= 0 && nx2 < width && ny2 >= 0 && ny2 < height && grid[nx2, ny2] != 1)
    {
        if (grid[nx2, ny2] == 3) won = true;

        // ✅ Erase old position (corridor) → only 1 cell redrawn
        grid[playerX, playerY] = 0;
        DrawCell(playerX, playerY);

        // ✅ Draw new position → only 1 cell redrawn
        playerX = nx2;
        playerY = ny2;
        grid[playerX, playerY] = 2;
        DrawCell(playerX, playerY);
    }
}

// ── Victory screen ──
Console.SetCursorPosition(0, offsetY + height + 3);
if (won)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("  ╔════════════════════════════════╗");
    Console.WriteLine("  ║   🎉  FÉLICITATIONS !  🎉      ║");
    Console.WriteLine("  ║   Vous avez trouvé la sortie ! ║");
    Console.WriteLine("  ╚════════════════════════════════╝");
    Console.ResetColor();
}
else
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("\n  Partie abandonnée. À bientôt !");
    Console.ResetColor();
}

Console.SetCursorPosition(0, offsetY + height + 8);
Console.WriteLine("  Appuyez sur une touche pour quitter...");
Console.CursorVisible = true;
Console.ReadKey(true);
