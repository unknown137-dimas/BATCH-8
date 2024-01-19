using System.Text;
using Spectre.Console;
using Spectre.Console.Rendering;

internal class Program
{
	// GAME CONFIGURATION
	const int BoardSize = 6;
	static int Roll {get; set;} = 3;

	// HERO ICONS
	static Dictionary<PieceTypes, string> heroIcons = new()
	{
		{PieceTypes.Warlock, "📕"},
		{PieceTypes.Knight, "🛡️"},
		{PieceTypes.Mage, "🪄"},
		{PieceTypes.Warrior, "⚔️"},
		{PieceTypes.Hunter, "🏹"},
		{PieceTypes.Assassin, "🗡️"},
		{PieceTypes.Shaman, "👻"},
		{PieceTypes.Druid, "🍃"},
		{PieceTypes.Witcher, "🐺"},
		{PieceTypes.Mech, "⚙️"},
		{PieceTypes.Priest, "✝️"},
		{PieceTypes.Wizard, "🔮"},
	};

	// GAME CONTROLLER INIT
	static GameController autoChess = new GameController(new Board(BoardSize));
	static Player? player1 = null;
	static Player? player2 = null;

	static void FigletTitle(string text)
	{
		AnsiConsole.Clear();
		AnsiConsole.Write(
			new FigletText(FigletFont.Load("../../../AutoChess/Assets/doom.flf"), text)
			.LeftJustified()
			.Color(Color.Red)
		);
	}

