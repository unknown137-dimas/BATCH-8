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

    public bool IsPositionEmpty(IPlayer player, Position position) => !PiecesPositions[player].ContainsKey(position);
    
    public bool AddHeroPosition(IPlayer player, Hero hero, Position position) => PiecesPositions[player].TryAdd(position, hero);

    public bool UpdateHeroPosition(IPlayer player, string heroId, Position newPosition)
    {
        bool result = false;
        foreach(var playerPiece in PiecesPositions[player])
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

    public bool RemoveHero(IPlayer player, string heroId)
    {
        bool result = false;
        foreach(var playerPiece in PiecesPositions[player])
        {
            if(playerPiece.Value.PieceId == heroId)
            {
                result = PiecesPositions[player].Remove(playerPiece.Key);
            }
        }
        return result;
    }

    public IEnumerable<Hero> GetAllEnemy(IPlayer player, Hero hero)
    {
        List<Hero> result = new();
        var heroCurrentPosition = hero.HeroPosition;
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
}