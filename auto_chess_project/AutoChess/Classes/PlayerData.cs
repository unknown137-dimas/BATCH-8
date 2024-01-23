class PlayerData
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

    public IPiece? GetPieceById(string heroId)
    {
        foreach(var piece in PlayerPieces)
        {
            if(piece.PieceId == heroId)
            {
                return piece;
            }
        }
        return null;
    }

    public int GetWinPoint()
    {
        int winPoint = 0;
        Win.ForEach(x => {
            if(x)
            {
                winPoint++;
            }
        });
        return winPoint;
    }
}