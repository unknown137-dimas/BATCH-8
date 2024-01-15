using Spectre.Console;
class Program
{
    static void Main()
    {
        // var panel = new Panel("");
        // panel.Width = 3;
        // panel.Height = 2;
        // panel.Padding = new Padding(0,0,0,0);
        // var column = new Columns(panel, panel, panel);
        // column.Expand = false;
        // column.Padding = new Padding(0,0,0,0);
        // var row = new Rows(column, column, column);
        // AnsiConsole.Write(row);

        // GAME CONFIGURATION
        int boardSize = 8;
        
        // MAIN MENU
        // TODO
        // 1. Layout
        var mainMenu = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
            .Title("Welcome to AutoChess")
            .PageSize(5)
            .AddChoices(
                new[] {
                "Start", "Exit"
                }
            )
        );

        // PICK MENU
        // TODO
        // 1. How to edit/remove item from user (if user want to change item that already picked)
        // 2. Pick menu layout (Live display?)
        if(mainMenu == "Start")
        {
            var board = new Board(boardSize);
            var autoChess = new GameController(board);
            List<Hero> heroesList = new() {
                new Hero("Hell Knight", PieceTypes.Knight, 700, 75, 5, 1),
                new Hero("Poisonous Worm", PieceTypes.Warlock, 600, 55, 0, 3),
                new Hero("God of Thunder", PieceTypes.Mage, 950, 60, 0, 3),
                new Hero("Swordman", PieceTypes.Warrior, 600, 67.5, 5, 2),
                new Hero("Egersis Ranger", PieceTypes.Hunter, 450, 45, 5, 5),
                new Hero("Shadowcrawler", PieceTypes.Assassin, 550, 85, 5, 2),
                new Hero("Storm Shaman", PieceTypes.Shaman, 800, 47.5, 5, 4),
                new Hero("Warpwood Sage", PieceTypes.Druid, 650, 75, 5, 2),
                new Hero("Fallen Witcher", PieceTypes.Witcher, 750, 70, 5, 1),
                new Hero("Heaven Bomber ", PieceTypes.Mech, 500, 45, 5, 4),
                new Hero("Goddess of Light", PieceTypes.Priest, 400, 52.5, 0, 4),
                new Hero("Grand Herald", PieceTypes.Wizard, 600, 55, 0, 4),
            };
            List<Hero> optionsList = new();
            var picked = new List<Hero>();
            while(picked.Count < 5)
            {
                autoChess.GenerateRandomPick(in heroesList, ref optionsList);
                var options = AnsiConsole.Prompt(
                    new MultiSelectionPrompt<Hero>()
                    .Title("Pick Number")
                    .NotRequired()
                    .PageSize(5)
                    .AddChoices(optionsList)
                );
                foreach(var pick in options)
                {
                    picked.Add(pick);
                }
            }
            // Keep first 5 item picked by user
            if(picked.Count > 5)
            {
                picked = picked[0..5];
            }
            // Display the result
            foreach(var pick in picked)
            {
                AnsiConsole.WriteLine(pick.ToString());
            }
        }
    }
}