using System;

// ============================================================
//  LABYRINTHE ASCII - C# Console
//  Tableau : int[50, 20]  (largeur=50, hauteur=20)
//  0 = couloir   1 = mur   2 = joueur   3 = sortie
//  Déplacement : Z/Q/S/D ou flèches
//  ✅ Optimisé : seules les cellules modifiées sont redessinées
//               via Console.SetCursorPosition()
// ============================================================

const int largeur = 50;
const int hauteur = 20;

var grille = new int[largeur, hauteur];

// Décalage vertical dans la console (nombre de lignes du titre)
const int offsetY = 3;
const int offsetX = 0;

// ── Génération du labyrinthe par « recursive backtracker » ──
var cellW = largeur / 2;   // 25
var cellH = hauteur / 2;   // 10

for (var y = 0; y < hauteur; y++)
    for (var x = 0; x < largeur; x++)
        grille[x, y] = 1;

var stackX = new int[cellW * cellH];
var stackY = new int[cellW * cellH];
var stackTop = 0;

var visited = new bool[cellW, cellH];

var dx = new[] { 0, 1, 0, -1 };
var dy = new[] { -1, 0, 1, 0 };

var rng = new Random();

var startCX = 0; var startCY = 0;
visited[startCX, startCY] = true;
grille[startCX * 2, startCY * 2] = 0;

stackX[stackTop] = startCX;
stackY[stackTop] = startCY;
stackTop++;

while (stackTop > 0)
{
    var cx = stackX[stackTop - 1];
    var cy = stackY[stackTop - 1];

    var ordre = new[] { 0, 1, 2, 3 };
    for (var i = 3; i > 0; i--)
    {
        var j = rng.Next(i + 1);
        var tmp = ordre[i]; ordre[i] = ordre[j]; ordre[j] = tmp;
    }

    var found = false;
    for (var d = 0; d < 4; d++)
    {
        var nx = cx + dx[ordre[d]];
        var ny = cy + dy[ordre[d]];
        if (nx >= 0 && nx < cellW && ny >= 0 && ny < cellH && !visited[nx, ny])
        {
            grille[cx * 2 + dx[ordre[d]], cy * 2 + dy[ordre[d]]] = 0;
            grille[nx * 2, ny * 2] = 0;
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

// ── Position joueur et sortie ──
var joueurX = 0; var joueurY = 0;
var sortieX = (cellW - 1) * 2;
var sortieY = (cellH - 1) * 2;

grille[joueurX, joueurY] = 2;
grille[sortieX, sortieY] = 3;

// ── Dessin initial complet (une seule fois) ──
Console.Clear();
Console.CursorVisible = false;

Console.SetCursorPosition(0, 0);
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("╔══════════════════════════════════════════════════╗");
Console.WriteLine("║          🏃 LABYRINTHE ASCII  C#  🏃             ║");
Console.WriteLine("╚══════════════════════════════════════════════════╝");
Console.ResetColor();

for (var y = 0; y < hauteur; y++)
{
    for (var x = 0; x < largeur; x++)
    {
        Console.SetCursorPosition(offsetX + x, offsetY + y);
        var cell = grille[x, y];
        if (cell == 1) { Console.ForegroundColor = ConsoleColor.DarkGray; Console.Write("█"); }
        else if (cell == 2) { Console.ForegroundColor = ConsoleColor.Yellow; Console.Write("@"); }
        else if (cell == 3) { Console.ForegroundColor = ConsoleColor.Green; Console.Write("★"); }
        else { Console.ForegroundColor = ConsoleColor.DarkBlue; Console.Write(" "); }
    }
}

Console.SetCursorPosition(0, offsetY + hauteur + 1);
Console.ForegroundColor = ConsoleColor.DarkCyan;
Console.Write("  [Z/↑] Haut   [S/↓] Bas   [Q/←] Gauche   [D/→] Droite   [Échap] Quitter");
Console.ResetColor();

// ── Action locale : redessiner UNE seule cellule via SetCursorPosition ──
void DessinerCellule(int cx, int cy)
{
    Console.SetCursorPosition(offsetX + cx, offsetY + cy);
    var cell = grille[cx, cy];
    if (cell == 1) { Console.ForegroundColor = ConsoleColor.DarkGray; Console.Write("█"); } // mur
    else if (cell == 2) { Console.ForegroundColor = ConsoleColor.Yellow; Console.Write("@"); } // perso
    else if (cell == 3) { Console.ForegroundColor = ConsoleColor.Green; Console.Write("★"); } // sortie
    else { Console.ForegroundColor = ConsoleColor.DarkBlue; Console.Write(" "); } //cases vide
    Console.ResetColor();
}

// ── Boucle de jeu ──
var gagné = false;

while (!gagné)
{
    ConsoleKey touche = Console.ReadKey(true).Key;

    var nx2 = joueurX;
    var ny2 = joueurY;

    if (touche == ConsoleKey.Z || touche == ConsoleKey.UpArrow) ny2--;
    else if (touche == ConsoleKey.S || touche == ConsoleKey.DownArrow) ny2++;
    else if (touche == ConsoleKey.Q || touche == ConsoleKey.LeftArrow) nx2--;
    else if (touche == ConsoleKey.D || touche == ConsoleKey.RightArrow) nx2++;
    else if (touche == ConsoleKey.Escape) break;

    if (nx2 >= 0 && nx2 < largeur && ny2 >= 0 && ny2 < hauteur && grille[nx2, ny2] != 1)
    {
        if (grille[nx2, ny2] == 3) gagné = true;

        // ✅ Efface l'ancienne position (couloir) → 1 seule case redessinée
        grille[joueurX, joueurY] = 0;
        DessinerCellule(joueurX, joueurY);

        // ✅ Dessine la nouvelle position → 1 seule case redessinée
        joueurX = nx2;
        joueurY = ny2;
        grille[joueurX, joueurY] = 2;
        DessinerCellule(joueurX, joueurY);
    }
}

// ── Écran de victoire ──
Console.SetCursorPosition(0, offsetY + hauteur + 3);
if (gagné)
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

Console.SetCursorPosition(0, offsetY + hauteur + 8);
Console.WriteLine("  Appuyez sur une touche pour quitter...");
Console.CursorVisible = true;
Console.ReadKey(true);
