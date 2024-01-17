interface IBoard
{
    int Width {get;}
    int Height {get;}
    Dictionary<IPlayer, Dictionary<Position, Hero>> PiecesPositions {get;}

    public bool AddPlayerToBoard(IPlayer player);
    public bool RemovePlayerFromBoard(IPlayer player);
    public Dictionary<Position, Hero> GetPlayerBoard(IPlayer player);
    public bool IsPositionEmpty(IPlayer player, Position position);
    public bool AddHeroPosition(IPlayer player, Hero hero, Position position);
    public bool UpdateHeroPosition(IPlayer player, string heroId, Position newPosition);
    public bool RemoveHero(IPlayer player, string heroId);
}