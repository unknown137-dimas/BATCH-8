class GameController
{
    private readonly IBoard _board;
    private readonly Dictionary<PieceTypes, int> _heroSlot = new() {{PieceTypes.Warrior, 3}, {PieceTypes.Hunter, 3}, {PieceTypes.Knight, 3}};
    private Dictionary<IPlayer, PlayerData> _players = new();
    public int PlayerHp {get; set;} = 10;
    public Status CurrentGameStatus {get; set;} = Status.NotInitialized;
    public Phases CurrentGamePhase {get; set;} = Phases.NotInitialized;

    public GameController(IBoard board)
    {
        CurrentGameStatus = Status.Initialized;
        _board = board;
    }

    public GameController(IBoard board, Dictionary<PieceTypes, int> heroSlot)
    {
        CurrentGameStatus = Status.Initialized;
        _board = board;
        _heroSlot = heroSlot;
    }

    public GameController(IBoard board, List<IPlayer> players)
    {
        CurrentGameStatus = Status.Initialized;
        _board = board;
        AddPlayer(players);
    }

    public GameController(IBoard board, List<IPlayer> players, Dictionary<PieceTypes, int> heroSlot)
    {
        CurrentGameStatus = Status.Initialized;
        _board = board;
        _heroSlot = heroSlot;
        AddPlayer(players);
    }

    // Get current game information
    public IEnumerable<IPlayer> GetPlayers() => _players.Keys;

    public int[] GetBoardSize() => [_board.Width, _board.Height];

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
    public IEnumerable<Hero> GetPlayerPieces(IPlayer player) => _players[player].PlayerPieces;
    public void AddPlayerPiece(IPlayer player, Hero piece) => _players[player].PlayerPieces.Add(piece);

    public void AddPlayerPiece(IPlayer player, IEnumerable<Hero> pieces) => _players[player].PlayerPieces.AddRange(pieces);

    public bool RemovePlayerPiece(IPlayer player, Hero piece) => _players[player].PlayerPieces.Remove(piece);

    public bool PutPlayerPiece(Hero piece, Position position)
    {
        if(_board.IsPositionEmpty(position))
        {
            if(_board.AddHeroPosition(piece, position))
            {
                piece.Move(position);
                return true;
            }
            return false;
        }
        return false;
    }

    public bool IsFinishedPutAllPieces(IPlayer player)
    {
        List<bool> piecePosition = new();
        foreach(var piece in GetPlayerPieces(player))
        {
            piecePosition.Add(Array.IndexOf(piece.GetPosition(), -1) >= 0 ? false : true);
        }
        return !piecePosition.Contains(false);
    }

    public bool IsValidPosition(Hero piece, Hero otherPiece) => piece.HeroPosition.X != otherPiece.HeroPosition.X || piece.HeroPosition.Y != otherPiece.HeroPosition.Y;

    public void NextPhase()
    {
        throw new NotImplementedException();
    }

    public bool CheckTargetAcquitition(IPlayer player, Hero piece, IPosition position)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IPosition> GetAllPossibleMove(IPlayer player, Hero piece)
    {
        throw new NotImplementedException();
    }
    
    public int Attack(IPlayer player, Hero piece, IPlayer enemyPlayer, Hero enemyPiece)
    {
        throw new NotImplementedException();
    }
    
    // Set current round winner
    public bool SetWinner(IPlayer player) => _players[player].Winner = true;

    // Generate random options
    public IEnumerable<T> GenerateRandomHeroList<T>(in List<T> source)
    {
        Random random = new();
        List<T> options = new();
        int n = 5;
        while(n > 0)
        {
            options.Add(source[random.Next(0, source.Count)]);
            n--;
        }
        return options;
    }
}