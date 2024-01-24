public interface IBoard
{
    int Width {get;}
    int Height {get;}
    Dictionary<IPlayer, Dictionary<IPosition, Guid>> PiecesPositions {get;}

    bool AddPlayerToBoard(IPlayer player);
    bool RemovePlayerFromBoard(IPlayer player);
    Dictionary<IPosition, Guid> GetPlayerBoard(IPlayer player);
}