	static IRenderable DisplayHeroStats(IEnumerable<string> heroList, int barWidth = 35)
	{
		List<IRenderable> heroStat = new();
		var heroDb = autoChess.HeroesDatabase;
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
	
	static IRenderable DisplayBoard(IPlayer? player = null)
	{
		 // RENDER BOARD
		int[] boardSize = autoChess.GetBoardSize();
		var board = player != null ? autoChess.GetPlayerBoard(player) : autoChess.GetAllHeroPosition();
		List<IRenderable> rowsList = new();
		for(int y = boardSize[1] - 1; y >= 0 ; y--)
		{
			List<IRenderable> columnsList = new();
			for(int x = 0; x < boardSize[0]; x++)
			{
				string? icons = null;
				if(board.TryGetValue(new Position(x, y), out string? heroId))
				{
					var hero = autoChess.GetPieceById(heroId);
					if(hero != null)
					{
						heroIcons.TryGetValue(hero.PieceType, out icons);
					}
				}
				columnsList.Add(
					new Panel(
						new Markup(icons ?? "")
					)
					{
						Width = 5,
						Height = 3,
						Padding = new Padding(0, 0, 0, 0)
					}
				);
			}
			rowsList.Add(
				new Columns(columnsList)
				{
					Expand = false,
					Padding = new Padding(0, 0, 0, 0)
				}
			);
		}
		return new Rows(rowsList);
	}

	static void PickHero(IPlayer player)
	{
		while(!autoChess.IsFinishedPickAllPieces(player) && Roll > 0)
		{
			FigletTitle("Pick Your Heroes");
			autoChess.CurrentGamePhase = Phases.ChoosingPieace;
			AnsiConsole.Write(new Rule($"[green]{player.Name}'s Heroes[/]"));
			AnsiConsole.Write(DisplayHeroStats(autoChess.GetPlayerPiecesName(player)));
			AnsiConsole.Write(new Rule("[blue]Hero Options[/]"));
			var optionsList = autoChess.GenerateRandomHeroList();
			AnsiConsole.Write(DisplayHeroStats(optionsList));
			var options = AnsiConsole.Prompt(
				new MultiSelectionPrompt<string>()
				.NotRequired()
				.PageSize(5)
				.AddChoices(optionsList)
				.InstructionsText(
					$"[grey](Press [blue]<space>[/] to select hero, [green]<enter>[/] to accept and re-Roll)[/]"
					)
			);
			// Set player pieces
			autoChess.AddPlayerPiece(player, options);
			Roll--;
		}
	}

	static void SetHeroPosition(IPlayer player)
	{
		// Loop until all player's piece on the board
		while(!autoChess.IsFinishedPutAllPieces(player))
		{
			FigletTitle("Place Your Heroes");
			AnsiConsole.Write(new Rule($"[green]{player.Name} Hero's Position[/]"));
			AnsiConsole.Write(DisplayBoard(player));
			AnsiConsole.Write(new Rule("[red]Set Hero's Position[/]"));
			var playerPieces = (List<IPiece>)autoChess.GetPlayerPieces(player);
			var playerPiece = AnsiConsole.Prompt(
				new SelectionPrompt<Hero>()
				.PageSize(5)
				.AddChoices(
					playerPieces.ConvertAll(piece => (Hero)piece)
				)
			);
			// Loop until piece placement success
			bool success = false;
			bool isSecondPlayer = ((List<IPlayer>)autoChess.GetPlayers()).IndexOf(player) == 1;
			int yMinCoor = isSecondPlayer ? (BoardSize % 2 == 0 ? BoardSize / 2 : (BoardSize / 2) + 1) + 1 : 1;
			int yMaxCoor = isSecondPlayer ? BoardSize : BoardSize / 2;
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
								< 1 => ValidationResult.Error($"[red]The coordinate range from 1 to {BoardSize}[/]"),
								>= BoardSize + 1 => ValidationResult.Error($"[red]The coordinate can't exceed the player's area ({BoardSize})[/]"),
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
								var value when value < yMinCoor => ValidationResult.Error($"[red]The coordinate range from {yMinCoor} to {yMaxCoor}[/]"),
								var value when value > yMaxCoor => ValidationResult.Error($"[red]The coordinate can't exceed the player's area ({yMaxCoor})[/]"),
								_ => ValidationResult.Success(),
							};
						}
					)
				);
				success = autoChess.PutPlayerPiece(player, playerPiece, new Position(pieceX - 1, pieceY - 1));
				if(!success)
				{
					AnsiConsole.Markup("[red]You can't put another hero in the same coordinate[/]\n");
				}
			}
		}
	}

	static void Main()
	{
		// Enable emoji support
		Console.OutputEncoding = Encoding.UTF8;

		// Add hero to hero database
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
		
		
		// MAIN MENU
		#region MAIN_MENU
		FigletTitle("AutoChess");
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
			// GAME MODE MENU
			var gameModeMenu = AnsiConsole.Prompt(
				new SelectionPrompt<string>()
				.AddChoices(
					[
						 "[[😎]] Player vs [[😎]] Player",
						 "[[😎]] Player vs [[🤖]] Bot"
					]
				)
			);
			
			// INPUT PLAYER NAME MENU
			player1 = new Player(AnsiConsole.Ask<string>("[[PLAYER 1]] What's your [green]name[/]?"));
			player2 = new Player(gameModeMenu.Contains("Bot") ? "BOT" : AnsiConsole.Ask<string>("[[PLAYER 2]] What's your [green]name[/]?"));
			
			// ADD PLAYER
			autoChess.AddPlayer(player1);
			autoChess.AddPlayer(player2);
			
			// PICK HERO MENU
			// TODO
			// 1. How to edit/remove item from user (if user want to change item that already picked)
			// 2. Fix BarChart
			// 3. Display hero type when choosing a hero
			// 4. Display how many roll left
			#region PICK_HERO_MENU
			if(!gameModeMenu.Contains("Bot"))
			{
				foreach(var player in autoChess.GetPlayers())
				{
					PickHero(player);
				}
			}
			else
			{
				PickHero(player1);
				
				// BOT 
				// Bot pick pieces
				autoChess.AddPlayerPiece(player2, autoChess.GenerateRandomHeroList());
			}
			#endregion
			
			// SET HERO POSITION MENU
			// 1. How to cancel setting position for the selected piece
			// 2. Ask confirmation to player for all position before going battle, if no, repeat to set hero position menu
			// 3. Add coordinate label to the board
			// 4. Add coordinate info to piece selection
			#region SET_HERO_POSITION_MENU
			autoChess.CurrentGamePhase = Phases.PlaceThePiece;
			if(!gameModeMenu.Contains("Bot"))
			{
				foreach(var player in autoChess.GetPlayers())
				{
					SetHeroPosition(player);
				}
			}
			else
			{
				SetHeroPosition(player1);
				
				// BOT
				// Bot put piece
				foreach(var piece in autoChess.GetPlayerData(player2).PlayerPieces)
				{
					bool success = false;
					while(!success)
					{
						int x = new Random().Next(0, BoardSize);
						int y = new Random().Next((BoardSize % 2 == 0 ? BoardSize / 2 : (BoardSize / 2) + 1), BoardSize);
						success = autoChess.PutPlayerPiece(player2, piece, new Position(x, y));
					}
				}
			}
			#endregion

			// PREVIEW MENU
			#region PREVIEW_MENU
			FigletTitle("Preview");
			AnsiConsole.Write(DisplayBoard());
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
			// // 8. How to handle multiple round
			// #endregion
		}
	}
}