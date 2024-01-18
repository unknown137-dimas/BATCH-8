using System.Text;
using Spectre.Console;
using Spectre.Console.Rendering;

internal class Program
{
	static void FigletTitle(string text)
	{
		AnsiConsole.Clear();
		AnsiConsole.Write(
			new FigletText(FigletFont.Load("../../../AutoChess/Assets/doom.flf"), text)
			.LeftJustified()
			.Color(Color.Red)
		);
	}

	static Columns DisplayHeroStats(IEnumerable<string> heroList, Dictionary<string, HeroDetails> heroDb, int barWidth = 35)
	{
		List<Panel> heroStat = new();
		foreach(var hero in heroList)
		{
			var heroPanel = new Panel(
				new BarChart()
				.Width(barWidth)
				.AddItem("HP", heroDb[hero].Hp, Color.Green)
				.AddItem("ATK", heroDb[hero].Attack, Color.Red3)
				.AddItem("Armor", heroDb[hero].Armor, Color.Blue)
				.AddItem("ATK Range", heroDb[hero].AttackRange, Color.Red1)
			).Header(new PanelHeader(hero));
			heroPanel.Padding = new Padding(0, 0, 0, 0);
			heroStat.Add(heroPanel);
		}
		return new Columns(heroStat);
	}

	static void Main()
	{
		// GAME CONFIGURATION
		Console.OutputEncoding = Encoding.UTF8;
		const int boardSize = 8;
		int roll = 3;

		// HERO ICONS
		Dictionary<PieceTypes, string> heroIcons = new()
		{
			{PieceTypes.Knight, "🛡️"},
			{PieceTypes.Warlock, "🌕"},
			{PieceTypes.Warrior, "⚔️"},
		};

		// BOARD
		var panel = new Panel(
			new Markup(heroIcons[PieceTypes.Knight])
		);
		panel.Width = 4;
		panel.Height = 3;
		panel.Padding = new Padding(0,0,0,0);
		var column = new Columns(panel, panel, panel);
		column.Expand = false;
		column.Padding = new Padding(0,0,0,0);
		var board = new Rows(column, column, column);
		
		
		// MAIN MENU
		#region MAIN_MENU
		FigletTitle("AutoChess");
		AnsiConsole.Write(board);
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
		#endregion

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
			// 2. Fix BarChart
			// 3. Display picked hero
			#region PICK_HERO_MENU
			autoChess.AddHero("Poisonous Worm", new HeroDetails(PieceTypes.Warlock, 600, 55, 0, 3));
			autoChess.AddHero("Hell Knight", new HeroDetails(PieceTypes.Knight, 700, 75, 5, 1));
			autoChess.AddHero("God of Thunder", new HeroDetails(PieceTypes.Mage, 950, 60, 0, 3));
			autoChess.AddHero("Swordman", new HeroDetails(PieceTypes.Warrior, 600, 67.5, 5, 2));
			autoChess.AddHero("Egersis Ranger", new HeroDetails(PieceTypes.Hunter, 450, 45, 5, 5));
			autoChess.AddHero("Shadowcrawler", new HeroDetails(PieceTypes.Assassin, 550, 85, 5, 2));
			autoChess.AddHero("Storm Shaman", new HeroDetails(PieceTypes.Shaman, 800, 47.5, 5, 4));
			autoChess.AddHero("Warpwood Sage", new HeroDetails(PieceTypes.Druid, 650, 75, 5, 2));
			autoChess.AddHero("Fallen Witcher", new HeroDetails(PieceTypes.Witcher, 750, 70, 5, 1));
			autoChess.AddHero("Heaven Bomber ", new HeroDetails(PieceTypes.Mech, 500, 45, 5, 4));
			autoChess.AddHero("Goddess of Light", new HeroDetails(PieceTypes.Priest, 400, 52.5, 0, 4));
			autoChess.AddHero("Grand Herald", new HeroDetails(PieceTypes.Wizard, 600, 55, 0, 4));
			var heroesDatabase = autoChess.HeroesDatabase;

			while(!autoChess.IsFinishedPickAllPieces(player) && roll > 0)
			{
				FigletTitle("Pick Your Heroes");
				autoChess.CurrentGamePhase = Phases.ChoosingPieace;
				AnsiConsole.Write(new Rule("[red]Picked Heroes[/]"));
				AnsiConsole.Write(DisplayHeroStats(autoChess.GetPlayerPiecesName(player), heroesDatabase));
				AnsiConsole.Write(new Rule("[red]Hero Options[/]"));
				var optionsList = autoChess.GenerateRandomHeroList();
				AnsiConsole.Write(DisplayHeroStats(optionsList, heroesDatabase));
				var options = AnsiConsole.Prompt(
					new MultiSelectionPrompt<string>()
					.NotRequired()
					.PageSize(5)
					.AddChoices(optionsList)
					.InstructionsText(
						$"[grey](Press [blue]<space>[/] to select hero, [green]<enter>[/] to accept and re-roll)[/]"
						)
				);
				// Set player pieces
				autoChess.AddPlayerPiece(player, options);
				roll--;
			}

			// BOT 
			// Bot pick pieces
			autoChess.AddPlayerPiece(bot, autoChess.GenerateRandomHeroList());
			#endregion
			
			// SET HERO POSITION MENU
			#region SET_HERO_POSITION_MENU
			autoChess.CurrentGamePhase = Phases.PlaceThePiece;
			// Loop until all player's piece on the board
			while(!autoChess.IsFinishedPutAllPieces(player))
			{
				FigletTitle("Place Your Heroes");
				AnsiConsole.Write(new Rule("[red]Player Hero's Position[/]"));
				var playerPieces = (List<IPiece>)autoChess.GetPlayerPieces(player);
				// TODO
				// 1. Change preview layout using board
				foreach(var piece in playerPieces)
				{
					var piecePosition = autoChess.GetHeroPosition(player, piece.PieceId);
					if(piecePosition is not null)
					{
						AnsiConsole.WriteLine($"{piece} | X:{piecePosition.X} Y:{piecePosition.Y}");
					}
				}
				AnsiConsole.Write(new Rule("[red]Set Hero's Position[/]"));
				var playerPiece = AnsiConsole.Prompt(
					new SelectionPrompt<Hero>()
					.PageSize(5)
					.AddChoices(
						playerPieces.ConvertAll(piece => (Hero)piece)
					)
				);
				// Loop until piece placement success
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
					success = autoChess.PutPlayerPiece(player, playerPiece, new Position(pieceX, pieceY));
					if(!success)
					{
						AnsiConsole.Markup("[red]You can't put another hero in the same coordinate[/]\n");
					}
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
					success = autoChess.PutPlayerPiece(bot, piece, new Position(x, y));
				}
			}
			#endregion

