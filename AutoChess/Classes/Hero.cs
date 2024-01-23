public class Hero : IPiece
{
    public string PieceId {get;}
    public string Name {get;}
    public PieceTypes PieceType {get;}
    public double Hp {get; set;}
    public double Attack {get;}
    public double Armor {get;}
    public int AttackRange {get;}

    public Hero(string name, PieceTypes heroType, double hp, double attack, double armor, int attackRange)
    {
        PieceId = Guid.NewGuid().ToString();
        Name = name;
        PieceType = heroType;
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

    public virtual void Skill()
    {
        throw new NotImplementedException();
    }

    public void AttackEnemy(IPiece otherPiece)
    {
        otherPiece.Hp -= Attack - otherPiece.Armor;
    }

    public override string ToString() => Name;
}