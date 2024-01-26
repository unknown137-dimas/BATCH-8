using AutoChess;

public class Assassin : Hero
{
	public Assassin(string name, HeroDetails heroDetails) : base(name, heroDetails)
	{
	}

    public Assassin(string name, PieceTypes heroType, double hp, double attack, double armor, int attackRange) : base(name, heroType, hp, attack, armor, attackRange)
    {
    }

    public override void Skill(GameController gameController)
	{
		Attack *= 1.1;
	}
}