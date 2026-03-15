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
const ConsoleColor StartColor       = ConsoleColor.Cyan;
#endregion 

var mazeSize = new Vec2d(width, height);
var mazeOffset = new Vec2d(offsetX, offsetY);
var screen = new ConsoleScreen();
var keyboardController = new KeyboardController();
var mazeGen = new MazeGen(mazeSize);
var maze = new Maze(mazeGen);
var player = new Player(maze, mazeOffset, PlayerColor, WallColor, CorridorColor, ExitColor, StartColor);

var mode = State.Playing;

DrawScreen();
player.Draw(screen);

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

    if (player.TryMove(screen, input.MoveDelta))
    {
        mode = State.Won;
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

void DrawScreen()
{
    Console.Clear();
    Console.CursorVisible = false;

    screen.DrawBoxedText(new Vec2d(0, 0), sHeader, InfoColor);
    maze.Draw(screen, mazeOffset, WallColor, CorridorColor, ExitColor, StartColor);
    screen.DrawText(new Vec2d(0, offsetY + height), keyboardController.InstructionsText, InstructionColor);
}
#endregion
