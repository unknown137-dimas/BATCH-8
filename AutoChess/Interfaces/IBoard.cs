public interface IBoard
{
    int Width {get;}
    int Height {get;}
    Dictionary<IPlayer, Dictionary<IPosition, string>> PiecesPositions {get;}

    bool AddPlayerToBoard(IPlayer player);
    bool RemovePlayerFromBoard(IPlayer player);
    Dictionary<IPosition, string> GetPlayerBoard(IPlayer player);
}