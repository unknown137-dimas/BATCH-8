using AutoChess;

public class Knight : Hero
{
	public Knight(string name, PieceTypes heroType, double hp, double attack, double armor, int attackRange) : base(name, heroType, hp, attack, armor, attackRange)
	{
	}
	
	public override void Skill()
	{
		Armor *= 1.05;
	}
}