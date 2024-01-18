interface IBoard
{
    int Width {get;}
    int Height {get;}
    Dictionary<IPlayer, Dictionary<Position, string>> PiecesPositions {get;}

    public bool AddPlayerToBoard(IPlayer player);
    public bool RemovePlayerFromBoard(IPlayer player);
    public Dictionary<Position, string> GetPlayerBoard(IPlayer player);
    public bool IsPositionEmpty(IPlayer player, Position position);
}