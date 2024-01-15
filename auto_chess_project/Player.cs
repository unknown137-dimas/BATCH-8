class Player : IPlayer
{
    public string Id  {get;}
    public string Name {get;}

    public Player(string name)
    {
        Id = new Guid().ToString();
        Name = name;
    }
}