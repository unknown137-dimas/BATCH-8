class Board : IBoard
{
    public int Width {get;}
    public int Height {get;}
    // TODO
    // Seperate between players
    private Dictionary<IPlayer, Dictionary<Position, Hero>> _piecesPositions = new();

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

    public bool IsPositionEmpty(IPlayer player, Position position) => !_piecesPositions[player].ContainsKey(position);
    
    public bool AddHeroPosition(IPlayer player, Hero hero, Position position) => _piecesPositions[player].TryAdd(position, hero);
}