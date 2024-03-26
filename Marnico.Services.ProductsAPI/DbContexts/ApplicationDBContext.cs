using Marnico.Services.ProductsAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace Marnico.Services.ProductsAPI.DbContexts
{
    public class ApplicationDBContext: DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {

        }
        public DbSet<Products> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Products>().HasData(new Products
            {
                ProductId = 1,
                Name = "Pizza",
                Price = 185,
                Description ="The best pizza you have had ever !",
                ImageUrl ="",
                CategoryName= "Main Food"
            });
            modelBuilder.Entity<Products>().HasData(new Products
            {
                ProductId = 2,
                Name = "Burger",
                Price = 165,
                Description = "The best Burger you have had ever !",
                ImageUrl = "",
                CategoryName = "Main Food"
            });
        }
    }
}
