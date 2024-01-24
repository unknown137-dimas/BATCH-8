using AutoChess;

public class Hero : IPiece
{
    public Guid PieceId {get;}
    public string Name {get;}
    public PieceTypes PieceType {get;}
    public double Hp {get; set;}
    public double Attack {get; private set;}
    public double Armor {get; private set;}
    public int AttackRange {get; private set;}

    public Hero(string name, PieceTypes heroType, double hp, double attack, double armor, int attackRange)
    {
        PieceId = Guid.NewGuid();
        Name = name;
        PieceType = heroType;
        Hp = hp;
        Attack = attack;
        Armor = armor;
        AttackRange = attackRange;
    }

    public Hero(string name, HeroDetails heroDetails)
    {
        PieceId = Guid.NewGuid();
        Name = name;
        PieceType = heroDetails.HeroType;
        Hp = heroDetails.Hp;
        Attack = heroDetails.Attack;
        Armor = heroDetails.Armor;
        AttackRange = heroDetails.AttackRange;
    }

    public virtual void Skill(GameController gameController)
    {
        if(gameController.TryGetPlayerByPieceId(PieceId, out IPlayer? player))
        {
            var enemyIdList = gameController.GetAllEnemyId(player!, this);
            var alliesList = gameController.GetPlayerPieces(player!);
            switch(PieceType)
            {
                case PieceTypes.Knight:
                    Armor *= 1.05;
                    break;
                case PieceTypes.Warlock:
                    AttackRange += 1;
                    foreach(var enemyId in enemyIdList)
                    {
                        if(gameController.TryGetPieceById(enemyId, out IPiece? enemy))
                        {
                            enemy!.Hp -= 250;
                        }
                    }
                    break;
                case PieceTypes.Mage:
                    AttackRange += 1;
                    foreach(var enemyId in enemyIdList)
                    {
                        if(gameController.TryGetPieceById(enemyId, out IPiece? enemy))
                        {
                            enemy!.Hp *= 0.9;
                        }
                    }
                    break;
                case PieceTypes.Warrior:
                    Armor *= 1.1;
                    break;
                case PieceTypes.Hunter:
                    Attack *= 1.05;
                    break;
                case PieceTypes.Assassin:
                    Attack *= 1.1;
                    break;
                case PieceTypes.Shaman:
                    foreach(var allies in alliesList)
                    {
                        allies.Hp += 200;
                    }
                    break;
                case PieceTypes.Druid:
                    foreach(var allies in alliesList)
                    {
                        allies.Hp += 250;
                    }
                    break;
                case PieceTypes.Witcher:
                    AttackRange += 1;
                    foreach(var enemyId in enemyIdList)
                    {
                        if(gameController.TryGetPieceById(enemyId, out IPiece? enemy))
                        {
                            enemy!.Hp -= new Random().Next(100, 200);
                        }
                    }
                    break;
                case PieceTypes.Mech:
                    Armor *= 1.02;
                    Attack *= 1.02;
                    break;
                case PieceTypes.Priest:
                    foreach(var allies in alliesList)
                    {
                        allies.Hp += 300;
                    }
                    break;
                case PieceTypes.Wizard:
                    AttackRange += 1;
                    foreach(var enemyId in enemyIdList)
                    {
                        if(gameController.TryGetPieceById(enemyId, out IPiece? enemy))
                        {
                            enemy!.Hp -= new Random().Next(100, 200);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }

    public void AttackEnemy(IPiece otherPiece)
    {
        otherPiece.Hp -= Attack - otherPiece.Armor;
    }

    public override string ToString() => Name;
}