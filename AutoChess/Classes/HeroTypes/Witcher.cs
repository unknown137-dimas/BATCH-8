using AutoChess;

public class Witcher : Hero
{
    public Witcher(string name, HeroDetails heroDetails) : base(name, heroDetails)
    {
    }

    public Witcher(string name, PieceTypes heroType, double hp, double attack, double armor, int attackRange) : base(name, heroType, hp, attack, armor, attackRange)
    {
    }

    public override void Skill(GameController gameController)
    {
        if(!gameController.TryGetPlayerByPieceId(PieceId, out IPlayer? player))
        {
            return;
        }
        AttackRange += 1;
        var enemyIdList = gameController.GetAllEnemyId(player!, this);
        foreach(var enemyId in enemyIdList)
        {
            if(gameController.TryGetPieceById(enemyId, out IPiece? enemy))
            {
                enemy!.Hp -= new Random().Next(100, 200);
            }
        }
    }
}