public interface IPiece
{
    string PieceId {get;}
    string Name {get;}
    double Hp {get; set;}
    double Attack {get;}
    double Armor {get;}
    int AttackRange {get;}
    PieceTypes PieceType {get;}
    void Skill();
    string ToString();
}