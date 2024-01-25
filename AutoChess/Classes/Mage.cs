using AutoChess;

public class Mage : Hero
{
	public Mage(string name, PieceTypes heroType, double hp, double attack, double armor, int attackRange) : base(name, heroType, hp, attack, armor, attackRange)
    {
    }

    public override void Skill(GameController gameController)
    {
        base.Skill(gameController);
    }
}