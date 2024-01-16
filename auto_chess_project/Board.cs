class Board : IBoard
{
    public int Width {get;}
    public int Height {get;}
    // TODO
    // Seperate between players
    private Dictionary<Position, Hero> _piecePositions = new();

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

    public bool IsPositionEmpty(Position position) => !_piecePositions.ContainsKey(position);
    
    public bool AddHeroPosition(Hero hero, Position position) => _piecePositions.TryAdd(position, hero);
}