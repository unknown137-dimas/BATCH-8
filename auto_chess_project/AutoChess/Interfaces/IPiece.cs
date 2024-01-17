interface IPiece
{
    string PieceId {get;}
    string Name {get;}
    double Hp {get;}
    double Attack {get;}
    double Armor {get;}
    double AttackRange {get;}
    PieceTypes PieceType {get;}
    public void Skill();
}