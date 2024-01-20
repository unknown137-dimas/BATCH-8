class GameController
{
	private readonly Board _board;
	public Dictionary<string, HeroDetails> HeroesDatabase {get; private set;} = new();
	private readonly Dictionary<PieceTypes, int> _heroSlot = new() {{PieceTypes.Warrior, 3}, {PieceTypes.Hunter, 3}, {PieceTypes.Knight, 3}};
	private Dictionary<IPlayer, PlayerData> _players = new();
	public int PlayerHp {get; set;} = 10;
	public int PlayerPiecesCount {get; set;} = 5;
	public Status CurrentGameStatus {get; set;} = Status.NotInitialized;
	public Phases CurrentGamePhase {get; set;} = Phases.NotInitialized;

	public GameController(Board board)
	{
		CurrentGameStatus = Status.Initialized;
		_board = board;
	}

	public GameController(Board board, Dictionary<PieceTypes, int> heroSlot)
	{
		CurrentGameStatus = Status.Initialized;
		_board = board;
		_heroSlot = heroSlot;
	}

	public GameController(Board board, Dictionary<IPlayer, Sides> players)
	{
		CurrentGameStatus = Status.Initialized;
		_board = board;
		AddPlayer(players);
	}

	public GameController(Board board, Dictionary<IPlayer, Sides> players, Dictionary<PieceTypes, int> heroSlot)
	{
		CurrentGameStatus = Status.Initialized;
		_board = board;
		_heroSlot = heroSlot;
		AddPlayer(players);
	}

	// Get current game information
	public IEnumerable<IPlayer> GetPlayers() => _players.Keys.ToList();

	public int[] GetBoardSize() => [_board.Width, _board.Height];

	public IPiece? GetPieceById(string heroId)
	{
		IPiece? result = null;
		foreach(var player in GetPlayers())
		{
			result = GetPlayerData(player).GetHeroById(heroId);
			if(result != null)
			{
				return result;
			}
		}
		return result;
	}

	public Sides GetPlayerSide(IPlayer player) => GetPlayerData(player).PlayerSide;

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

	public IEnumerable<Sides> GetGameSides() => Enum.GetValues(typeof(Sides)).Cast<Sides>().ToList();
	
	// Manage hero database
	public bool AddHero(string heroName, HeroDetails heroDetails) => HeroesDatabase.TryAdd(heroName, heroDetails);
	
	public bool RemoveHero(string heroName)  => HeroesDatabase.Remove(heroName);
	
	public IEnumerable<string> GetHeroName() => HeroesDatabase.Keys.ToList();
	
	public HeroDetails GetHeroDetails(string heroName) => HeroesDatabase[heroName];

	// Manage player
	public bool AddPlayer(IPlayer newPlayer, Sides playerSide)
	{
		var addPlayer = _players.TryAdd(newPlayer, new PlayerData(PlayerHp, playerSide));
		var addPlayerBoard = _board.AddPlayerToBoard(newPlayer);
		return !new List<bool>([addPlayer, addPlayerBoard]).Contains(false);
	}

	public void AddPlayer(Dictionary<IPlayer, Sides> newPlayers)
	{
		foreach(var player in newPlayers)
		{
			AddPlayer(player.Key, player.Value);
		}
	}

	public bool RemovePlayer(IPlayer player)
	{
		var playerRemove = _players.Remove(player);
		var removePlayerBoard = _board.RemovePlayerFromBoard(player);
		return !new List<bool>([playerRemove, removePlayerBoard]).Contains(false);
	}

	public PlayerData GetPlayerData(IPlayer player) => _players[player];

	// Manage player's piece
	public IEnumerable<IPiece> GetPlayerPieces(IPlayer player) => _players[player].PlayerPieces;

	public IPiece? GetPlayerPiece(IPlayer player, string heroId) => GetPlayerData(player).GetHeroById(heroId);

	public IEnumerable<string> GetPlayerPiecesName(IPlayer player) => ((List<IPiece>)GetPlayerPieces(player)).ConvertAll(piece => piece.Name);
	
	public bool AddPlayerPiece(IPlayer player, string heroName)
	{
		if(GetPlayerPieces(player).Count() < PlayerPiecesCount)
		{
			_players[player].PlayerPieces.Add(new Hero(heroName, HeroesDatabase[heroName]));
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

	public bool RemovePlayerPiece(IPlayer player, IPiece piece) => _players[player].PlayerPieces.Remove(piece);

	// Manage board
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

	// Manage Battle
	public IEnumerable<string> GetAllEnemy(IPlayer player, IPiece hero) => _board.GetAllEnemyId(player, hero);

	public string Attack(IPlayer player, string heroId, IPlayer otherPlayer, string otherHeroId)
	{
		throw new NotImplementedException();
	}
	
	// Set current round winner
	public bool SetWinner(IPlayer player) => _players[player].Winner = true;

	// Generate random options
	public IEnumerable<string> GenerateRandomHeroList()
	{
		Random random = new();
		List<string> options = new();
		List<string> heroNames = HeroesDatabase.Keys.ToList().ConvertAll(hero => hero.ToString());
		int n = PlayerPiecesCount;
		while(n > 0)
		{
			options.Add(heroNames[random.Next(0, heroNames.Count)]);
			n--;
		}
		return options;
	}
}