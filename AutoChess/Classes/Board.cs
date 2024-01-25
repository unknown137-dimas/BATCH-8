public class Board : IBoard
{
	public int Width {get;}
	public int Height {get;}
	public Dictionary<IPlayer, Dictionary<IPosition, Guid>> PiecesPositions {get; private set;} = new();

	public Board(int size)
	{
		Width = size;
		Height = size;
	}

	public Board(int width, int height)
	{
		Width = width;
		Height = height;
	}

	public bool AddPlayerToBoard(IPlayer player) => PiecesPositions.TryAdd(player, new Dictionary<IPosition, Guid>());
	
	public bool RemovePlayerFromBoard(IPlayer player) => PiecesPositions.Remove(player);
	
	public Dictionary<IPosition, Guid> GetPlayerBoard(IPlayer player)
	{
		if(PiecesPositions.TryGetValue(player, out Dictionary<IPosition, Guid>? playerBoardResult))
		{
			return playerBoardResult;
		}
		throw new KeyNotFoundException();
	}

	public bool TryGetPlayerBoard(IPlayer player, out Dictionary<IPosition, Guid>? playerBoardResult)
	{
		if(PiecesPositions.TryGetValue(player, out Dictionary<IPosition, Guid>? result))
		{
			playerBoardResult = result;
			return true;
		}
		playerBoardResult = null;
		return false;
	}

	public bool AddHeroPosition(IPlayer player, Guid heroId, IPosition position)
	{
		if(TryGetPlayerBoard(player, out Dictionary<IPosition, Guid>? result))
		{
			return result!.TryAdd(position, heroId);
		}
		return false;
	}

	public bool UpdateHeroPosition(IPlayer player, Guid heroId, IPosition newPosition)
	{
		if(!TryGetHeroPosition(player, heroId, out _))
		{
			return false;
		}
		if(RemoveHeroPosition(player, heroId))
		{
			return AddHeroPosition(player, heroId, newPosition);
		}
		return false;
	}
	
	public IPosition GetHeroPosition(IPlayer player, Guid heroId)
	{
		if(!TryGetPlayerBoard(player, out Dictionary<IPosition, Guid>? result))
		{
			throw new KeyNotFoundException();
		}
		foreach(var playerPiece in result!)
		{
			if(playerPiece.Value == heroId)
			{
				return playerPiece.Key;
			}
		}
		throw new KeyNotFoundException();
	}

	public bool TryGetHeroPosition(IPlayer player, Guid heroId, out IPosition? positionResult)
	{
		if(!TryGetPlayerBoard(player, out Dictionary<IPosition, Guid>? result))
		{
			positionResult = null;
			return false;
		}
		foreach(var playerPiece in result!)
		{
			if(playerPiece.Value == heroId)
			{
				positionResult = playerPiece.Key;
				return true;
			}
		}
		positionResult = null;
		return false;
	}

	public bool RemoveHeroPosition(IPlayer player, Guid heroId)
	{
		if(!TryGetPlayerBoard(player, out Dictionary<IPosition, Guid>? boardResult))
		{
			return false;
		}
		foreach(var playerPiece in boardResult!)
		{
			if(playerPiece.Value == heroId)
			{
				return boardResult.Remove(playerPiece.Key);
			}
		}
		return false;
	}

	public IEnumerable<Guid> GetAllEnemyId(IPlayer player, IPiece hero)
	{
		if(!TryGetHeroPosition(player, hero.PieceId, out IPosition? result))
		{
			return Enumerable.Empty<Guid>();
		}
		List<Guid> allEnemyId = new();
		foreach(var playerBoard in PiecesPositions)
		{
			if(playerBoard.Key == player)
			{
				continue;
			}
			foreach(var enemyHero in playerBoard.Value)
			{
				if(!result!.IsInRange(enemyHero.Key, hero.AttackRange))
				{
					continue;
				}
				allEnemyId.Add(enemyHero.Value);
			}
		}
		return allEnemyId;
	}
}