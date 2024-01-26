namespace AutoChess;

public class GameController
{
	private readonly IBoard _board;
	public Dictionary<string, HeroDetails> HeroesDatabase {get; private set;} = new();
	private Dictionary<IPlayer, PlayerData> _players = new();
	public int PlayerHp {get;} = 3;
	public int PlayerPiecesCount {get;} = 5;
	public Status CurrentGameStatus {get; private set;} = Status.NotInitialized;
	public Phases CurrentGamePhase {get; private set;} = Phases.NotInitialized;

	public GameController(IBoard board)
	{
		SetGameStatus(Status.Initialized);
		_board = board;
	}

	public GameController(IBoard board, int playerPiecesCount, int playerHp)
	{
		SetGameStatus(Status.Initialized);
		_board = board;
		PlayerPiecesCount = playerPiecesCount;
		PlayerHp = playerHp;
	}
	
	public GameController(IBoard board, IDictionary<IPlayer, Sides> players)
	{
		SetGameStatus(Status.Initialized);
		_board = board;
		AddPlayer(players);
	}

	public GameController(IBoard board, int playerPiecesCount, int playerHp, IDictionary<IPlayer, Sides> players)
	{
		SetGameStatus(Status.Initialized);
		_board = board;
		PlayerPiecesCount = playerPiecesCount;
		PlayerHp = playerHp;
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
				GetPlayers().ToList().ForEach(player => SetWinner(player, false));
				break;
			case Phases.BattleEnd:
				if(TryGetRoundWinner(out IPlayer? roundWinner, out RoundResult roundResult) && roundResult != RoundResult.Draw)
				{
					SetRoundWinner(roundWinner!);
					SetWinner(roundWinner!);
				}
				break;
			case Phases.TheChampion:
				if(TryGetChampion(out IPlayer? champion))
				{
					SetWinner(champion!);
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
	/// Gets the piece with the specified ID.
	/// </summary>
	/// <param name="heroId">The unique identifier of the piece to retrieve.</param>
	/// <returns>The piece with the specified ID.</returns>
	/// <exception cref="KeyNotFoundException">Thrown if the piece with the specified ID is not found.</exception>
	public IPiece GetPieceById(Guid heroId)
	{
		foreach(var player in GetPlayers())
		{
			if(!TryGetPlayerData(player, out PlayerData? result))
			{
				continue;
			}
			if(result!.TryGetPieceById(heroId, out IPiece? pieceResult))
			{
				return pieceResult!;
			}
		}
		throw new KeyNotFoundException();
	}

	/// <summary>
	/// Tries to retrieve a piece by its unique identifier.
	/// </summary>
	/// <param name="heroId">The unique identifier of the hero associated with the piece.</param>
	/// <param name="pieceResult">When this method returns, contains the piece associated with the specified heroId, if the heroId is found; otherwise, null.</param>
	/// <returns>
	/// true if the heroId was found and the associated piece was retrieved successfully; otherwise, false.
	/// </returns>	
	public bool TryGetPieceById(Guid heroId, out IPiece? pieceResult)
	{
		foreach(var player in GetPlayers())
		{
			if(!TryGetPlayerData(player, out PlayerData? result))
			{
				continue;
			}
			if(result!.TryGetPieceById(heroId, out IPiece? piece))
			{
				pieceResult = piece;
				return true;
			}
		}
		pieceResult = null;
		return false;
	}

	/// <summary>
	/// Gets the side of the specified player.
	/// </summary>
	/// <param name="player">The player for which to retrieve the side.</param>
	/// <returns>The side of the specified player, or Sides.Unknown if the player is not found.</returns>
	public Sides GetPlayerSide(IPlayer player)
	{
		if(TryGetPlayerData(player, out PlayerData? data))
		{
			return data!.PlayerSide;
		}
		return Sides.Unknown;
	}

	/// <summary>
	/// Tries to retrieve the side enum associated with the specified player.
	/// </summary>
	/// <param name="player">The player for whom to retrieve the side.</param>
	/// <param name="playerSideResult">When this method returns, contains the side enum associated with the specified player, if the player is found; otherwise, Side.Unknown.</param>
	/// <returns>
	/// true if the player was found and the associated side was retrieved successfully; otherwise, false.
	/// </returns>
	public bool TryGetPlayerSide(IPlayer player, out Sides playerSideResult)
	{
		if(TryGetPlayerData(player, out PlayerData? result))
		{
			playerSideResult = result!.PlayerSide;
			return true;
		}
		playerSideResult = Sides.Unknown;
		return false;
	}

	/// <summary>
	/// Gets the player with the specified piece ID.
	/// </summary>
	/// <param name="heroId">The unique identifier of the piece associated with the player to retrieve.</param>
	/// <returns>The player with the specified piece ID.</returns>
	/// <exception cref="KeyNotFoundException">Thrown if the player with the specified piece ID is not found.</exception>
	public IPlayer GetPlayerByPieceId(Guid heroId)
	{
		foreach(var player in GetPlayers())
		{
			if(!TryGetPlayerData(player, out PlayerData? result))
			{
				continue;
			}
			if(result!.TryGetPieceById(heroId, out _))
			{
				return player;
			}
		}
		throw new KeyNotFoundException();
	}

	/// <summary>
	/// Tries to retrieve the player associated with the specified hero ID.
	/// </summary>
	/// <param name="heroid">The unique identifier of the hero associated with the player.</param>
	/// <param name="playerResult">When this method returns, contains the player associated with the specified hero ID, if found; otherwise, null.</param>
	/// <returns>
	/// true if the hero ID was found and the associated player was retrieved successfully; otherwise, false.
	/// </returns>
	public bool TryGetPlayerByPieceId(Guid heroId, out IPlayer? playerResult)
	{
		foreach(var player in GetPlayers())
		{
			if(!TryGetPlayerData(player, out PlayerData? result))
			{
				continue;
			}
			if(result!.TryGetPieceById(heroId, out _))
			{
				playerResult = player;
				return true;
			}
		}
		playerResult = null;
		return false;
	}

	/// <summary>
	/// Gets a list representing the possible sides (enum values) in the game.
	/// </summary>
	/// <returns>
	/// A <see cref="List{T}"/> containing all possible sides in the game,
	/// represented by the <see cref="Sides"/> enum values.
	/// </returns>
	public IEnumerable<Sides> GetSidesOptions() => ((Sides[])Enum.GetValues(typeof(Sides))).Where(side => side != Sides.Unknown);

	/// <summary>
	/// Gets the win points for the specified player.
	/// </summary>
	/// <param name="player">The player for whom to retrieve the win points.</param>
	/// <returns>
	/// The integer representing the win points for the specified player.
	/// </returns>
	public int GetPlayerWinPoint(IPlayer player)
	{
		if(!TryGetPlayerData(player, out PlayerData? result))
		{
			return 0;
		}
		return result!.Win.Where(winStatus => winStatus == true).Count();
	}

	/// <summary>
	/// Gets the winner of the current round.
	/// </summary>
	/// <returns>The winner of the current round.</returns>
	/// <exception cref="Exception">Thrown if there is no round winner (due to a draw or no winner yet).</exception>
	public IPlayer GetRoundWinner()
	{
		var playerOne = GetPlayers().ToArray()[0];
		var playerTwo = GetPlayers().ToArray()[1];
		if(TryGetPlayerBoard(playerOne, out var playerOneBoard) && TryGetPlayerBoard(playerTwo, out var playerTwoBoard))
		{
			if(playerOneBoard!.Count == 0 && playerTwoBoard!.Count > 0)
			{
				return playerTwo;
			}
			else if(playerOneBoard!.Count > 0 && playerTwoBoard!.Count == 0)
			{
				return playerOne;
			}
			else if(playerOneBoard!.Count == 0 && playerTwoBoard!.Count == 0)
			{
				throw new Exception("No Round Winner");
			}
		}
		throw new Exception("No Round Winner");
	}

	/// <summary>
	/// Tries to get the winner of the current round and the round result.
	/// </summary>
	/// <param name="winnerResult">The winner of the current round (null for draw).</param>
	/// <param name="roundResult">The result of the current round (Unknown, Draw, PlayerOneWin, or PlayerTwoWin).</param>
	/// <returns>True if there is a winner or draw, false otherwise.</returns>
	public bool TryGetRoundWinner(out IPlayer? winnerResult, out RoundResult roundResult)
	{
		var playerOne = GetPlayers().ToArray()[0];
		var playerTwo = GetPlayers().ToArray()[1];
		if(TryGetPlayerBoard(playerOne, out var playerOneBoard) && TryGetPlayerBoard(playerTwo, out var playerTwoBoard))
		{
			if(playerOneBoard!.Count == 0 && playerTwoBoard!.Count > 0)
			{
				winnerResult = playerTwo;
				roundResult = RoundResult.PlayerTwoWin;
				return true;
			}
			else if(playerOneBoard!.Count > 0 && playerTwoBoard!.Count == 0)
			{
				winnerResult = playerOne;
				roundResult = RoundResult.PlayerOneWin;
				return true;
			}
			else if(playerOneBoard!.Count == 0 && playerTwoBoard!.Count == 0)
			{
				winnerResult = null;
				roundResult = RoundResult.Draw;
				return true;
			}
		}
		roundResult = RoundResult.Unknown;
		winnerResult = null;
		return false;
	}
	
	/// <summary>
	/// Gets the champion of the game.
	/// </summary>
	/// <returns>The champion player.</returns>
	/// <exception cref="Exception">Thrown if there is no champion.</exception>
	public IPlayer GetChampion()
	{
		var playerOne = GetPlayers().ToArray()[0];
		var playerTwo = GetPlayers().ToArray()[1];
		var playerOneWinPoint = GetPlayerWinPoint(playerOne);
		var playerTwoWinPoint = GetPlayerWinPoint(playerTwo);
		if(playerOneWinPoint > playerTwoWinPoint)
		{
			return playerOne;
		}
		else if(playerOneWinPoint < playerTwoWinPoint)
		{
			return playerTwo;
		}
		else
		{
			throw new Exception("No Champion");
		}
	}

	/// <summary>
	/// Tries to retrieve the overall game champion.
	/// </summary>
	/// <param name="championResult">When this method returns, contains the player who is the overall game champion, if available; otherwise, null.</param>
	/// <returns>
	/// true if there is an overall game champion and the associated player was retrieved successfully; otherwise, false.
	/// </returns>
	public bool TryGetChampion(out IPlayer? championResult)
	{
		var playerOne = GetPlayers().ToArray()[0];
		var playerTwo = GetPlayers().ToArray()[1];
		var playerOneWinPoint = GetPlayerWinPoint(playerOne);
		var playerTwoWinPoint = GetPlayerWinPoint(playerTwo);
		if(playerOneWinPoint > playerTwoWinPoint)
		{
			championResult = playerOne;
			return true;
		}
		else if(playerOneWinPoint < playerTwoWinPoint)
		{
			championResult = playerTwo;
			return true;
		}
		else
		{
			championResult = null;
			return false;
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
	public bool AddHero(string heroName, HeroDetails heroDetails)
	{
		if(Enum.IsDefined(typeof(PieceTypes), heroDetails.HeroType))
		{
			return HeroesDatabase.TryAdd(heroName, heroDetails);
		}
		return false;
	}
	
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
	/// Gets the details of the hero with the specified name.
	/// </summary>
	/// <param name="heroName">The name of the hero.</param>
	/// <returns>The details of the hero.</returns>
	/// <exception cref="KeyNotFoundException">Thrown if the hero with the specified name is not found.</exception>
	public HeroDetails GetHeroDetails(string heroName)
	{
		if(HeroesDatabase.TryGetValue(heroName, out HeroDetails? result))
		{
			return result;
		}
		throw new KeyNotFoundException();
	}

	/// <summary>
	/// Tries to retrieve the details of a hero based on the provided hero name.
	/// </summary>
	/// <param name="heroName">The name of the hero for which to retrieve details.</param>
	/// <param name="heroDetailResult">When this method returns, contains the details of the hero, if available; otherwise, null.</param>
	/// <returns>
	/// true if the details of the hero with the provided name were found and retrieved successfully; otherwise, false.
	/// </returns>
	public bool TryGetHeroDetails(string heroName, out HeroDetails? heroDetailResult)
	{
		if(HeroesDatabase.TryGetValue(heroName, out HeroDetails? result))
		{
			heroDetailResult = result;
			return true;
		}
		heroDetailResult = null;
		return false;
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
		if(Enum.IsDefined(typeof(Sides), playerSide))
		{
			var addPlayer = _players.TryAdd(newPlayer, new PlayerData(PlayerHp, playerSide));
			var addPlayerBoard = _board.AddPlayerToBoard(newPlayer);
			return !new List<bool>([addPlayer, addPlayerBoard]).Contains(false);
		}
		return false;
	}

	/// <summary>
	/// Adds multiple players to the game with their specified sides using a dictionary.
	/// </summary>
	/// <param name="newPlayers">A dictionary containing players and their corresponding sides.</param>
	public void AddPlayer(IDictionary<IPlayer, Sides> newPlayers)
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
	/// Gets the data of the specified player.
	/// </summary>
	/// <param name="player">The player for which to retrieve the data.</param>
	/// <returns>The data of the specified player.</returns>
	/// <exception cref="KeyNotFoundException">Thrown if the player is not found in the dictionary.</exception>
	public PlayerData GetPlayerData(IPlayer player)
	{
		if(_players.TryGetValue(player, out PlayerData? result))
		{
			return result;
		}
		throw new KeyNotFoundException();
	}
	
	/// <summary>
	/// Tries to retrieve the data associated with the specified player.
	/// </summary>
	/// <param name="player">The player for which to retrieve data.</param>
	/// <param name="playerDataResult">When this method returns, contains the data associated with the specified player, if available; otherwise, null.</param>
	/// <returns>
	/// true if the data associated with the player was found and retrieved successfully; otherwise, false.
	/// </returns>
	public bool TryGetPlayerData(IPlayer player, out PlayerData? playerDataResult)
	{
		if(_players.TryGetValue(player, out PlayerData? result))
		{
			playerDataResult = result;
			return true;
		}
		playerDataResult = null;
		return false;
	}
	#endregion

	// Manage player's piece
	#region MANAGE_PLAYER_PIECE
	
	/// <summary>
	/// Gets a list of pieces owned by the specified player.
	/// </summary>
	/// <param name="player">The player for whom to retrieve the pieces.</param>
	/// <returns>
	/// A <see cref="List{T}"/> of pieces owned by the specified player.
	/// </returns>
	public IEnumerable<IPiece> GetPlayerPieces(IPlayer player)
	{
		if(TryGetPlayerData(player, out PlayerData? output))
		{
			return output!.PlayerPieces;
		}
		return Enumerable.Empty<IPiece>();
	}

	/// <summary>
	/// Gets the piece of the specified hero associated with the player.
	/// </summary>
	/// <param name="player">The player for which to retrieve the piece.</param>
	/// <param name="heroId">The unique identifier of the hero piece to retrieve.</param>
	/// <returns>The piece of the specified hero associated with the player.</returns>
	/// <exception cref="KeyNotFoundException">Thrown if the player or hero piece is not found in the dictionary.</exception>
	public IPiece GetPlayerPiece(IPlayer player, Guid heroId)
	{
		if(!TryGetPlayerData(player, out PlayerData? result))
		{
			throw new KeyNotFoundException();
		}
		if(result!.TryGetPieceById(heroId, out IPiece? pieceResult))
		{
			return pieceResult!;
		}
		throw new KeyNotFoundException();
	}

	/// <summary>
	/// Tries to retrieve the piece associated with the specified player and hero ID.
	/// </summary>
	/// <param name="player">The player for which to retrieve the piece.</param>
	/// <param name="heroId">The unique identifier of the hero associated with the piece.</param>
	/// <param name="pieceResult">When this method returns, contains the piece associated with the specified player and hero ID, if available; otherwise, null.</param>
	/// <returns>
	/// true if the piece associated with the player and hero ID was found and retrieved successfully; otherwise, false.
	/// </returns>
	public bool TryGetPlayerPiece(IPlayer player, Guid heroId, out IPiece? pieceResult)
	{
		if(!TryGetPlayerData(player, out PlayerData? result))
		{
			pieceResult = null;
			return false;
		}
		if(result!.TryGetPieceById(heroId, out IPiece? piece))
		{
			pieceResult = piece!;
			return true;
		}
		pieceResult = null;
		return false;
	}

	/// <summary>
	/// Gets a list of names of pieces owned by the specified player.
	/// </summary>
	/// <param name="player">The player for whom to retrieve the piece names.</param>
	/// <returns>
	/// A <see cref="List{T}"/> of strings representing the names of pieces owned by the specified player.
	/// </returns>
	public IEnumerable<string> GetPlayerPiecesName(IPlayer player) => GetPlayerPieces(player).ToList().ConvertAll(piece => piece.Name);
	
	/// <summary>
	/// Adds a new piece to the specified player based on the given hero name.
	/// </summary>
	/// <param name="player">The player to whom the new piece will be added.</param>
	/// <param name="heroName">The name of the hero associated with the new piece.</param>
	/// <returns>
	/// <c>true</c> if the piece is successfully added to the player; otherwise, <c>false</c>.
	/// </returns>
	public bool AddPlayerPiece(IPlayer player, IPiece newHero)
	{
		if(!(GetPlayerPieces(player).Count() < PlayerPiecesCount))
		{
			return false;
		}
		if(TryGetPlayerData(player, out PlayerData? result))
		{
			result!.PlayerPieces.Add(newHero);
			return true;
		}
		return false;
	}

	/// <summary>
	/// Adds multiple pieces to the specified player based on the given list of hero names.
	/// </summary>
	/// <param name="player">The player to whom the new pieces will be added.</param>
	/// <param name="heroNames">The list of hero names associated with the new pieces.</param>	
	public void AddPlayerPiece(IPlayer player, IEnumerable<IPiece> heroes)
	{
		foreach(var hero in heroes)
		{
			AddPlayerPiece(player, hero);
		}
	}

	/// <summary>
	/// Removes a specific piece from the specified player data.
	/// </summary>
	/// <param name="player">The player from whom to remove the piece.</param>
	/// <param name="piece">The piece to be removed from the player data.</param>
	/// <returns>
	/// <c>true</c> if the piece is successfully removed from the player data; otherwise, <c>false</c>.
	/// </returns>
	public bool RemovePlayerPiece(IPlayer player, IPiece piece)
	{
		if(TryGetPlayerData(player, out PlayerData? result))
		{
			return result!.PlayerPieces.Remove(piece);
		}
		return false;
	}

	/// <summary>
	/// Clears the collection of pieces owned by the specified player.
	/// </summary>
	/// <param name="player">The player whose pieces will be cleared.</param>
	public void ClearPlayerPieces(IPlayer player)
	{
		if(TryGetPlayerData(player, out PlayerData? result))
		{
			result!.PlayerPieces.Clear();
		}
	}

	/// <summary>
	/// Clears the collection of pieces owned by all players in the game.
	/// </summary>
	public void ClearPlayerPieces() => GetPlayers().ToList().ForEach(ClearPlayerPieces);
	#endregion

	// Manage board
	#region MANAGE_BOARD
	
	/// <summary>
	/// Gets the board of the specified player.
	/// </summary>
	/// <param name="player">The player for which to retrieve the board.</param>
	/// <returns>The board of the specified player.</returns>
	/// <exception cref="KeyNotFoundException">Thrown if the player is not found in the dictionary.</exception>
	public IDictionary<IPosition, Guid> GetPlayerBoard(IPlayer player)
	{
		if(_board.TryGetPlayerBoard(player, out var result))
		{
			return result!;
		}
		throw new KeyNotFoundException();
	}

	/// <summary>
	/// Tries to retrieve the board data associated with the specified player.
	/// </summary>
	/// <param name="player">The player for which to retrieve the board data.</param>
	/// <param name="playerBoardResult">When this method returns, contains the board data associated with the specified player, if available; otherwise, an empty dictionary.</param>
	/// <returns>
	/// true if the board data associated with the player was found and retrieved successfully; otherwise, false.
	/// </returns>	
	public bool TryGetPlayerBoard(IPlayer player, out IDictionary<IPosition, Guid> playerBoardResult)
	{
		if(_board.TryGetPlayerBoard(player, out var result))
		{
			playerBoardResult = result!;
			return true;
		}
		playerBoardResult = new Dictionary<IPosition, Guid>();
		return false;
	}

	/// <summary>
	/// Gets the position of the specified hero associated with the player.
	/// </summary>
	/// <param name="player">The player for which to retrieve the hero position.</param>
	/// <param name="heroId">The unique identifier of the hero for which to retrieve the position.</param>
	/// <returns>The position of the specified hero associated with the player.</returns>
	/// <exception cref="KeyNotFoundException">Thrown if the player or hero position is not found in the dictionary.</exception>
	public IPosition GetHeroPosition(IPlayer player, Guid heroId)
	{
		if(!_players.ContainsKey(player))
		{
			throw new KeyNotFoundException();
		}
		if(_board.TryGetHeroPosition(player, heroId, out IPosition? result))
		{
			return result!;
		}
		throw new KeyNotFoundException();
	}

	/// <summary>
	/// Tries to retrieve the position of a hero associated with the specified player and hero ID.
	/// </summary>
	/// <param name="player">The player for which to retrieve the hero's position.</param>
	/// <param name="heroId">The unique identifier of the hero for which to retrieve the position.</param>
	/// <param name="positionResult">When this method returns, contains the position of the hero, if available; otherwise, null.</param>
	/// <returns>
	/// true if the hero's position was found and retrieved successfully; otherwise, false.
	/// </returns>
	public bool TryGetHeroPosition(IPlayer player, Guid heroId, out IPosition? positionResult)
	{
		if(!_players.ContainsKey(player))
		{
			positionResult = null;
			return false;
		}
		if(_board.TryGetHeroPosition(player, heroId, out IPosition? result))
		{
			positionResult = result!;
			return true;
		}
		positionResult = null;
		return false;
	}

	/// <summary>
	/// Gets the positions of all heroes from all players on the boards.
	/// </summary>
	/// <returns>
	/// A <see cref="Dictionary{TKey, TValue}"/> where keys represent positions on the boards,
	/// and values represent the piece IDs of the heroes placed on those positions.
	/// </returns>
	public IDictionary<IPosition, Guid> GetAllHeroPosition()
	{
		Dictionary<IPosition, Guid> allHeroPosition = new();
		foreach(var piecesPosition in _board.PiecesPositions.Values)
		{
			foreach(var piecePosition in piecesPosition)
			{
				allHeroPosition.TryAdd(piecePosition.Key, piecePosition.Value);
			}
		}
		return allHeroPosition;
	}

	/// <summary>
	/// Updates the position of a specific piece from a specific player on the board.
	/// </summary>
	/// <param name="player">The player for whom to update the piece's position.</param>
	/// <param name="heroId">The identifier of the piece for which to update the position.</param>
	/// <param name="newPosition">The new position where the piece will be placed.</param>
	/// <returns>
	/// <c>true</c> if the piece's position is successfully updated; otherwise, <c>false</c>.
	/// </returns>
	public bool UpdateHeroPosition(IPlayer player, Guid heroId, IPosition newPosition) => _board.UpdateHeroPosition(player, heroId, newPosition);

	/// <summary>
	/// Places a specific piece from specific player to the board at the specified position.
	/// </summary>
	/// <param name="player">The player on whose board the piece will be placed.</param>
	/// <param name="piece">The piece to be placed on the player's board.</param>
	/// <param name="position">The position where the piece will be placed.</param>
	/// <returns>
	/// <c>true</c> if the piece is successfully placed on the player's board; otherwise, <c>false</c>.
	/// </returns>
	public bool PutPlayerPiece(IPlayer player, IPiece piece, IPosition position)
	{
		if(_board.TryGetHeroPosition(player, piece.PieceId, out _))
		{
			return _board.UpdateHeroPosition(player, piece.PieceId, position);
		}
		return _board.AddHeroPosition(player, piece.PieceId, position);
	}

	/// <summary>
	/// Checks whether the specified player has finished picking all heroes.
	/// </summary>
	/// <param name="player">The player to check for hero picking completion.</param>
	/// <returns>
	/// <c>true</c> if the player has finished picking all heroes; otherwise, <c>false</c>.
	/// </returns>
	public bool IsFinishedPickAllPieces(IPlayer player) => GetPlayerPieces(player).Count() == PlayerPiecesCount;

	/// <summary>
	/// Checks whether the specified player has finished putting all pieces on the board.
	/// </summary>
	/// <param name="player">The player to check for piece placement completion.</param>
	/// <returns>
	/// <c>true</c> if the player has finished putting all pieces on the board; otherwise, <c>false</c>.
	/// </returns>
	public bool IsFinishedPutAllPieces(IPlayer player)
	{
		if(_board.TryGetPlayerBoard(player, out IDictionary<IPosition, Guid>? result))
		{
			return result!.Count == GetPlayerPieces(player).Count();
		}
		return false;
	}

	/// <summary>
	/// Checks whether the specified position on the board is currently empty.
	/// </summary>
	/// <param name="newPosition">The position to check for emptiness.</param>
	/// <returns>
	/// <c>true</c> if the specified position is currently empty; otherwise, <c>false</c>.
	/// </returns>
	public bool IsValidPosition(IPosition newPosition) => !GetAllHeroPosition().ContainsKey(newPosition);

	/// <summary>
	/// Removes a hero piece from the specified player's board based on the given hero ID.
	/// </summary>
	/// <param name="player">The player from whose board to remove the hero piece.</param>
	/// <param name="heroId">The identifier of the hero to be removed from the board.</param>
	/// <returns>
	/// <c>true</c> if the hero piece is successfully removed from the board; otherwise, <c>false</c>.
	/// </returns>
	public bool RemoveHeroFromBoard(IPlayer player, Guid heroId)
	{
		if(!TryGetHeroPosition(player, heroId, out IPosition? result))
		{
			return false;
		}
		if(TryGetPlayerBoard(player, out var playerBoardResult))
		{
			return playerBoardResult!.Remove(result!);
		}
		return false;
	}

	/// <summary>
	/// Clears the boards of all players, removing all pieces from each board.
	/// </summary>
	public void ClearBoard() => GetPlayers().ToList().ForEach(player => GetPlayerBoard(player).Clear());
	#endregion

	// Manage Battle
	#region MANAGE_BATTLE
	
	/// <summary>
	/// Gets a list of piece IDs representing all enemy pieces within the attack range of the specified hero for the given player.
	/// </summary>
	/// <param name="player">The player for whom to retrieve enemy pieces.</param>
	/// <param name="hero">The hero for which to find enemy pieces.</param>
	/// <returns>
	/// A <see cref="List{T}"/> of piece IDs representing all enemy pieces within the attack range of the specified hero for the given player,
	/// or an empty enumerable if there are no enemies in range.
	/// </returns>
	public IEnumerable<Guid> GetAllEnemyId(IPlayer player, IPiece hero) => _board.GetAllEnemyId(player, hero);


	/// <summary>
	/// Performs an attack action for the specified piece belonging to the given player.
	/// </summary>
	/// <param name="player">The player who owns the attacking piece.</param>
	/// <param name="piece">The attacking piece.</param>
	public async Task Attack(IPlayer player, IPiece piece)
	{
		var skillTrigger = piece.Hp * 0.30;
		var skillStop = piece.Hp * 0.15;
		if(piece.Hp <= 0)
		{
			RemoveHeroFromBoard(player, piece.PieceId);
		}
		if(piece.Hp <= skillTrigger && piece.Hp >= skillStop)
		{
			piece.Skill(this);
			await Task.Delay(200);
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
				if(TryGetPieceById(enemyId, out IPiece? enemy) && enemy!.Hp > 0)
				{
					((Hero)piece).AttackEnemy(enemy!);
				}
			}
		}
		await Task.Delay(1000);
	}
	
	/// <summary>
	/// Sets the winner of the current round and performs corresponding actions, such as
	/// adding win points to the winner and reducing the other player's HP.
	/// </summary>
	/// <param name="winner">The player who won the current round.</param>
	public void SetRoundWinner(IPlayer winner)
	{
		foreach(var player in GetPlayers())
		{
			if(!TryGetPlayerData(player, out PlayerData? result))
			{
				continue;
			}
			if(player.PlayerId == winner.PlayerId)
			{
				result!.Win.Add(true);
			}
			else
			{
				result!.Win.Add(false);
				result!.Hp--;
			}
		}
	}

	/// <summary>
	/// Sets the specified player as the overall game winner or loser based on the provided win state.
	/// </summary>
	/// <param name="player">The player to set as the overall game winner or loser.</param>
	/// <param name="isWin">A boolean indicating whether the player is the overall game winner (true) or loser (false).</param>
	public void SetWinner(IPlayer player, bool isWin = true)
	{
		if(TryGetPlayerData(player, out PlayerData? data))
		{
			data!.Winner = isWin;
		}
	}
	#endregion

	/// <summary>
	/// Generates a list of random hero names picked from the predefined set of heroes.
	/// </summary>
	/// <returns>
	/// A <see cref="List{T}"/> of strings representing the randomly picked hero names.
	/// </returns>
	public IEnumerable<string> GenerateRandomHeroList()
	{
		var heroNames = GetHeroName().ToList();
		if(heroNames.Count == 0)
		{
			return Enumerable.Empty<string>();
		}
		int n = 4;
		List<string> options = new();
		while(n > 0)
		{
			options.Add(heroNames[new Random().Next(0, heroNames.Count)]);
			n--;
		}
		return options;
	}
}