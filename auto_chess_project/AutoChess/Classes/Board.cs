class Board : IBoard
{
	public int Width {get;}
	public int Height {get;}
	public Dictionary<IPlayer, Dictionary<IPosition, string>> PiecesPositions {get; private set;} = new();

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

	public bool AddPlayerToBoard(IPlayer player) => PiecesPositions.TryAdd(player, new Dictionary<IPosition, string>());
	
	public bool RemovePlayerFromBoard(IPlayer player) => PiecesPositions.Remove(player);
	
	public Dictionary<IPosition, string> GetPlayerBoard(IPlayer player) => PiecesPositions[player];

	public bool IsPositionEmpty(IPlayer player, IPosition position) => !GetPlayerBoard(player).ContainsKey(position);
	
	public bool AddHeroPosition(IPlayer player, string heroId, IPosition position) => GetPlayerBoard(player).TryAdd(position, heroId);

	public bool UpdateHeroPosition(IPlayer player, string heroId, IPosition newPosition)
	{
		bool result = false;
		foreach(var playerPiece in GetPlayerBoard(player))
		{
			if(playerPiece.Value == heroId)
			{
				var pieceId = playerPiece.Value;
				if(RemoveHero(player, heroId))
				{
					result = AddHeroPosition(player, pieceId, newPosition);
				}
			}
		}
		return result;
	}
	
	public IPosition? GetHeroPosition(IPlayer player, string heroId)
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

	public bool RemoveHero(IPlayer player, string heroId)
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

	public IEnumerable<string> GetAllEnemyId(IPlayer player, IPiece hero)
	{
		List<string> result = new();
		IPosition? heroCurrentPosition = GetHeroPosition(player, hero.PieceId);
		if(heroCurrentPosition is not null)
		{
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
		}
		return result;
	}
}