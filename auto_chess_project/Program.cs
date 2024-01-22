﻿using System.Text;
using Spectre.Console;
using Spectre.Console.Rendering;

internal class Program
{
	// GAME CONFIGURATION
	const int Size = 4;
	const int maxPlayerPieces = 4;
	const int initPlayerHp = 3;
	static int maxRoll {get;} = 3;
	static int maxRound {get;} = 3;

	// HERO ICONS
	static Dictionary<PieceTypes, string> heroIcons = new()
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
	static GameController autoChess = new GameController(new Board(Size), maxPlayerPieces, initPlayerHp);
	static int[] boardSize = autoChess.GetBoardSize();
	static Player? player1 = null;
	static Player? player2 = null;

	static void FigletTitle(string text)
	{
		AnsiConsole.Clear();
		AnsiConsole.Write(
			new FigletText(FigletFont.Load("../../../AutoChess/Assets/doom.flf"), text)
			.Centered()
			.Color(Color.OrangeRed1)
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
				.Label(hero)
				.CenterLabel()
				.AddItem("HP", ScaleHeroStat((int)Math.Round(heroDb[hero].Hp), 1200, barWidth), Color.Green1)
				.AddItem("ATK", ScaleHeroStat((int)Math.Round(heroDb[hero].Attack), 400, barWidth), Color.Red1)
				.AddItem("ATK Range", ScaleHeroStat(heroDb[hero].AttackRange, 30, barWidth), Color.DarkOrange)
				.AddItem("Armor", ScaleHeroStat((int)Math.Round(heroDb[hero].Armor), 30, barWidth), Color.Blue1)
				.HideValues()
				.WithMaxValue(barWidth)
			).Header(new PanelHeader($"[[{heroIcons[heroDb[hero].HeroType]}]] {heroDb[hero].HeroType}").Centered());
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
		for(int y = boardSize[1] - 1; y >= 0; y--)
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
						playerSide = autoChess.GetPlayerSide(autoChess.GetPlayerByPieceId(heroId)!).ToString();
						heroIcons.TryGetValue(hero.PieceType, out icons);
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

	static void PickHero(IPlayer player)
	{
		int roll = maxRoll;
		while(!autoChess.IsFinishedPickAllPieces(player) && roll >= 0)
		{
			FigletTitle("Pick Your Heroes");
			AnsiConsole.Write(new Rule($"[{autoChess.GetPlayerData(player).PlayerSide}]{player.Name}'s Heroes [[{autoChess.GetPlayerPieces(player).Count()}/{autoChess.PlayerPiecesCount}]][/]"));
			AnsiConsole.Write(DisplayHeroStats(autoChess.GetPlayerPiecesName(player)));
			AnsiConsole.Write(new Rule($"[{autoChess.GetPlayerData(player).PlayerSide}]Hero Options | Roll Chance [[{roll}]][/]"));
			var optionsList = ((List<string>)autoChess.GenerateRandomHeroList())[0..autoChess.GetBoardSize()[0]];
			AnsiConsole.Write(DisplayHeroStats(optionsList));
			var options = AnsiConsole.Prompt(
				new MultiSelectionPrompt<string>()
				.NotRequired()
				.PageSize(5)
				.AddChoices(optionsList)
				.InstructionsText(
					$"[grey](Press [yellow1]<space>[/] to select hero, [green1]<enter>[/] to accept and re-Roll)[/]"
					)
				.HighlightStyle(autoChess.GetPlayerData(player).PlayerSide.ToString())
			);
			
			// Set player pieces
			autoChess.AddPlayerPiece(player, options);
			roll--;
		}
	}

	static void SetHeroPosition(IPlayer player)
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
			bool isSecondPlayer = ((List<IPlayer>)autoChess.GetPlayers()).IndexOf(player) == 1;
			int yMinCoor = isSecondPlayer ? (boardSize[1] % 2 == 0 ? boardSize[1] / 2 : (boardSize[1] / 2) + 1) + 1 : 1;
			int yMaxCoor = isSecondPlayer ? boardSize[1] : boardSize[1] / 2;
			var heroX = "?";
			var heroY = "?";
			AnsiConsole.Write(new Rule($"[{autoChess.GetPlayerData(player).PlayerSide}]{player.Name} Hero's Position[/]"));
			AnsiConsole.Write(DisplayBoard(player));
			AnsiConsole.MarkupLine($"The valid [green1]X coordinate[/] is between [green1]1[/] and [green1]{boardSize[0]}[/]");
			AnsiConsole.MarkupLine($"The valid [green1]Y coordinate[/] is between [green1]{yMinCoor}[/] and [green1]{yMaxCoor}[/]");
			AnsiConsole.Write(new Rule($"[{autoChess.GetPlayerData(player).PlayerSide}]{player.Name}'s Heroes [[{autoChess.GetPlayerPieces(player).Count()}/{autoChess.PlayerPiecesCount}]][/]"));
			AnsiConsole.Write(DisplayHeroStats(autoChess.GetPlayerPiecesName(player)));
			AnsiConsole.Write(new Rule($"[{autoChess.GetPlayerData(player).PlayerSide}]Set Hero's Position[/]"));
			var playerPieces = (List<IPiece>)autoChess.GetPlayerPieces(player);
			var playerPiece = AnsiConsole.Prompt(
				new SelectionPrompt<Hero>()
				.PageSize(5)
				.AddChoices(
					playerPieces.ConvertAll(piece => (Hero)piece)
				)
				.HighlightStyle(autoChess.GetPlayerData(player).PlayerSide.ToString())
			);
			
			// Loop until piece placement success
			bool success = false;
			var heroPosition = autoChess.GetHeroPosition(player, playerPiece.PieceId);
			if(heroPosition != null)
			{
				heroX = (heroPosition.X + 1).ToString();
				heroY = (heroPosition.Y + 1).ToString();
			}
			AnsiConsole.Write(new Markup($"{playerPiece}'s Current Position : X = {heroX}, Y = {heroY}\n"));
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
				AnsiConsole.Write(new Rule($"[{autoChess.GetPlayerData(player).PlayerSide}]{player.Name} Hero's Position[/]"));
				AnsiConsole.Write(DisplayBoard(player));
				confirm = AnsiConsole.Confirm("Are you sure about your heroes placement?");
			}
		}
	}

	static void DisplayResult()
	{
		foreach(var player in autoChess.GetPlayers())
		{
			var playerData = autoChess.GetPlayerData(player);
			StringBuilder battleResult = new();
			foreach(var result in playerData.Win)
			{
				battleResult.Append(result ? "🏆" : "💀");
			}
			StringBuilder healthPoint = new();
			for(int i = 0; i < playerData.Hp; i++)
			{
				healthPoint.Append("❤️");
			}
			AnsiConsole.Write(new Rule($"[{autoChess.GetPlayerData(player).PlayerSide}][[{(playerData.Winner ? "🏆" : "💀")}]] {player.Name}[/]\n"));
			AnsiConsole.Write(new Markup($"[[❤️]] Health Point : {healthPoint}\n"));
			AnsiConsole.Write(new Markup($"[[⚔️]] Battle Result : {battleResult}\n"));
		}
	}

	static int ScaleHeroStat(int originalValue, int originalMax, int scaleMax) => (originalValue * (scaleMax - 1) / originalMax) + 1;
	
	static void Main()
	{
		// Enable emoji support
		Console.OutputEncoding = Encoding.UTF8;

		// Add hero to hero database
		#region HERO_INIT
		// Knight
		autoChess.AddHero("Frost Knight", new HeroDetails(PieceTypes.Knight, 500, 47.5, 5, 3));
		autoChess.AddHero("Hell Knight", new HeroDetails(PieceTypes.Knight, 700, 75, 5, 1));
		autoChess.AddHero("Evil Knight", new HeroDetails(PieceTypes.Knight, 750, 50, 10, 2));
		autoChess.AddHero("Dragon Knight", new HeroDetails(PieceTypes.Knight, 800, 65, 10, 1));
		// Warlock
		autoChess.AddHero("Desperate Doctor", new HeroDetails(PieceTypes.Warlock, 550, 45, 5, 3));
		autoChess.AddHero("Poisonous Worm", new HeroDetails(PieceTypes.Warlock, 600, 55, 0, 3));
		autoChess.AddHero("Venomancer", new HeroDetails(PieceTypes.Warlock, 1000, 65, 0, 1));
		autoChess.AddHero("Dark Spirit", new HeroDetails(PieceTypes.Warlock, 1000, 50, 5, 3));
		// Mage
		autoChess.AddHero("Ogre Mage", new HeroDetails(PieceTypes.Mage, 700, 60, 5, 2));
		autoChess.AddHero("Thunder Spirit", new HeroDetails(PieceTypes.Mage, 750, 60, 5, 3));
		autoChess.AddHero("Tortola Elder", new HeroDetails(PieceTypes.Mage, 650, 42.5, 5, 5));
		autoChess.AddHero("God of Thunder", new HeroDetails(PieceTypes.Mage, 950, 60, 0, 3));
		// Warrior
		autoChess.AddHero("Tusk Champion", new HeroDetails(PieceTypes.Warrior, 650, 52.5, 5, 1));
		autoChess.AddHero("Swordman", new HeroDetails(PieceTypes.Warrior, 600, 67.5, 5, 2));
		autoChess.AddHero("God of War", new HeroDetails(PieceTypes.Warrior, 800, 70, 6, 2));
		autoChess.AddHero("Pirate Captain", new HeroDetails(PieceTypes.Warrior, 950, 82.5, 8, 2));
		autoChess.AddHero("Sacred Lancer", new HeroDetails(PieceTypes.Warrior, 1050, 90, 10, 3));
		// Hunter
		autoChess.AddHero("Egersis Ranger", new HeroDetails(PieceTypes.Hunter, 450, 45, 5, 5));
		autoChess.AddHero("Dwarf Sniper", new HeroDetails(PieceTypes.Hunter, 450, 70, 5, 5));
		autoChess.AddHero("Bobo", new HeroDetails(PieceTypes.Hunter, 600, 60, 5, 4));
		autoChess.AddHero("Spider Queen", new HeroDetails(PieceTypes.Hunter, 900, 95, 5, 1));
		autoChess.AddHero("Tsunami Stalker", new HeroDetails(PieceTypes.Hunter, 950, 50, 5, 1));
		// Assassin
		autoChess.AddHero("Soul Breaker", new HeroDetails(PieceTypes.Assassin, 550, 60, 5, 1));
		autoChess.AddHero("Abyssalcrawler", new HeroDetails(PieceTypes.Assassin, 500, 55, 5, 2));
		autoChess.AddHero("Shadowcrawler", new HeroDetails(PieceTypes.Assassin, 550, 85, 5, 2));
		autoChess.AddHero("Thorn Predator", new HeroDetails(PieceTypes.Assassin, 700, 67.5, 10, 1));
		// Shaman
		autoChess.AddHero("Defector", new HeroDetails(PieceTypes.Shaman, 550, 45, 5, 3));
		autoChess.AddHero("Storm Shaman", new HeroDetails(PieceTypes.Shaman, 800, 47.5, 5, 4));
		autoChess.AddHero("The Scryer", new HeroDetails(PieceTypes.Shaman, 1000, 75, 5, 4));
		// Druid
		autoChess.AddHero("Unicorn", new HeroDetails(PieceTypes.Druid, 400, 55, 5, 3));
		autoChess.AddHero("Wisper Seer", new HeroDetails(PieceTypes.Druid, 500, 47.5, 2, 3));
		autoChess.AddHero("Warpwood Sage", new HeroDetails(PieceTypes.Druid, 650, 75, 5, 2));
		autoChess.AddHero("Razorclaw", new HeroDetails(PieceTypes.Druid, 800, 55, 0, 3));
		autoChess.AddHero("Khan", new HeroDetails(PieceTypes.Druid, 1000, 115, 10, 1));
		// Witcher
		autoChess.AddHero("Taboo Witcher", new HeroDetails(PieceTypes.Witcher, 550, 50, 5, 2));
		autoChess.AddHero("Fallen Witcher", new HeroDetails(PieceTypes.Witcher, 750, 70, 5, 1));
		// Mech
		autoChess.AddHero("Heaven Bomber", new HeroDetails(PieceTypes.Mech, 500, 45, 5, 4));
		autoChess.AddHero("Ripper", new HeroDetails(PieceTypes.Mech, 800, 57.5, 6, 1));
		autoChess.AddHero("Gem Artisan", new HeroDetails(PieceTypes.Mech, 800, 55, 6, 1));
		autoChess.AddHero("Helicopter", new HeroDetails(PieceTypes.Mech, 900, 77.5, 10, 3));
		// Priest
		autoChess.AddHero("Goddess of Light", new HeroDetails(PieceTypes.Priest, 400, 52.5, 0, 4));
		autoChess.AddHero("Fortune Teller", new HeroDetails(PieceTypes.Priest, 550, 62.5, 5, 3));
		autoChess.AddHero("Cave Prodigy", new HeroDetails(PieceTypes.Priest, 800, 50, 6, 3));
		// Wizard
		autoChess.AddHero("Grand Herald", new HeroDetails(PieceTypes.Wizard, 600, 55, 0, 4));
		autoChess.AddHero("Grimtouch", new HeroDetails(PieceTypes.Wizard, 750, 80, 5, 3));
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
			player1 = new Player(AnsiConsole.Ask<string>("[[PLAYER 1]] What's your [green1]name[/]?"));
			var sidesOptions = (List<Sides>)autoChess.GetGameSides();
			var player1Side = AnsiConsole.Prompt(
				new SelectionPrompt<Sides>()
				.Title("Choose your [green1]side[/]?")
				.AddChoices(
					sidesOptions
				)
				.HighlightStyle(Color.Yellow1)
			);
			player2 = new Player(gameModeMenu.Contains("Bot") ? "BOT" : AnsiConsole.Ask<string>("[[PLAYER 2]] What's your [green1]name[/]?"));
			
			// ADD PLAYER
			autoChess.AddPlayer(player1, player1Side);
			sidesOptions.Remove(player1Side);
			autoChess.AddPlayer(player2, sidesOptions[0]);
			#endregion
			
			// LOOP ALL ROUND
			for(int round  = 1; round <= maxRound; round++)
			{
				// PICK HERO MENU
				// TODO
				// 1. How to edit/remove item from user (if user want to change item that already picked)
				// 2. Display hero type when choosing a hero
				#region PICK_HERO_MENU
				autoChess.SetGamePhase(Phases.ChoosingPiece);
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
				autoChess.SetGamePhase(Phases.PlaceThePiece);
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
							int x = new Random().Next(0, boardSize[0]);
							int y = new Random().Next(boardSize[1] % 2 == 0 ? boardSize[1] / 2 : (boardSize[1] / 2) + 1, boardSize[1]);
							success = autoChess.PutPlayerPiece(player2, piece, new Position(x, y));
						}
					}
				}
				#endregion

				// BATTLE VIEW
				#region BATTLE_VIEW
				autoChess.SetGameStatus(Status.OnGoing);
				autoChess.SetGamePhase(Phases.BattleBegin);
				while(autoChess.GetRoundWinner() == null)
				{
					FigletTitle("Battle");
					AnsiConsole.Write(new Rule($"[red]Round {round}[/]"));
					AnsiConsole.Write(DisplayBoard());
					
					// Display each player's hero health
					foreach(var player in autoChess.GetPlayers())
					{
						foreach(var piece in autoChess.GetPlayerPieces(player))
						{
							AnsiConsole.Write(new Markup($"[{autoChess.GetPlayerData(player).PlayerSide}]{piece.Name} | {piece.Hp}[/]\n"));
						};
					};
					
					// Create a task for each player's piece
					foreach(var player in autoChess.GetPlayers())
					{
						foreach(var piece in autoChess.GetPlayerPieces(player))
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
				AnsiConsole.Write(new Markup($"[rapidblink yellow1] Press any key to move to the next round...[/]\n").Centered());
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