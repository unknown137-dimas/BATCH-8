using Microsoft.EntityFrameworkCore;
using code_first_db;

class Southwind : DbContext
{
	public DbSet<Category> Categories {get; set;}
	public DbSet<Product> Products {get; set;}
	public DbSet<Supplier> Suppliers {get; set;}
	
	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		// optionsBuilder.UseSqlite("FileName=./Southwind.db");
		optionsBuilder.UseNpgsql("Host=localhost;Database=myDB;Username=postgres;Password=postgresB8");
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Category>(category => 
		{
			category.HasKey(c => c.CategoryId);
			category.Property(c => c.CategoryId).ValueGeneratedOnAdd();
			category.Property(c => c.CategoryName).IsRequired(true).HasMaxLength(20);
			category.Property(c => c.Description).IsRequired(false);
			category.HasMany(c => c.Products);
		});
		
		modelBuilder.Entity<Product>(product => 
		{
			product.HasKey(p => p.ProductId);
			product.Property(p => p.ProductId).ValueGeneratedOnAdd();
			product.Property(p => p.ProductName).IsRequired(true).HasMaxLength(40);
			product.Property(p => p.Cost).HasColumnName("UnitPrice").HasColumnType("money");
			product.Property(p => p.Stock).HasColumnName("UnitsInStock").HasColumnType("smallint");
			product.HasOne(p => p.Category);
			product.HasOne(p => p.Supplier);
		});
		
		modelBuilder.Entity<Supplier>(supplier => 
		{
			supplier.HasKey(s => s.SupplierId);
			supplier.Property(s => s.SupplierId).ValueGeneratedOnAdd();
			supplier.Property(s => s.CompanyName).IsRequired(true).HasMaxLength(40);
			supplier.Property(s => s.ContactName).IsRequired(true).HasMaxLength(30);
			supplier.Property(s => s.ContactTitle).IsRequired(true).HasMaxLength(30);
			supplier.Property(s => s.Address).IsRequired(true).HasMaxLength(60);
			supplier.Property(s => s.City).IsRequired(true).HasMaxLength(15);
			supplier.Property(s => s.Region).IsRequired(false).HasMaxLength(15);
			supplier.Property(s => s.PostalCode).IsRequired(true).HasMaxLength(10);
			supplier.Property(s => s.Country).IsRequired(true).HasMaxLength(15);
			supplier.Property(s => s.Phone).IsRequired(true).HasMaxLength(24);
			supplier.Property(s => s.Fax).IsRequired(false).HasMaxLength(24);
			supplier.Property(s => s.HomePage).IsRequired(false);
			supplier.HasMany(s => s.Products);
		});
	}
}