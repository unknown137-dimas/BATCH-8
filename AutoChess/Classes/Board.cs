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
	
	public IPosition? GetHeroPosition(IPlayer player, Guid heroId)
	{
		foreach(var playerPiece in GetPlayerBoard(player))
		{
			if(playerPiece.Value == heroId)
			{
				return playerPiece.Key;
			}
		}
		return null;
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
		IPosition? heroCurrentPosition = GetHeroPosition(player, hero.PieceId);
		if(heroCurrentPosition is not null)
		{
			List<Guid> result = new();
			foreach(var playerBoard in PiecesPositions)
			{
				if(playerBoard.Key != player)
				{
					foreach(var enemyHero in playerBoard.Value)
					{
						if(heroCurrentPosition.IsInRange(enemyHero.Key, hero.AttackRange))
						{
							result.Add(enemyHero.Value);
						}
					}
				}
			}
			return result;
		}
		return Enumerable.Empty<Guid>();
	}
}