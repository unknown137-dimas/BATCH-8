interface IPosition
{
    int X {get;}
    int Y {get;}
    public void UpdatePosition(int newX, int newY);
    public int[] GetPosition();
}