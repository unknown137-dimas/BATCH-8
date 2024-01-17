interface IBoard
{
    int Width {get;}
    int Height {get;}
    Dictionary<IPlayer, Dictionary<Position, Hero>> PiecesPositions {get;}

    public bool AddPlayer(IPlayer player);
    public bool RemovePlayer(IPlayer player);
    public bool IsPositionEmpty(IPlayer player, Position position);
    public bool AddHeroPosition(IPlayer player, Hero hero, Position position);
    // TODO
    // 1. Add Board Collection? Nested List?
    // 2. Add GetBoard Method
    // 3. Add UpdateHeroPosition Method
    // 4. ADd RemoveHero Method
}