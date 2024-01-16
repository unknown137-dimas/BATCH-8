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
            // 1. How to edit/remove item from user (if user want to change item that already playerHeroes)
            // 2. Pick menu layout (Live display?)
            Dictionary<string, HeroDetails> heroesDatabase = new()
            {
                {"Hell Knight", new HeroDetails(PieceTypes.Knight, 700, 75, 5, 1)},
                {"Poisonous Worm", new HeroDetails(PieceTypes.Warlock, 600, 55, 0, 3)},
                {"God of Thunder", new HeroDetails(PieceTypes.Mage, 950, 60, 0, 3)},
                {"Swordman", new HeroDetails(PieceTypes.Warrior, 600, 67.5, 5, 2)},
                {"Egersis Ranger", new HeroDetails(PieceTypes.Hunter, 450, 45, 5, 5)},
                {"Shadowcrawler", new HeroDetails(PieceTypes.Assassin, 550, 85, 5, 2)},
                {"Storm Shaman", new HeroDetails(PieceTypes.Shaman, 800, 47.5, 5, 4)},
                {"Warpwood Sage", new HeroDetails(PieceTypes.Druid, 650, 75, 5, 2)},
                {"Fallen Witcher", new HeroDetails(PieceTypes.Witcher, 750, 70, 5, 1)},
                {"Heaven Bomber ", new HeroDetails(PieceTypes.Mech, 500, 45, 5, 4)},
                {"Goddess of Light", new HeroDetails(PieceTypes.Priest, 400, 52.5, 0, 4)},
                {"Grand Herald", new HeroDetails(PieceTypes.Wizard, 600, 55, 0, 4)},
            };
            var heroesOptions = new List<string>();
            foreach(var key in heroesDatabase.Keys)
            {
                heroesOptions.Add(key.ToString());
            }
            List<Hero> playerHeroes = (List<Hero>)autoChess.GetPlayerPieces(player);
            var heroOptionsStat = new List<IRenderable>();

            while(playerHeroes.Count < 5 && roll > 0)
            {
                FigletTitle(font, "Pick Your Heroes");
                autoChess.CurrentGamePhase = Phases.ChoosingPieace;
                var optionsList = autoChess.GenerateRandomHeroList<string>(in heroesOptions);
                heroOptionsStat.Clear();
                int barWidth = 25;
                foreach(var hero in optionsList)
                {
                    var heroPanel = new Panel(
                        new BarChart()
                        .Width(barWidth)
                        .AddItem("HP", heroesDatabase[hero].Hp, Color.Green)
                        .AddItem("ATK", heroesDatabase[hero].Attack, Color.Red3)
                        .AddItem("Armor", heroesDatabase[hero].Armor, Color.Blue)
                        .AddItem("ATK Range", heroesDatabase[hero].AttackRange, Color.Red1)
                    ).Header(new PanelHeader(hero));
                    heroPanel.Padding = new Padding(0, 0, 0, 0);
                    heroOptionsStat.Add(heroPanel);
                }
                AnsiConsole.Write(new Columns(heroOptionsStat));
                var options = AnsiConsole.Prompt(
                    new MultiSelectionPrompt<string>()
                    .NotRequired()
                    .PageSize(5)
                    .AddChoices(optionsList)
                    .InstructionsText(
                        $"[grey](Press [blue]<space>[/] to select hero, [green]<enter>[/] to accept and re-roll)[/]"
                        )
                );
                foreach(var pick in options)
                {
                    // Set player piece
                    autoChess.AddPlayerPiece(player, new Hero(pick, heroesDatabase[pick]));
                }
                roll--;
            }
            // Keep first 5 item playerHeroes by user
            if(playerHeroes.Count > 5)
            {
                playerHeroes = playerHeroes[0..5];
            }

            // BOT 
            // Set bot piece
            foreach(var botPick in autoChess.GenerateRandomHeroList(in heroesOptions))
            {
                autoChess.AddPlayerPiece(bot, new Hero(botPick, heroesDatabase[botPick]));
            }
            
            // SET HERO POSITION MENU
            FigletTitle(font, "Place Your Heroes");
            autoChess.CurrentGamePhase = Phases.PlaceThePiece;
            var playerPieces = autoChess.GetPlayerData(player).PlayerPieces;

            foreach(var piece in playerPieces)
            {
                piece.Move(new Random().Next(0, boardSize), new Random().Next(0, boardSize));
            }

            foreach(var piece in playerPieces)
            {
                AnsiConsole.WriteLine($"{piece} | X:{piece.X} Y:{piece.Y} | ID:{piece.PieceId}");
            }

            // var piece = AnsiConsole.Prompt(
            //     new SelectionPrompt<string>()
            //     .PageSize(5)
            //     .AddChoices(
            //         playerPieces
            //     )
            // );


            // Display the result
            var rule = new Rule("[red]Hero Picked[/]");
            AnsiConsole.Write(rule);
            foreach(var pick in playerHeroes)
            {
                AnsiConsole.WriteLine(pick.ToString());
            }
        }
    }
}