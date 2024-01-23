public interface IPosition
{
    int X {get;}
    int Y {get;}
    void UpdatePosition(int newX, int newY);
    int[] GetPosition();
    bool IsInRange(IPosition otherPosition, int range);
}