namespace AutoChess;

public class GameController
{
	private readonly Board _board;
	public Dictionary<string, HeroDetails> HeroesDatabase {get; private set;} = new();
	private readonly Dictionary<PieceTypes, int> _heroSlot = new() {{PieceTypes.Warrior, 3}, {PieceTypes.Hunter, 3}, {PieceTypes.Knight, 3}};
	private Dictionary<IPlayer, PlayerData> _players = new();
	public int PlayerHp {get;} = 3;
	public int PlayerPiecesCount {get;} = 5;
	public Status CurrentGameStatus {get; private set;} = Status.NotInitialized;
	public Phases CurrentGamePhase {get; private set;} = Phases.NotInitialized;

	public GameController(Board board)
	{
		SetGameStatus(Status.Initialized);
		_board = board;
	}

	public GameController(Board board, int playerPiecesCount, int playerHp)
	{
		SetGameStatus(Status.Initialized);
		_board = board;
		PlayerPiecesCount = playerPiecesCount;
		PlayerHp = playerHp;
	}

	public GameController(Board board, Dictionary<PieceTypes, int> heroSlot)
	{
		SetGameStatus(Status.Initialized);
		_board = board;
		_heroSlot = heroSlot;
	}

	public GameController(Board board, Dictionary<IPlayer, Sides> players)
	{
		SetGameStatus(Status.Initialized);
		_board = board;
		AddPlayer(players);
	}

	public GameController(Board board, Dictionary<IPlayer, Sides> players, Dictionary<PieceTypes, int> heroSlot)
	{
		SetGameStatus(Status.Initialized);
		_board = board;
		_heroSlot = heroSlot;
		AddPlayer(players);
	}

	// Manage current game information
	#region MANAGE_CURRENT_GAME_INFO

	/// <summary>
	/// Set current game status
	/// </summary>
	/// <param name="gameStatus"></param>
	public void SetGameStatus(Status gameStatus) => CurrentGameStatus = gameStatus;

	/// <summary>
	/// Sets the current game phase to the specified value.
	/// </summary>
	/// <param name="gamePhase">The new game phase to set.</param>
	/// <remarks>
	/// The game phase determines the current phase of the game.
	/// </remarks>
	public void SetGamePhase(Phases gamePhase)
	{
		CurrentGamePhase = gamePhase;
		switch(gamePhase)
		{
			case Phases.ChoosingPiece:
				ClearPlayerPieces();
				break;
			case Phases.PlaceThePiece:
				ClearBoard();
				break;
			case Phases.BattleBegin:
				((List<IPlayer>)GetPlayers()).ForEach(player => SetWinner(player, false));
				break;
			case Phases.BattleEnd:
				var roundWinner = GetRoundWinner();
				if(roundWinner != null)
				{
					SetRoundWinner(roundWinner);
					SetWinner(roundWinner);
				}
				break;
			case Phases.TheChampion:
				var champion = GetChampion();
				if(champion != null)
				{
					SetWinner(champion);
				}
				break;
			default:
				break;
		}

	}

	/// <summary>
	/// Get list of player
	/// </summary>
	/// <returns>The list of player</returns>
	public IEnumerable<IPlayer> GetPlayers() => _players.Keys.ToList();

	/// <summary>
	/// Gets the width and height of the board.
	/// </summary>
	/// <returns>
	/// An array containing two elements:
	///   - The first element represents the width of the board.
	///   - The second element represents the height of the board.
	/// </returns>
	public int[] GetBoardSize() => [_board.Width, _board.Height];

	/// <summary>
	/// Return a piece instance from the player data based on the specified piece ID.
	/// </summary>
	/// <param name="heroId">The unique identifier of the piece to retrieve.</param>
	/// <returns>
	/// The <see cref="Piece"/> instance corresponding to the specified piece ID, 
	/// or <c>null</c> if no piece with the given ID is found.
	/// </returns>
	public IPiece? GetPieceById(string heroId)
	{
		IPiece? result = null;
		foreach(var player in GetPlayers())
		{
			result = GetPlayerData(player).GetPieceById(heroId);
			if(result != null)
			{
				return result;
			}
		}
		return result;
	}

	/// <summary>
	/// Gets the side for the specified player.
	/// </summary>
	/// <param name="player">The player for whom to determine the side.</param>
	/// <returns>
	/// The <see cref="Sides"/> enum representing the side for the specified player.
	/// </returns>
	public Sides? GetPlayerSide(IPlayer player) => GetPlayerData(player).PlayerSide;

