using System.Text;
using System.Threading;
using Spectre.Console;
using Spectre.Console.Rendering;

internal class Program
{
	// GAME CONFIGURATION
	const int BoardSize = 4;
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
	static GameController autoChess = new GameController(new Board(BoardSize), BoardSize);
	static Player? player1 = null;
	static Player? player2 = null;

	static void FigletTitle(string text)
	{
		AnsiConsole.Clear();
		AnsiConsole.Write(
			new FigletText(FigletFont.Load("../../../AutoChess/Assets/doom.flf"), text)
			.Centered()
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
			).Header(new PanelHeader(hero).Centered());
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
				string? playerSide = null;
				if(board.TryGetValue(new Position(x, y), out string? heroId))
				{
					var hero = autoChess.GetPieceById(heroId);
					if(hero != null)
					{
						playerSide = autoChess.GetPlayerSide(autoChess.GetPlayerByPieceId(heroId)).ToString();
						heroIcons.TryGetValue(hero.PieceType, out icons);
					}
				}
				var label = "";
				if(icons != null && playerSide!= null)
				{
					label = $"[underline {playerSide}]{icons}[/]";
				}
				columnsList.Add(
					new Panel(
						new Markup(label)
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
				}
			);
		}
		return Align.Center(new Rows(rowsList));
	}

	static void PickHero(IPlayer player)
	{
		while(!autoChess.IsFinishedPickAllPieces(player) && Roll > 0)
		{
			FigletTitle("Pick Your Heroes");
			autoChess.CurrentGamePhase = Phases.ChoosingPieace;
			AnsiConsole.Write(new Rule($"[{autoChess.GetPlayerData(player).PlayerSide}]{player.Name}'s Heroes [[{autoChess.GetPlayerPieces(player).Count()}/{autoChess.PlayerPiecesCount}]][/]"));
			AnsiConsole.Write(DisplayHeroStats(autoChess.GetPlayerPiecesName(player)));
			AnsiConsole.Write(new Rule($"[blue]Hero Options | Roll Chance [[{Roll}]][/]"));
			var optionsList = ((List<string>)autoChess.GenerateRandomHeroList())[0..BoardSize];
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
		bool confirm = false;
		while(!autoChess.IsFinishedPutAllPieces(player) || !confirm)
		{
			FigletTitle("Place Your Heroes");
			AnsiConsole.Write(new Rule($"[{autoChess.GetPlayerData(player).PlayerSide}]{player.Name} Hero's Position[/]"));
			AnsiConsole.Write(DisplayBoard(player));
			AnsiConsole.Write(new Rule($"[{autoChess.GetPlayerData(player).PlayerSide}]{player.Name}'s Heroes [[{autoChess.GetPlayerPieces(player).Count()}/{autoChess.PlayerPiecesCount}]][/]"));
			AnsiConsole.Write(DisplayHeroStats(autoChess.GetPlayerPiecesName(player)));
			AnsiConsole.Write(new Rule("[blue]Set Hero's Position[/]"));
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
				var heroPosition = autoChess.GetHeroPosition(player, playerPiece.PieceId);
				var heroX = "?";
				var heroY = "?";
				if(heroPosition != null)
				{
					heroX = (heroPosition.X + 1).ToString();
					heroY = (heroPosition.Y + 1).ToString();
				}
				AnsiConsole.Write(new Markup($"Current Position : X = {heroX}, Y = {heroY}\n"));
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
			if(autoChess.IsFinishedPutAllPieces(player))
			{
				FigletTitle($"{player}'s Preview");
				AnsiConsole.Write(new Rule($"[{autoChess.GetPlayerData(player).PlayerSide}]{player.Name} Hero's Position[/]"));
				AnsiConsole.Write(DisplayBoard(player));
				confirm = AnsiConsole.Confirm("Are you sure about your heroes placement?");
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
			#region GAME_MODE_MENU
			FigletTitle("Game Mode");
			var gameModeMenu = AnsiConsole.Prompt(
				new SelectionPrompt<string>()
				.AddChoices(
					[
						 "[[😎]] Player vs [[😎]] Player",
						 "[[😎]] Player vs [[🤖]] Bot"
					]
				)
			);
			#endregion
			
			// INPUT PLAYER NAME MENU
			#region PLAYER_MENU
			FigletTitle("Enter Your Name");
			player1 = new Player(AnsiConsole.Ask<string>("[[PLAYER 1]] What's your [green]name[/]?"));
			var sidesOptions = (List<Sides>)autoChess.GetGameSides();
			var player1Side = AnsiConsole.Prompt(
				new SelectionPrompt<Sides>()
				.Title("Choose your [green]side[/]?")
				.AddChoices(
					sidesOptions
				)
			);
			player2 = new Player(gameModeMenu.Contains("Bot") ? "BOT" : AnsiConsole.Ask<string>("[[PLAYER 2]] What's your [green]name[/]?"));
			
			// ADD PLAYER
			autoChess.AddPlayer(player1, player1Side);
			sidesOptions.Remove(player1Side);
			autoChess.AddPlayer(player2, sidesOptions[0]);
			#endregion
			
			// PICK HERO MENU
			// TODO
			// 1. How to edit/remove item from user (if user want to change item that already picked)
			// 2. Fix BarChart
			// 3. Display hero type when choosing a hero
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
			// TODO
			// 1. How to cancel setting position for the selected piece
			// 2. Display coordinate label to the board
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

			// BATTLE VIEW
			// TODO
			// 1. Repeat for all round
			// 2. How to handle multiple round
			#region BATTLE_VIEW
			int round = 1;
			while(true)
			{
				FigletTitle("Battle");
				AnsiConsole.Write(new Rule($"[red]Round {round}[/]"));
				AnsiConsole.Write(DisplayBoard());
				foreach(var player in autoChess.GetPlayers())
				{
					foreach(var piece in autoChess.GetPlayerPieces(player))
					{
						AnsiConsole.Write(new Markup($"[{autoChess.GetPlayerData(player).PlayerSide}]{piece.Name} | {piece.Hp}[/]\n"));
					};
				};
				
				if(autoChess.GetPlayerPieces(player1).Count() == 0)
				{
					autoChess.SetRoundWinner(player2);
					autoChess.GetPlayerData(player2).Winner = true;
					break;
				}
				else if(autoChess.GetPlayerPieces(player2).Count() == 0)
				{
					autoChess.SetRoundWinner(player1);
					autoChess.GetPlayerData(player1).Winner = true;
					break;
				}

				foreach(var player in autoChess.GetPlayers())
				{
					foreach(var piece in autoChess.GetPlayerPieces(player))
					{
						Task.Run(() => autoChess.Attack(player, piece));
					};
				};
				Thread.Sleep(1000);
			}
			#endregion

			// ROUND RESULT
			#region ROUND_RESULT
			FigletTitle($"Round {round} Result");
			foreach(var player in autoChess.GetPlayers())
			{
				var playerData = autoChess.GetPlayerData(player);
				var winIcon = "";
				if(playerData.Winner)
				{
					winIcon = "🏆";
				}
				AnsiConsole.Write(new Rule($"[{autoChess.GetPlayerData(player).PlayerSide}][[{winIcon}]] {player.Name}[/]\n"));
				AnsiConsole.Write(new Markup($"[[❤️]] Health Point : {playerData.Hp}\n"));
				AnsiConsole.Write(new Markup($"[[🏆]] Win Point : {playerData.Win}\n"));
			}
			#endregion
		}
	}
}