interface IBoard
{
    int Width {get;}
    int Height {get;}
}

interface IPlayer
{
    string Id {get;}
    string Name {get;}
}

interface IPosition
{
    int X {get;}
    int Y {get;}
    public void Move(int newX, int newY);
    public int[] GetPosition();
}

interface IPiece
{
    Guid PieceId {get;}
    string Name {get;}
    int Hp {get;}
    int Attack {get;}
    int Armor {get;}
    int AttackRange {get;}
    PieceTypes PieceType {get;}
    public Guid GetId();
    public PieceTypes GetPieceTypes();
    public IEnumerable<IPiece> GetTarget();
    public void Skill();
}