	/// <summary>
	/// Gets the player who owns the hero with the specified identifier.
	/// </summary>
	/// <param name="heroId">The identifier of the hero.</param>
	/// <returns>
	/// The <see cref="Player"/> instance representing the player who owns the hero with the specified identifier,
	/// or <c>null</c> if no player owns the hero with the given identifier.
	/// </returns>
	public IPlayer? GetPlayerByPieceId(string heroId)
	{
		foreach(var playerPieces in _board.PiecesPositions)
		{
			foreach(var hero in playerPieces.Value)
			{
				if(hero.Value == heroId)
				{
					return playerPieces.Key;
				}
			}
		}
		return null;
	}

	/// <summary>
	/// Gets a list representing the possible sides (enum values) in the game.
	/// </summary>
	/// <returns>
	/// A <see cref="List{T}"/> containing all possible sides in the game,
	/// represented by the <see cref="Sides"/> enum values.
	/// </returns>
	public IEnumerable<Sides> GetGameSides() => Enum.GetValues(typeof(Sides)).Cast<Sides>().ToList();

	/// <summary>
	/// Gets the win points for the specified player.
	/// </summary>
	/// <param name="player">The player for whom to retrieve the win points.</param>
	/// <returns>
	/// The integer representing the win points for the specified player.
	/// </returns>
	public int GetPlayerWinPoint(IPlayer player)
	{
		int winPoint = 0;
		foreach(var winStatus in GetPlayerData(player).Win)
		{
			if(winStatus)
			{
				winPoint++;
			}
			
		}
		return winPoint;
	}

	/// <summary>
	/// Gets the winner of the current round.
	/// </summary>
	/// <returns>
	/// The <see cref="Player"/> instance representing the winner of the current round,
	/// or <c>null</c> if there is no winner yet.
	/// </returns>
	public IPlayer? GetRoundWinner()
	{
		var player1 = ((List<IPlayer>)GetPlayers())[0];
		var player2 = ((List<IPlayer>)GetPlayers())[1];
		if(GetPlayerBoard(player1).Count == 0 && GetPlayerBoard(player2).Count > 0)
		{
			return player2;
		}
		else if(GetPlayerBoard(player1).Count > 0 && GetPlayerBoard(player2).Count == 0)
		{
			return player1;
		}
		else
		{
			return null;
		}
	}
	
	/// <summary>
	/// Gets the champion of the game.
	/// </summary>
	/// <returns>
	/// The <see cref="Player"/> instance representing the champion of the game,
	/// or <c>null</c> if the champion has not been decided yet.
	/// </returns>
	public IPlayer? GetChampion()
	{
		var player1 = ((List<IPlayer>)GetPlayers())[0];
		var player2 = ((List<IPlayer>)GetPlayers())[1];
		var player1WinPoint = GetPlayerWinPoint(player1);
		var player2WinPoint = GetPlayerWinPoint(player2);
		if(player1WinPoint > player2WinPoint)
		{
			return player1;
		}
		else if(player1WinPoint < player2WinPoint)
		{
			return player2;
		}
		else
		{
			return null;
		}
	}
	#endregion

	// Manage hero database
	#region MANAGE_HERO_DATABASE
	
	/// <summary>
	/// Adds a new hero to the game.
	/// </summary>
	/// <param name="heroName">The name of the new hero to be added.</param>
	/// <param name="heroDetails">The details of the new hero to be added.</param>
	/// <returns>
	/// <c>true</c> if the hero is successfully added; otherwise, <c>false</c>.
	/// </returns>
	public bool AddHero(string heroName, HeroDetails heroDetails) => HeroesDatabase.TryAdd(heroName, heroDetails);
	
	/// <summary>
	/// Adds multiple heroes to the game using a dictionary of hero names and their details.
	/// </summary>
	/// <param name="heroes">A dictionary containing hero names and their corresponding details.</param>
	public void AddHero(Dictionary<string, HeroDetails> heroes)
	{
		foreach(var hero in heroes)
		{
			AddHero(hero.Key, hero.Value);
		}
	}
	
	/// <summary>
	/// Removes a hero from the game based on the specified hero name.
	/// </summary>
	/// <param name="heroName">The name of the hero to be removed.</param>
	/// <returns>
	/// <c>true</c> if the hero is successfully removed; otherwise, <c>false</c>.
	/// </returns>
	public bool RemoveHero(string heroName)  => HeroesDatabase.Remove(heroName);
	
