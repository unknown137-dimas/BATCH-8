public struct HeroDetails
{
    public PieceTypes HeroType;
    public double Hp;
    public double Attack;
    public double Armor;
    public double AttackRange;
    public HeroDetails(PieceTypes heroType, double hp, double attack, double armor, double attackRange)
    {
        HeroType = heroType;
        Hp = hp;
        Attack = attack;
        Armor = armor;
        AttackRange = attackRange;
    }
}