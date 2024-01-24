public class PlayerData
{
    public List<IPiece> PlayerPieces = new();
    public bool Winner;
    public int Hp;
    public List<bool> Win = new();

    public Sides PlayerSide;

    public PlayerData(int hp, Sides playerSide)
    {
        Hp = hp;
        PlayerSide = playerSide;
    }

    public IPiece GetPieceById(Guid heroId)
    {
        foreach(var piece in PlayerPieces)
        {
            if(piece.PieceId == heroId)
            {
                return piece;
            }
        }
        throw new KeyNotFoundException();
    }

    public bool TryGetPieceById(Guid heroId, out IPiece? pieceResult)
    {
        foreach(var piece in PlayerPieces)
        {
            if(piece.PieceId == heroId)
            {
                pieceResult = piece;
                return true;
            }
        }
        pieceResult = null;
        return false;
    }


    public int GetWinPoint()
    {
        int winPoint = 0;
        Win.ForEach(win => {
            if(win)
            {
                winPoint++;
            }
        });
        return winPoint;
    }
}