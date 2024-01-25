public class Player : IPlayer
{
    public Guid PlayerId  {get;}
    public string Name {get;}

    public Player(string name)
    {
        PlayerId = Guid.NewGuid();
        Name = name;
    }
}