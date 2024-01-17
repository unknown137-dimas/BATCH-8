class GameController
{
	private readonly IBoard _board;
	public Dictionary<string, HeroDetails> HeroesDatabase {get; private set;} = new();
	private readonly Dictionary<PieceTypes, int> _heroSlot = new() {{PieceTypes.Warrior, 3}, {PieceTypes.Hunter, 3}, {PieceTypes.Knight, 3}};
	private Dictionary<IPlayer, PlayerData> _players = new();
	public int PlayerHp {get; set;} = 10;
	public Status CurrentGameStatus {get; set;} = Status.NotInitialized;
	public Phases CurrentGamePhase {get; set;} = Phases.NotInitialized;

	public GameController(IBoard board)
	{
		CurrentGameStatus = Status.Initialized;
		_board = board;
	}

	public GameController(IBoard board, Dictionary<PieceTypes, int> heroSlot)
	{
		CurrentGameStatus = Status.Initialized;
		_board = board;
		_heroSlot = heroSlot;
	}

	public GameController(IBoard board, List<IPlayer> players)
	{
		CurrentGameStatus = Status.Initialized;
		_board = board;
		AddPlayer(players);
	}

	public GameController(IBoard board, List<IPlayer> players, Dictionary<PieceTypes, int> heroSlot)
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
		var addPlayerBoard = _board.AddPlayer(newPlayer);
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
		var removePlayerBoard = _board.RemovePlayer(player);
		return !new List<bool>([playerRemove, removePlayerBoard]).Contains(false);
	}

	public PlayerData GetPlayerData(IPlayer player) => _players[player];

	// Manage player's piece
	public IEnumerable<Hero> GetPlayerPieces(IPlayer player) => _players[player].PlayerPieces;
	public void AddPlayerPiece(IPlayer player, Hero piece) => _players[player].PlayerPieces.Add(piece);

	public void AddPlayerPiece(IPlayer player, IEnumerable<Hero> pieces) => _players[player].PlayerPieces.AddRange(pieces);

	public bool RemovePlayerPiece(IPlayer player, Hero piece) => _players[player].PlayerPieces.Remove(piece);

	public bool PutPlayerPiece(IPlayer player, Hero piece, Position position)
	{
		if(_board.IsPositionEmpty(player, position))
		{
			if(_board.AddHeroPosition(player, piece, position))
			{
				piece.Move(position);
				return true;
			}
			return false;
		}
		return false;
	}

	public bool IsFinishedPutAllPieces(IPlayer player)
	{
		List<bool> piecePosition = new();
		foreach(var piece in GetPlayerPieces(player))
		{
			piecePosition.Add(Array.IndexOf(piece.GetPosition(), -1) >= 0 ? false : true);
		}
		return !piecePosition.Contains(false);
	}

	public bool IsValidPosition(Hero piece, Hero otherPiece) => piece.HeroPosition.X != otherPiece.HeroPosition.X || piece.HeroPosition.Y != otherPiece.HeroPosition.Y;

	public void NextPhase()
	{
		throw new NotImplementedException();
	}

	public bool CheckTargetAcquitition(IPlayer player, Hero piece, IPosition position)
	{
		throw new NotImplementedException();
	}

	public IEnumerable<IPosition> GetAllPossibleMove(IPlayer player, Hero piece)
	{
		throw new NotImplementedException();
	}
	
	public int Attack(IPlayer player, Hero piece, IPlayer enemyPlayer, Hero enemyPiece)
	{
		throw new NotImplementedException();
	}
	
	// Set current round winner
	public bool SetWinner(IPlayer player) => _players[player].Winner = true;

	// Generate random options
	public IEnumerable<T> GenerateRandomHeroList<T>(in List<T> source)
	{
		Random random = new();
		List<T> options = new();
		int n = 5;
		while(n > 0)
		{
			options.Add(source[random.Next(0, source.Count)]);
			n--;
		}
		return options;
	}
}