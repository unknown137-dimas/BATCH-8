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
	public void SetGameStatus(Status gameStatus) => CurrentGameStatus = gameStatus;

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

	public IEnumerable<IPlayer> GetPlayers() => _players.Keys.ToList();

	public int[] GetBoardSize() => [_board.Width, _board.Height];

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

	// Manage hero database
	public bool AddHero(string heroName, HeroDetails heroDetails) => HeroesDatabase.TryAdd(heroName, heroDetails);
	
	public void AddHero(Dictionary<string, HeroDetails> heroes)
	{
		foreach(var hero in heroes)
		{
			AddHero(hero.Key, hero.Value);
		}
	}
	
	public bool RemoveHero(string heroName)  => HeroesDatabase.Remove(heroName);
	
	public IEnumerable<string> GetHeroName() => HeroesDatabase.Keys.ToList().ConvertAll(hero => hero.ToString());
	
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

	public bool RemoveHeroFromBoard(IPlayer player, string heroId) => GetPlayerBoard(player).Remove(GetHeroPosition(player, heroId)!);

	public void ClearBoard() => ((List<IPlayer>)GetPlayers()).ForEach(player => GetPlayerBoard(player).Clear());

	// Manage Battle
	public IEnumerable<string> GetAllEnemyId(IPlayer player, IPiece hero) => _board.GetAllEnemyId(player, hero);

	public async Task Attack(IPlayer player, IPiece piece)
	{
		if(piece.Hp <= 0)
		{
			RemovePlayerPiece(player, piece);
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
		await Task.Delay(100);
	}
	
	// Set current round winner
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

	// Generate random options
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