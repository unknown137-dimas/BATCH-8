using AutoChess;

public class Warrior : Hero
{
    public Warrior(string name, HeroDetails heroDetails) : base(name, heroDetails)
    {
    }

    public Warrior(string name, PieceTypes heroType, double hp, double attack, double armor, int attackRange) : base(name, heroType, hp, attack, armor, attackRange)
    {
    }

    public override void Skill()
    {
        Armor *= 1.1;
    }
}