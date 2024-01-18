interface IBoard
{
    int Width {get;}
    int Height {get;}
    Dictionary<IPlayer, Dictionary<IPosition, string>> PiecesPositions {get;}

    public bool AddPlayerToBoard(IPlayer player);
    public bool RemovePlayerFromBoard(IPlayer player);
    public Dictionary<IPosition, string> GetPlayerBoard(IPlayer player);
}