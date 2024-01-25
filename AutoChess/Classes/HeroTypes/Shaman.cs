using AutoChess;

public class Shaman : Hero
{
	public Shaman(string name, PieceTypes heroType, double hp, double attack, double armor, int attackRange) : base(name, heroType, hp, attack, armor, attackRange)
	{
	}

    public override void Skill(GameController gameController)
    {
        if(!gameController.TryGetPlayerByPieceId(PieceId, out IPlayer? player))
        {
            return;
        }
        gameController.GetPlayerPieces(player!).Select(allies => allies.Hp += 200);
    }
}