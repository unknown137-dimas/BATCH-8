class GameController
{
    private readonly IBoard _board;
    private readonly Dictionary<PieceTypes, int> _heroSlot = new() {{PieceTypes.Warrior, 3}, {PieceTypes.Hunter, 3}, {PieceTypes.Knight, 3}};
    private Dictionary<IPlayer, PlayerData> _players = new();
    public int PlayerHp {get; set;} = 10;
    private GameStatus _gameStatus = GameStatus.NotInitialized;
    private PhaseStatus _gamePhase = PhaseStatus.NotInitialized;

    public GameController(IBoard board)
    {
        _gameStatus = GameStatus.Initialized;
        _board = board;
    }

    public GameController(IBoard board, Dictionary<PieceTypes, int> heroSlot)
    {
        _gameStatus = GameStatus.Initialized;
        _board = board;
        _heroSlot = heroSlot;
    }

    public GameController(IBoard board, List<IPlayer> players)
    {
        _gameStatus = GameStatus.Initialized;
        _board = board;
        AddPlayer(players);
    }

    public GameController(IBoard board, List<IPlayer> players, Dictionary<PieceTypes, int> heroSlot)
    {
        _gameStatus = GameStatus.Initialized;
        _board = board;
        _heroSlot = heroSlot;
        AddPlayer(players);
    }

    // Get current game information
    public IEnumerable<IPlayer> GetPlayers() => _players.Keys;

    public int[] GetBoardSize() => [_board.Width, _board.Height];

    public IEnumerable<IPiece> GetPlayerPieces(IPlayer player) => _players[player].PlayerPieces;

    // Manage player
    public bool AddPlayer(IPlayer newPlayer) => _players.TryAdd(newPlayer, new PlayerData(PlayerHp));

    public void AddPlayer(IEnumerable<IPlayer> newPlayers)
    {
        foreach(var player in newPlayers)
        {
            AddPlayer(player);
        }
    }

    public bool RemovePlayer(IPlayer player) => _players.Remove(player);

    public PlayerData GetPlayerData(IPlayer player) => _players[player];

    // Manage player's piece
    public void AddPiece(IPlayer player, IPiece piece) => _players[player].PlayerPieces.Add(piece);

    public bool RemovePiece(IPlayer player, IPiece piece) => _players[player].PlayerPieces.Remove(piece);

    public void NextPhase()
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
    
    // Set current round winner
    public bool SetWinner(IPlayer player) => _players[player].Winner = true;

    // Generate random options
    public void GenerateRandomPick(in List<Hero> source, ref List<Hero> options)
    {
        var random = new Random();
        options.Clear();
        int n = 5;
        while(n > 0)
        {
            options.Add(source[random.Next(0,source.Count)]);
            n--;
        }
    }
}