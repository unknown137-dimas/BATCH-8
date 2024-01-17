class Board : IBoard
{
    public int Width {get;}
    public int Height {get;}
    // TODO
    // Seperate between players
    public Dictionary<IPlayer, Dictionary<Position, Hero>> PiecesPositions {get; private set;} = new();

    public Board(int size)
    {
        Width = size;
        Height = size;
    }

    public Board(int width, int height)
    {
        Width = width;
        Height = height;
    }

    public bool AddPlayer(IPlayer player) => PiecesPositions.TryAdd(player, new Dictionary<Position, Hero>());
    
    public bool RemovePlayer(IPlayer player) => PiecesPositions.Remove(player);

    public bool IsPositionEmpty(IPlayer player, Position position) => !PiecesPositions[player].ContainsKey(position);
    
    public bool AddHeroPosition(IPlayer player, Hero hero, Position position) => PiecesPositions[player].TryAdd(position, hero);
}