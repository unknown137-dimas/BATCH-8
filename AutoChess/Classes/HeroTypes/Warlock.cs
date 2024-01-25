using AutoChess;

public class Warlock : Hero
{
	public Warlock(string name, PieceTypes heroType, double hp, double attack, double armor, int attackRange) : base(name, heroType, hp, attack, armor, attackRange)
    {
    }

    public override void Skill(GameController gameController)
    {
        if(!gameController.TryGetPlayerByPieceId(PieceId, out IPlayer? player))
        {
            return;
        }
        var enemyIdList = gameController.GetAllEnemyId(player!, this);
        AttackRange += 1;
        foreach(var enemyId in enemyIdList)
        {
            if(gameController.TryGetPieceById(enemyId, out IPiece? enemy))
            {
                enemy!.Hp -= 250;
            }
        }
    }
}