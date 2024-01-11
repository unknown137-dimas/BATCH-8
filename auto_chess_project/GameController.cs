class GameController
{
    private readonly IBoard _board;
    private readonly Dictionary<PieceTypes, int> _heroSlot = new() {{PieceTypes.Warrior, 3}, {PieceTypes.Archer, 3}, {PieceTypes.Tanker, 3}};
    private readonly Dictionary<IPlayer, PlayerData>? _players;

    public GameController(IBoard board, List<IPlayer> players)
    {
        _board = board;
        foreach(var player in players)
        {
            _players[player] = new PlayerData();
        }
    }
    public GameController(IBoard board, List<IPlayer> players, Dictionary<PieceTypes, int> heroSlot)
    {
        _board = board;
        _heroSlot = heroSlot;
        foreach(var player in players)
        {
            _players[player] = new PlayerData();
        }
    }
    public bool AddPiece(IPlayer player, IPiece piece)
    {
        throw new NotImplementedException();
    }
    public void NextPhase()
    {

    }
    public IPlayer GetPlayer()
    {
        throw new NotImplementedException();
    }
    public bool RandomizePlayerSide(IPlayer player)
    {
        throw new NotImplementedException();
    }
    public Sides GetPlayerSide()
    {
        throw new NotImplementedException();
    }
    public int[] GetBoardSize()
    {
        throw new NotImplementedException();
    }
    public IEnumerable<IPiece> GetPlayerPiece(IPlayer player)
    {
        throw new NotImplementedException();
    }
    public bool PutPiece(IPlayer player, IPiece piece, IPosition position)
    {
        throw new NotImplementedException();
    }
    public bool CheckTargetAcquitition(IPlayer player, IPiece piece, IPosition position)
    {
        throw new NotImplementedException();
    }
    public IEnumerable<IPosition> GetAllPossibleMove(IPlayer player, IPiece piece)
    {
        throw new NotImplementedException();
    }
    public int Attack(IPlayer player, IPiece piece, IPlayer enemyPlayer, IPiece enemyPiece)
    {
        throw new NotImplementedException();
    }
    public bool SetWinner(IPlayer player)
    {
        throw new NotImplementedException();
    }
}