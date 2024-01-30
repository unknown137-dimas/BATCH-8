using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Supplier
{
	[Key]
	public int SupplierId {get; set;}
	[Required]
	[StringLength(40)]
	public string CompanyName {get; set;} = null!;
	[StringLength(30)]
	public string ContactName {get; set;} = null!;
	[StringLength(30)]
	public string ContactTitle {get; set;} = null!;
	[StringLength(60)]
	public string Address {get; set;} = null!;
	[StringLength(15)]
	public string City {get; set;} = null!;
	[StringLength(15)]
	public string? Region {get; set;}
	[StringLength(10)]
	public string PostalCode {get; set;} = null!;
	[StringLength(15)]
	public string Country {get; set;} = null!;
	[StringLength(24)]
	public string Phone {get; set;} = null!;
	[StringLength(24)]
	public string? Fax {get; set;}
	[Column(TypeName = "ntext")]
	public string? HomePage {get; set;}
	public ICollection<Product> Products {get; set;} = null!;
	
	public Supplier()
	{
		Products = new HashSet<Product>();
	}
}