	/// <summary>
	/// Gets a list of hero names from the game.
	/// </summary>
	/// <returns>
	/// A <see cref="List{T}"/> of strings representing the hero names in the game.
	/// </returns>
	public IEnumerable<string> GetHeroName() => HeroesDatabase.Keys.ToList().ConvertAll(hero => hero.ToString());
	
	/// <summary>
	/// Gets the details of a hero based on the specified hero name.
	/// </summary>
	/// <param name="heroName">The name of the hero for which to retrieve details.</param>
	/// <returns>
	/// The <see cref="HeroDetails"/> struct representing the details of the specified hero.
	/// </returns>
	public HeroDetails? GetHeroDetails(string heroName)
	{
		HeroesDatabase.TryGetValue(heroName, out HeroDetails? result);
		return result;
	}
	#endregion

	// Manage player
	#region MANAGE_PLAYER

	/// <summary>
	/// Adds a new player to the game with the specified side.
	/// </summary>
	/// <param name="newPlayer">The new player to be added.</param>
	/// <param name="side">The side (enum) to which the new player belongs.</param>
	/// <returns>
	/// <c>true</c> if the player is successfully added; otherwise, <c>false</c>.
	/// </returns>
	public bool AddPlayer(IPlayer newPlayer, Sides playerSide)
	{
		var addPlayer = _players.TryAdd(newPlayer, new PlayerData(PlayerHp, playerSide));
		var addPlayerBoard = _board.AddPlayerToBoard(newPlayer);
		return !new List<bool>([addPlayer, addPlayerBoard]).Contains(false);
	}

	/// <summary>
	/// Adds multiple players to the game with their specified sides using a dictionary.
	/// </summary>
	/// <param name="newPlayers">A dictionary containing players and their corresponding sides.</param>
	public void AddPlayer(Dictionary<IPlayer, Sides> newPlayers)
	{
		foreach(var player in newPlayers)
		{
			AddPlayer(player.Key, player.Value);
		}
	}

	/// <summary>
	/// Removes a player from the game based on the specified player.
	/// </summary>
	/// <param name="player">The player to be removed.</param>
	/// <returns>
	/// <c>true</c> if the player is successfully removed; otherwise, <c>false</c>.
	/// </returns>
	public bool RemovePlayer(IPlayer player)
	{
		var playerRemove = _players.Remove(player);
		var removePlayerBoard = _board.RemovePlayerFromBoard(player);
		return !new List<bool>([playerRemove, removePlayerBoard]).Contains(false);
	}

	/// <summary>
	/// Gets the <see cref="PlayerData"/> instance associated with the specified player.
	/// </summary>
	/// <param name="player">The player for whom to retrieve the data instance.</param>
	/// <returns>
	/// The <see cref="PlayerData"/> instance associated with the specified player,
	/// or <c>null</c> if the player's data is not found.
	/// </returns>
	public PlayerData? GetPlayerData(IPlayer player)
	{
		_players.TryGetValue(player, out PlayerData? result);
		return result;
	}
	#endregion

	// Manage player's piece
	#region MANAGE_PLAYER_PIECE
	public IEnumerable<IPiece> GetPlayerPieces(IPlayer player) => GetPlayerData(player).PlayerPieces;

	public IPiece? GetPlayerPiece(IPlayer player, string heroId) => GetPlayerData(player).GetPieceById(heroId);

	public IEnumerable<string> GetPlayerPiecesName(IPlayer player) => ((List<IPiece>)GetPlayerPieces(player)).ConvertAll(piece => piece.Name);
	
	public bool AddPlayerPiece(IPlayer player, string heroName)
	{
		if(GetPlayerPieces(player).Count() < PlayerPiecesCount)
		{
			GetPlayerData(player).PlayerPieces.Add(new Hero(heroName, HeroesDatabase[heroName]));
			return true;
		}
		return false;
	}

	public void AddPlayerPiece(IPlayer player, IEnumerable<string> heroNames)
	{
		foreach(var heroName in heroNames)
		{
			AddPlayerPiece(player, heroName);
		}
	}

	public bool RemovePlayerPiece(IPlayer player, IPiece piece) => GetPlayerData(player).PlayerPieces.Remove(piece);

	public void ClearPlayerPieces(IPlayer player) => GetPlayerData(player).PlayerPieces.Clear();

