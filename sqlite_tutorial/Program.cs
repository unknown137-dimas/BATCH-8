using Microsoft.EntityFrameworkCore;

class Program
{
	static async Task Main()
	{
		// READ THE DB
		using(Northwind db = new())
		{
			// // List all category in db
			// var categories = db.Categories.Include(c => c.Products);
			// foreach(var category in categories)
			// {
			// 	Console.WriteLine($"{category.CategoryId} : {category.Description}");
			// 	// List all product for each category
			// 	foreach(var product in category.Products)
			// 	{
			// 		Console.WriteLine($"	{product.ProductId} : {product.ProductName} : {product.Cost}");
			// 	}
			// }
			
			// // List all product in db
			// var products = db.Products.Include(p => p.Category);
			// foreach(var product in products)
			// {
			// 	Console.WriteLine($"{product.Category.CategoryName} : {product.ProductId} : {product.ProductName} : {product.Cost}");
			// }
			
			// List all product with stock less than 10 and its supplier
			var products = db.Products
			.Include(p => p.Category)
			.Include(p => p.Supplier)
			.Where(p => p.Stock < 10)
			.OrderBy(p => p.Stock)
			.ThenBy(p => p.ProductName);
			
			Console.WriteLine($"{"Category",-15} : {"Product Name",-32} : Product Stock");
			Console.WriteLine("-----------------------------------------------------------------");
			foreach(var product in products)
			{
				Console.WriteLine($"{product.Category.CategoryName,-15} : {product.ProductName,-32} : {product.Stock}");
			}
			
			Console.WriteLine($"{"Supplier Name",-40} : {"Supplier CP",-20} : {"Supplier Contact",-20} : Product Name");
			Console.WriteLine("--------------------------------------------------------------------------------------------------");
			foreach(var product in products)
			{
				Console.WriteLine($"{product.Supplier.CompanyName,-40} : {product.Supplier.ContactName,-20} : {product.Supplier.Phone,-20} : {product.ProductName}");
			}
		}
		
		// // CHANGE THE DB
		// using(Northwind db = new())
		// {
		// 	var newCategories = new List<Category>()
		// 	{
		// 		new Category()
		// 		{
		// 			CategoryName = "Automotive",
		// 			Description = "Automotive, Vehicle Parts"
		// 		},
		// 		new Category()
		// 		{
		// 			CategoryName = "Electronic & Gadget",
		// 			Description = "Electronic, Gadget, Computer Parts"
		// 		},
		// 	};
			
		// 	await db.Categories.AddRangeAsync(newCategories);
		// 	await db.SaveChangesAsync();
		// }
		
		// UPDATE THE DB
		using(Northwind db = new())
		{
			var result = db.Categories.FirstOrDefault(c => c.CategoryName.Contains("Automotive"));
			
			if(result is not null)
			{
				result.Description = "Vehicle Parts, Car, Truck, Motorcycle";
				await db.SaveChangesAsync();
			}
			
		}
	}
}

