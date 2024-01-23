public class Player : IPlayer
{
    public string PlayerId  {get;}
    public string Name {get;}

    public Player(string name)
    {
        PlayerId = Guid.NewGuid().ToString();
        Name = name;
    }
}