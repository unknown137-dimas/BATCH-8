using AutoChess;

public class Mech : Hero
{
	public Mech(string name, PieceTypes heroType, double hp, double attack, double armor, int attackRange) : base(name, heroType, hp, attack, armor, attackRange)
	{
	}

    public override void Skill()
    {
        Armor *= 1.02;
        Attack *= 1.02;
    }
}