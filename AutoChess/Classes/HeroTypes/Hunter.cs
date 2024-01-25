using AutoChess;

public class Hunter : Hero
{
	public Hunter(string name, PieceTypes heroType, double hp, double attack, double armor, int attackRange) : base(name, heroType, hp, attack, armor, attackRange)
	{
	}

    public override void Skill()
    {
        Attack *= 1.05;
    }
}