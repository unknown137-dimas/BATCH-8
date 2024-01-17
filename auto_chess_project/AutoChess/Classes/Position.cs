class Position : IPosition
{
    public int X {get; private set;} = -1;
    public int Y {get; private set;} = -1;

    public Position() {}
    
    public Position(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        
        var other = obj as Position;
        if (ReferenceEquals(other, null))
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return X == other.X && Y == other.Y;
    }

    public override int GetHashCode() =>  X.GetHashCode() + Y.GetHashCode();

    public void UpdatePosition(int newX, int newY)
    {
        X = newX;
        Y = newY;
    }

    public int[] GetPosition() => [X, Y];

    public bool IsInRange(IPosition otherPosition, int range) => (otherPosition.X - X) <= range && (otherPosition.Y - Y) <= range;

    public static bool operator ==(Position self, Position other) => self.Equals(other);
    public static bool operator !=(Position self, Position other) => !self.Equals(other);
}