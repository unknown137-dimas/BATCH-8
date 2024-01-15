class Hero : IPiece, IPosition
{
    public Guid PieceId {get;}
    public string Name {get;}
    public PieceTypes PieceType {get;}
    public double Hp {get;}
    public double Attack {get;}
    public double Armor {get;}
    public double AttackRange {get;}
    public int X {get; internal set;}
    public int Y {get; internal set;}

    public Hero(string name, PieceTypes pieceType, double hp, double attack, double armor, double attackRange)
    {
        PieceId = Guid.NewGuid();
        Name = name;
        PieceType = pieceType;
        Hp = hp;
        Attack = attack;
        Armor = armor;
        AttackRange = attackRange;
    }

    public Guid GetId() => PieceId;

    public PieceTypes GetPieceTypes() => PieceType;

    public int[] GetPosition() => [X, Y];

    public virtual void Move(int newX, int newY)
    {
        X = newX;
        Y = newY;
    }

    public virtual IEnumerable<IPiece> GetTarget()
    {
        throw new NotImplementedException();
    }

    public virtual void Skill()
    {
        throw new NotImplementedException();
    }

    public override string ToString() => Name;
}