class PlayerData
{
    public List<Hero> PlayerPieces = new();
    public bool Winner;
    public int Hp;
    public int Win;

    public PlayerData(int hp)
    {
        Hp = hp;
    }

    public Hero? GetHeroById(string heroId)
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