using AutoChess;

public interface IPiece
{
    Guid PieceId {get;}
    string Name {get;}
    double Hp {get; set;}
    double Attack {get;}
    double Armor {get;}
    int AttackRange {get;}
    PieceTypes PieceType {get;}
    void Skill();
    void Skill(GameController gameController);
    string ToString();
}