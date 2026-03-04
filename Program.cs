using System;

// ============================================================
//  LABYRINTHE ASCII - C# Console
//  Tableau : int[50, 20]  (width=50, height=20)
//  0 = couloir   1 = mur   2 = player   3 = exit
//  Déplacement : Z/Q/S/D ou flèches
//  ✅ Optimisé : seules les cellules modifiées sont redessinées
//               via Console.SetCursorPosition()
// ============================================================

const[,] grid = new const[50, 20];

const width = 50;
const height = 20;

// Décalage vertical dans la console (nombre de lignes du titre)
const offsetY = 3;
const offsetX = 0;

// ── Génération du labyrinthe par « recursive backtracker » ──
var cellW = width / 2;   // 25
var cellH = height / 2;   // 10

for (var y = 0; y < height; y++)
    for (var x = 0; x < width; x++)
        grid[x, y] = 1;

var stackX = new var[cellW * cellH];
var stackY = new var[cellW * cellH];
const stackTop = 0;

var[,] visited = new var[cellW, cellH]; // var si possible

const[] dx = { 0, 1, 0, -1 };
const[] dy = { -1, 0, 1, 0 };

Random rng = new Random();

const startCX = 0, startCY = 0; // var si il a y a une case sur 00
visited[startCX, startCY] = true;
grid[startCX * 2, startCY * 2] = 0;

stackX[stackTop] = startCX;
stackY[stackTop] = startCY;
stackTop++;

while (stackTop > 0)
{
    var cx = stackX[stackTop - 1];
    var cy = stackY[stackTop - 1];

    const[] ordre = { 0, 1, 2, 3 };
    for (var i = 3; i > 0; i--)
    {
        var j = rng.Next(i + 1);
        var tmp = ordre[i]; ordre[i] = ordre[j]; ordre[j] = tmp;
    }

    bool found = false; //const si le bool ne change pas
    for (var d = 0; d < 4; d++)
    {
        var nx = cx + dx[ordre[d]];
        var ny = cy + dy[ordre[d]];
        if (nx >= 0 && nx < cellW && ny >= 0 && ny < cellH && !visited[nx, ny])
        {
            grid[cx * 2 + dx[ordre[d]], cy * 2 + dy[ordre[d]]] = 0;
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

// ── Position player et exit ──
var playerX = 0;
var playerY = 0;
var exitX = (cellW - 1) * 2;
var exitY = (cellH - 1) * 2;

grid[playerX, playerY] = 2;
grid[exitX, exitY] = 3;

// ── Dessin initial complet (une seule fois) ──
Console.Clear();
Console.CursorVisible = false;

Console.SetCursorPosition(0, 0);
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("╔══════════════════════════════════════════════════╗");
Console.WriteLine("║          🏃 LABYRINTHE ASCII  C#  🏃             ║");
Console.WriteLine("╚══════════════════════════════════════════════════╝");
Console.ResetColor();

for (var y = 0; y < height; y++)
{
    for (var x = 0; x < width; x++)
    {
        Console.SetCursorPosition(offsetX + x, offsetY + y);
        var cell = grid[x, y];
        if (cell == 1)      { Console.ForegroundColor = ConsoleColor.DarkGray;  Console.Write("█"); }
        else if (cell == 2) { Console.ForegroundColor = ConsoleColor.Yellow;    Console.Write("@"); }
        else if (cell == 3) { Console.ForegroundColor = ConsoleColor.Green;     Console.Write("★"); }
        else                { Console.ForegroundColor = ConsoleColor.DarkBlue;  Console.Write("·"); }
    }
}

Console.SetCursorPosition(0, offsetY + height + 1);
Console.ForegroundColor = ConsoleColor.DarkCyan;
Console.Write("  [Z/↑] Haut   [S/↓] Bas   [Q/←] Gauche   [D/→] Droite   [Échap] Quitter");
Console.ResetColor();

// ── Action locale : redessiner UNE seule cellule via SetCursorPosition ──
void DessinerCellule(var cx, var cy)
{
    Console.SetCursorPosition(offsetX + cx, offsetY + cy);
    var cell = grid[cx, cy];
    if (cell == 1)      { Console.ForegroundColor = ConsoleColor.DarkGray;  Console.Write("█"); } // wall
    else if (cell == 2) { Console.ForegroundColor = ConsoleColor.Yellow;    Console.Write("@"); } // player
    else if (cell == 3) { Console.ForegroundColor = ConsoleColor.Green;     Console.Write("★"); }
    else                { Console.ForegroundColor = ConsoleColor.DarkBlue;  Console.Write(" "); }
    Console.ResetColor();
}

// ── Boucle de jeu ──
bool gagné = false;

while (!gagné)
{
    ConsoleKey touche = Console.ReadKey(true).Key;

    var nx2 = playerX;
    var ny2 = playerY;

    if      (touche == ConsoleKey.Z || touche == ConsoleKey.UpArrow)    ny2--;
    else if (touche == ConsoleKey.S || touche == ConsoleKey.DownArrow)  ny2++;
    else if (touche == ConsoleKey.Q || touche == ConsoleKey.LeftArrow)  nx2--;
    else if (touche == ConsoleKey.D || touche == ConsoleKey.RightArrow) nx2++;
    else if (touche == ConsoleKey.Escape) break;

    if (nx2 >= 0 && nx2 < width && ny2 >= 0 && ny2 < height && grid[nx2, ny2] != 1)
    {
        if (grid[nx2, ny2] == 3) gagné = true;

        // ✅ Efface l'ancienne position (couloir) → 1 seule case redessinée
        grid[playerX, playerY] = 0;
        DessinerCellule(playerX, playerY);

        // ✅ Dessine la nouvelle position → 1 seule case redessinée
        playerX = nx2;
        playerY = ny2;
        grid[playerX, playerY] = 2;
        DessinerCellule(playerX, playerY);
    }
}

// ── Écran de victoire ──
Console.SetCursorPosition(0, offsetY + height + 3);
if (gagné)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("  ╔════════════════════════════════╗");
    Console.WriteLine("  ║   🎉  FÉLICITATIONS !  🎉      ║");
    Console.WriteLine("  ║   Vous avez trouvé la exit ! ║");
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
