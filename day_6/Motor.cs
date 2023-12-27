class Motor
{
	public int price;
	public Motor(int price) => this.price = price;
	public static Motor operator +(Motor self, Motor other) => new Motor(self.price + other.price);
}