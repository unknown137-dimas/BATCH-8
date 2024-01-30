using Microsoft.EntityFrameworkCore;

class Northwind : DbContext
{
	public DbSet<Category> Categories {get; set;}
	public DbSet<Product> Products {get; set;}
	public DbSet<Supplier> Suppliers {get; set;}
	
	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseSqlite("FileName=./Northwind.db");
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Product>(product => 
		{
			product.HasKey(p => p.ProductId);
			product.Property(p => p.ProductName).IsRequired(true).HasMaxLength(40);
			product.Property(p => p.Cost).HasColumnName("UnitPrice").HasColumnType("money");
			product.Property(p => p.Stock).HasColumnName("UnitsInStock").HasColumnType("smallint");
			product.HasOne(p => p.Category).WithMany(c => c.Products);
			product.HasOne(p => p.Supplier).WithMany(c => c.Products);
		});
	}
}