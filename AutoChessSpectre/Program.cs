using System.Runtime.Serialization.Json;
using System.Text;
using Spectre.Console;
using Spectre.Console.Rendering;
using AutoChess;

internal class Program
{
	// GAME CONFIGURATION
	private const int size = 6;
	private const int maxPlayerPieces = 4;
	private const int initPlayerHp = 1;
	private static int maxRoll {get;} = 3;
	private static int maxRound {get;} = 1;

	// HERO ICONS
	private static Dictionary<PieceTypes, string> heroIcons = new()
	{
		{PieceTypes.Warlock, "📕"},
		{PieceTypes.Knight, "🐴"},
		{PieceTypes.Mage, "🪄"},
		{PieceTypes.Warrior, "🛡️"},
		{PieceTypes.Hunter, "🏹"},
		{PieceTypes.Assassin, "🗡️"},
		{PieceTypes.Shaman, "👻"},
		{PieceTypes.Druid, "🍃"},
		{PieceTypes.Witcher, "🐺"},
		{PieceTypes.Mech, "🦾"},
		{PieceTypes.Priest, "📖"},
		{PieceTypes.Wizard, "🔮"},
	};

	// GAME CONTROLLER INIT
	private static GameController autoChess = new GameController(new Board(size), maxPlayerPieces, initPlayerHp);
	private static int[] boardSize = autoChess.GetBoardSize();
	private static Player? playerOne = null;
	private static Player? playerTwo = null;

	private static void FigletTitle(string text)
	{
		AnsiConsole.Clear();
		AnsiConsole.Write(
			new FigletText(FigletFont.Load("Assets/doom.flf"), text)
			.Centered()
			.Color(Color.OrangeRed1)
		);
	}

	private static IRenderable DisplayHeroStats(IEnumerable<string> heroList, int barWidth = 35)
	{
		List<IRenderable> heroStat = new();
		foreach(var hero in heroList)
		{
			if(autoChess.TryGetHeroDetails(hero, out HeroDetails? heroDetail))
			{
				var heroPanel = new Panel(
					new BarChart()
					.Width(barWidth)
					.Label(hero)
					.CenterLabel()
					.AddItem("HP", ScaleHeroStat((int)Math.Round(heroDetail!.Hp), 1200, barWidth), Color.Green1)
					.AddItem("ATK", ScaleHeroStat((int)Math.Round(heroDetail!.Attack), 400, barWidth), Color.Red1)
					.AddItem("ATK Range", ScaleHeroStat(heroDetail!.AttackRange, 30, barWidth), Color.DarkOrange)
					.AddItem("Armor", ScaleHeroStat((int)Math.Round(heroDetail!.Armor), 30, barWidth), Color.Blue1)
					.HideValues()
					.WithMaxValue(barWidth)
				).Header(new PanelHeader($"[[{heroIcons[heroDetail!.HeroType]}]] {heroDetail!.HeroType}").Centered());
				heroPanel.Padding = new Padding(0, 0, 0, 0);
				heroStat.Add(heroPanel);
			}
		}
		return new Columns(heroStat);
	}
	
	private static IRenderable DisplayHeroPosition(IEnumerable<IPiece> playerPieces)
	{
		List<IRenderable> allHeroPosition = new();
		foreach(var piece in playerPieces)
		{
			var heroX = "?";
			var heroY = "?";
			if(autoChess.TryGetPlayerByPieceId(piece.PieceId, out IPlayer? player))
			{
				if(autoChess.TryGetHeroPosition(player!, piece.PieceId, out IPosition? heroPosition))
				{
					heroX = (heroPosition!.X + 1).ToString();
					heroY = (heroPosition!.Y + 1).ToString();
				}
			}
			allHeroPosition.Add(
				new Markup(
					$"X = [{(heroX != "?" ? "green1" : "red1")}]{heroX}[/], Y = [{(heroY != "?" ? "green1" : "red1")}]{heroY}[/]"
				).Centered()
			);
		}
		return new Columns(allHeroPosition);
	}
	
