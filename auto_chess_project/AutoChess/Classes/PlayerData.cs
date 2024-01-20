class PlayerData
{
    public List<IPiece> PlayerPieces = new();
    public bool Winner;
    public int Hp;
    public int Win;

    public Sides PlayerSide;

    public PlayerData(int hp, Sides playerSide)
    {
        Hp = hp;
        PlayerSide = playerSide;
    }

    public IPiece? GetHeroById(string heroId)
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
}