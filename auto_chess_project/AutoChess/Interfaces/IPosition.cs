interface IPosition
{
    int X {get;}
    int Y {get;}
    public void UpdatePosition(int newX, int newY);
    public int[] GetPosition();
    public bool IsInRange(IPosition otherPosition, int range);
}