			// PREVIEW MENU
			// TODO
			// 1. Change preview layout using board
			#region PREVIEW_MENU
			FigletTitle("Preview");
			// Display player piece's position
			AnsiConsole.Write(new Rule("[red]Player Hero's Position[/]"));
			foreach(var piece in autoChess.GetPlayerData(player).PlayerPieces)
			{
				IPosition? piecePosition = autoChess.GetHeroPosition(player, piece.PieceId);
				if(piecePosition is not null)
				{
					AnsiConsole.WriteLine($"{piece} | X:{piecePosition.X} Y:{piecePosition.Y}");
				}
			}
			
			// Display BOT piece's position
			AnsiConsole.Write(new Rule("[red]Bot Hero's Position[/]"));
			foreach(var piece in autoChess.GetPlayerData(bot).PlayerPieces)
			{
				IPosition? piecePosition = autoChess.GetHeroPosition(player, piece.PieceId);
				if(piecePosition is not null)
				{
					AnsiConsole.WriteLine($"{piece} | X:{piecePosition.X} Y:{piecePosition.Y}");
				}
			}
			#endregion

			// BATTLE VIEW
			// #region BATTLE_VIEW
			// FigletTitle("Battle");
			// int round = 1;
			// AnsiConsole.Write(new Rule($"[red]Round {round}[/]"));
			// // TODO
			// // 1. Move player's piece around the board
			// // 2. Scan for other player's piece
			// // 3. Attack other player pieces
			// // 4. Repeat until 1 player left
			// // 5. Display round winner
			// // 6. Repeat for all round
			// #endregion
		}
	}
}