class Hero : IPiece
{
    public string PieceId {get;}
    public string Name {get;}
    public PieceTypes PieceType {get;}
    public double Hp {get;}
    public double Attack {get;}
    public double Armor {get;}
    public double AttackRange {get;}
    public Position HeroPosition = new();

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

    public int[] GetPosition() => HeroPosition.GetPosition();

    public void Move(int newX, int newY) => HeroPosition.Move(newX, newY);
    public void Move(Position newPosition) => HeroPosition.Move(newPosition.X, newPosition.Y);

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