	private static IRenderable DisplayBoard(IPlayer? player = null)
	{
		// RENDER BOARD
		int[] boardSize = autoChess.GetBoardSize();
		var board = player != null ? autoChess.GetPlayerBoard(player) : autoChess.GetAllHeroPosition();
		List<IRenderable> rowsList = new();
		for(int y = boardSize[1] - 1; y >= 0; y--)
		{
			List<IRenderable> columnsList = new();
			for(int x = 0; x < boardSize[0]; x++)
			{
				string? icons = null;
				string? playerSide = null;
				if(board.TryGetValue(new Position(x, y), out Guid heroId))
				{
					if(autoChess.TryGetPieceById(heroId, out IPiece? piece))
					{
						heroIcons.TryGetValue(((Hero)piece!).PieceType, out icons);
					}
					if(autoChess.TryGetPlayerByPieceId(heroId, out IPlayer? playerResult))
					{
						if(autoChess.TryGetPlayerSide(playerResult!, out Sides sideResult))
						{
							playerSide = sideResult.ToString();
						}
					}
				}
				var label = "";
				if(icons != null && playerSide != null)
				{
					label = $"[bold underline {playerSide}]{icons}[/]";
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

	private static void PickHero(IPlayer player)
	{
		int roll = maxRoll;
		while(!autoChess.IsFinishedPickAllPieces(player) && roll >= 0)
		{
			FigletTitle("Pick Your Heroes");
			if(!autoChess.TryGetPlayerSide(player, out Sides playerSideResult))
			{
				Environment.Exit(0);
			}
			AnsiConsole.Write(new Rule($"[{playerSideResult.ToString().ToLower()}]{player.Name}'s Heroes [[{autoChess.GetPlayerPieces(player).Count()}/{autoChess.PlayerPiecesCount}]][/]"));
			AnsiConsole.Write(DisplayHeroStats(autoChess.GetPlayerPiecesName(player)));
			AnsiConsole.Write(new Rule($"[{playerSideResult.ToString().ToLower()}]Hero Options | Roll Chance [[{roll}]][/]"));
			var optionsList = autoChess.GenerateRandomHeroList().ToList();
			if(optionsList == Enumerable.Empty<string>())
			{
				Environment.Exit(0);
			}
			AnsiConsole.Write(DisplayHeroStats(optionsList));
			var options = AnsiConsole.Prompt(
				new MultiSelectionPrompt<string>()
				.NotRequired()
				.PageSize(5)
				.AddChoices(optionsList)
				.InstructionsText(
					$"[grey](Press [yellow1]<space>[/] to select hero, [green1]<enter>[/] to accept and re-Roll)[/]"
					)
				.HighlightStyle(playerSideResult.ToString().ToLower())
			);
			
			// Set player pieces
			AddPieceToPlayer(player, options);
			roll--;
		}
	}

	private static void SetHeroPosition(IPlayer player)
	{
		// Loop until all player's piece on the board
		bool confirm = false;
		if(autoChess.GetPlayerPieces(player).Count() == 0)
		{
			confirm = true;
		}
		while(!autoChess.IsFinishedPutAllPieces(player) || !confirm)
		{
			FigletTitle("Place Your Heroes");
			if(autoChess.TryGetPlayerSide(player, out Sides playerSideResult))
			{
				bool isSecondPlayer = autoChess.GetPlayers().ToList().IndexOf(player) == 1;
				int yMinCoor = isSecondPlayer ? (boardSize[1] % 2 == 0 ? boardSize[1] / 2 : (boardSize[1] / 2) + 1) + 1 : 1;
				int yMaxCoor = isSecondPlayer ? boardSize[1] : boardSize[1] / 2;
				
				AnsiConsole.Write(new Rule($"[{playerSideResult.ToString().ToLower()}]{player.Name} Hero's Position[/]"));
				AnsiConsole.Write(DisplayBoard(player));
				AnsiConsole.MarkupLine($"The valid [green1]X coordinate[/] is between [green1]1[/] and [green1]{boardSize[0]}[/]");
				AnsiConsole.MarkupLine($"The valid [green1]Y coordinate[/] is between [green1]{yMinCoor}[/] and [green1]{yMaxCoor}[/]");
				AnsiConsole.Write(new Rule($"[{playerSideResult.ToString().ToLower()}]{player.Name}'s Heroes [[{autoChess.GetPlayerPieces(player).Count()}/{autoChess.PlayerPiecesCount}]][/]"));
				AnsiConsole.Write(DisplayHeroStats(autoChess.GetPlayerPiecesName(player)));
				AnsiConsole.Write(DisplayHeroPosition(autoChess.GetPlayerPieces(player)));
				AnsiConsole.Write(new Rule($"[{playerSideResult.ToString().ToLower()}]Set Hero's Position[/]"));
				var playerPieces = autoChess.GetPlayerPieces(player).ToList();
				var playerPiece = AnsiConsole.Prompt(
					new SelectionPrompt<Hero>()
					.PageSize(5)
					.AddChoices(
						playerPieces.ConvertAll(piece => (Hero)piece)
					)
					.HighlightStyle(playerSideResult.ToString().ToLower())
				);
				
				// Loop until piece placement success
				bool success = false;
				while(!success)
				{
					var pieceX = AnsiConsole.Prompt(
						new TextPrompt<int>($"{playerPiece}'s [green1]X[/] position?")
						.PromptStyle("green1")
						.ValidationErrorMessage("[red]That's not a valid coordinate[/]")
						.Validate(coordinate =>
							{
								return coordinate switch
								{
									< 1 => ValidationResult.Error($"[red]The coordinate range from 1 to {boardSize[0]}[/]"),
									var value when value >= boardSize[0] + 1 => ValidationResult.Error($"[red]The coordinate can't exceed the player's area ({boardSize[0]})[/]"),
									_ => ValidationResult.Success(),
								};
							}
						)
					);

					var pieceY = AnsiConsole.Prompt(
						new TextPrompt<int>($"{playerPiece}'s [green1]Y[/] position?")
						.PromptStyle("green1")
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
					AnsiConsole.Write(new Rule($"[{playerSideResult.ToString().ToLower()}]{player.Name} Hero's Position[/]"));
					AnsiConsole.Write(DisplayBoard(player));
					confirm = AnsiConsole.Confirm("Are you sure about your heroes placement?");
				}
			}
		}
	}

	private static void DisplayResult()
	{
		foreach(var player in autoChess.GetPlayers())
		{
			if(autoChess.TryGetPlayerData(player, out PlayerData? playerDataResult))
			{
				StringBuilder battleResult = new();
				foreach(var result in playerDataResult!.Win)
				{
					battleResult.Append(result ? "🏆" : "💀");
				}
				StringBuilder healthPoint = new();
				for(int i = 0; i < playerDataResult!.Hp; i++)
				{
					healthPoint.Append("❤️");
				}
				AnsiConsole.Write(new Rule($"[{playerDataResult!.PlayerSide.ToString().ToLower()}][[{(playerDataResult!.Winner ? "🏆" : "💀")}]] {player.Name}[/]\n"));
				AnsiConsole.Write(new Markup($"[[❤️]] Health Point : {healthPoint}\n"));
				AnsiConsole.Write(new Markup($"[[⚔️]] Battle Result : {battleResult}\n"));
			}
		}
	}

	private static int ScaleHeroStat(int originalValue, int originalMax, int scaleMax) => (originalValue * (scaleMax - 1) / originalMax) + 1;

	private static void CreatePlayer(ref List<Sides> sidesOptions, int playerCount)
	{
		for(int i = 1; i <= playerCount; i++)
		{
			var player = new Player(AnsiConsole.Ask<string>($"[[PLAYER {i}]] What's your [green1]name[/]?"));
			var playerSide = AnsiConsole.Prompt(
				new SelectionPrompt<Sides>()
				.Title("Choose your [green1]side[/]?")
				.AddChoices(
					sidesOptions
				)
				.HighlightStyle(Color.Yellow1)
			);
			autoChess.AddPlayer(player, playerSide);
			sidesOptions.Remove(playerSide);
		}
	}
	
	private static void AddPieceToPlayer(IPlayer player, List<string> pieceChoices)
	{
		foreach(var piece in pieceChoices)
		{
			if(autoChess.HeroesDatabase.TryGetValue(piece, out HeroDetails? heroDetail))
			{
				Hero newHero;
				switch(heroDetail.HeroType)
				{
					case PieceTypes.Knight:
						newHero = new Knight(piece, heroDetail);
						break;
					case PieceTypes.Warlock:
						newHero = new Warlock(piece, heroDetail);
						break;
					case PieceTypes.Mage:
						newHero = new Mage(piece, heroDetail);
						break;
					case PieceTypes.Warrior:
						newHero = new Warrior(piece, heroDetail);
						break;
					case PieceTypes.Hunter:
						newHero = new Hunter(piece, heroDetail);
						break;
					case PieceTypes.Assassin:
						newHero = new Assassin(piece, heroDetail);
						break;
					case PieceTypes.Shaman:
						newHero = new Shaman(piece, heroDetail);
						break;
					case PieceTypes.Druid:
						newHero = new Druid(piece, heroDetail);
						break;
					case PieceTypes.Witcher:
						newHero = new Witcher(piece, heroDetail);
						break;
					case PieceTypes.Mech:
						newHero = new Mech(piece, heroDetail);
						break;
					case PieceTypes.Priest:
						newHero = new Priest(piece, heroDetail);
						break;
					case PieceTypes.Wizard:
						newHero = new Wizard(piece, heroDetail);
						break;	
					default:
						newHero = new Hero(piece, heroDetail);
						break;
				}
				autoChess.AddPlayerPiece(player, newHero!);
			}
		}
	}
	
	static void Main()
	{
		// Enable emoji support
		Console.OutputEncoding = Encoding.UTF8;

		// Load hero from json file to hero database
		#region HERO_INIT
		// Read hero_database.json file
		string jsonPath = "Assets/hero_database.json";
		DataContractJsonSerializer jsonSerializer = new(typeof(Dictionary<string, HeroDetails>));
		if(!File.Exists(jsonPath))
		{
			AnsiConsole.Write(new Markup($"[rapidblink red1]Hero database is empty, please add hero first[/]\n").Centered());
			Console.ReadKey();
			Environment.Exit(0);
		}
		using(FileStream fs = new(jsonPath, FileMode.Open))
		{
			var heroes = (Dictionary<string, HeroDetails>?)jsonSerializer.ReadObject(fs);
			if(heroes != null)
			{
				autoChess.AddHero(heroes);
			}
		}
		#endregion
		
		// MAIN MENU
		#region MAIN_MENU
		FigletTitle("AutoChess");
		var mainMenu = AnsiConsole.Prompt(
			new SelectionPrompt<string>()
			.PageSize(5)
			.AddChoices(
				[
					"✅ Start",
					"❌ Exit"
				]
			)
			.HighlightStyle(new Style(Color.Yellow1))
		);
		#endregion

		if(mainMenu.Contains("Start"))
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
				.HighlightStyle(Color.Yellow1)
			);
			#endregion

			// INPUT PLAYER NAME MENU
			#region PLAYER_MENU
			FigletTitle("Enter Your Name");
			var sidesOptions = autoChess.GetSidesOptions().ToList();
			int playerCount = 2;
			if(gameModeMenu.Contains("Bot"))
			{
				playerCount = 1;
				CreatePlayer(ref sidesOptions, playerCount);

				// BOT
				var botSide = sidesOptions[new Random().Next(sidesOptions.Count())];
				autoChess.AddPlayer(new Player("BOT"), botSide);
				sidesOptions.Remove(botSide);
			}
			else
			{
				CreatePlayer(ref sidesOptions, playerCount);
			}
			playerOne = (Player)autoChess.GetPlayers().ToArray()[0];
			playerTwo = (Player)autoChess.GetPlayers().ToArray()[1];
			#endregion

			// LOOP ALL ROUND
			for (int round = 1; round <= maxRound; round++)
			{
				// PICK HERO MENU
				// TODO
				// 1. How to edit/remove item from user (if user want to change item that already picked)
				#region PICK_HERO_MENU
				autoChess.SetGamePhase(Phases.ChoosingPiece);
				if(!gameModeMenu.Contains("Bot"))
				{
					foreach (var player in autoChess.GetPlayers())
					{
						PickHero(player);
					}
				}
				else
				{
					PickHero(playerOne);

					// BOT 
					// Bot pick pieces
					int roll = maxRoll;
					while (!autoChess.IsFinishedPickAllPieces(playerTwo) && roll >= 0)
					{
						var options = autoChess.GenerateRandomHeroList().ToList();
						if(options == Enumerable.Empty<string>())
						{
							Environment.Exit(0);
						}
						AddPieceToPlayer(playerTwo, options[0..new Random().Next(options.Count + 1)]);
						roll--;
					}
				}
				#endregion

				// SET HERO POSITION MENU
				// TODO
				// 1. How to cancel setting position for the selected piece
				// 2. Display coordinate label to the board
				#region SET_HERO_POSITION_MENU
				autoChess.SetGamePhase(Phases.PlaceThePiece);
				if(!gameModeMenu.Contains("Bot"))
				{
					foreach (var player in autoChess.GetPlayers())
					{
						SetHeroPosition(player);
					}
				}
				else
				{
					SetHeroPosition(playerOne);

					// BOT
					// Bot put piece
					if(autoChess.TryGetPlayerData(playerTwo, out PlayerData? playerDataResult))
					{
						foreach (var piece in playerDataResult!.PlayerPieces)
						{
							bool success = false;
							while (!success)
							{
								int x = new Random().Next(0, boardSize[0]);
								int y = new Random().Next(boardSize[1] % 2 == 0 ? boardSize[1] / 2 : (boardSize[1] / 2) + 1, boardSize[1]);
								success = autoChess.PutPlayerPiece(playerTwo, piece, new Position(x, y));
							}
						}
					}
				}
				#endregion

				// BATTLE VIEW
				#region BATTLE_VIEW
				autoChess.SetGameStatus(Status.OnGoing);
				autoChess.SetGamePhase(Phases.BattleBegin);
				while (!autoChess.TryGetRoundWinner(out _, out _))
				{
					FigletTitle("Battle");
					AnsiConsole.Write(new Rule($"[yellow1]Round {round}[/]"));
					AnsiConsole.Write(DisplayBoard());

					// Display each player's hero health
					List<IRenderable> playerColumn = new();
					foreach (var player in autoChess.GetPlayers())
					{
						int barWidth = 45;
						var heroHealthBar = new BarChart().Width(barWidth).WithMaxValue(barWidth).HideValues();
						foreach (var playerPiece in autoChess.GetPlayerPieces(player))
						{
							if(autoChess.TryGetPieceById(playerPiece.PieceId, out IPiece? piece))
							{
								heroHealthBar.AddItem(piece!.Hp > 0 ? piece!.Name : $"[[💀]] {piece!.Name}", ScaleHeroStat((int)Math.Round(piece.Hp), 1200, barWidth), Color.Green1);
							}
						};
						if(autoChess.TryGetPlayerSide(player, out Sides playerSideResult))
						{
							playerColumn.Add(new Panel(heroHealthBar).Header(new PanelHeader($"[{playerSideResult.ToString().ToLower()} bold]{player.Name}[/]").Centered()));
						}
					};
					AnsiConsole.Write(new Rule("Hero Health Statistic"));
					AnsiConsole.Write(new Columns(playerColumn));

					// Create a task for each player's piece
					foreach (var player in autoChess.GetPlayers())
					{
						foreach (var piece in autoChess.GetPlayerPieces(player))
						{
							Task.Run(() => autoChess.Attack(player, piece));
						};
					};
					Thread.Sleep(700);
				}
				#endregion

				// ROUND RESULT
				#region ROUND_RESULT
				autoChess.SetGamePhase(Phases.BattleEnd);
				FigletTitle($"Round {round} Result");
				DisplayResult();
				AnsiConsole.Write(new Markup($"[rapidblink yellow1]Press any key to move to the next round...[/]\n").Centered());
				Console.ReadLine();
				#endregion
			}

			// FINAL RESULT
			#region FINAL_RESULT
			autoChess.SetGameStatus(Status.End);
			autoChess.SetGamePhase(Phases.TheChampion);
			FigletTitle($"Final Result");
			DisplayResult();
			#endregion
		}
	}
}