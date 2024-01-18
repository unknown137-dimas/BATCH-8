public struct HeroDetails
{
    public PieceTypes HeroType;
    public double Hp;
    public double Attack;
    public double Armor;
    public int AttackRange;
    public HeroDetails(PieceTypes heroType, double hp, double attack, double armor, int attackRange)
    {
        HeroType = heroType;
        Hp = hp;
        Attack = attack;
        Armor = armor;
        AttackRange = attackRange;
    }
}