namespace code_first_db;

public class Category
{
	public int CategoryId { get; set; }
	public string CategoryName { get; set; } = null!;
	public string? Description { get; set; }
	public ICollection<Product> Products { get; set; }
	
	public Category()
	{
		Products = new HashSet<Product>();
	}
}
