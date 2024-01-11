
abstract class Archer : IPiece, IPosition
{
    public Guid PieceId {get;}
    public string Name {get;}
    public PieceTypes PieceType {get;}
    public int Hp {get;}
    public int Damage {get;}
    public int AttackRange {get;}
    public int X {get; internal set;}
    public int Y {get; internal set;}

    public Archer(string name, int hp, int damage, int attackRange)
    {
        PieceId = Guid.NewGuid();
        Name = name;
        PieceType = PieceTypes.Archer;
        Hp = hp;
        Damage = damage;
        AttackRange = attackRange;
    }

    public Guid GetId() => PieceId;

    public PieceTypes GetPieceTypes() => PieceType;

    public int[] GetPosition() => [X, Y];

    public IPiece GetTarget()
    {
        throw new NotImplementedException();
    }

    public void Move(int newX, int newY)
    {
        X = newX;
        Y = newY;
    }
}