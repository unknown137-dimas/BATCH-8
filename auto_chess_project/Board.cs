class Board : IBoard
{
    public int Width {get;}

    public int Height {get;}

    public Board(int size)
    {
        Width = size;
        Height = size;
    }
    public Board(int width, int height)
    {
        Width = width;
        Height = height;
    }
}