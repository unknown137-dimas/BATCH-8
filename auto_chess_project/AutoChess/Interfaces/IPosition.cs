interface IPosition
{
    int X {get;}
    int Y {get;}
    public void Move(int newX, int newY);
    public int[] GetPosition();
}