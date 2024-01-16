interface IBoard
{
    int Width {get;}
    int Height {get;}
    public bool IsPositionEmpty(Position position);
    public bool AddHeroPosition(Hero hero, Position position);
}

interface IPlayer
{
    string PlayerId {get;}
    string Name {get;}
}

interface IPosition
{
    int X {get;}
    int Y {get;}
    public void Move(int newX, int newY);
    public int[] GetPosition();
}

interface IPiece
{
    string PieceId {get;}
    string Name {get;}
    double Hp {get;}
    double Attack {get;}
    double Armor {get;}
    double AttackRange {get;}
    PieceTypes PieceType {get;}
    public IEnumerable<IPiece> GetTarget();
    public void Skill();
}