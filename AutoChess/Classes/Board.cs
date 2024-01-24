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
	
	public Dictionary<IPosition, Guid> GetPlayerBoard(IPlayer player) => PiecesPositions[player];

	public bool AddHeroPosition(IPlayer player, Guid heroId, IPosition position) => GetPlayerBoard(player).TryAdd(position, heroId);

	public bool UpdateHeroPosition(IPlayer player, Guid heroId, IPosition newPosition)
	{
		bool result = false;
		if(GetHeroPosition(player, heroId) != null)
		{
			if(RemoveHeroPosition(player, heroId))
			{
				result = AddHeroPosition(player, heroId, newPosition);
			}
		}
		return result;
	}
	
	public IPosition GetHeroPosition(IPlayer player, Guid heroId)
	{
		foreach(var playerPiece in GetPlayerBoard(player))
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
		foreach(var playerPiece in GetPlayerBoard(player))
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
		bool result = false;
		foreach(var playerPiece in GetPlayerBoard(player))
		{
			if(playerPiece.Value == heroId)
			{
				result = GetPlayerBoard(player).Remove(playerPiece.Key);
			}
		}
		return result;
	}

	public IEnumerable<Guid> GetAllEnemyId(IPlayer player, IPiece hero)
	{
		if(TryGetHeroPosition(player, hero.PieceId, out IPosition? result))
		{
			List<Guid> allEnemyId = new();
			foreach(var playerBoard in PiecesPositions)
			{
				if(playerBoard.Key != player)
				{
					foreach(var enemyHero in playerBoard.Value)
					{
						if(result!.IsInRange(enemyHero.Key, hero.AttackRange))
						{
							allEnemyId.Add(enemyHero.Value);
						}
					}
				}
			}
			return allEnemyId;
		}
		return Enumerable.Empty<Guid>();
	}
}