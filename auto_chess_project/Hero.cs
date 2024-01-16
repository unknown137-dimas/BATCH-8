class Hero : IPiece, IPosition
{
    public string PieceId {get;}
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
        PieceId = Guid.NewGuid().ToString();
        Name = name;
        PieceType = pieceType;
        Hp = hp;
        Attack = attack;
        Armor = armor;
        AttackRange = attackRange;
    }

    public Hero(string name, HeroDetails heroDetails)
    {
        PieceId = Guid.NewGuid().ToString();
        Name = name;
        PieceType = heroDetails.HeroType;
        Hp = heroDetails.Hp;
        Attack = heroDetails.Attack;
        Armor = heroDetails.Armor;
        AttackRange = heroDetails.AttackRange;
    }

    public int[] GetPosition() => [X, Y];

    public virtual void Move(int newX, int newY)
    {
        X = newX;
        Y = newY;
    }

    public virtual void Move(IPosition newPosition)
    {
        X = newPosition.X;
        Y = newPosition.Y;
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