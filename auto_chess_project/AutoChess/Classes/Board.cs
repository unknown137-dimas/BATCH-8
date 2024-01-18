class Board : IBoard
{
	public int Width {get;}
	public int Height {get;}
	public Dictionary<IPlayer, Dictionary<Position, Hero>> PiecesPositions {get; private set;} = new();

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

	public bool AddPlayerToBoard(IPlayer player) => PiecesPositions.TryAdd(player, new Dictionary<Position, Hero>());
	
	public bool RemovePlayerFromBoard(IPlayer player) => PiecesPositions.Remove(player);
	
	public Dictionary<Position, Hero> GetPlayerBoard(IPlayer player) => PiecesPositions[player];

	public bool IsPositionEmpty(IPlayer player, Position position) => !GetPlayerBoard(player).ContainsKey(position);
	
	public bool AddHeroPosition(IPlayer player, Hero hero, Position position) => GetPlayerBoard(player).TryAdd(position, hero);

	public bool UpdateHeroPosition(IPlayer player, string heroId, Position newPosition)
	{
		bool result = false;
		foreach(var playerPiece in GetPlayerBoard(player))
		{
			if(playerPiece.Value.PieceId == heroId)
			{
				var piece = playerPiece.Value;
				if(RemoveHero(player, heroId))
				{
					result = AddHeroPosition(player, piece, newPosition);
				}
			}
		}
		return result;
	}
	
	public Position GetHeroPosition(IPlayer player, string heroId)
	{
		foreach(var playerPiece in GetPlayerBoard(player))
		{
			if(playerPiece.Value.PieceId == heroId)
			{
				return playerPiece.Key;
			}
		}
		return new Position();
	}

	public bool RemoveHero(IPlayer player, string heroId)
	{
		bool result = false;
		foreach(var playerPiece in GetPlayerBoard(player))
		{
			if(playerPiece.Value.PieceId == heroId)
			{
				result = GetPlayerBoard(player).Remove(playerPiece.Key);
			}
		}
		return result;
	}

	public IEnumerable<Hero> GetAllEnemy(IPlayer player, Hero hero)
	{
		List<Hero> result = new();
		Position? heroCurrentPosition = GetHeroPosition(player, hero.PieceId);
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