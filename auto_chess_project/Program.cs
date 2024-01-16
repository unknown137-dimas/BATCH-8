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
        const int boardSize = 8;
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
                var optionsList = autoChess.GenerateRandomHeroList(in heroesOptions);
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
            // Bot pick piece
            foreach(var botPick in autoChess.GenerateRandomHeroList(in heroesOptions))
            {
                autoChess.AddPlayerPiece(bot, new Hero(botPick, heroesDatabase[botPick]));
            }
            
            // SET HERO POSITION MENU
            autoChess.CurrentGamePhase = Phases.PlaceThePiece;
            while(!autoChess.IsFinishedPutAllPieces(player))
            {
                FigletTitle(font, "Place Your Heroes");
                AnsiConsole.Write(new Rule("[red]Player Hero's Position[/]"));
                var playerPieces = autoChess.GetPlayerData(player).PlayerPieces;
                foreach(var piece in playerPieces)
                {
                    AnsiConsole.WriteLine($"{piece} | X:{piece.HeroPosition.X} Y:{piece.HeroPosition.Y} | ID:{piece.PieceId}");
                }
                var playerPiece = AnsiConsole.Prompt(
                    new SelectionPrompt<Hero>()
                    .PageSize(5)
                    .AddChoices(
                        playerPieces
                    )
                );
                bool success = false;
                while(!success)
                {
                    var pieceX = AnsiConsole.Prompt(
                        new TextPrompt<int>($"{playerPiece}'s [green]X[/] position?")
                        .PromptStyle("green")
                        .ValidationErrorMessage("[red]That's not a valid coordinate[/]")
                        .Validate(coordinate =>
                            {
                                return coordinate switch
                                {
                                    < 0 => ValidationResult.Error("[red]The coordinate must be positive[/]"),
                                    >= boardSize => ValidationResult.Error($"[red]The coordinate can't exceed the player's area ({boardSize})[/]"),
                                    _ => ValidationResult.Success(),
                                };
                            }
                        )
                    );

                    var pieceY = AnsiConsole.Prompt(
                        new TextPrompt<int>($"{playerPiece}'s [green]Y[/] position?")
                        .PromptStyle("green")
                        .ValidationErrorMessage("[red]That's not a valid coordinate[/]")
                        .Validate(coordinate =>
                            {
                                return coordinate switch
                                {
                                    < 0 => ValidationResult.Error("[red]The coordinate must be positive[/]"),
                                    >= boardSize / 2 => ValidationResult.Error($"[red]The coordinate can't exceed the player's area ({boardSize / 2})[/]"),
                                    _ => ValidationResult.Success(),
                                };
                            }
                        )
                    );
                    success = autoChess.PutPlayerPiece(playerPiece, new Position(pieceX, pieceY));
                }
            }

            // BOT
            // Bot put piece
            foreach(var piece in autoChess.GetPlayerData(bot).PlayerPieces)
            {
                bool success = false;
                while(!success)
                {
                    int x = new Random().Next(0, boardSize);
                    int y = new Random().Next(4, boardSize);
                    success = autoChess.PutPlayerPiece(piece, new Position(x, y));
                }
            }

            // Display BOT piece's position
            AnsiConsole.Write(new Rule("[red]Bot Hero's Position[/]"));
            foreach(var piece in autoChess.GetPlayerData(bot).PlayerPieces)
            {
                AnsiConsole.WriteLine($"{piece} | X:{piece.HeroPosition.X} Y:{piece.HeroPosition.Y} | ID:{piece.PieceId}");
            }

            // // Display the result
            // var rule = new Rule("[red]Hero Picked[/]");
            // AnsiConsole.Write(rule);
            // foreach(var pick in playerHeroes)
            // {
            //     AnsiConsole.WriteLine(pick.ToString());
            // }
        }
    }
}