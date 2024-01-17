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
}