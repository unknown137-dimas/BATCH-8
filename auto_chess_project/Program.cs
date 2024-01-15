using Spectre.Console;
using Spectre.Console.Rendering;
class Program
{
    static void FigletTitle(FigletFont font, string text)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(
            new FigletText(font, text)
            .LeftJustified()
            .Color(Color.Red)
        );
    }

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
        int roll = 3;
        var font = FigletFont.Load("../../../defaultFont.flf");
        
        // MAIN MENU
        // TODO
        // 1. Layout
        FigletTitle(font, "AutoChess");
        var mainMenu = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
            .PageSize(5)
            .AddChoices(
                [
                    "Start",
                    "Exit"
                ]
            )
        );

        if(mainMenu == "Start")
        {
            // PLAYER NAME INPUT MENU
            var playerName = AnsiConsole.Ask<string>("What's your [green]name[/]?");

            // GAME CONTROLLER INIT
            var autoChess = new GameController(new Board(boardSize));
            var player = new Player(playerName);
            var bot = new Player("BOT");
            autoChess.AddPlayer(player);
            autoChess.AddPlayer(bot);
            
            // PICK HERO MENU
            // TODO
            // 1. How to edit/remove item from user (if user want to change item that already playerPicked)
            // 2. Pick menu layout (Live display?)
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
            var playerPicked = new List<Hero>();

            while(playerPicked.Count < 5 && roll > 0)
            {
                FigletTitle(font, "Pick Your Heroes");
                var optionsList = autoChess.GenerateRandomHeroList(in heroesList);
                var pickHeroList = new List<IRenderable>();
                int barWidth = 25;
                // var pickHeroLayout = new Layout("Root")
                //     .SplitColumns(
                //         new Layout("1"),
                //         new Layout("2"),
                //         new Layout("3"),
                //         new Layout("4"),
                //         new Layout("5")
                //     );
                foreach(var hero in optionsList)
                {
                    var heroPanel = new Panel(
                        new BarChart()
                        .Width(barWidth)
                        .AddItem("HP", hero.Hp, Color.Green)
                        .AddItem("ATK", hero.Attack, Color.Red3)
                        .AddItem("Armor", hero.Armor, Color.Blue)
                        .AddItem("ATK Range", hero.AttackRange, Color.Red1)
                    ).Header(new PanelHeader(hero.ToString()));
                    heroPanel.Padding = new Padding(0, 0, 0, 0);
                    pickHeroList.Add(heroPanel);
                }
                AnsiConsole.Write(new Columns(pickHeroList));
                var options = AnsiConsole.Prompt(
                    new MultiSelectionPrompt<Hero>()
                    .NotRequired()
                    .PageSize(5)
                    .AddChoices(optionsList)
                    .InstructionsText(
                        $"[grey](Press [blue]<space>[/] to select hero, [green]<enter>[/] to accept and re-roll)[/]"
                        )
                );
                foreach(var pick in options)
                {
                    playerPicked.Add(pick);
                }
                roll--;
            }
            // Keep first 5 item playerPicked by user
            if(playerPicked.Count > 5)
            {
                playerPicked = playerPicked[0..5];
            }

            // Set player piece
            autoChess.AddPlayerPiece(player, playerPicked);
            autoChess.AddPlayerPiece(bot, autoChess.GenerateRandomHeroList(in heroesList));
            

            // SET HERO POSITION MENU
            FigletTitle(font, "Place Your Heroes");



            // Display the result
            var rule = new Rule("[red]Hero Picked[/]");
            AnsiConsole.Write(rule);
            foreach(var pick in playerPicked)
            {
                AnsiConsole.WriteLine(pick.ToString());
            }
        }
    }
}