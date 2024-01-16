interface IBoard
{
    int Width {get;}
    int Height {get;}
}

interface IPlayer
{
    string PlayerId {get;}
    string Name {get;}
}

interface IPosition
{
    int X {get;}
    int Y {get;}
    public void Move(int newX, int newY);
    public void Move(IPosition newPosition);
    public int[] GetPosition();
    public bool IsValidPosition(int otherX, int otherY);
    public bool IsValidPosition(IPosition otherPosition);
}

interface IPiece
{
    string PieceId {get;}
    string Name {get;}
    double Hp {get;}
    double Attack {get;}
    double Armor {get;}
    double AttackRange {get;}
    PieceTypes PieceType {get;}
    public IEnumerable<IPiece> GetTarget();
    public void Skill();
}