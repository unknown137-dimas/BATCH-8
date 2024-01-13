class GameController
{
    private readonly IBoard _board;
    private readonly Dictionary<PieceTypes, int> _heroSlot = new() {{PieceTypes.Warrior, 3}, {PieceTypes.Hunter, 3}, {PieceTypes.Knight, 3}};
    private readonly Dictionary<IPlayer, PlayerData>? _players;
    private GameStatus _gameStatus = GameStatus.NotInitialized;
    private PhaseStatus _gamePhase = PhaseStatus.NotInitialized;

    public GameController(IBoard board, List<IPlayer> players)
    {
        _gameStatus = GameStatus.Initialized;
        _board = board;
        foreach(var player in players)
        {
            _players[player] = new PlayerData();
        }
    }

    public GameController(IBoard board, List<IPlayer> players, Dictionary<PieceTypes, int> heroSlot)
    {
        _gameStatus = GameStatus.Initialized;
        _board = board;
        _heroSlot = heroSlot;
        foreach(var player in players)
        {
            _players[player] = new PlayerData();
        }
    }

    public void AddPiece(IPlayer player, IPiece piece) => _players[player].PlayerPieces.Add(piece);

    public bool RemovePiece(IPlayer player, IPiece piece) => _players[player].PlayerPieces.Remove(piece);

    public void NextPhase()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IPlayer> GetPlayers() => _players.Keys;

    public int[] GetBoardSize() => [_board.Width, _board.Height];

    public IEnumerable<IPiece> GetPlayerPieces(IPlayer player) => _players[player].PlayerPieces;

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
    
    public bool SetWinner(IPlayer player) => _players[player].Winner = true;
}