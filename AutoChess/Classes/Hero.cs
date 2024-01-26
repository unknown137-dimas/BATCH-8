using AutoChess;

public class Hero : IPiece
{
    public Guid PieceId {get;}
    public string Name {get;}
    public PieceTypes PieceType {get;}
    public double Hp {get; set;}
    public double Attack {get; set;}
    public double Armor {get; set;}
    public int AttackRange {get; set;}

    public Hero(string name, HeroDetails heroDetails)
    {
        PieceId = Guid.NewGuid();
        Name = name;
        PieceType = heroDetails.HeroType;
        Hp = heroDetails.Hp;
        Attack = heroDetails.Attack;
        Armor = heroDetails.Armor;
        AttackRange = heroDetails.AttackRange;
    }

    public Hero(string name, PieceTypes heroType, double hp, double attack, double armor, int attackRange)
    {
        PieceId = Guid.NewGuid();
        Name = name;
        PieceType = heroType;
        Hp = hp;
        Attack = attack;
        Armor = armor;
        AttackRange = attackRange;
    }

    public virtual void Skill()
    {
    }
    
    public virtual void Skill(GameController gameController)
    {
    }

    public void AttackEnemy(IPiece otherPiece)
    {
        otherPiece.Hp -= Attack - otherPiece.Armor;
    }

    public override string ToString() => Name;
}