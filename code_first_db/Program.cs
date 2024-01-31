using code_first_db;

class Program
{
	static async Task Main()
	{
		using(Southwind sw = new())
		{
			List<Category> newCategories = [
				new Category()
				{
					CategoryName = "Electronics",
					Description = "Electronics and Parts"
				},
				new Category()
				{
					CategoryName = "Automotive",
					Description = "Automotives Parts"
				},
				new Category()
				{
					CategoryName = "Seafood",
					Description = "Fish, Shrimp, and Sea Shell"
				},
				new Category()
				{
					CategoryName = "Beverage",
					Description = "Drinks and Beverages"
				}
			];
			
			foreach(var newCategory in newCategories)
			{
				var result = sw.Categories.FirstOrDefault(c => c.CategoryName.Contains(newCategory.CategoryName));
				if(result is null)
				{
					await sw.Categories.AddAsync(newCategory);
				}
			}
			await sw.SaveChangesAsync();
		}
	}
}