	public void ClearPlayerPieces() => ((List<IPlayer>)GetPlayers()).ForEach(player => ClearPlayerPieces(player));
	#endregion

	// Manage board
	#region MANAGE_BOARD
	public Dictionary<IPosition, string> GetPlayerBoard(IPlayer player) => _board.GetPlayerBoard(player);

	public IPosition? GetHeroPosition(IPlayer player, string heroId) => _board.GetHeroPosition(player, heroId);

	public Dictionary<IPosition, string> GetAllHeroPosition()
	{
		Dictionary<IPosition, string> allHeroPosition = new();
		foreach(var piecesPosition in _board.PiecesPositions.Values)
		{
			foreach(var piecePosition in piecesPosition)
			{
				allHeroPosition.TryAdd(piecePosition.Key, piecePosition.Value);
			}
		}
		return allHeroPosition;
	}

	public bool UpdateHeroPosition(IPlayer player, string heroId, IPosition newPosition) => _board.UpdateHeroPosition(player, heroId, newPosition);

	public bool PutPlayerPiece(IPlayer player, IPiece piece, IPosition position)
	{
		if(_board.GetHeroPosition(player, piece.PieceId) == null)
		{
			return _board.AddHeroPosition(player, piece.PieceId, position);
		}
		return _board.UpdateHeroPosition(player, piece.PieceId, position);
	}

	public bool IsFinishedPickAllPieces(IPlayer player) => GetPlayerPieces(player).Count() == PlayerPiecesCount;

	public bool IsFinishedPutAllPieces(IPlayer player) => _board.GetPlayerBoard(player).Count == GetPlayerPieces(player).Count();

	public bool IsValidPosition(IPosition newPosition) => !GetAllHeroPosition().ContainsKey(newPosition);

	public bool RemoveHeroFromBoard(IPlayer player, string heroId) => GetPlayerBoard(player).Remove(GetHeroPosition(player, heroId)!);

	public void ClearBoard() => ((List<IPlayer>)GetPlayers()).ForEach(player => GetPlayerBoard(player).Clear());
	#endregion

	// Manage Battle
	#region MANAGE_BATTLE
	public IEnumerable<string> GetAllEnemyId(IPlayer player, IPiece hero) => _board.GetAllEnemyId(player, hero);

	public async Task Attack(IPlayer player, IPiece piece)
	{
		if(piece.Hp <= 0)
		{
			RemoveHeroFromBoard(player, piece.PieceId);
		}
		if(GetAllEnemyId(player, piece).Count() == 0)
		{
			bool move = true;
			while(move)
			{
				int[] boardSize = GetBoardSize();
				int x = new Random().Next(0, boardSize[0]);
				int y = new Random().Next(0, boardSize[1]);
				var newPosition = new Position(x, y);
				if(IsValidPosition(newPosition))
				{
					UpdateHeroPosition(player, piece.PieceId, newPosition);
					move = false;
				}
			}
		}
		else
		{
			foreach(var enemyId in GetAllEnemyId(player, piece))
			{
				var enemy = GetPieceById(enemyId);
				if(enemy != null && enemy.Hp > 0)
				{
					((Hero)piece).AttackEnemy(enemy);
				}
			}
		}
		await Task.Delay(1000);
	}
	
	public void SetRoundWinner(IPlayer winner)
	{
		foreach(var player in GetPlayers())
		{
			if(player.PlayerId == winner.PlayerId)
			{
				GetPlayerData(player).Win.Add(true);
			}
			else
			{
				GetPlayerData(player).Win.Add(false);
				GetPlayerData(player).Hp--;
			}
		}
	}

	// Set winner state
	public void SetWinner(IPlayer player, bool IsWin = true) => GetPlayerData(player).Winner = IsWin;
	#endregion

	/// <summary>
	/// Generates a list of random hero names picked from the predefined set of heroes.
	/// </summary>
	/// <returns>
	/// A <see cref="List{T}"/> of strings representing the randomly picked hero names.
	/// </returns>
	public IEnumerable<string>? GenerateRandomHeroList()
	{
		List<string> options = new();
		var heroNames = (List<string>)GetHeroName();
		if(heroNames.Count == 0)
		{
			return null;
		}
		int n = 4;
		while(n > 0)
		{
			options.Add(heroNames[new Random().Next(0, heroNames.Count)]);
			n--;
		}
		return options;
	}
}