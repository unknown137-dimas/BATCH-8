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

	public GameController(Board board, List<IPlayer> players)
	{
		CurrentGameStatus = Status.Initialized;
		_board = board;
		AddPlayer(players);
	}

	public GameController(Board board, List<IPlayer> players, Dictionary<PieceTypes, int> heroSlot)
	{
		CurrentGameStatus = Status.Initialized;
		_board = board;
		_heroSlot = heroSlot;
		AddPlayer(players);
	}

	// Get current game information
	public IEnumerable<IPlayer> GetPlayers() => _players.Keys;

	public int[] GetBoardSize() => [_board.Width, _board.Height];
	
	// Manage hero database
	public bool AddHero(string heroName, HeroDetails heroDetails) => HeroesDatabase.TryAdd(heroName, heroDetails);
	
	public bool RemoveHero(string heroName)  => HeroesDatabase.Remove(heroName);
	
	public IEnumerable<string> GetHeroName() => HeroesDatabase.Keys.ToList();
	
	public HeroDetails GetHeroDetails(string heroName) => HeroesDatabase[heroName];

	// Manage player
	public bool AddPlayer(IPlayer newPlayer)
	{
		var addPlayer = _players.TryAdd(newPlayer, new PlayerData(PlayerHp));
		var addPlayerBoard = _board.AddPlayerToBoard(newPlayer);
		return !new List<bool>([addPlayer, addPlayerBoard]).Contains(false);
	}

	public void AddPlayer(IEnumerable<IPlayer> newPlayers)
	{
		foreach(var player in newPlayers)
		{
			AddPlayer(player);
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

	public bool IsPieceExistOnBoard(IPlayer player, string heroId) => _board.GetPlayerBoard(player).ContainsValue(heroId);

	public IPosition? GetHeroPosition(IPlayer player, string heroId) => _board.GetHeroPosition(player, heroId);

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

	public bool IsValidPosition(IPlayer player, IPiece piece, IPlayer otherPlayer, IPiece otherPiece) => _board.GetHeroPosition(player, piece.PieceId)?.X != _board.GetHeroPosition(otherPlayer, otherPiece.PieceId)?.X || _board.GetHeroPosition(player, piece.PieceId)?.Y != _board.GetHeroPosition(otherPlayer, otherPiece.PieceId)?.Y;

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