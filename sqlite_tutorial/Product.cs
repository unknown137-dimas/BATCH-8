public class Product
{
	public int ProductId {get; set;}
	public string ProductName {get; set;} = null!;
	public int CategoryId {get; set;}
	public Category Category {get; set;} = null!;
	public int SupplierId {get; set;}
	public Supplier Supplier {get; set;} = null!;
	public double Cost {get; set;}
	public short Stock {get; set;}
}